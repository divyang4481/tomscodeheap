// ========================================================================
// File:     IStateMachine.cs
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
using CH.Froorider.Codeheap.StateMachine.Events;
using CH.Froorider.Codeheap.StateMachine.States;

namespace CH.Froorider.Codeheap.StateMachine
{
	/// <summary>
	/// Defines the functionality a state machine offers.
	/// According to the UML definition an <see cref="IStateMachine"/> represents the state chart diagram type.
	/// </summary>
	public interface IStateMachine
	{
		/// <summary>
		/// Gets the name this state machine identifies.
		/// </summary>
		/// <value>The name of the <see cref="IStateMachine"/> as a string.</value>
		string Name { get; }

		/// <summary>
		/// Adds an <see cref="IState"/> to this <see cref="IStateMachine"/>.
		/// </summary>
		/// <param name="state">The state to add.</param>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="state"/> is a <see langword="null"/> reference.</exception>
		void AddState(IState state);

		/// <summary>
		/// Triggers the <see cref="IStateMachine"/> to handle the given <see cref="CH.Froorider.Codeheap.StateMachine.Events.EventMessage"/> according to the actual <see cref="IState"/>.
		/// </summary>
		/// <param name="eventToHandle">The <see cref="EventMessage"/> to handle.</param>
		/// <exception cref="ArgumentNullException">Is thrown when <paramref name="eventToHandle"/> is a <see langword="null"/> reference.</exception>
		void HandleEvent(EventMessage eventToHandle);

		/// <summary>
		/// Occurs when the actual <see cref="IState"/> has been changed.
		/// </summary>
		event EventHandler<StateChangedEventArgs> StateChanged;
	}
}
