namespace My.Threading
{
    public interface ILock
    {
        /// <summary>
        /// Enters the lock, so you can do your actions in a safe manner.
        /// </summary>
        void Enter();

        /// <summary>
        /// Exits the lock. If the same thread exits and enters the lock constantly, it will
        /// probably got the lock many times before letting other threads get it, even if those
        /// other threads started to wait before the actual thread releases the lock. Fairness
        /// is not a strong point of this lock.
        /// </summary>
        void Exit();
    }
}