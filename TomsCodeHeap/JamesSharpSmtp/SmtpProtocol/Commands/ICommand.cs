using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    /// <summary>
    /// Contract for an command implementation.
    /// </summary>
    internal interface ICommand
    {
        string Request { get; set; }
        string Response { get; set; }
        bool MoreMessagesExpected();
        void Execute();
    }
}
