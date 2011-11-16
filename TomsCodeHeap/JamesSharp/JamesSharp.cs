// ========================================================================
// File:     JamesSharp.cs
// 
// Author:   $Author$
// Date:     $LastChangeDate$
// Revision: $Revision$
// ========================================================================
// Copyright [2011] [$Author$]
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ========================================================================

using System;
using System.ServiceModel;
using CH.Froorider.JamesSharpContracts.Protocols;
using log4net;

namespace CH.Froorider.JamesSharp
{
    using System.ServiceModel.Description;

    /// <summary>
    /// Main loop of the server. Starts the server and loads all extensions.
    /// </summary>
    public class JamesSharp
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(JamesSharp));

        public void StartUp()
        {
            _logger.Info("Starting James Sharp");
            ServiceHost host;
            using (host = new ServiceHost(typeof(BaseProtocol), new Uri("net.tcp://localhost:25")))
            {
                ServiceMetadataBehavior smb = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (smb == null)
                {
                    host.Description.Behaviors.Add(new ServiceMetadataBehavior());
                }

                // Add MEX endpoint
                host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

                // Add application endpoint
                NetTcpBinding tcpBinding = new NetTcpBinding();
                tcpBinding.Security.Mode = SecurityMode.None;
                //WSHttpBinding httpBinding = new WSHttpBinding { Security = new WSHttpSecurity { Mode = SecurityMode.None } };
                host.AddServiceEndpoint(typeof(IProtocol), tcpBinding, "");

                _logger.Info("Starting host on port 25");
                host.Open();
                foreach (var se in host.Description.Endpoints)
                    _logger.InfoFormat("Service started on A: {0}, B: {1}, C: {2}", se.Address, se.Binding.Name, se.Contract.Name);
            }
        }
    }
}
