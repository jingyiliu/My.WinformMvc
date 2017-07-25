//using System;
//using System.Diagnostics.CodeAnalysis;

////namespace My.System
//namespace My.Foundation
//{
//    /// <summary>
//    /// Base class for implementing an object which exposes events and implements the disposable pattern.
//    /// </summary>
//    /// <remarks>
//    /// Derive from this class, to implement the disposable pattern correctly.
//    /// </remarks>
//    public abstract class Disposable : IDisposable
//    {
//        /// <summary>
//        /// Finalizes the instance. Should never be called.
//        /// </summary>
//        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
//        ~Disposable()
//        {
//            Dispose(false);
//        }

//        /// <summary>
//        /// Disposes the instance.
//        /// </summary>
//        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        //public bool Disposed
//        //{
//        //    get { return _isDisposed; }
//        //}

//        /// <summary>
//        /// Releases resources.
//        /// </summary>
//        /// <param name="disposing">
//        /// <c>true</c> to release both managed and unmanaged resources; 
//        /// <c>false</c> to release only unmanaged resources.
//        /// </param>
//        protected virtual void Dispose(bool disposing)
//        {
//            //if (disposing)
//            //    DisposeManagedResources();
//            //DisposeUnmanagedResources();
//        }
//    }
//}


using System;
using System.Diagnostics.CodeAnalysis;

//namespace My.System
namespace My.Foundation
{
    /// <summary>
    /// Base class for implementing an object which exposes events and implements the disposable pattern.
    /// </summary>
    /// <remarks>
    /// Derive from this class, to implement the disposable pattern correctly.
    /// </remarks>
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