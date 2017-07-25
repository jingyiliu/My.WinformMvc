using System.Threading;
using LockIntegralType = System.Int32;
using My.IoC.Helpers;
// I really want to use 64 bit variables, but in my computer (32 bit) it is 50% slower.
// So, I am keeping the 32 bit one. On 64 bits, the writeBitShift can be 48, the upgradeBitShift is 32.

namespace My.Threading
{
    /// <summary>
    /// A "real slim" reader writer lock.
    /// Many readers can read at a time and only one writer is allowed.
    /// Reads can be recursive, but a try to a recursive write will cause a dead-lock.
    /// Note that this is a struct, so don't assign it to a local variable.
    /// </summary>
    public struct SpinReaderWriterLockSlim : IReaderWriterLockSlim
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
        #endregion

        #region EnterReadLock
        /// <summary>
        /// Enters a read lock.
        /// </summary>
        public void EnterReadLock()
        {
            //var spinWait = new SpinWait();
            while (true)
            {
                LockIntegralType result = Interlocked.Increment(ref _lockValue);
                if ((result >> _writeBitShift) == 0)
                    return;

                Interlocked.Decrement(ref _lockValue);

                var spinCount = 0;
                while (true)
                {
                    //spinWait.SpinOnce();
                    Spin.Wait(spinCount++);

                    result = Interlocked.CompareExchange(ref _lockValue, 1, 0);
                    if (result == 0)
                        return;

                    if ((result >> _writeBitShift) == 0)
                        break;
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
            Interlocked.Decrement(ref _lockValue);
        }
        #endregion

        #region EnterUpgradeableLock
        /// <summary>
        /// Enters an upgradeable lock (it is a read lock, but it can be upgraded).
        /// Only one upgradeable lock is allowed at a time.
        /// </summary>
        public void EnterUpgradeableLock()
        {
            //var spinWait = new SpinWait();
            while (true)
            {
                LockIntegralType result = Interlocked.Add(ref _lockValue, _upgradeLockValue);
                if ((result >> _upgradeBitShift) == 1)
                    return;

                Interlocked.Add(ref _lockValue, _upgradeUnlockValue);
                var spinCount = 0;
                while (true)
                {
                    //spinWait.SpinOnce();
                    Spin.Wait(spinCount++);

                    result = Interlocked.CompareExchange(ref _lockValue, _upgradeLockValue, 0);
                    if (result == 0)
                        return;

                    if ((result >> _upgradeBitShift) == 0)
                        break;
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
            Interlocked.Add(ref _lockValue, _upgradeUnlockValue);
        }

        /// <summary>
        /// Exits a previously entered upgradeable lock.
        /// </summary>
        public void ExitUpgradeableLock(bool upgraded)
        {
            if (upgraded)
                Interlocked.Add(ref _lockValue, _someExclusiveUnlockValue);
            else
                Interlocked.Add(ref _lockValue, _upgradeUnlockValue);
        }
        #endregion

        #region UpgradeToWriteLock
        /// <summary>
        /// upgrades to write-lock. You must already own a Upgradeable lock and you must first exit the write lock then the Upgradeable lock.
        /// </summary>
        public void UncheckedUpgradeToWriteLock()
        {
            //var spinWait = new SpinWait();
            LockIntegralType result = Interlocked.Add(ref _lockValue, _writeLockValue);
            var spinCount = 0;
            while ((result & _allReadsValue) != 0)
            {
                //spinWait.SpinOnce();
                Spin.Wait(spinCount++);

                result = Interlocked.CompareExchange(ref _lockValue, 0, 0);
            }
        }
        /// <summary>
        /// upgrades to write-lock. You must already own a Upgradeable lock and you must first exit the write lock then the Upgradeable lock.
        /// </summary>
        public void UpgradeToWriteLock(ref bool upgraded)
        {
            if (upgraded)
                return;

            //var spinWait = new SpinWait();
            LockIntegralType result = Interlocked.Add(ref _lockValue, _writeLockValue);
            var spinCount = 0;
            while ((result & _allReadsValue) != 0)
            {
                //spinWait.SpinOnce();
                Spin.Wait(spinCount++);
                result = Interlocked.CompareExchange(ref _lockValue, 0, 0);
            }

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
        }
        #endregion

        #region EnterWriteLock
        /// <summary>
        /// Enters write-lock.
        /// </summary>
        public void EnterWriteLock()
        {
            LockIntegralType result = Interlocked.CompareExchange(ref _lockValue, _writeLockValue, 0);
            if (result == 0)
                return;
            var spinCount = 0;
            //var spinWait = new SpinWait();
            // we need to try again.
            for (int i = 0; i < 100; i++)
            {
                //spinWait.SpinOnce();
                Spin.Wait(spinCount++);

                result = Interlocked.CompareExchange(ref _lockValue, _writeLockValue, 0);
                if (result == 0)
                    return;

                // try to be the first locker.
                if ((result >> _writeBitShift) == 0)
                    break;
            }

            // From this moment, we have priority.
            while (true)
            {
                result = Interlocked.Add(ref _lockValue, _writeLockValue);
                if (result == _writeLockValue)
                    return;

                if ((result >> _writeBitShift) == 1)
                {
                    spinCount = 0;
                    // we obtained the write lock, but there may be readers,
                    // so we wait until they release the lock.
                    while (true)
                    {
                        //spinWait.SpinOnce();
                        Spin.Wait(spinCount++);

                        result = Interlocked.CompareExchange(ref _lockValue, 0, 0);
                        if (result == _writeLockValue)
                            return;
                    }
                }
                else
                {
                    // we need to try again.
                    Interlocked.Add(ref _lockValue, _writeUnlockValue);
                    spinCount = 0;
                    while (true)
                    {
                        //spinWait.SpinOnce();
                        Spin.Wait(spinCount++);

                        result = Interlocked.CompareExchange(ref _lockValue, _writeLockValue, 0);
                        if (result == 0)
                            return;

                        // try to be the first locker.
                        if ((result >> _writeBitShift) == 0)
                            break;
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
        }
        #endregion
    }
}
