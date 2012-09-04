namespace CH.Froorider.JamesSharpContracts.Protocols
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// Base implemenation of a protocol. Is used by the host to load and start the protocol.
    /// </summary>
    public abstract class BaseProtocol : IProtocol
    {
        private NetworkStream _stream;
        private byte[] _buffer = new byte[1024];

        public NetworkStream StreamToProcess
        {
            get
            {
                return _stream;
            }
            set
            {
                if (value != null)
                {
                    _stream = value;
                }
            }
        }

        public virtual ProtocolType TypeOfProtocol
        {
            get
            {
                return ProtocolType.TcpProtocol;
            }
        }

        public virtual void ProcessConnection(object message)
        {
            Console.WriteLine("Processing Message.");
            StringBuilder completeMessage = new StringBuilder();

            // Incoming message may be larger than the buffer size.
            do
            {
                int numberOfBytesRead = _stream.Read(_buffer, 0, _buffer.Length);
                completeMessage.AppendFormat("{0}", Encoding.ASCII.GetString(_buffer, 0, numberOfBytesRead));
            }
            while (_stream.DataAvailable);

            Console.WriteLine("Received message: " + completeMessage);

            _buffer = Encoding.ASCII.GetBytes("Hallo du FooBar!");
            _stream.Write(_buffer, 0, _buffer.Length);
            _stream.Close();
        }
    }
}
