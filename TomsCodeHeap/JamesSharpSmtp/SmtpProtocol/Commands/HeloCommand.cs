using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    internal class HeloCommand : Command
    {

        #region ICommand Members

        public override void Execute()
        {
            Console.WriteLine("Helo Command called");
            Response = _replyCodes.GetMessageForCode(250);
        }

        public override bool MoreMessagesExpected()
        {
            return true;
        }

        #endregion
    }
}
