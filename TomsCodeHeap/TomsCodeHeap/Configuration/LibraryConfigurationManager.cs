// ========================================================================
// File:     LibraryConfigurationManager.cs
//
// Author:   $Author$
// Date:     
// Revision: $Revision$
// ========================================================================
// Copyright [2009] [$Author$]
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using log4net;

namespace Ch.Froorider.Codeheap.Configuration
{
    /// <summary>
    /// Class loads the configuration for a library out of an XML Document.
    /// The settings are made available for the application. This makes the use of an
    /// central .config file useless.
    /// </summary>
    /// <remarks>
    /// Storing of values is not supported yet.
    /// </remarks>
    public static class LibraryConfigurationManager
    {
        /// <summary>
        /// Local used logger
        /// </summary>
        private static ILog logger = LogManager.GetLogger(typeof(LibraryConfigurationManager));

        /// <summary>
        /// Collection holds the key and values read out of the config file.
        /// </summary>
        private static NameValueCollection loadedKeys = new NameValueCollection();

        /// <summary>
        /// Gets a collection of all loaded key,values of the config file.
        /// </summary>
        public static NameValueCollection LoadedKeys
        {
            get { return loadedKeys; }
        }

        /// <summary>
        /// Loads the configuration out of the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <exception cref="ArgumentNullException">Is thrown when the file not exists or is not valid.</exception>
        public static void Load(string filename)
        {
            // An already loaded configuration is not overwritten
            if (loadedKeys.HasKeys())
            {
                return;
            }

            if (String.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("Filename is null or empty.");
            }

            // Get the application folder.
            string codebase = Assembly.GetExecutingAssembly().GetName().CodeBase;
            codebase = codebase.Replace(@"file:///", string.Empty);
            string applicationFolder = Path.GetDirectoryName(codebase);

            // Get the complete path to the config file. 
            string configFile = Path.Combine(applicationFolder, filename);
            if (!File.Exists(@configFile))
            {
                logger.Error("Path to config file is not valid: " + configFile);
                throw new ArgumentException("Specified config file does not exist.");
            }

            // Now we are ready to do the main work
            logger.Debug("Loading config file into XDocument");
            XElement document = XElement.Load(XmlReader.Create(configFile), LoadOptions.None);
            logger.Info("Loaded document: " + document.ToString());
            IEnumerable<XElement> elements = from element in document.Elements("appSettings") select element;
            foreach (XElement currentElement in elements)
            {
                logger.Debug("Parsing element: " + currentElement.ToString());
                IEnumerable<XElement> settings = from addEntry in currentElement.Elements("add") select addEntry;
                foreach (XElement currentSetting in settings)
                {
                    logger.Debug("Parsing element: " + currentSetting.ToString());
                    IEnumerable<XAttribute> attributes = currentSetting.Attributes();
                    string keyName = string.Empty;
                    string keyValue = string.Empty;

                    foreach (XAttribute currentAttribute in attributes)
                    {
                        logger.Debug("Parsing Attribute: " + currentAttribute.Name);
                        logger.Debug("Attribute value: " + currentAttribute.Value);
                        if (currentAttribute.Name.Equals((XName)"key"))
                        {
                            keyName = currentAttribute.Value;
                        }

                        if (currentAttribute.Name.Equals((XName)"value"))
                        {
                            keyValue = currentAttribute.Value;
                        }
                    }

                    loadedKeys.Add(keyName, keyValue);
                }
            }
        }
    }
}
