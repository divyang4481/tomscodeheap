using CH.Froorider.JamesSharpContracts.Protocols;
using System.ComponentModel.Composition;
using System;
using JamesSharpSmtp.SmtpProtocol;
using System.Text;

namespace JamesSharpSmtp
{
    [Export(typeof(IProtocol))]
    public class SmtpMock : BaseProtocol
    {
        private byte[] _buffer = new byte[1024];

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
            StringBuilder completeMessage = new StringBuilder();
            // Incoming message may be larger than the buffer size.
            do
            {
                int numberOfBytesRead = StreamToProcess.Read(_buffer, 0, _buffer.Length);
                completeMessage.AppendFormat("{0}", Encoding.ASCII.GetString(_buffer, 0, numberOfBytesRead));
            }
            while (StreamToProcess.DataAvailable);

            Console.WriteLine("Received message: " + completeMessage);

            _buffer = Encoding.ASCII.GetBytes("HELO you!");
            StreamToProcess.Write(_buffer, 0, _buffer.Length);
            StreamToProcess.Close();
        }
    }
}
