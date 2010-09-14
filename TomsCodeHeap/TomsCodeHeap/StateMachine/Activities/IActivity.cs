// ========================================================================
// File:     IActivity.cs
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
	/// An <see cref="IActivity"/> contains / implements the logic either an entry, do or exit action of an <see cref="CH.Froorider.Codeheap.StateMachine.States.IState"/>
	/// should execute.
	/// </summary>
	public interface IActivity
	{
		/// <summary>
		/// Gets the name (identifier) of this <see cref="IActivity"/>.
		/// </summary>
		/// <value>The name (identifier) as a string.</value>
		string Name { get; }

		/// <summary>
		/// Performs the logic this <see cref="IActivity"/> defines / implements.
		/// </summary>
		void PerformActivity();
	}
}
