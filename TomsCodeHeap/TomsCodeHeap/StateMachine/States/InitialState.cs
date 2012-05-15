// ========================================================================
// File:     InitialState.cs
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

using CH.Froorider.Codeheap.StateMachine.Activities;
using log4net;

namespace CH.Froorider.Codeheap.StateMachine.States
{
    /// <summary>
    /// Pseudo state representing the intial state of an <see cref="IStateMachine"/>.
    /// </summary>
    internal class InitialState : State
    {
        #region fields

        private static ILog logger = LogManager.GetLogger(typeof(InitialState));
        // private PseudoStateKind stateKind = PseudoStateKind.Initial;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InitialState"/> class.
        /// </summary>
        protected internal InitialState()
            : base("InitialState")
        {
        }

        #endregion

        #region State implementation

        /// <summary>
        /// Performs the entry <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
        /// </summary>
        protected internal override void PerformEntryActivity()
        {
            logger.Debug("Nothing to do here");
        }

        /// <summary>
        /// Performs the do <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
        /// </summary>
        protected internal override void PerformDoActivity()
        {
            if (this.activities.ContainsKey(ActivityType.Do))
            {
                this.activities[ActivityType.Do].PerformActivity();
            }
        }

        /// <summary>
        /// Performs the exit <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
        /// </summary>
        protected internal override void PerformExitActivity()
        {
            logger.Debug("Nothing to do here");
        }

        #endregion
    }
}
