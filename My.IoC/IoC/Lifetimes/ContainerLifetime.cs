
using System;
using My.IoC.Core;

namespace My.IoC.Lifetimes
{
    /// <summary>
    /// Ensures that only one instance of the given service can exist within the current <see cref="ObjectContainer"/>,
    /// and get disposed when the <see cref="ObjectContainer"/> is disposed if it implements <see cref="IDisposable"/>.
    /// </summary>
    /// <typeparam name="T">The type of the service.</typeparam>
    class NonDisposableContainerLifetime<T> : SingletonLifetime<T>
    {
        protected override T BuildSingletonInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            return DoBuildInstance(scope, injectionOperator, parameters);
        }

        protected override T BuildSingletonInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            return DoBuildInstance(context, injectionOperator, parameters);
        }
    }

    class DisposableContainerLifetime<T> : SingletonLifetime<T>
    {
        protected override T BuildSingletonInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            var instance = DoBuildInstance(scope, injectionOperator, parameters);
            var disposable = instance as IDisposable;
            var matchingScope = scope.ContainerScope;
            lock (matchingScope.SyncRoot)
                matchingScope.RegisterForDisposal(disposable);
            return instance;
        }

        protected override T BuildSingletonInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            var instance = DoBuildInstance(context, injectionOperator, parameters);
            var disposable = instance as IDisposable;
            var matchingScope = context.LifetimeScope.ContainerScope;
            lock (matchingScope.SyncRoot)
                matchingScope.RegisterForDisposal(disposable);
            return instance;
        }
    }
}
