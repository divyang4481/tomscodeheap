using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    /// <summary>
    /// Contract for an command implementation.
    /// </summary>
    interface ICommand
    {
        void Execute(string data);
    }
}
