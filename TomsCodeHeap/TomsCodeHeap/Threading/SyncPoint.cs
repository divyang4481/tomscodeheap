//========================================================================
//File:     SyncPoint.cs
//
//Author:   $Author$
//Date:     
//Revision: $Revision$
//========================================================================
//Copyright [2009] [$Author$]
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//========================================================================

namespace ch.froorider.codeheap.Threading
{
    /// <summary>
    /// Defines the values a sync point can have. Default is SYNC_FREE
    /// </summary>
    internal enum SyncPoint
    {
        /// <summary>
        /// Timer has been stopped. Elapsed event are not "desired" anymore.
        /// </summary>
        TIMER_STOPPED = -1,

        /// <summary>
        /// No "lock" on the timer. Free for use.
        /// </summary>
        SYNC_FREE = 0,

        /// <summary>
        /// The elapsed event of the timer has been raised and is "performed". Wait until it is finished.
        /// </summary>
        ELAPSED_EVENT_RUNNING = 1
    };
}