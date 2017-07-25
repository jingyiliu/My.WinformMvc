
using System.Threading;

namespace My.Threading
{
    /// <summary>
    /// Caution: 
    /// Do not use this class in the lock statement, like this: lock (monitorLockSlim) { }, 
    /// because this might cause dead lock.
    /// </summary>
    public class MonitorLock : ILock
    {
        public void Enter()
        {
            Monitor.Enter(this);
        }

        public void Exit()
        {
            Monitor.Exit(this);
        }
    }
}
