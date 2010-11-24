// ========================================================================
// File:     ITransition.cs
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

using CH.Froorider.Codeheap.StateMachine.Events;
using CH.Froorider.Codeheap.StateMachine.States;

namespace CH.Froorider.Codeheap.StateMachine.Transitions
{
    /// <summary>
    /// A transition defines the path from one <see cref="IState"/> to another <see cref="IState"/>.
    /// It is triggered by a an <see cref="EventMessage"/>.
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// Gets the start of the Transition.
        /// </summary>
        /// <value>The <see cref="IState"/> where the transition starts.</value>
        IState FromState { get; }

        /// <summary>
        /// Gets the end of the Transition.
        /// </summary>
        /// <value>The <see cref="IState"/> where the transition ends.</value>
        IState ToState { get; }

        /// <summary>
        /// Gets the object which triggers this transition.
        /// </summary>
        /// <value>The trigger as an instance of an <see cref="EventMessage"/>.</value>
        EventMessage EventTrigger { get; }

        /// <summary>
        /// Gets the name (or identifier) of this transition.
        /// </summary>
        /// <value>The name of the transition as a string.</value>
        string Name { get; }

        /// <summary>
        /// Performs the <see cref="ITransition"/> between two <see cref="IState"/>s.
        /// </summary>
        /// <param name="triggerToHandle">The trigger to handle as an <see cref="EventMessage"/>.</param>
        void PerformTransition(EventMessage triggerToHandle);
    }
}
