using System;
using System.Reflection;
using System.Threading;

namespace My.IoC.Helpers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    static class SystemHelper
    {
        public static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
        public static readonly bool MultiProcessors = Environment.ProcessorCount > 1;

        /// <summary>
        /// The thread in which the <see cref="ObjectContainer"/> is created.
        /// </summary>
        public static readonly Thread InitialThread = Thread.CurrentThread;

        /// <summary>
        /// The ManagedThreadId of the thread in which the <see cref="ObjectContainer"/> is created.
        /// </summary>
        public static readonly int InitialThreadId = InitialThread.ManagedThreadId;

        /// <summary>
        /// Gets a value indicating whether we are running in the same thread as the <see cref="ObjectContainer"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if we are running in the same thread as the <see cref="ObjectContainer"/>; otherwise, <c>false</c>.
        /// </value>
        public static bool RunningInInitialThread
        {
            get { return InitialThreadId == Thread.CurrentThread.ManagedThreadId; }
        }

        /// <summary>
        /// Asserts that the code can not be accessed from assemblies other than the <see cref="CurrentAssembly"/>.
        /// </summary>
        /// <param name="callingAssembly">The calling assembly.</param>
        public static void AssertNoAccessFromOutside(Assembly callingAssembly)
        {
            if (!ReferenceEquals(CurrentAssembly, callingAssembly))
                throw new InvalidOperationException("Can not access from outside!");
        }
    }
}
