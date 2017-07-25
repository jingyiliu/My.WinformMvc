
using System;
using My.IoC.Helpers;

namespace My.IoC.Core
{
    class ThreadedRegistrationCommitter
    {
        [ThreadStatic]
        static RegistrationCommitter _threadedCommitter;
        readonly RegistrationCommitter _mainCommitter = new RegistrationCommitter();

        public RegistrationCommitter GetRegistrationCommitter()
        {
            // If we are in the application thread (main thread), just reuse the default RegistrationCommitter,
            // instead of creating a new one.
            if (SystemHelper.RunningInInitialThread)
                return _mainCommitter;

            var threadedCommitter = _threadedCommitter;
            if (threadedCommitter == null)
            {
                threadedCommitter = new RegistrationCommitter();
                _threadedCommitter = threadedCommitter;
            }

            return threadedCommitter;
        }
    }
}
