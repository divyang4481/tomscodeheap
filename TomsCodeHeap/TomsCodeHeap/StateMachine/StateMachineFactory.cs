// ========================================================================
// File:     StateMachineFactory.cs
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

namespace CH.Froorider.Codeheap.StateMachine
{
    /// <summary>
    /// Factory class to generate the desired <see cref="IStateMachine"/>.
    /// </summary>
    public static class StateMachineFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IStateMachine"/>.
        /// </summary>
        /// <param name="owner">The owner of this created <see cref="IStateMachine"/>.</param>
        /// <param name="startState">The <see cref="IState"/> which should be reached after initialization.</param>
        /// <param name="name">The name of the <see cref="IStateMachine"/>.</param>
        /// <returns>An instance of <see cref="IStateMachine"/>.</returns>
        /// <exception cref="ArgumentNullException"> Is thrown when
        ///		<para><paramref name="owner"/> is a <see langword="null"/> reference</para>
        ///		<para>- or -</para>
        ///		<para><paramref name="startState"/> is a <see langword="null"/> reference.</para>
        /// </exception>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="name"/> is null or empty.</exception>
        public static IStateMachine CreateStateMachine(object owner, IState startState, string name)
        {
            StateMachine machine = new StateMachine(owner, name);
            machine.AddState(startState as State);
            return machine;
        }
    }
}
