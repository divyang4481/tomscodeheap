using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    internal class QuitCommand : Command
    {
        public override void Execute()
        {
            Console.WriteLine("Quit Command called");
            Response = _replyCodes.GetMessageForCode(221);
        }

        public override bool MoreMessagesExpected()
        {
            return false;
        }
    }
}
