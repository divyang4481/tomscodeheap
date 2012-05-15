// ========================================================================
// File:     EventMessage.cs
// 
// Author:   $Author$
// Date:     $LastChangedDate: 2010-11-24 18:06:57 +0100 (Mi, 24 Nov 2010) $
// Revision: $Revision: 78 $
// ========================================================================
// Copyright [2010] [$Author$]
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
using System.Collections.ObjectModel;
using System.Linq;

namespace CH.Froorider.Codeheap.StateMachine.Events
{
    /// <summary>
    /// Base class for all event messages, which are used in a <see cref="IStateMachine"/>. 
    /// </summary>
    public class EventMessage
    {
        #region Fields

        private readonly string name = string.Empty;

        private List<object> parameters = new List<object>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of this <see cref="EventMessage"/>.
        /// </summary>
        /// <value>The name of this <see cref="EventMessage"/> as a string.</value>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the parameters associated with this <see cref="EventMessage"/>.
        /// </summary>
        /// <value>The parameters of this <see cref="EventMessage"/> if there are parameters. An empty list if there are none.</value>
        public ReadOnlyCollection<object> Parameters
        {
            get
            {
                ReadOnlyCollection<object> readOnlyCollection = new ReadOnlyCollection<object>(this.parameters);
                return readOnlyCollection;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessage"/> class.
        /// </summary>
        public EventMessage()
        {
            this.name = this.GetType().FullName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessage"/> class.
        /// </summary>
        /// <param name="name">The used name of this <see cref="EventMessage"/>.</param>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="name"/> is either null, empty or only white space.</exception>
        public EventMessage(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The name of the state must contain at least one character.", "name");
            }

            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessage"/> class.
        /// </summary>
        /// <param name="name">The used name of this <see cref="EventMessage"/>.</param>
        /// <param name="parameters">The parameters which should be associated with this event.</param>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="name"/> is either null, empty or only white space.</exception>
        /// <exception cref="ArgumentNullException">Is thrown when <paramref name="parameters"/> is a <see langword="null"/> reference.</exception>
        public EventMessage(string name, params object[] parameters)
            : this(name)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            this.parameters = parameters.ToList<object>();
        }

        #endregion
    }
}
