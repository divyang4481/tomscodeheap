using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    internal class NoopCommand : Command
    {
        public override void Execute()
        {
            Console.WriteLine("NOOP Command called");
            Response = _replyCodes.GetMessageForCode(220);
        }

        public override bool MoreMessagesExpected()
        {
            return true;
        }
    }
}
