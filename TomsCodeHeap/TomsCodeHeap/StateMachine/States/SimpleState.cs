// ========================================================================
// File:     SimpleState.cs
// 
// Author:   $Author$
// Date:     $LastChangedDate$
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
using CH.Froorider.Codeheap.StateMachine.Activities;
using CH.Froorider.Codeheap.StateMachine.Transitions;
using log4net;

namespace CH.Froorider.Codeheap.StateMachine.States
{
    /// <summary>
    /// Base implemenation of a very simple state. This kind of state contains no submachines.
    /// </summary>
    internal class SimpleState : State
    {
        #region fields

        private static ILog logger = LogManager.GetLogger(typeof(SimpleState));
        // private Dictionary<string, ITransition> transitions = new Dictionary<string, ITransition>();
        // private Dictionary<ActivityType, IActivity> activities = new Dictionary<ActivityType, IActivity>();

        #endregion

        #region Constructor

        /// <summary>
        /// Prevents a defualt instance of the <see cref="SimpleState"/> class being created from outside.
        /// </summary>
        private SimpleState()
            : base(typeof(SimpleState).Name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleState"/> class.
        /// </summary>
        /// <param name="name">The name of the <see cref="IState"/>.</param>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="name"/> is null or empty.</exception>
        protected SimpleState(string name)
            : base(name)
        {
        }

        #endregion

        #region IState implementation

        /// <summary>
        /// Adds a <see cref="ITransition"/> to this <see cref="IState"/>.
        /// </summary>
        /// <param name="transition">An instance of <see cref="ITransition"/>.</param>
        /// <exception cref="ArgumentNullException">Is thrown when <paramref name="transition"/> is a <see langword="null"/> reference.</exception>
        internal void AddTransition(ITransition transition)
        {
            if (transition == null)
            {
                throw new ArgumentNullException("transition");
            }

            this.transitions.Add(transition.EventTrigger.Name, transition);
        }

        /// <summary>
        /// Adds an <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> to this state. The <see cref="ActivityType"/> defines if
        /// it should be added as an enty,exit or do activtiy.
        /// </summary>
        /// <param name="activity">The activity as an instance of <see cref="IActivity"/>.</param>
        /// <param name="typeOfActivity">One of the values of <see cref="ActivityType"/>.</param>
        /// <exception cref="ArgumentException">Is thrown when this <see cref="IState"/> already has an <see cref="IActivity"/> of type <see cref="ActivityType"/>.</exception>
        /// <exception cref="ArgumentNullException">Is thrown when <paramref name="activity"/> is a <see langword="null"/> reference.</exception>
        internal void AddActivity(IActivity activity, ActivityType typeOfActivity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException("activity");
            }

            switch (typeOfActivity)
            {
                case ActivityType.Entry:
                case ActivityType.Do:
                case ActivityType.Exit:
                    this.activities.Add(typeOfActivity, activity);
                    break;
                case ActivityType.Undefined:
                default:
                    throw new NotImplementedException("Activities of type " + typeOfActivity + " are not implemented.");
            }
        }

        #endregion

        #region SimpleState Functionality

        /// <summary>
        /// Performs the entry <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
        /// </summary>
        protected internal override void PerformEntryActivity()
        {
            if (this.activities.ContainsKey(ActivityType.Entry))
            {
                logger.InfoFormat("Performing entry activity of state: {0}", this.Name);
                this.activities[ActivityType.Entry].PerformActivity();
            }
        }

        /// <summary>
        /// Performs the do <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
        /// </summary>
        protected internal override void PerformDoActivity()
        {
            if (this.activities.ContainsKey(ActivityType.Do))
            {
                logger.InfoFormat("Performing do activity of state: {0}", this.Name);
                this.activities[ActivityType.Do].PerformActivity();
            }
        }

        /// <summary>
        /// Performs the exit <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
        /// </summary>
        protected internal override void PerformExitActivity()
        {
            if (this.activities.ContainsKey(ActivityType.Exit))
            {
                logger.InfoFormat("Performing exit activity of state: {0}", this.Name);
                this.activities[ActivityType.Exit].PerformActivity();
            }
        }

        #endregion
    }
}
