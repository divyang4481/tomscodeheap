namespace JamesSharpSmtp.SmtpProtocol.Commands
{
    /// <summary>
    /// Contract for an command implementation.
    /// </summary>
    internal interface ICommand
    {
        #region Public Properties

        string Request { get; set; }

        string Response { get; set; }

        #endregion

        #region Public Methods

        void Execute();

        bool MoreMessagesExpected();

        #endregion
    }
}