// ========================================================================
// File:     StateMachine.cs
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
using CH.Froorider.Codeheap.StateMachine.Events;
using CH.Froorider.Codeheap.StateMachine.States;
using log4net;

namespace CH.Froorider.Codeheap.StateMachine
{
	/// <summary>
	/// Base class implementation for an <see cref="IStateMachine"/> which is maintained by it's owner,
	/// and knows only simple <see cref="CH.Froorider.Codeheap.StateMachine.Transitions.ITransition"/>s.
	/// </summary>
	internal class StateMachine : IStateMachine
	{
		#region fields

		private static ILog logger = LogManager.GetLogger(typeof(StateMachine));
		private readonly string name = string.Empty;
		private readonly object owner;
		private List<State> states;
		private State actualState;

		/// <summary>
		/// Occurs when the <see cref="IState"/> has been changed.
		/// </summary>
		public event EventHandler<StateChangedEventArgs> StateChanged;

		#endregion

		#region Constructor

		/// <summary>
		/// Prevents a default instance of the <see cref="StateMachine"/> class from being created.
		/// </summary>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="owner"/> is a <see langword="null"/> reference.</exception>
		private StateMachine(object owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			this.owner = owner;
			this.name = this.GetType().FullName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StateMachine"/> class.
		/// </summary>
		/// <param name="owner">The owner of this <see cref="IStateMachine"/>.</param>
		/// <param name="name">The name of the <see cref="IStateMachine"/>.</param>
		/// <exception cref="ArgumentException">Is thrown when <paramref name="name"/> is null or empty.</exception>
		/// <exception cref="ArgumentNullException"> Is thrown when
		///		<para><paramref name="owner"/> is a <see langword="null"/> reference</para>
		///		<para>- or -</para>
		///		<para><paramref name="startState"/> is a <see langword="null"/> reference.</para>
		/// </exception>
		public StateMachine(object owner, string name)
			: this(owner)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Name must contain at least one character.", "name");
			}

			this.name = name;
			this.states = new List<State>();
		}

		#endregion

		#region IStateMachine implementation

		/// <summary>
		/// Gets the name this state machine identifies.
		/// </summary>
		/// <value>The name of the <see cref="IStateMachine"/> as a string.</value>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the acutal <see cref="IState"/>.
		/// </summary>
		/// <value>The actual state as an <see cref="IState"/>.</value>
		public IState ActualState
		{
			get
			{
				return this.actualState;
			}
		}

		/// <summary>
		/// Triggers the <see cref="IStateMachine"/> to handle the given <see cref="EventMessage"/> according to the actual <see cref="IState"/>.
		/// </summary>
		/// <param name="eventToHandle">The <see cref="EventMessage"/> to handle.</param>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="eventToHandle"/> is a <see langword="null"/> reference.</exception>
		public void HandleEvent(EventMessage eventToHandle)
		{
			if (eventToHandle == null)
			{
				throw new ArgumentNullException("eventToHandle");
			}

			logger.DebugFormat("Handling event named {0} on state {1}", eventToHandle.Name, this.actualState.Name);

			if (this.actualState.HasTransitionForTrigger(eventToHandle.Name))
			{
				IState target = this.actualState.ProcessTrigger(eventToHandle);
				if (target.Name != this.actualState.Name)
				{
					this.actualState = (State)target;
				}
			}
			else
			{
				logger.Info("No transitions defined for this trigger.");
			}
		}

		/// <summary>
		/// Adds an <see cref="IState"/> to this <see cref="IStateMachine"/>.
		/// </summary>
		/// <param name="state">The state to add.</param>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="state"/> is a <see langword="null"/> reference.</exception>
		public void AddState(IState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException("state");
			}

			this.states.Add(state as State);
		}

		/// <summary>
		/// Raises the <see cref="E:StateChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="StateChangedEventArgs"/> instance containing the event data.</param>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="e"/> is a <see langword="null"/> reference.</exception>
		protected virtual void OnStateChanged(StateChangedEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}

			EventHandler<StateChangedEventArgs> localHandler = this.StateChanged;

			if (localHandler != null)
			{
				localHandler(this, e);
			}
		}

		#endregion
	}
}
