// ========================================================================
// File:     PseudoStateKind.cs
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
	/// Enumerates the kinds of pseudo <see cref="IState"/>'s.
	/// </summary>
	public enum PseudoStateKind
	{
		/// <summary>
		/// Default value.
		/// </summary>
		Undefined = 0,

		/// <summary>
		/// Initial pseudo state.
		/// </summary>
		Initial = 1,

		/// <summary>
		/// State contains the deep history.
		/// </summary>
		DeepHistory = 2,

		/// <summary>
		/// State contains the shallow history.
		/// </summary>
		ShallowHistory = 3,

		/// <summary>
		/// State is a container to maintain joined transition(s) in a graph.
		/// </summary>
		Join = 4,

		/// <summary>
		/// State is a container to maintain forked transition(s) in a graph.
		/// </summary>
		Fork = 5,

		/// <summary>
		/// State is a container to maintain a junction point in a graph.
		/// </summary>
		Junction = 6,

		/// <summary>
		/// State is a container to maintain a choice in a graph.
		/// </summary>
		Choice = 7,

		/// <summary>
		/// State represents the entry point in a graph.
		/// </summary>
		EntryPoint = 8,

		/// <summary>
		/// State represents the exit point in a graph.
		/// </summary>
		ExitPoint = 9,

		/// <summary>
		/// State represents the termination point of the graph.
		/// </summary>
		Terminate = 10
	}
}