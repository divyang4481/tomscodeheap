namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    internal abstract class Command : ICommand
    {
        #region Constants and Fields

        protected ReplyCodes _replyCodes = new ReplyCodes();

        #endregion

        #region Public Properties

        public string Request { get; set; }

        public string Response { get; set; }

        #endregion

        #region Public Methods

        public abstract void Execute();

        public virtual bool MoreMessagesExpected()
        {
            return false;
        }

        #endregion
    }
}