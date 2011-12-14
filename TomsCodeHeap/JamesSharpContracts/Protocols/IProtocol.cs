namespace CH.Froorider.JamesSharpContracts.Protocols
{
    using System.Net.Sockets;

    /// <summary>
    /// Defines the base things you can do with a protocol.
    /// Protocols are exported, so they can be imported by MEF and added to the servers functionality.
    /// </summary>
    public interface IProtocol
    {
        NetworkStream StreamToProcess { get; set; }

        ProtocolType TypeOfProtocol { get; }

        void ProcessConnection(object message);
    }
}
