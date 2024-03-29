﻿// ========================================================================
// File:     TransistionKind.cs
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

namespace CH.Froorider.Codeheap.StateMachine.Transitions
{
    /// <summary>
    /// Enumerates the kinds of pseudo <see cref="CH.Froorider.Codeheap.StateMachine.States.IState"/>'s.
    /// </summary>
    public enum TransitionKind
    {
        /// <summary>
        /// Default value.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Transition is an internal transistion.
        /// </summary>
        Internal = 1,

        /// <summary>
        /// Transistion is a local transition.
        /// </summary>
        Local = 2,

        /// <summary>
        /// Transition is an external transition.
        /// </summary>
        External = 3
    }
}