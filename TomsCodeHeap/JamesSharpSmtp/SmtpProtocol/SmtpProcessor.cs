using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JamesSharpSmtp.SmtpProtocol.Commands;

namespace JamesSharpSmtp.SmtpProtocol
{
    /// <summary>
    /// Receiver class for the requests. Can process and execute SMTP requests (or command strings if you like more)
    /// </summary>
    internal class SmtpProcessor
    {
        private Dictionary<SmtpVerbs, ICommand> commandMap = new Dictionary<SmtpVerbs, ICommand>();
        private ReplyCodes _replyCodes = new ReplyCodes();

        internal SmtpProcessor()
        {
            var heloCommand = new HeloCommand();
            commandMap.Add(SmtpVerbs.HELO, new HeloCommand());
            commandMap.Add(SmtpVerbs.EHLO, new HeloCommand());
            commandMap.Add(SmtpVerbs.QUIT, new QuitCommand());
            commandMap.Add(SmtpVerbs.NOOP, new NoopCommand());
        }

        public string ProcessMessage(string message)
        {
            string response = _replyCodes.GetMessageForCode(502);

            foreach (SmtpVerbs verb in Enum.GetValues(typeof(SmtpVerbs)))
            {
                var verbName = Enum.GetName(typeof(SmtpVerbs), verb);
                if (message.Contains(verbName))
                {
                    Console.WriteLine(string.Format("Found {0} in message {1}", verbName, message));
                    var command = commandMap.FirstOrDefault(x => x.Key == verb).Value;
                    if (command != null)
                    {
                        command.Execute();
                        CanSessionBeEnded = !command.MoreMessagesExpected();
                        return command.Response;
                    }
                }
            }

            return response;
        }

        public bool CanSessionBeEnded { internal get; set; }
    }
}
