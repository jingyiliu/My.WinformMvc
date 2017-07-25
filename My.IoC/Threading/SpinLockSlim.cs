using System.Threading;

namespace My.Threading
{
    /// <summary>
    /// A real slim SpinWait implementation (the Microsoft version is slower than 
    /// the normal use of the lock keyword for uncontended locks).
    /// This lock never verifies ownership, so it will dead-lock if you try
    /// to enter it twice and it will allow one thread to enter the lock and
    /// another thread to release it (if you want that, it will be great... if not,
    /// you will be causing bugs).
    /// It should only be used in situations where the lock is expected to be
    /// held for very short times and when performance is really critical.
    /// </summary>
    public struct SpinLockSlim : ILock
    {
        int _locked; // Resource lock: 0--free, 1--occupied

        /// <summary>
        /// Enters the lock, so you can do your actions in a safe manner.
        /// </summary>
        public void Enter()
        {
            if (Interlocked.CompareExchange(ref _locked, 1, 0) == 0)
                return;

            var spinCount = 0;
            while (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
                Spin.Wait(spinCount++);
        }

        /// <summary>
        /// Exits the lock. If the same thread exits and enters the lock constantly, it will
        /// probably got the lock many times before letting other threads get it, even if those
        /// other threads started to wait before the actual thread releases the lock. Fairness
        /// is not a strong point of this lock.
        /// </summary>
        public void Exit()
        {
            // There is no need to use a "volatile" write as all .NET writes 
            // have "release" semantics.
            _locked = 0;
        }
    }
}
