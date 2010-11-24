// ========================================================================
// File:     Vertex.cs
// 
// Author:   $Author$
// Date:     $LastChangeDate$
// Revision: $Revision$
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
using CH.Froorider.Codeheap.StateMachine.States;
using CH.Froorider.Codeheap.StateMachine.Transitions;

namespace CH.Froorider.Codeheap.StateMachine
{
    /// <summary>
    /// A vertext is an abstaraction of a node in a state machine graph.
    /// In can be the source or destination of any number of transitions.
    /// </summary>
    public class Vertex
    {
        #region fields

        private List<ITransition> outgoing = new List<ITransition>();
        private List<ITransition> incoming = new List<ITransition>();
        private Region container;

        #endregion

        #region properties

        /// <summary>
        /// Gets the outgoing transitions which are "bound" with this vertex.
        /// </summary>
        /// <value>A list of <see cref="ITransition"/>s.</value>
        public List<ITransition> Outgoing
        {
            get { return this.outgoing; }
        }

        /// <summary>
        /// Gets the incoming transitions which are "bound" with this vertex.
        /// </summary>
        /// <value>A list of <see cref="ITransition"/>s.</value>
        public List<ITransition> Incoming
        {
            get { return this.incoming; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        public Vertex()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex"/> class.
        /// </summary>
        /// <param name="container">The <see cref="Region"/> this node is associated with.</param>
        /// <exception cref="ArgumentNullException"><paramref name="container"/> is a <see langword="null"/> reference.</exception>
        public Vertex(Region container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            this.container = container;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Adds / links an incoming transition with this vertex.
        /// </summary>
        /// <param name="incomingTransition">An instance of <see cref="ITransition"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="incomingTransition"/> is a <see langword="null"/> reference.</exception>
        /// <exception cref="ArgumentException">Is thrown when the target of the <paramref name="incomingTransition"/> is not this vertex.</exception>
        public void AddIncomingTransition(ITransition incomingTransition)
        {
            if (incomingTransition == null)
            {
                throw new ArgumentNullException("incomingTransition");
            }

            if (incomingTransition.ToState != this)
            {
                throw new ArgumentException("This transition is not an incoming transition.", "incomingTransition");
            }

            this.incoming.Add(incomingTransition);
        }

        /// <summary>
        /// Adds / links an outgoing transition with this vertex.
        /// </summary>
        /// <param name="outgoingTransition">An instance of <see cref="ITransition"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="outgoingTransition"/> is a <see langword="null"/> reference.</exception>
        /// <exception cref="ArgumentException">Is thrown when the target of the <paramref name="outgoingTransition"/> is not this vertex.</exception>
        public void AddOutgoingTransition(ITransition outgoingTransition)
        {
            if (outgoingTransition == null)
            {
                throw new ArgumentNullException("outgoingTransition");
            }

            if (outgoingTransition.FromState != this)
            {
                throw new ArgumentException("This transition is not an outgoing transition.", "outgoingTransition");
            }

            this.outgoing.Add(outgoingTransition);
        }

        /// <summary>
        /// Gets the state machine this vertex belongs to.
        /// </summary>
        /// <returns>An instance of <see cref="IStateMachine"/>.</returns>
        public IStateMachine ContainingStateMachine()
        {
            if (this.container != null)
            {
                return this.container.ContainingStateMachine();
            }
            else
            {
                if (this is PseudoState)
                {
                    PseudoState pseudoState = this as PseudoState;
                    return pseudoState.ContainingStateMachine();
                }
                else
                {
                    ConnectionPointReference connectionPointReference = this as ConnectionPointReference;
                    return connectionPointReference.ContainingStateMachine();
                }
            }
        }

        #endregion
    }
}
