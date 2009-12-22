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
	/// Basic implementation of an HTTP - Server. Can be used to avoid the use of .NET Remoting or
	/// IIS.
	/// The server can be started and stopped using the similar named methods. It is a non-blocking
	/// Server. Started in it's own thread it does not block the other threads while listening on a port.
	/// </summary>
	public class SimpleHttpServer : IDisposable
	{
		#region constants

		private const int ExitThread = 0;
		private const int LookForNextConnection = 1;
		private const int DefaultPortNumber = 80;
		private const int BiggestWellKnownPortNumber = 1024;
		private const int BiggestPossiblePortNumber = 65535;

		#endregion

		#region fields

		private static ILog logger = LogManager.GetLogger(typeof(SimpleHttpServer));

		// Messaging & Signaling infrastructure
		private AutoResetEvent stopThreadEvent = new AutoResetEvent(false);
		private AutoResetEvent listenForConnection = new AutoResetEvent(false);
		private WaitHandle[] signals = new WaitHandle[2];

		// private HTTPServerState httpServerState; -> Not used at the moment
		private BaseHttpServiceEndpoint serviceEndpoint;
		private Thread listeningForConnectionLoop;
		private HttpListener httpListener;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the port where this HTTP Server is listening to.
		/// </summary>
		/// <value>An integer in the range [1024 - 65535].</value>
		public int ListeningPort { get; private set; }

		#endregion

		#region constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleHttpServer"/> class.
		/// </summary>
		/// <remarks>If the port number is in range of the well-known ports [0 - 1024] the default port 80 is taken.</remarks>
		/// <param name="service">The HTTP Service implementation which can handle the incoming requests.</param>
		/// <param name="portNumber">The port where this server should listen on incoming connections. Must be > 1023.</param>
		/// <exception cref="PlatformNotSupportedException">Is thrown when the underlying operating system does not support <see cref="HttpListener"/>s.</exception>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="service"/> is a null reference.</exception>
		/// <exception cref="ArgumentException">Is thrown when <paramref name="portNumber"/> is bigger than 65535.</exception>
		public SimpleHttpServer(BaseHttpServiceEndpoint service, int portNumber)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service", "The Service endpoint must be an instance.");
			}

			if (portNumber > SimpleHttpServer.BiggestPossiblePortNumber)
			{
				throw new ArgumentException("The port number must be smaller than "+(SimpleHttpServer.BiggestPossiblePortNumber+1)+".", "portNumber");
			}

			if (portNumber < SimpleHttpServer.BiggestWellKnownPortNumber)
			{
				logger.WarnFormat("Using default port number {0} to listen on HTTP requests.", portNumber);
				this.ListeningPort = DefaultPortNumber;
			}

			this.httpListener = new HttpListener();
			this.httpListener.Prefixes.Add(String.Concat("http://+:", this.ListeningPort, "/"));
			this.serviceEndpoint = service;
			logger.DebugFormat(CultureInfo.CurrentCulture, "HTTP Listener created on port: " + portNumber);

			this.signals[ExitThread] = this.stopThreadEvent;
			this.signals[LookForNextConnection] = this.listenForConnection;
		}

		#endregion

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
				logger.Info("Starting HTTP server.");
				this.httpListener.Start();
				this.listeningForConnectionLoop = new Thread(this.Execute);
				this.listeningForConnectionLoop.Name = "HTTP Server";
				this.listeningForConnectionLoop.Start();
				logger.Info("HTTP server started.");
			}
			catch (HttpListenerException hle)
			{
				logger.Error(hle);
				throw;
			}
			catch (ObjectDisposedException ode)
			{
				logger.Error(ode);
				throw;
			}
		}

		/// <summary>
		/// Signal the HTTP Server to stop.
		/// </summary>
		public void Stop()
		{
			this.stopThreadEvent.Set();
			try
			{
				this.httpListener.Stop();
				this.httpListener.Close();
			}
			catch (ObjectDisposedException ode)
			{
				logger.DebugFormat(CultureInfo.CurrentCulture, "Http Listener was already disposed. {0}", ode.Message);
			}

			this.listeningForConnectionLoop.Join();
			logger.Info("HTTP server stopped");
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
				logger.Debug("Waiting for incoming connections.");
				try
				{
					HttpListenerContext context = this.httpListener.GetContext();
					this.OnAcceptedConnection(context);
				}
				catch (HttpListenerException hle)
				{
					logger.WarnFormat(CultureInfo.CurrentCulture, "HttpListenerException caught with Error code: {0} and message: {1}", hle.ErrorCode, hle.Message);
				}
				catch (InvalidOperationException ioe)
				{
					logger.WarnFormat(CultureInfo.CurrentCulture, "Invalid operation caught on HTTP - Listener. Shutting down. Cause: {0}", ioe.Message);
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
				logger.Debug("Incoming connection accepted.");
				logger.DebugFormat(CultureInfo.CurrentCulture, "Accepted request method: {0}", connection.Request.HttpMethod);
				logger.DebugFormat(CultureInfo.CurrentCulture, "Client IP and Port: {0}", connection.Request.UserHostAddress);
				logger.DebugFormat(CultureInfo.CurrentCulture, "Client name: {0}", connection.Request.UserHostName);
				logger.Debug("Processing input.");

				// It is possible that this server host another service than an a base service
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
					SimpleHttpServer.logger.WarnFormat("Unsupported service endpoint type. Type: {0}", this.serviceEndpoint.GetType().ToString());
				}

				this.listenForConnection.Set();
			}
			catch (HttpListenerException hle)
			{
				logger.ErrorFormat(CultureInfo.CurrentCulture, "HttpListenerException caught with Error code: " + hle.ErrorCode + " and message: " + hle.Message);
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
					this.stopThreadEvent.Close();
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
