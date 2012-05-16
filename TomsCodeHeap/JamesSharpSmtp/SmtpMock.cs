using CH.Froorider.JamesSharpContracts.Protocols;
using System.ComponentModel.Composition;
using System;
using JamesSharpSmtp.SmtpProtocol;
using System.Text;
using System.IO;

namespace JamesSharpSmtp
{
    [Export(typeof(IProtocol))]
    [ExportMetadata("PortNumber", 25)]
    [ExportMetadata("ProtocolName", "SMTP")]
    public class SmtpMock : BaseProtocol
    {
        private byte[] _buffer = new byte[1024];
        private ReplyCodes _codeTable = new ReplyCodes();

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

            WriteToStream(_codeTable.GetMessageForCode(220));

            string command = this.ReadFromStream();
            Console.WriteLine("Recieved command: " + command);

            WriteToStream(_codeTable.GetMessageForCode(221));
            StreamToProcess.Close();
        }

        private void WriteToStream(string message)
        {
            try
            {
                _buffer = Encoding.ASCII.GetBytes(message);
                StreamToProcess.Write(_buffer, 0, _buffer.Length);
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Could not write on the stream:");
                Console.WriteLine(e.Message);
            }
        }

        private string ReadFromStream()
        {
            StringBuilder completeMessage = new StringBuilder();
            try
            {
                do
                {
                    int numberOfBytesRead = StreamToProcess.Read(_buffer, 0, _buffer.Length);
                    completeMessage.AppendFormat("{0}", Encoding.ASCII.GetString(_buffer, 0, numberOfBytesRead));
                }
                while (StreamToProcess.DataAvailable);
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The stream could not be read:");
                Console.WriteLine(e.Message);
                return string.Empty;
            }

            Console.WriteLine("Received message: " + completeMessage);
            return completeMessage.ToString();
        }
    }
}
