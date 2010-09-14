// ========================================================================
// File:     IState.cs
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
using CH.Froorider.Codeheap.StateMachine.Events;
using CH.Froorider.Codeheap.StateMachine.Transitions;

namespace CH.Froorider.Codeheap.StateMachine.States
{
	/// <summary>
	/// An <see cref="IState"/> defines the functionality an owner of an <see cref="IStateMachine"/> can consume.
	/// A state is a condition during the life of it's owner, which satisfy some condition, performs some action
	/// or waits for some event.
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// Gets the name of this <see cref="IState"/>.
		/// </summary>
		/// <value>The name of the <see cref="IState"/> as a string.</value>
		string Name { get; }

		/// <summary>
		/// Adds a <see cref="ITransition"/> to this <see cref="IState"/>.
		/// </summary>
		/// <param name="transition">An instance of <see cref="ITransition"/>.</param>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="transition"/> is a <see langword="null"/> reference.</exception>
		void AddTransition(ITransition transition);

		/// <summary>
		/// Adds an <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> to this state. The <see cref="ActivityType"/> defines if
		/// it should be added as an enty,exit or do activtiy.
		/// </summary>
		/// <param name="activity">The activity as an instance of <see cref="IActivity"/>.</param>
		/// <param name="typeOfActivity">One of the values of <see cref="ActivityType"/>.</param>
		/// <exception cref="ArgumentException">Is thrown when this <see cref="IState"/> already has an <see cref="IActivity"/> of type <see cref="ActivityType"/>.</exception>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="activity"/> is a <see langword="null"/> reference.</exception>
		void AddActivity(IActivity activity, ActivityType typeOfActivity);

		/// <summary>
		/// Determines whether this <see cref="IState"/> has a <see cref="ITransition"/> for the given trigger name.
		/// </summary>
		/// <param name="triggerName">Name of the trigger as a string.</param>
		/// <returns>
		/// 	Returns <see langword="true"/> if it has transition for trigger with the specified trigger name; otherwise, <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentException">Is thrown when <paramref name="triggerName"/> is a null or empty string.</exception>
		bool HasTransitionForTrigger(string triggerName);

		/// <summary>
		/// Fires the trigger towards this <see cref="IState"/>.
		/// </summary>
		/// <param name="trigger">The trigger as an instance of <see cref="EventMessage"/>.</param>
		/// <returns>The reference to the <see cref="IState"/> which has been reached after the transition.</returns>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="trigger"/> is a <see langword="null"/> reference.</exception>
		IState ProcessTrigger(EventMessage trigger);

		/// <summary>
		/// Performs the entry <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
		/// </summary>
		void PerformEntryActivity();

		/// <summary>
		/// Performs the do <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
		/// </summary>
		void PerformDoActivity();

		/// <summary>
		/// Performs the exit <see cref="CH.Froorider.Codeheap.StateMachine.Activities.IActivity"/> of this <see cref="IState"/>.
		/// </summary>
		void PerformExitActivity();
	}
}
