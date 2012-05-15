// ========================================================================
// File:     StateChangedEventArgs.cs
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
using CH.Froorider.Codeheap.StateMachine.States;

namespace CH.Froorider.Codeheap.StateMachine.Events
{
    /// <summary>
    /// Basic event to inform observers of the <see cref="CH.Froorider.Codeheap.StateMachine.IStateMachine"/> that
    /// it's <see cref="CH.Froorider.Codeheap.StateMachine.States.IState"/> has been changed.
    /// </summary>
    public class StateChangedEventArgs : EventArgs
    {
        #region Fields

        private IState fromState;
        private IState toState;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the <see cref="IState"/> where the transition has been started.
        /// </summary>
        /// <value>The name of the <see cref="IState"/> as a string.</value>
        public string FromState
        {
            get
            {
                return this.fromState.Name;
            }
        }

        /// <summary>
        ///  Gets the name of the <see cref="IState"/> where the transition has been endend.
        /// </summary>
        /// <value>The name of the <see cref="IState"/> as a string.</value>
        public string ToState
        {
            get
            {
                return this.toState.Name;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="fromState">The <see cref="IState"/> in which the <see cref="IStateMachine"/> was before.</param>
        /// <param name="toState">The <see cref="IState"/> in which the <see cref="IStateMachine"/> is now.</param>
        /// <exception cref="ArgumentNullException"> Is thrown when
        ///		<para><paramref name="fromState"/> is a <see langword="null"/> reference</para>
        ///		<para>- or -</para>
        ///		<para><paramref name="toState"/> is a <see langword="null"/> reference.</para>
        /// </exception>
        public StateChangedEventArgs(IState fromState, IState toState)
        {
            if (fromState == null)
            {
                throw new ArgumentNullException("fromState");
            }

            if (toState == null)
            {
                throw new ArgumentNullException("toState");
            }

            this.fromState = fromState;
            this.toState = toState;
        }

        #endregion
    }
}
