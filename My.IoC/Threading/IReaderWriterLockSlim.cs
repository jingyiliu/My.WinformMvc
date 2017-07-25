
namespace My.Threading
{
	/// <summary>
	/// Interface that must be implemented by reader-writer lock
	/// classes that are "slim". That is, they have Enter/Exit
	/// methods instead of returning disposable instances (which
	/// makes them faster, but more error prone).
	/// </summary>
	public interface IReaderWriterLockSlim
	{
		/// <summary>
		/// Enters a read lock. Many readers can enter
		/// the read lock at the same time.
		/// </summary>
		void EnterReadLock();

		/// <summary>
		/// Exits a previously entered read lock.
		/// </summary>
		void ExitReadLock();

		/// <summary>
		/// Enters an upgradeable read lock. Many read locks can
		/// be obtained at the same time that a single upgradeable
		/// read lock is active, but two upgradeable or an
		/// upgradeable and an write lock are not permitted.
		/// </summary>
		void EnterUpgradeableLock();

		/// <summary>
		/// Exits a previously entered upgradeable read lock.
		/// You should pass the boolean telling if the lock was
		/// upgraded or not.
		/// </summary>
		/// <param name="upgraded"></param>
		void ExitUpgradeableLock(bool upgraded);

		/// <summary>
		/// Exits an upgradeable read lock, considering it was never
		/// upgraded.
		/// </summary>
		void UncheckedExitUpgradeableLock();

		/// <summary>
		/// Upgraded a previously obtained upgradeable lock to a write
		/// lock, but does not check if the lock was already upgraded.
		/// </summary>
		void UncheckedUpgradeToWriteLock();

		/// <summary>
		/// Upgraded the upgradeable lock to a write lock, checking if
		/// it was already upgraded or not (and also updating the upgraded
		/// boolean). To upgrade, the lock will wait all readers to end.
		/// </summary>
		void UpgradeToWriteLock(ref bool upgraded);

		/// <summary>
		/// Exits an upgradeable lock that was also upgraded in a single
		/// task.
		/// </summary>
		void UncheckedExitUpgradedLock();

		/// <summary>
		/// Enters a write lock. That is, the lock will only be obtained when
		/// there are no readers, be them upgradeable or not.
		/// </summary>
		void EnterWriteLock();

		/// <summary>
		/// Exits a previously obtained write lock.
		/// </summary>
		void ExitWriteLock();
	}
}
