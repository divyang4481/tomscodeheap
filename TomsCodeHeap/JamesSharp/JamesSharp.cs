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

    /// <summary>
    /// Main loop of the server. Starts the server and loads all extensions.
    /// </summary>
    public class JamesSharp
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(JamesSharp));
        private CompositionContainer _container;

        [Import(typeof(IProtocol))]
        private IProtocol _protocol;

        public void StartUp()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IProtocol).Assembly));

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


            TcpServer server = new TcpServer(_protocol, 25);
            _logger.Info("Starting server");
            server.Start();
            _logger.Info("\nHit enter to continue...");
            Console.ReadLine();
            _logger.Info("Stopping server.");
            server.Stop();
            _logger.Info("Hit enter to exit.");
            Console.ReadLine();
        }
    }
}
