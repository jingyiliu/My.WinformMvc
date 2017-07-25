using System;
using My.IoC.Core;

namespace My.IoC.Lifetimes
{
    /// <summary>
    /// Ensures that only one object instance can exist within a given <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <remarks>
    /// If the object instance implements <see cref="IDisposable"/>, 
    /// it will be disposed when the <see cref="ILifetimeScope"/> ends.
    /// </remarks>
    class NonDisposableScopeLifetime<T> : Lifetime<T>
    {
        #region Lifetime<T> Members

        public override T BuildInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            var matchingScope = scope.SharingScope;
            Lifetime.ThrowWhenMatchingScopeIsNull(matchingScope, _error);
            lock (matchingScope.SyncRoot)
            {
                var cachedInstance = matchingScope.GetInstance(injectionOperator.ObjectDescription);
                if (cachedInstance != null)
                    return (T)cachedInstance;

                var instance = DoBuildInstance(scope, injectionOperator, parameters);
                matchingScope.SetInstance(injectionOperator.ObjectDescription, instance);
                return instance;
            }
        }

        public override T BuildInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            var matchingScope = context.LifetimeScope.SharingScope;
            Lifetime.ThrowWhenMatchingScopeIsNull(matchingScope, _error);
            lock (matchingScope.SyncRoot)
            {
                var cachedInstance = matchingScope.GetInstance(injectionOperator.ObjectDescription);
                if (cachedInstance != null)
                    return (T)cachedInstance;

                var instance = DoBuildInstance(context, injectionOperator, parameters);
                matchingScope.SetInstance(injectionOperator.ObjectDescription, instance);
                return instance;
            }
        }

        #endregion
    }

    class DisposableScopeLifetime<T> : Lifetime<T>
    {
        #region Lifetime<T> Members

        public override T BuildInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            var matchingScope = scope.SharingScope;
            Lifetime.ThrowWhenMatchingScopeIsNull(matchingScope, _error);
            lock (matchingScope.SyncRoot)
            {
                var cachedInstance = matchingScope.GetInstance(injectionOperator.ObjectDescription);
                if (cachedInstance != null)
                    return (T)cachedInstance;

                var instance = DoBuildInstance(scope, injectionOperator, parameters);
                matchingScope.SetInstance(injectionOperator.ObjectDescription, instance);
                var disposable = instance as IDisposable;
                matchingScope.RegisterForDisposal(disposable);
                return instance;
            }
        }

        public override T BuildInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            var matchingScope = context.LifetimeScope.SharingScope;
            Lifetime.ThrowWhenMatchingScopeIsNull(matchingScope, _error);
            lock (matchingScope.SyncRoot)
            {
                var cachedInstance = matchingScope.GetInstance(injectionOperator.ObjectDescription);
                if (cachedInstance != null)
                    return (T)cachedInstance;

                var instance = DoBuildInstance(context, injectionOperator, parameters);
                matchingScope.SetInstance(injectionOperator.ObjectDescription, instance);
                var disposable = instance as IDisposable;
                matchingScope.RegisterForDisposal(disposable);
                return instance;
            }
        }

        #endregion
    }
}
