﻿using System.ServiceModel;
namespace CH.Froorider.JamesSharpContracts.Protocols
{
    /// <summary>
    /// Defines the base things you can do with a protocol.
    /// Protocols are exported, so they can be imported by MEF and added to the servers functionality.
    /// </summary>
    [ServiceContract]
    public interface IProtocol
    {
        [OperationContract]
        void ProcessMessage();
    }
}