using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    internal class HeloCommand : ICommand
    {
        #region ICommand Members

        public void Execute(string data)
        {
            Console.WriteLine(data);
        }

        #endregion
    }
}
