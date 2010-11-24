// ========================================================================
// File:     ActivityType.cs
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

namespace CH.Froorider.Codeheap.StateMachine.Activities
{
    /// <summary>
    /// Defines if the <see cref="IActivity"/> is used in an <see cref="CH.Froorider.Codeheap.StateMachine.States.IState"/> 
    /// either as an entry, exit, or do action.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Action is an entry action of an <see cref="CH.Froorider.Codeheap.StateMachine.States.IState"/>.
        /// </summary>
        Entry = 1,

        /// <summary>
        /// Action is an do action of an <see cref="CH.Froorider.Codeheap.StateMachine.States.IState"/>.
        /// </summary>
        Do = 2,

        /// <summary>
        /// Action is an exit action of an <see cref="CH.Froorider.Codeheap.StateMachine.States.IState"/>.
        /// </summary>
        Exit = 3
    }
}
