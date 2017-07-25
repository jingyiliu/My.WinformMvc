
using System;
using My.IoC.Core;
using My.IoC.Lifetimes;

namespace My.IoC
{
    public class LifetimeScopeEndedEventArgs : EventArgs
    {
        readonly ILifetimeScope _lifetimeScope;

        /// <summary>
        /// Create an instance of the <see cref="LifetimeScopeEndedEventArgs"/> class.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope that is ending.</param>
        public LifetimeScopeEndedEventArgs(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// The lifetime scope that is ending.
        /// </summary>
        public ILifetimeScope LifetimeScope
        {
            get { return _lifetimeScope; }
        }
    }

    /// <summary>
    /// Represents a lifetime scope (unit of work) that the instances lives in.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: The <see cref="ILifetimeScope"/> is closely related to the <see cref="Lifetime{T}"/>,
    /// and the later should know how to use it.
    /// The lifetime is the time duration where a object/variable is in a valid state.
    /// The lifetime scope is the region or section of code where an variable can be accessed.
    /// </remarks>
    public interface ILifetimeScope : IObjectResolver, IDisposable
    {
        /// <summary>
        /// Gets the sync root.
        /// </summary>
        object SyncRoot { get; }
        /// <summary>
        /// Begins a transient lifetime scope.
        /// </summary>
        /// <returns>A TransientLifetimeScope</returns>
        ILifetimeScope BeginLifetimeScope();
        /// <summary>
        /// Adds instances that should be disposed when the lifetime scope closes.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        void RegisterForDisposal(IDisposable disposable);
        /// <summary>
        /// Occurs when the lifetime scope is closing.
        /// </summary>
        event Action<LifetimeScopeEndedEventArgs> LifetimeScopeEnded;
    }

    public interface ISharingLifetimeScope : ILifetimeScope
    {
        ISharingLifetimeScope ContainerScope { get; }
        ISharingLifetimeScope SharingScope { get; }

        object GetInstance(ObjectDescription description);
        void SetInstance(ObjectDescription description, object instance);
    }
}
