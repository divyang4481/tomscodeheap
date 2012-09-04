using System;
using System.Collections.Generic;

namespace JamesSharpSmtp.SmtpProtocol
{
    using System.Globalization;

    internal class ReplyCodes
    {
        private readonly Dictionary<int, string> _replyCodes = new Dictionary<int, string>();

        internal ReplyCodes()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            _replyCodes.Add(211, "System status, or system help reply");
            _replyCodes.Add(214, "Help message [this reply is useful only to the human user]");
            _replyCodes.Add(220, "<domain> Service ready");
            _replyCodes.Add(221, "<domain> Service closing transmission channel");
            _replyCodes.Add(250, "Requested mail action okay, completed");
            _replyCodes.Add(251, "User not local; will forward to <forward-path>");
            _replyCodes.Add(252, "Cannot VRFY user, but will accept message and attempt delivery");
            _replyCodes.Add(354, "Start mail input; end with <CRLF>.<CRLF>");
            _replyCodes.Add(421, "<domain> Service not available, closing transmission channel");
            _replyCodes.Add(450, "Requested mail action not taken: mailbox unavailable");
            _replyCodes.Add(451, "Requested action aborted: local error in processing");
            _replyCodes.Add(452, "Requested action not taken: insufficient system storage");
            _replyCodes.Add(500, "Syntax error, command unrecognized");
            _replyCodes.Add(501, "Syntax error in parameters or arguments");
            _replyCodes.Add(502, "Command not implemented");
            _replyCodes.Add(503, "Bad sequence of commands");
            _replyCodes.Add(504, "Command parameter not implemented");
            _replyCodes.Add(550, "Requested action not taken: mailbox unavailable");
            _replyCodes.Add(551, "User not local; please try <forward-path>");
            _replyCodes.Add(552, "Requested mail action aborted: exceeded storage allocation");
            _replyCodes.Add(553, "Requested action not taken: mailbox name not allowed");
            _replyCodes.Add(554, "Transaction failed");
        }

        internal string GetMessageForCode(int code)
        {
            return String.Format("{0} {1}", code.ToString(CultureInfo.InvariantCulture), _replyCodes[code]);
        }
    }
}
