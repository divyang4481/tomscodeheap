using CH.Froorider.JamesSharpContracts.Protocols;
using System.ComponentModel.Composition;
using System.Text;
using System;

namespace JamesSharpPop
{
    [Export(typeof(IProtocol))]
    [ExportMetadata("PortNumber", 335)]
    [ExportMetadata("ProtocolName", "POP3")]
    public class PopMock : BaseProtocol
    {
        private byte[] _buffer = new byte[1024];

        public override void ProcessConnection(object message)
        {
            Console.WriteLine("James Sharp POP Server is processing message.");
            StringBuilder completeMessage = new StringBuilder();
            // Incoming message may be larger than the buffer size.
            do
            {
                int numberOfBytesRead = StreamToProcess.Read(_buffer, 0, _buffer.Length);
                completeMessage.AppendFormat("{0}", Encoding.ASCII.GetString(_buffer, 0, numberOfBytesRead));
            }
            while (StreamToProcess.DataAvailable);

            Console.WriteLine("Received message: " + completeMessage);

            _buffer = Encoding.ASCII.GetBytes("POP3 implemented!");
            StreamToProcess.Write(_buffer, 0, _buffer.Length);
            StreamToProcess.Close();
        }
    }
}
