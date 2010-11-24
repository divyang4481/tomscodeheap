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
        /// Determines whether this instance is composite.
        /// </summary>
        /// <returns>
        /// 	<see langword="true"/> if this instance is composite; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsComposite();

        /// <summary>
        /// Determines whether this instance is orthogonal.
        /// </summary>
        /// <returns>
        /// 	<see langword="true"/> if this instance is orthogonal; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsOrthogonal();

        /// <summary>
        /// Determines whether this instance is simple.
        /// </summary>
        /// <returns>
        /// 	<see langword="true"/> if this instance is simple; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsSimple();

        /// <summary>
        /// Determines whether this instance is an <see cref="IState"/> of an <see cref="IState"/>.
        /// </summary>
        /// <returns>
        /// 	<see langword="true"/> if [is submachine state]; otherwise, <see langword="false"/>.
        /// </returns>
        bool IsSubmachineState();

        /// <summary>
        /// Gets the state machine this state belongs to.
        /// </summary>
        /// <returns>An instance of <see cref="IStateMachine"/>.</returns>
        IStateMachine ContainingStateMachine();
    }
}
