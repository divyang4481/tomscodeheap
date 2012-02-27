using CH.Froorider.JamesSharpContracts.Protocols;
using System.ComponentModel.Composition;
using System;
using JamesSharpSmtp.SmtpProtocol;
using System.Text;

namespace JamesSharpSmtp
{
    [Export(typeof(IProtocol))]
    [ExportMetadata("PortNumber", 25)]
    [ExportMetadata("ProtocolName", "SMTP")]
    public class SmtpMock : BaseProtocol
    {
        private byte[] _buffer = new byte[1024];
        private ReplyCodes _codeTabel = new ReplyCodes();

        public override ProtocolType TypeOfProtocol
        {
            get
            {
                return SmtpProtocolType.SmtpProtocol;
            }
        }

        public override void ProcessConnection(object message)
        {
            Console.WriteLine("James Sharp SMTP Server is processing message.");

            WriteToStream(_codeTabel.GetMessageForCode(220));
            
            string command = this.ReadFromStream();
            Console.WriteLine("Recieved command: " + command);

            StreamToProcess.Close();
        }

        private void WriteToStream(string message)
        {
            _buffer = Encoding.ASCII.GetBytes(message);
            StreamToProcess.Write(_buffer, 0, _buffer.Length);
           
        }

        private string ReadFromStream()
        {
            StringBuilder completeMessage = new StringBuilder();
            // Incoming message may be larger than the buffer size.
            do
            {
                int numberOfBytesRead = StreamToProcess.Read(_buffer, 0, _buffer.Length);
                completeMessage.AppendFormat("{0}", Encoding.ASCII.GetString(_buffer, 0, numberOfBytesRead));
            }
            while (StreamToProcess.DataAvailable);

            Console.WriteLine("Received message: " + completeMessage);
            return completeMessage.ToString();
        }
    }
}
