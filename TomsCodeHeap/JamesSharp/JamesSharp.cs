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

namespace CH.Froorider.JamesSharp
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using CH.Froorider.JamesSharp.Server;
    using CH.Froorider.JamesSharpContracts.Protocols;
    using log4net;
    using System.Configuration;
    using System.Collections.Generic;

    /// <summary>
    /// Main loop of the server. Starts the server and loads all extensions.
    /// </summary>
    public class JamesSharp
    {
        private static readonly string _defaultDirectoryPath = "C:\\Extensions";
        private static readonly ILog _logger = LogManager.GetLogger(typeof(JamesSharp));
        private CompositionContainer _container;

        [ImportMany(typeof(IProtocol))]
// ReSharper disable FieldCanBeMadeReadOnly.Local
        private IEnumerable<Lazy<IProtocol, IProtocolData>> _protocols;
// ReSharper restore FieldCanBeMadeReadOnly.Local
        private readonly IList<TcpServer> _servers = new List<TcpServer>();

        public JamesSharp(IEnumerable<Lazy<IProtocol, IProtocolData>> protocols)
        {
            _protocols = protocols;
        }

        public JamesSharp()
        {
        }

        public void StartUp()
        {
            _logger.Info("Booting JamesSharp");

            string extensionDirectory = ConfigurationManager.AppSettings["ExtensionsDirectory"];
            if (string.IsNullOrEmpty(extensionDirectory))
            {
                extensionDirectory = _defaultDirectoryPath;
            }
            _logger.Debug("Loading extensions from: " + extensionDirectory);

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(extensionDirectory));

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                _logger.Info("Filling the catalog.");
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }

            foreach (var protocol in _protocols)
            {
                TcpServer server = new TcpServer(protocol.Value, protocol.Metadata.PortNumber);
                _logger.Info("Starting " + protocol.Metadata.ProtocolName + " server.");
                server.Start();
                _servers.Add(server);
            }

            _logger.Info("Hit enter to continue...");
            Console.ReadLine();

            foreach (var server in _servers)
            {
                _logger.Info("Stopping server.");
                server.Stop();
            }

            _logger.Info("Hit enter to exit.");
            Console.ReadLine();

            _logger.Info("Shutting down JamesSharp");
        }
    }
}
