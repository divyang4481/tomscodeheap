using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    abstract class Command : ICommand
    {
        private string _request;
        private string _response;
        protected ReplyCodes _replyCodes = new ReplyCodes();
        #region ICommand Members

        public string Request
        {
            get
            {
                return _request;
            }
            set
            {
                _request = value;
            }
        }

        public string Response
        {
            get { return _response; }
            set
            {
                _response = value;
            }
        }

        public virtual bool MoreMessagesExpected()
        {
            return false;
        }

        #endregion

        public abstract void Execute();
    }
}
