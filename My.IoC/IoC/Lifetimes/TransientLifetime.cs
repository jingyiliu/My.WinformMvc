
using System;
using My.IoC.Core;

namespace My.IoC.Lifetimes
{
    class NonDisposableTransientLifetime<T> : Lifetime<T>
    {
        #region Lifetime<T> Members

        public override T BuildInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            return DoBuildInstance(scope, injectionOperator, parameters);
        }

        public override T BuildInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            return DoBuildInstance(context, injectionOperator, parameters);
        }

        #endregion
    }

    class DisposableTransientLifetime<T> : Lifetime<T>
    {
        #region Lifetime<T> Members

        public override T BuildInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            var matchingScope = scope.SharingScope;
            Lifetime.ThrowWhenMatchingScopeIsNull(matchingScope, _error);
            var instance = DoBuildInstance(scope, injectionOperator, parameters);
            var disposable = instance as IDisposable;
            lock (matchingScope.SyncRoot)
                matchingScope.RegisterForDisposal(disposable);
            return instance;
        }

        public override T BuildInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            var matchingScope = context.LifetimeScope.SharingScope;
            Lifetime.ThrowWhenMatchingScopeIsNull(matchingScope, _error);
            var instance = DoBuildInstance(context, injectionOperator, parameters);
            var disposable = instance as IDisposable;
            lock (matchingScope.SyncRoot)
                matchingScope.RegisterForDisposal(disposable);
            return instance;
        }

        #endregion
    }
}
