using System;
using System.Diagnostics.CodeAnalysis;

namespace My.WinformMvc.Core
{
    public abstract class Disposable : IDisposable
    {
        bool _isDisposed;

        /// <summary>
        /// Finalizes the instance. Should never be called.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        ~Disposable()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            if (!_isDisposed)
            {
                Dispose(true);
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public bool Disposed
        {
            get { return _isDisposed; }
        }

        /// <summary>
        /// Releases resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            //if (disposing)
            //    DisposeManagedResources();
            //DisposeUnmanagedResources();
        }
    }
}
