// ========================================================================
// File:     Shell.cs
// 
// Author:   
// Date:     $LastChangedDate$
// Revision: $Revision$
// ========================================================================
// Copyright [2012] [$Author$]
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

namespace CH.Froorider.Codeheap.Shell
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;

    using log4net;

    /// <summary>
    /// Simple POSIX shell implementation
    /// </summary>
    public class Shell
    {
        #region fields
        
        private static readonly string defaultDirectoryPath = "C:\\Extensions";
        private static readonly ILog logger = LogManager.GetLogger(typeof(Shell));

        private string prompt = "> ";
        private string exitKeyword;
        private CompositionContainer container;

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        [ImportMany(typeof(ICommand))]
        private IEnumerable<Lazy<ICommand, ICommandMetadata>> commands;
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        #endregion

        #region Properties

        public string ExitKeyword
        {
            get
            {
                return exitKeyword;
            }
            set
            {
                exitKeyword = value ?? "Exit";
            }
        }

        public string Prompt
        {
            get
            {
                return prompt;
            }
            set
            {
                prompt = value ?? "> ";
            }
        }

        public ConsoleColor? PromptBackColor
        {
            get;
            set;
        }

        public ConsoleColor? PromptForeColor
        {
            get;
            set;
        }

        public ConsoleColor? BackColor
        {
            get;
            set;
        }

        public ConsoleColor? ForeColor
        {
            get;
            set;
        }

        public bool ClearScreenOnStart
        {
            get;
            set;
        }

        public bool ClearScreenOnEnd
        {
            get;
            set;
        }

        public bool SpaceBeforeCommand
        {
            get;
            set;
        }

        public bool SpaceAfterCommand
        {
            get;
            set;
        }

        #endregion

        #region methods
        
        public void Init()
        {
            string extensionDirectory = ConfigurationManager.AppSettings["ExtensionsDirectory"];
            if (string.IsNullOrEmpty(extensionDirectory))
            {
                extensionDirectory = defaultDirectoryPath;
            }
            logger.Debug("Loading extensions from: " + extensionDirectory);

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(extensionDirectory));

            //Create the CompositionContainer with the parts in the catalog
            container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                logger.Info("Filling the catalog.");
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public void ProcessCommands()
        {
            string input = ShellPrompt();
            while (!input.Equals(ExitKeyword, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    input = ShellPrompt();
                    continue;
                }

                foreach (var command in commands)
                {
                    Console.WriteLine("Available command: " + command.Metadata.Name + " " + command.Metadata.Description);
                }

                input = this.ShellPrompt();
            }
        }

        private string ShellPrompt()
        {
            this.Write(PromptForeColor, PromptBackColor, Prompt);

            if (ForeColor.HasValue)
                Console.ForegroundColor = ForeColor.Value;
            if (BackColor.HasValue)
                Console.BackgroundColor = BackColor.Value;
            return Console.ReadLine();
        }

        private void Write(ConsoleColor? foreColor, ConsoleColor? backColor, string text, params object[] args)
        {
            if (foreColor.HasValue)
                Console.ForegroundColor = foreColor.Value;
            if (backColor.HasValue)
                Console.BackgroundColor = backColor.Value;
            Console.Write(text, args);
            Console.ResetColor();
        }

        private void WriteLine(ConsoleColor? foreColor, ConsoleColor? backColor, string text, params object[] args)
        {
            Write(foreColor, backColor, text, args);
            Console.WriteLine();
        }

        private void WriteBlankLine()
        {
            Console.WriteLine();
        }

        #endregion
    }
}
