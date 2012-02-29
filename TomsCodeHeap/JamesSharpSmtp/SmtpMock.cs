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
            using (StreamToProcess)
            {
                Console.WriteLine("James Sharp SMTP Server is processing message.");

                WriteToStream(_codeTable.GetMessageForCode(220));

                string command = this.ReadFromStream();
                Console.WriteLine("Recieved command: " + command);

                StreamToProcess.Close();
            }
        }

        private void WriteToStream(string message)
        {
            try
            {
                using (var writer = new StreamWriter(StreamToProcess))
                {
                    writer.WriteLine(message);
                }
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
                using (var reader = new StreamReader(StreamToProcess))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        completeMessage.Append(line);
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The stream could not be read:");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Received message: " + completeMessage);
            return completeMessage.ToString();
        }
    }
}
