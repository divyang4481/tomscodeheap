// ========================================================================
// File:     PseudoState.cs
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

namespace CH.Froorider.Codeheap.StateMachine.States
{
    /// <summary>
    /// A pseudo state is an abstraction that encopasses different types of transient vertices in the state machine graph.
    /// </summary>
    public class PseudoState : Vertex
    {
        #region fields

        private PseudoStateKind kind = PseudoStateKind.Initial;
        private IStateMachine statemachine;
        private IState state;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the kind of pseudo state.
        /// </summary>
        /// <value>A vlaue of <see cref="PseudoStateKind"/>.</value>
        public PseudoStateKind Kind
        {
            get { return this.kind; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoState"/> class.
        /// </summary>
        public PseudoState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoState"/> class.
        /// </summary>
        /// <param name="kind">The kind or type of Pseudo state.</param>
        /// <param name="context">The associated state machine to this pseudo state.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is a <see langword="null"/> reference.</exception>
        /// <exception cref="ArgumentException">Is thrown when the <paramref name="kind"/> is not of type Entry or Exit point.</exception>
        public PseudoState(PseudoStateKind kind, IStateMachine context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (kind != PseudoStateKind.EntryPoint || kind != PseudoStateKind.ExitPoint)
            {
                throw new ArgumentException("Kind of pseudo state must be of type EntryPoint or ExitPoint.");
            }

            this.kind = kind;
            this.statemachine = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoState"/> class.
        /// </summary>
        /// <param name="kind">The kind or type of Pseudo state.</param>
        /// <param name="context">The associated state owning this pseudo state.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is a <see langword="null"/> reference.</exception>
        public PseudoState(PseudoStateKind kind, IState context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.kind = kind;
            this.state = context;
        }

        #endregion
    }
}
