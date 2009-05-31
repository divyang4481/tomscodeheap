//========================================================================
//File:     Scheduler.cs
//
//Author:   $Author$
//Date:     $LastChangedDate$
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
          
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace ch.froorider.codeheap.Threading
{
    /// <summary>
    /// This class represents a scheduler. Cleints can register triggers at the scheduler which signals them
    /// in a defined time interval to wake up.
    /// The class is a singleton, so only one instance is running. The scheduler itself is using his own thread
    /// and does not need a hosting thread.
    /// </summary>
    public class Scheduler : IScheduler, IDisposable
    {
        #region fields

        /// <summary>
        /// The reference on the one and only instance of this class.
        /// </summary>
        private static volatile Scheduler instance;

        /// <summary>
        /// Used to synchronize the creation of the scheduler.
        /// </summary>
        private static object instanceLocker = new object();

        /// <summary>
        /// This is the internal bookkeeping of the triggers. Contains a reference on all registered triggers.
        /// </summary>
        /// <remarks>
        /// The list contains a reference on all created schedules and is periodically scanned to
        /// look if a signal has to be set.
        /// </remarks>
        private static volatile List<ISchedule> triggerList = new List<ISchedule>();

        private static System.Timers.Timer timer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class. (Default constructor)
        /// </summary>
        private Scheduler()
        {
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Elapsed += this.timer_Elapsed;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gives you access on the <see cref="IScheduler"/> representation of the Scheduler.
        /// </summary>
        /// <remarks>
        /// This singleton creator uses double-locking with explicit memory barries to ensure that
        /// this is thread-safe in a multi-threaded and multi-core environment. 
        /// </remarks>
        /// <returns>An <see cref="IScheduler"/>. </returns>
        public static IScheduler Instance()
        {
            if (Scheduler.instance == null)
            {
                lock (instanceLocker)
                {
                    if (Scheduler.instance == null)
                    {
                        Scheduler.instance = new Scheduler();
                    }
                }
            }
            return Scheduler.instance;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Handles the Elapsed event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IScheduler Members

        /// <summary>
        /// Adds a new <see cref="IScheduler"/> with a specified period.
        /// </summary>
        /// <param name="period">The period of the new <see cref="ISchedule"/>.</param>
        /// <param name="enable">If set to <see langword="true"/>, enables the new <see cref="ISchedule"/> immediately.</param>
        /// <returns>
        /// Returns the new <see cref="ISchedule"/>.
        /// </returns>
        public ISchedule Add(TimeSpan period, bool enable)
        {
            ISchedule trigger = new SignalSchedule(period,this);
            trigger.Enabled = enable;
            Scheduler.triggerList.Add(trigger);
            
            //Recalculation of the timer's period.
            if (!timer.Enabled)
            {
                timer.Interval = period.Ticks;
                timer.Enabled = true;
            }
            else
            {
                IEnumerable<ISchedule> orderedList = triggerList.OrderByDescending<ISchedule,long>(p => p.Period.Ticks);
                timer.Interval = SortedList  .Period.Ticks;
            }

            return trigger;
        }

        /// <summary>
        /// Removes and <see cref="IDisposable.Dispose"/>s the first occurrence of a specific <see cref="ISchedule"/>
        /// from this <see cref="IScheduler"/>.
        /// </summary>
        /// <param name="schedule">The <see cref="ISchedule"/> to remove from this <see cref="IScheduler"/>.</param>
        /// <returns>
        /// 	<see langword="true"/> if <paramref name="schedule"/> was successfully removed from this <see cref="IScheduler"/>; otherwise, <see langword="false"/>.
        /// This method also returns false if <paramref name="schedule"/> is not found in this <see cref="IScheduler"/>.
        /// </returns>
        public bool Remove(ISchedule schedule)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of <see cref="ISchedule"/>s owned by this <see cref="IScheduler"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of <see cref="ISchedule"/>s owned by this <see cref="IScheduler"/>
        /// </returns>
        public int Count
        {
            get { return Scheduler.triggerList.Count; }
        }

        /// <summary>
        /// Gets the <see cref="ISchedule"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <value>The <see cref="ISchedule"/> at the specified index.</value>
        public ISchedule this[int index]
        {
            get { return Scheduler.triggerList[index]; }
        }

        /// <summary>
        /// Determines whether this <see cref="IScheduler"/> contains a specific <see cref="ISchedule"/>.
        /// </summary>
        /// <param name="schedule">The <see cref="ISchedule"/> to locate in this <see cref="IScheduler"/>.</param>
        /// <returns>
        /// 	<see langword="true"/> if <paramref name="schedule"/> is found in this <see cref="IScheduler"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Contains(ISchedule schedule)
        {
            return Scheduler.triggerList.Exists(p => p.Equals(schedule));
        }

        /// <summary>
        /// Determines the index of a specific <see cref="ISchedule"/> in this <see cref="IScheduler"/>.
        /// </summary>
        /// <param name="schedule">The <see cref="ISchedule"/> to locate in this <see cref="IScheduler"/>.</param>
        /// <returns>
        /// The index of <paramref name="schedule"/> if found in this <see cref="IScheduler"/>; otherwise, -1.
        /// </returns>
        public int IndexOf(ISchedule schedule)
        {
            return Scheduler.triggerList.FindIndex(p => p.Equals(schedule));
        }

        /// <summary>
        /// Removes the <see cref="ISchedule"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="ISchedule"/> to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<ISchedule> Members

        /// <summary>
        /// Gibt einen Enumerator zurück, der die Auflistung durchläuft.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Collections.Generic.IEnumerator`1"/>, der zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        public IEnumerator<ISchedule> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gibt einen Enumerator zurück, der eine Auflistung durchläuft.
        /// </summary>
        /// <returns>
        /// Ein <see cref="T:System.Collections.IEnumerator"/>-Objekt, das zum Durchlaufen der Auflistung verwendet werden kann.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDisposable - implementation

        /// <summary>
        /// Disposes the unmanaged ressources
        /// </summary>
        /// <param name="disposing">Always true.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Dispose - pattern.
        /// Call base.Dispose() first if possible, then the Dispose of the class and finally
        /// Supress the GC-Finalizer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
