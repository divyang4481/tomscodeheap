// ========================================================================
// File:     SimpleHttpServer.cs
//
// Author:   $Author$
// Date:     $LastChangedDate$
// Revision: $Revision$
// ========================================================================
// Copyright [2009] [$Author$]
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

using System;
using System.Globalization;
using System.Net;
using System.Threading;
using log4net;

namespace CH.Froorider.Codeheap.Http
{
	/// <summary>
    /// Defines all states this HTTP Server can have. Is internal because only the PAC itself
    /// must have knowledge about the state etc. of this server. The outside world has no interest on it.
    /// </summary>
    internal enum HTTPServerState
    {
        /// <summary>
        /// Server state is not set. Default value. Also nessecary to fulfill code rule CA1008.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Server is initialized, ready for work but the mian loop is not yet started.
        /// </summary>
        Created = 1,

        /// <summary>
        /// Main loop is running, but does no work and waits for input.
        /// </summary>
        Idle = 2,

        /// <summary>
        /// Main loop is running and waiting for incoming connections.
        /// </summary>
        Listening = 3,

        /// <summary>
        /// The main loop is doing his primary work. Don't interrupt him until he has finished it's work. 
        /// </summary>
        Working = 4,

        /// <summary>
        /// Main loop has ended. Server can be shutdown.
        /// </summary>
        Stopped = 5
    }

    /// <summary>
    /// Basic implementation of an HTTP - Server. Can be used to avoid the use of .NET Remoting or
    /// IIS.
    /// The server can be started and stopped using the similar named methods. It is a non-blocking
    /// Server. Started in it's own thread it does not block the other threads while listening on a port.
    /// </summary>
	public class SimpleHttpServer : IDisposable
	{
		private const int ExitThread = 0;
		private const int LookForNextConnection = 1;
		private const string DefaultPortNumber = "11000";

		private static readonly object threadSafer = new object();
		private static ILog logger = LogManager.GetLogger(typeof(SimpleHttpServer));

		// Messaging & Signaling infrastructure
		private AutoResetEvent stopThreadEvent = new AutoResetEvent(false);
		private AutoResetEvent listenForConnection = new AutoResetEvent(false);
		private WaitHandle[] signals = new WaitHandle[2];

		// private HTTPServerState httpServerState; -> Not used at the moment
		private BaseHttpServiceEndpoint serviceEndpoint;
		private Thread listeningForConnectionLoop;
		private HttpListener httpListener;

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleHttpServer"/> class.
		/// If no port is configured the server is listening on port number 11000.
		/// </summary>
		/// <param name="service">The HTTP Service implementation which can handle the incoming requests.</param>
		/// <param name="portNumber">The port where this server should listen on incoming connections.</param>
		public SimpleHttpServer(BaseHttpServiceEndpoint service, int portNumber)
		{
			if (String.IsNullOrEmpty(portNumber.ToString(CultureInfo.InvariantCulture)) || portNumber == 0)
			{
				logger.WarnFormat("Using default port number {0} to listen on XML-RPC messages.", portNumber);
				portNumber = Convert.ToInt32(DefaultPortNumber, CultureInfo.CurrentCulture);
			}

			this.httpListener = new HttpListener();
			this.httpListener.Prefixes.Add(String.Concat("http://+:", portNumber, "/"));
			logger.DebugFormat("HTTP Listener created on port: " + portNumber);
			this.serviceEndpoint = service;

			this.signals[ExitThread] = this.stopThreadEvent;
			this.signals[LookForNextConnection] = this.listenForConnection;
		}

		#region public methods

		/// <summary>
		/// Starts the server. The server does his work, until the Stop() - method signals
		/// him to shutdown.
		/// </summary>
		/// <exception cref="HttpListenerException">Is thrown when the HTTP stream couldn't be processed.</exception>
		/// <exception cref="ObjectDisposedException">Is thrown when the underlying network stream was already closed.</exception>
		public void Start()
		{
			try
			{
				logger.InfoFormat("Starting HTTP server.");
				this.httpListener.Start();
				this.listeningForConnectionLoop = new Thread(this.Execute);
				this.listeningForConnectionLoop.Name = "HTTP Server";
				this.listeningForConnectionLoop.Start();
				logger.InfoFormat("HTTP server started.");
			}
			catch (HttpListenerException hle)
			{
				logger.Error(hle);
				throw hle;
			}
			catch (ObjectDisposedException ode)
			{
				logger.Error(ode);
				throw ode;
			}
		}

		/// <summary>
		/// Signal the HTTP Server to stop. Blocks until all server parts have been stopped.<br></br><b>This method is thread-safe.</b>
		/// </summary>
		public void Stop()
		{
			lock (threadSafer)
			{
				this.stopThreadEvent.Set();
				try
				{
					this.httpListener.Stop();
					this.httpListener.Close();
				}
				catch (ObjectDisposedException ode)
				{
					logger.DebugFormat("Http Listener was already disposed. {0}", ode.Message);
				}

				this.listeningForConnectionLoop.Join();
				logger.InfoFormat("HTTP server stopped");
			}
		}

		/// <summary>
		/// Main functionality. Does the work, checks the events and so. Is called
		/// started by the Start-method. Loops until someone calls the Stop-method.
		/// </summary>
		private void Execute()
		{
			logger.Debug("Start listening.");
			int signalIndex;

			do
			{
				logger.DebugFormat("Waiting for incoming connections.");
				try
				{
					HttpListenerContext context = this.httpListener.GetContext();
					this.OnAcceptedConnection(context);
				}
				catch (HttpListenerException hle)
				{
					logger.WarnFormat("HttpListenerException caught with Error code: {0} and message: {1}", hle.ErrorCode, hle.Message);
				}
				catch (InvalidOperationException ioe)
				{
					logger.WarnFormat("Invalid operation caught on HTTP - Listener. Shutting down. Cause: {0}", ioe.Message);
					this.stopThreadEvent.Set();
				}

				signalIndex = WaitHandle.WaitAny(this.signals);
			}
			while (signalIndex != ExitThread);
		}

		#endregion

		#region private methods

		/// <summary>
		/// Async method is called after a HTTP - connection is accepted. This method
		/// takes the connection and processes it.
		/// </summary>
		/// <param name="connection">The context given by the accepted connection.</param>
		private void OnAcceptedConnection(HttpListenerContext connection)
		{
			try
			{
				logger.DebugFormat("Incoming connection accepted.");
				logger.DebugFormat("Accepted request method: {0}", connection.Request.HttpMethod);
				logger.DebugFormat("Client IP and Port: {0}", connection.Request.UserHostAddress);
				logger.DebugFormat("Client name: {0}", connection.Request.UserHostName);
				logger.DebugFormat("Processing input.");

				// It is possible that this server host another service than an Pac Service
				// so we filter this out here.
				if (this.serviceEndpoint.GetType().Equals(typeof(BaseHttpServiceEndpoint)))
				{
					BaseHttpServiceEndpoint endpoint = (BaseHttpServiceEndpoint)this.serviceEndpoint;
					connection.Response.SendChunked = false;
					endpoint.Context = connection;

					// We put the execution of each request into it's own background thread.
					ThreadPool.QueueUserWorkItem(endpoint.ProcessConnection);
				}
				else
				{
					// ATTENTION: This is blocking => Runs in the same thread!
					// HAT: Should we really do this, or actively refuse the incoming request?
					this.serviceEndpoint.ProcessRequest(connection);
				}

				this.listenForConnection.Set();
			}
			catch (HttpListenerException hle)
			{
				logger.ErrorFormat("HttpListenerException caught with Error code: " + hle.ErrorCode + " and message: " + hle.Message);
			}
		}

		#endregion

		#region IDisposable

		private bool disposed;

		/// <summary>
		/// Disposes the unmanaged ressources.
		/// </summary>
		/// <param name="disposing">Always true.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					// For AutoResetEvent it is not necessary to implement the Dispose-Pattern. 
					// See MSDN -> WaitHandle class; But we do it here to satisfy CA1063
					this.stopThreadEvent.SafeWaitHandle.Dispose();
					this.stopThreadEvent.Close();
					this.listenForConnection.SafeWaitHandle.Dispose();
					this.listenForConnection.Close();
					this.httpListener.Close();
				}
			}

			this.disposed = true;
		}

		/// <summary>
		/// Dispose - pattern.
		/// Call base.Dispose() first if possible, then the Dispose of the class and finally
		/// Supress the GC-Finalizer.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="SimpleHttpServer"/> class.
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="SimpleHttpServer"/> is reclaimed by garbage collection.
		/// </summary>
		~SimpleHttpServer()
		{
			this.Dispose(false);
		}

		#endregion
	}
}
