namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    using System;

    internal class HeloCommand : Command
    {
        #region Public Methods

        public override void Execute()
        {
            Console.WriteLine("Helo Command called");
            this.Response = this._replyCodes.GetMessageForCode(250);
        }

        public override bool MoreMessagesExpected()
        {
            return true;
        }

        #endregion
    }
}