// ========================================================================
// File:     ISchedule.cs
// 
// Author:   $Author$
// Date:     $LastChangedDate$
// Revision: $Revision$
// ========================================================================
// Copyright [2009] [$Author$]
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ch.froorider.codeheap.Threading
{
    /// <summary>
    /// This interface declares the offered functionality by a sheduled object.
    /// A scheduled object can be managed by a Scheduler. The client using this
    /// schedule defines when to be triggered.
    /// As trigger acts an AutoResetEvent. This signal can be used by the client to
    /// wake up.
    /// </summary>
    public interface ISchedule : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="IScheduler"/> that this <see cref="ISchedule"/> belongs to.
        /// </summary>
        IScheduler Owner
        {
            get;
        }

        /// <summary>
        /// Gets the period of this <see cref="ISchedule"/>.
        /// </summary>
        TimeSpan Period
        {
            get;
        }

        /// <summary>
        /// Gets the signal that a client of this <see cref="ISchedule"/>
        /// waits for to be signaled periodically.
        /// </summary>
        AutoResetEvent ScheduleSignal
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ISchedule"/> is currently active.
        /// </summary>
        /// <remarks>
        /// If you enable the Schedule, the NextDueTime is set to <see cref="DateTime.Now"/> + period.
        /// If you disable the Schedule, the NextDueTime is set to <see cref="DateTime.MaxValue"/>.
        /// </remarks>
        bool Enabled
        {
            get;

            set;
        }

        /// <summary>
        /// Gets the next time when <see cref="ScheduleSignal"/> will be <see cref="M:AutoResetEvent.Set"/>;
        /// or <see cref="DateTime.MaxValue"/> if <see cref="Enabled"/> is <see langword="false"/>.
        /// </summary>
        DateTime NextDueTime
        {
            get;
        }

        #endregion Properties
    }
}
