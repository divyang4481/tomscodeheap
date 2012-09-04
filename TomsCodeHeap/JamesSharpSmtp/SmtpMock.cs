using CH.Froorider.JamesSharpContracts.Protocols;
using JamesSharpSmtp.SmtpProtocol;
using System;
using System.ComponentModel.Composition;
using System.Text;

namespace JamesSharpSmtp
{
    [Export(typeof(IProtocol))]
    [ExportMetadata("PortNumber", 25)]
    [ExportMetadata("ProtocolName", "SMTP")]
    public class SmtpMock : BaseProtocol
    {
        private readonly byte[] _buffer = new byte[1024];
        private readonly ReplyCodes _codeTable = new ReplyCodes();

        public override ProtocolType TypeOfProtocol
        {
            get
            {
                return SmtpProtocolType.SmtpProtocol;
            }
        }

        public override void ProcessConnection(object message)
        {
            SmtpProcessor processor = new SmtpProcessor();
            Console.WriteLine("James Sharp SMTP Server is processing message.");

            WriteToStream(_codeTable.GetMessageForCode(220));

            do
            {
                string request = this.ReadFromStream();
                Console.WriteLine("Recieved command: " + request);
                string response = processor.ProcessMessage(request);
                WriteToStream(response);
            }
            while (!processor.CanSessionBeEnded);

            StreamToProcess.Close();
            Console.WriteLine("Closed channel.");
        }

        internal void WriteToStream(string message)
        {
            try
            {
                byte[] encodedMessage = Encoding.ASCII.GetBytes(message);
                StreamToProcess.Write(encodedMessage, 0, encodedMessage.Length);
                StreamToProcess.Flush();
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("Could not write on the stream:");
                Console.WriteLine(e.Message);
            }
        }

        internal string ReadFromStream()
        {
            StringBuilder completeMessage = new StringBuilder();
            try
            {
                do
                {
                    _buffer.Initialize();
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
