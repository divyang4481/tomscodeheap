namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    using System;

    internal class NoopCommand : Command
    {
        #region Public Methods

        public override void Execute()
        {
            Console.WriteLine("NOOP Command called");
            this.Response = this._replyCodes.GetMessageForCode(220);
        }

        public override bool MoreMessagesExpected()
        {
            return true;
        }

        #endregion
    }
}