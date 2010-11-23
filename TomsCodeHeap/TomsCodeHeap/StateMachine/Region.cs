// ========================================================================
// File:     Region.cs
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
    /// A region is an orthogonal part of either a composite state or a state machine. It contains states and transitions.
    /// </summary>
    public class Region
    {
        #region fields

        private IStateMachine stateMachineContext;
        private IState stateContext;
        private List<IState> states = new List<IState>();
        private List<ITransition> transitions = new List<ITransition>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="context">The <see cref="IStateMachine"/> this region belongs to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is a <see langword="null"/> reference.</exception>
        /// <exception cref="ArgumentException">Is thrown when this region is already associated.</exception>
        public Region(IStateMachine context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (stateContext != null)
            {
                throw new ArgumentException("This region is already associated to a state.", "context");
            }

            this.stateMachineContext = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="context">The <see cref="IState"/> this region belongs to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is a <see langword="null"/> reference.</exception>
        /// <exception cref="ArgumentException">Is thrown when this region is already associated.</exception>
        public Region(IState context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (stateMachineContext != null)
            {
                throw new ArgumentException("This region is already associated to a state machine.", "context");
            }

            this.stateContext = context;
        }

        #endregion
    }
}
