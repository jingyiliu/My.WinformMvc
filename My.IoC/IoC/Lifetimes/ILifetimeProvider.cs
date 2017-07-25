using System;

namespace My.IoC.Lifetimes
{
    /// <summary>
    /// Defines a way to create an instance of <see cref="Lifetime{T}"/> implementation.
    /// </summary>
    public interface ILifetimeProvider
    {
        Lifetime<T> GetLifetime<T>();
    }

    public class ScopeLifetimeProvider : ILifetimeProvider
    {
        public Lifetime<T> GetLifetime<T>()
        {
            if ((typeof (IDisposable).IsAssignableFrom(typeof (T))))
                return new DisposableScopeLifetime<T>();
            return new NonDisposableScopeLifetime<T>();
        }
    }

    public class ContainerLifetimeProvider : ILifetimeProvider
    {
        public Lifetime<T> GetLifetime<T>()
        {
            if ((typeof(IDisposable).IsAssignableFrom(typeof(T))))
                return new DisposableContainerLifetime<T>();
            return new NonDisposableContainerLifetime<T>();
        }
    }

    public class TransientLifetimeProvider : ILifetimeProvider
    {
        public Lifetime<T> GetLifetime<T>()
        {
            if ((typeof(IDisposable).IsAssignableFrom(typeof(T))))
                return new DisposableTransientLifetime<T>();
            return new NonDisposableTransientLifetime<T>();
        }
    }
}
