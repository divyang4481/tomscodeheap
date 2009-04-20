using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ch.froorider.codeheap.Threading
{
    /// <summary>
    /// This class acts as an internal sheduler for the drivers. Each driver has his Metronom where he can register
    /// "timed - triggers". These triggers are executed, maintained and hosted by this class and signal back to a driver
    /// that one of his timeouts had been reached.
    /// </summary>
    public class Scheduler : IDisposable
    {
        #region fields

        /// <summary>
        /// Defines the values a sync point can have. Default is SYNC_FREE
        /// </summary>
        private enum SyncPoint
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

        /// <summary>
        /// This is the internal bookkeeping of the triggers.
        /// </summary>
        /// <remarks>
        /// Key := The timer associated with the trigger.
        /// Value := the AutoResetEvent to set, when the timer has been fired.
        /// </remarks>
        private Dictionary<System.Timers.Timer, AutoResetEvent> triggerList = new Dictionary<System.Timers.Timer, AutoResetEvent>();

        /// <summary>
        /// Bookkeeping of the timers and its sync objects. 
        /// </summary>
        /// <remarks>
        /// Key := The timer to sync. Corresponds with the time in the trigger list
        /// Value := The value used to sync the timer.
        /// </remarks>
        private Dictionary<System.Timers.Timer, int> timerSyncList = new Dictionary<System.Timers.Timer, int>();

        /// <summary>
        /// Reference to synchronize the different actions performed on a timer (stop, elapsed_event, ...)
        /// </summary>
        private int syncPoint = (int)SyncPoint.SYNC_FREE;

        /// <summary>
        /// Synchronizes the add / delete on the dictionaries
        /// </summary>
        private object dictionarySync = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class. (Default constructor)
        /// </summary>
        internal Scheduler()
        {
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Registers the trigger in the sheduler. After registration the timer to the trigger is automatically started.
        /// </summary>
        /// <param name="signalToSet">The signal to set when the timer has elapsed.</param>
        /// <param name="triggerTime">The time between two signalizations.</param>
        internal void RegisterTrigger(AutoResetEvent signalToSet, TimeSpan triggerTime)
        {
            System.Timers.Timer timer = new System.Timers.Timer(triggerTime.TotalMilliseconds);
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

            lock (dictionarySync)
            {
                this.timerSyncList.Add(timer, syncPoint);
                this.triggerList.Add(timer, signalToSet);
            }

            timer.Enabled = true;
        }

        /// <summary>
        /// Stop and deregisters a timer in the sheduler. The timer is "killed."
        /// </summary>
        /// <param name="signalToSet">The trigger which should be de-registered.</param>
        internal void DeRegisterTrigger(AutoResetEvent signalToSet)
        {
            foreach (var pair in triggerList)
            {
                if (pair.Value.Equals(signalToSet))
                {
                    int syncPointOfTimer = timerSyncList[pair.Key];
                    //Wait as long as there is a pending timer_elapsed_Event
                    while (Interlocked.CompareExchange(ref syncPointOfTimer, (int)SyncPoint.TIMER_STOPPED, (int)SyncPoint.SYNC_FREE) != (int)SyncPoint.SYNC_FREE)
                    {
                        Thread.Sleep(10);
                    }
                    var currentTimer = pair.Key;
                    currentTimer.Stop();

                    lock (dictionarySync)
                    {
                        timerSyncList.Remove(pair.Key);
                        triggerList.Remove(pair.Key);
                    }

                    currentTimer.Dispose();
                    break;
                }
            }
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
            System.Timers.Timer senderTimer = sender as System.Timers.Timer;
            int syncPointOfTimer = timerSyncList[senderTimer];
            //Look if there is no previous called stop()
            if (Interlocked.CompareExchange(ref syncPointOfTimer, (int)SyncPoint.ELAPSED_EVENT_RUNNING, (int)SyncPoint.SYNC_FREE) == (int)SyncPoint.SYNC_FREE)
            {
                //It is possible that .Dispose() was already called or the trigger is already removed (deregistered)
                if (this.triggerList != null && this.triggerList.ContainsKey(senderTimer))
                {
                    this.triggerList[senderTimer].Set();
                    senderTimer.Enabled = true;
                }

                //"Reset" the sync point -> Now someone else can do something with our sync point
                syncPointOfTimer = (int)SyncPoint.SYNC_FREE;
            }
            //else don't trigger the event anymore
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
                //Because we perform an operation on an IEnumerable, we must copy it first.
                List<AutoResetEvent> registeredTriggers = triggerList.Values.ToList<AutoResetEvent>();
                foreach (AutoResetEvent currentTrigger in registeredTriggers)
                {
                    DeRegisterTrigger(currentTrigger);
                }
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
