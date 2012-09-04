namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    using System;

    internal class QuitCommand : Command
    {
        #region Public Methods

        public override void Execute()
        {
            Console.WriteLine("Quit Command called");
            this.Response = this._replyCodes.GetMessageForCode(221);
        }

        public override bool MoreMessagesExpected()
        {
            return false;
        }

        #endregion
    }
}