namespace CH.Froorider.JamesSharp.Server
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using CH.Froorider.JamesSharpContracts.Protocols;
    using log4net;

    /// <summary>
    /// Implementation of a basic TCP Server. Incoming connections are "routed" to the protocol implemementation.
    /// </summary>
    internal class TcpServer : IDisposable
    {
        #region constants

        private const int ExitThread = 0;
        private const int LookForNextConnection = 1;
        private const int DefaultPortNumber = 808;
        private const int SmallestPossiblePortNumber = 1;
        private const int BiggestPossiblePortNumber = 65535;

        #endregion

        #region fields

        private static readonly ILog _logger = LogManager.GetLogger(typeof(TcpServer));

        // Messaging & Signaling infrastructure
        private readonly AutoResetEvent _stopThreadEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _listenForConnection = new AutoResetEvent(false);
        private readonly WaitHandle[] _signals = new WaitHandle[2];

        private readonly IProtocol _serviceEndpoint;
        private Thread _listeningForConnectionLoop;
        private readonly TcpListener _tcpListener;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the port where this TCP Server is listening to.
        /// </summary>
        /// <value>An integer in the range [1024 - 65535].</value>
        public int ListeningPort { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpServer"/> class.
        /// </summary>
        /// <remarks>If the port number is in range of the well-known ports [0 - 1024] the default port 80 is taken.</remarks>
        /// <param name="service">The TCP Service implementation which can handle the incoming requests.</param>
        /// <param name="portNumber">The port where this server should listen on incoming connections. Must be > 1023.</param>
        /// <exception cref="PlatformNotSupportedException">Is thrown when the underlying operating system does not support <see cref="HttpListener"/>s.</exception>
        /// <exception cref="ArgumentNullException">Is thrown when <paramref name="service"/> is a null reference.</exception>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="portNumber"/> is bigger than 65535.</exception>
        public TcpServer(IProtocol service, int portNumber)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service", "The Service endpoint must be an instance.");
            }

            if (portNumber > TcpServer.BiggestPossiblePortNumber)
            {
                throw new ArgumentException("The port number must be smaller than " + (TcpServer.BiggestPossiblePortNumber + 1) + ".", "portNumber");
            }

            if (portNumber < TcpServer.SmallestPossiblePortNumber)
            {
                _logger.WarnFormat("Using default port number {0} to listen on TCP requests.", portNumber);
                this.ListeningPort = DefaultPortNumber;
            }

            var localAddress = IPAddress.Parse("127.0.0.1");
            _tcpListener = new TcpListener(localAddress, portNumber);
            _serviceEndpoint = service;
            _logger.DebugFormat(CultureInfo.CurrentCulture, "TCP Listener created on port: " + portNumber);

            this._signals[ExitThread] = this._stopThreadEvent;
            this._signals[LookForNextConnection] = this._listenForConnection;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Starts the server. The server does his work, until the Stop() - method signals
        /// him to shutdown.
        /// </summary>
        /// <exception cref="HttpListenerException">Is thrown when the TCP stream couldn't be processed.</exception>
        /// <exception cref="ObjectDisposedException">Is thrown when the underlying network stream was already closed.</exception>
        public void Start()
        {
            try
            {
                _logger.Debug("Starting TCP server.");
                _tcpListener.Start();
                _listeningForConnectionLoop = new Thread(this.Execute) { Name = _serviceEndpoint.TypeOfProtocol.ToString() };
                _listeningForConnectionLoop.Start();
                _logger.Debug(_serviceEndpoint.TypeOfProtocol+" server started.");
            }
            catch (SocketException se)
            {
                _logger.Error(se);
                throw;
            }
            catch (ObjectDisposedException ode)
            {
                _logger.Error(ode);
                throw;
            }
        }

        /// <summary>
        /// Signal the TCP Server to stop.
        /// </summary>
        public void Stop()
        {
            _stopThreadEvent.Set();
            try
            {
                _tcpListener.Stop();
            }
            catch (ObjectDisposedException ode)
            {
                _logger.DebugFormat(CultureInfo.CurrentCulture, "TCP Listener was already disposed. {0}", ode.Message);
            }

            this._listeningForConnectionLoop.Join();
            _logger.Info("TCP server stopped");
        }

        /// <summary>
        /// Main functionality. Does the work, checks the events and so. Is called
        /// started by the Start-method. Loops until someone calls the Stop-method.
        /// </summary>
        private void Execute()
        {
            _logger.Debug("Start listening.");
            int signalIndex;

            do
            {
                _logger.Debug("Waiting for incoming connections.");
                try
                {
                    TcpClient context = _tcpListener.AcceptTcpClient();
                    OnAcceptedConnection(context);
                }
                catch (SocketException se)
                {
                    _logger.WarnFormat(CultureInfo.CurrentCulture, "SocketException caught with Error code: {0} and message: {1}", se.ErrorCode, se.Message);
                }
                catch (InvalidOperationException ioe)
                {
                    _logger.WarnFormat(CultureInfo.CurrentCulture, "Invalid operation caught on TCP - Listener. Shutting down. Cause: {0}", ioe.Message);
                    _stopThreadEvent.Set();
                }

                signalIndex = WaitHandle.WaitAny(_signals);
            }
            while (signalIndex != ExitThread);
        }

        #endregion

        #region private methods

        /// <summary>
        /// Async method is called after a TCP - connection is accepted. This method
        /// takes the connection and processes it.
        /// </summary>
        /// <param name="connection">The context given by the accepted connection.</param>
        private void OnAcceptedConnection(TcpClient connection)
        {
            try
            {
                _logger.Debug("Incoming connection accepted.");
                _logger.DebugFormat(CultureInfo.CurrentCulture, "Accepted request from: {0}", connection.Client.RemoteEndPoint);
                _logger.Debug("Processing input.");

                var endpoint = _serviceEndpoint;
                endpoint.StreamToProcess = connection.GetStream();

                // We put the execution of each request into it's own background thread.
                ThreadPool.QueueUserWorkItem(endpoint.ProcessConnection);

                _listenForConnection.Set();
            }
            catch (SocketException se)
            {
                _logger.ErrorFormat(CultureInfo.CurrentCulture, "TcpListenerException caught with Error code: " + se.ErrorCode + " and message: " + se.Message);
            }
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        /// <summary>
        /// Disposes the unmanaged ressources.
        /// </summary>
        /// <param name="disposing">Always true.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _tcpListener.Stop();
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Dispose - pattern.
        /// Call base.Dispose() first if possible, then the Dispose of the class and finally
        /// Supress the GC-Finalizer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TcpServer"/> class.
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TcpServer"/> is reclaimed by garbage collection.
        /// </summary>
        ~TcpServer()
        {
            Dispose(false);
        }

        #endregion
    }
}
