using System.Threading;
using LockIntegralType = System.Int32;
// I am using direct reads that are non atomic if 64 bits variables are used on 32 bit computers.

namespace My.Threading
{
    /// <summary>
    /// An optimistic reader writer lock that is as fast as the 
    /// SpinReaderWriterLock/SpinReaderWriterLockSlim when the lock
    /// can be obtained immediately. But, if that's not the case, instead
    /// of spinning it will enter a real wait state. So, this one is preferreable
    /// if the locks are expected to be of large duration (100 milliseconds or more)
    /// while the SpinReaderWriterLock is preferreable if the waits are usually
    /// very small.
    /// Note that this class has both the Enter/Exit pairs (slim version) and the 
    /// methods that return a disposable object to release the lock (the non-slim
    /// version).
    /// </summary>
    public sealed class OptimisticReaderWriterLock : IReaderWriterLockSlim
    {
        #region Consts
        const int _writeBitShift = 24;
        const int _upgradeBitShift = 16;

        const LockIntegralType _writeLockValue = ((LockIntegralType)1) << _writeBitShift;
        const LockIntegralType _writeUnlockValue = -_writeLockValue;
        const LockIntegralType _upgradeLockValue = ((LockIntegralType)1) << _upgradeBitShift;
        const LockIntegralType _upgradeUnlockValue = -_upgradeLockValue;
        const LockIntegralType _allReadsValue = _upgradeLockValue - 1;
        const LockIntegralType _someExclusiveLockValue = _writeLockValue | _upgradeLockValue;
        const LockIntegralType _someExclusiveUnlockValue = -_someExclusiveLockValue;
        #endregion

        #region Fields
        LockIntegralType _lockValue;
        LockIntegralType _waitingValue;
        #endregion

        #region EnterReadLock
        /// <summary>
        /// Enters a read lock.
        /// </summary>
        public void EnterReadLock()
        {
            while (true)
            {
                LockIntegralType result = Interlocked.Increment(ref _lockValue);
                if (result < _writeLockValue)
                    return;

                lock (this)
                {
                    // here we can read directly.						
                    // also, if everything is OK we can return
                    // directly as we still hold the readers count
                    // at +1.
                    if (_lockValue < _writeLockValue)
                        return;

                    _waitingValue++;
                    result = Interlocked.Decrement(ref _lockValue);
                    if (result < _writeLockValue)
                    {
                        _waitingValue--;
                        continue;
                    }

                    while (true)
                    {
                        Monitor.Wait(this);

                        // again, we can read directly.
                        if (_lockValue < _writeLockValue)
                        {
                            _waitingValue--;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
        #region ExitReadLock
        /// <summary>
        /// Exits a read-lock. Take care not to exit more times than you entered, as there is no check for that.
        /// </summary>
        public void ExitReadLock()
        {
            int result = Interlocked.Decrement(ref _lockValue);
            if ((result & _allReadsValue) == 0)
                if (_waitingValue > 0)
                    lock (this)
                        Monitor.PulseAll(this);

            // We need to Pulse all threads when there are no more readers
            // because we may have a thread waiting to obtain a write lock and an
            // upgradeable lock trying to upgrade. A simple pulse will free the
            // thread that wants the write lock, but it will not be able to get
            // it because there's the upgradeable lock already acquired.
        }
        #endregion

        #region EnterUpgradeableLock
        /// <summary>
        /// Enters an upgradeable lock (it is a read lock, but it can be upgraded).
        /// Only one upgradeable lock is allowed at a time.
        /// </summary>
        public void EnterUpgradeableLock()
        {
            while (true)
            {
                LockIntegralType result = Interlocked.Add(ref _lockValue, _upgradeLockValue);
                if ((result >> _upgradeBitShift) == 1)
                    return;

                lock (this)
                {
                    // here we can read directly.						
                    // also, if everything is OK we can return
                    // directly as we still hold the upgradeable count
                    // at +1.
                    if ((_lockValue >> _upgradeBitShift) == 1)
                        return;

                    _waitingValue++;
                    result = Interlocked.Add(ref _lockValue, _upgradeUnlockValue);
                    if ((result >> _upgradeBitShift) == 0)
                    {
                        _waitingValue--;
                        continue;
                    }

                    // maybe we just forbid the thread that has the
                    // upgradeable lock from upgrading to the write lock.
                    // Also, as this may not be the next thread waiting, we
                    // need to pulse all.
                    if ((result >> _upgradeBitShift) == 1)
                        Monitor.PulseAll(this);

                    while (true)
                    {
                        Monitor.Wait(this);

                        // again, we can read directly.
                        if ((_lockValue >> _upgradeBitShift) == 0)
                        {
                            _waitingValue--;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
        #region ExitUpgradeableLock
        /// <summary>
        /// Exits a previously obtained upgradeable lock without
        /// verifying if it was upgraded or not.
        /// </summary>
        public void UncheckedExitUpgradeableLock()
        {
            var result = Interlocked.Add(ref _lockValue, _upgradeUnlockValue);

            if ((result & _allReadsValue) == 0)
                if (_waitingValue > 0)
                    lock (this)
                        Monitor.Pulse(this);

            // Here we pulse, instead of PulseAll, because we never block
            // readers and it is guaranteed that there was no other thread with
            // an upgradeable lock trying to upgrade it. So, if there's a thread
            // trying to obtain either an upgradeable or writeable lock, we pulse
            // that single thread.
        }

        /// <summary>
        /// Exits a previously entered upgradeable lock.
        /// </summary>
        public void ExitUpgradeableLock(bool upgraded)
        {
            if (upgraded)
                UncheckedExitUpgradedLock();
            else
                UncheckedExitUpgradeableLock();
        }
        #endregion

        #region UpgradeToWriteLock
        /// <summary>
        /// Upgrades to write-lock. You must already own a Upgradeable lock and you must first exit the write lock then the upgradeable lock.
        /// </summary>
        public void UncheckedUpgradeToWriteLock()
        {
            while (true)
            {
                LockIntegralType result = Interlocked.CompareExchange(ref _lockValue, _someExclusiveLockValue, _upgradeLockValue);
                if (result == _upgradeLockValue)
                    return;

                lock (this)
                {
                    if (_lockValue == _upgradeLockValue)
                        continue;

                    _waitingValue++;

                    Monitor.Wait(this);
                    while (true)
                    {
                        if (_lockValue == _upgradeLockValue)
                        {
                            _waitingValue--;
                            break;
                        }

                        Monitor.Wait(this);
                    }
                }
            }
        }
        /// <summary>
        /// upgrades to write-lock. You must already own a Upgradeable lock and you must first exit the write lock then the Upgradeable lock.
        /// </summary>
        public void UpgradeToWriteLock(ref bool upgraded)
        {
            if (upgraded)
                return;

            UncheckedUpgradeToWriteLock();
            upgraded = true;
        }
        #endregion
        #region UncheckedExitUpgradedLock
        /// <summary>
        /// Releases the Upgradeable lock and the upgraded version of it (the write lock)
        /// at the same time.
        /// Releasing the write lock and the upgradeable lock has the same effect, but
        /// it's slower.
        /// </summary>
        public void UncheckedExitUpgradedLock()
        {
            Interlocked.Add(ref _lockValue, _someExclusiveUnlockValue);

            if (_waitingValue > 0)
                lock (this)
                    Monitor.PulseAll(this);
        }
        #endregion

        #region EnterWriteLock
        /// <summary>
        /// Enters write-lock.
        /// </summary>
        public void EnterWriteLock()
        {
            while (true)
            {
                LockIntegralType result = Interlocked.CompareExchange(ref _lockValue, _writeLockValue, 0);
                if (result == 0)
                    return;

                lock (this)
                {
                    if (_lockValue == 0)
                        continue;

                    _waitingValue++;

                    Monitor.Wait(this);
                    while (true)
                    {
                        if (_lockValue == 0)
                        {
                            _waitingValue--;
                            break;
                        }

                        Monitor.Wait(this);
                    }
                }
            }
        }
        #endregion
        #region ExitWriteLock
        /// <summary>
        /// Exits write lock. Take care to exit only when you entered, as there is no check for that.
        /// </summary>
        public void ExitWriteLock()
        {
            Interlocked.Add(ref _lockValue, _writeUnlockValue);

            if (_waitingValue > 0)
                lock (this)
                    Monitor.PulseAll(this);
        }
        #endregion
    }
}
