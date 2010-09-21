// ========================================================================
// File:     State.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using CH.Froorider.Codeheap.StateMachine.Transitions;
using CH.Froorider.Codeheap.StateMachine.Activities;
using CH.Froorider.Codeheap.StateMachine.Events;

namespace CH.Froorider.Codeheap.StateMachine.States
{
	/// <summary>
	/// Abstract base class for all states.
	/// </summary>
	internal abstract class State : IState
	{
		#region fields

		private static ILog logger = LogManager.GetLogger(typeof(State));
		private readonly string name = String.Empty;

		protected Dictionary<string, ITransition> transitions = new Dictionary<string, ITransition>();
		protected Dictionary<ActivityType, IActivity> activities = new Dictionary<ActivityType, IActivity>();

		#endregion

		#region Constructor

		/// <summary>
		/// Prevents an instance of the <see cref="State"/> class being created from outside.
		/// </summary>
		private State()
		{
			this.name = this.GetType().FullName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleState"/> class.
		/// </summary>
		/// <param name="name">The name of the <see cref="IState"/>.</param>
		/// <exception cref="ArgumentException">Is thrown when <paramref name="name"/> is null or empty.</exception>
		protected State(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("The name must contain at least one character.", "name");
			}

			this.name = name;
		}

		#endregion

		#region Basic implementation of IState

		/// <summary>
		/// Gets the name of this <see cref="IState"/>.
		/// </summary>
		/// <value>The name of the <see cref="IState"/> as a string.</value>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Determines whether this instance is an <see cref="IState"/> of an <see cref="IState"/>.
		/// </summary>
		/// <returns>
		/// 	<see langword="true"/> if [is submachine state]; otherwise, <see langword="false"/>.
		/// </returns>
		public virtual bool IsSubmachineState()
		{
			return false;
		}

		/// <summary>
		/// Determines whether this instance is simple.
		/// </summary>
		/// <returns>
		/// 	<see langword="true"/> if this instance is simple; otherwise, <see langword="false"/>.
		/// </returns>
		public virtual bool IsSimple()
		{
			return true;
		}

		/// <summary>
		/// Determines whether this instance is orthogonal.
		/// </summary>
		/// <returns>
		/// 	<see langword="true"/> if this instance is orthogonal; otherwise, <see langword="false"/>.
		/// </returns>
		public virtual bool IsOrthogonal()
		{
			return false;
		}

		/// <summary>
		/// Determines whether this instance is composite.
		/// </summary>
		/// <returns>
		/// 	<see langword="true"/> if this instance is composite; otherwise, <see langword="false"/>.
		/// </returns>
		public virtual bool IsComposite()
		{
			return false;
		}

		#endregion

		#region common functionality for all type of states

		/// <summary>
		/// Determines whether this <see cref="IState"/> has a <see cref="ITransition"/> for the given trigger name.
		/// </summary>
		/// <param name="triggerName">Name of the trigger as a string.</param>
		/// <returns>
		/// Returns <see langword="true"/> if it has transition for trigger with the specified trigger name; otherwise, <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentException">Is thrown when <paramref name="triggerName"/> is a null or empty string.</exception>
		protected internal bool HasTransitionForTrigger(string triggerName)
		{
			if (String.IsNullOrEmpty(triggerName))
			{
				throw new ArgumentException("The trigger name must contain at least one character.", "triggerName");
			}

			return this.transitions.ContainsKey(triggerName);
		}

		/// <summary>
		/// Fires the trigger towards this <see cref="IState"/>.
		/// </summary>
		/// <param name="trigger">The trigger as an instance of <see cref="EventMessage"/>.</param>
		/// <returns>
		/// The reference to the <see cref="IState"/> which has been reached after the transition.
		/// </returns>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="trigger"/> is a <see langword="null"/> reference.</exception>
		protected internal virtual IState ProcessTrigger(EventMessage trigger)
		{
			if (trigger == null)
			{
				throw new ArgumentNullException("trigger");
			}

			this.transitions[trigger.Name].PerformTransition(trigger);
			return this.transitions[trigger.Name].ToState;
		}

		#endregion

		#region abstract blueprint for all types of states

		/// <summary>
		/// Performs the entry <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
		/// </summary>
		protected internal abstract void PerformEntryActivity();

		/// <summary>
		/// Performs the do <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
		/// </summary>
		protected internal abstract void PerformDoActivity();

		/// <summary>
		/// Performs the exit <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
		/// </summary>
		protected internal abstract void PerformExitActivity();

		#endregion
	}
}
