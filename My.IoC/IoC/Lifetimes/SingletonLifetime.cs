using My.IoC.Core;

namespace My.IoC.Lifetimes
{
    public abstract class SingletonLifetime<T> : Lifetime<T>
    {
        T _instance;

        #region Lifetime<T> Members

        public override T BuildInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            if (injectionOperator.Resolved)
                return _instance;
            _instance = BuildSingletonInstance(scope, injectionOperator, parameters);
            return _instance;
        }

        public override T BuildInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            if (injectionOperator.Resolved)
                return _instance;
            _instance = BuildSingletonInstance(context, injectionOperator, parameters);
            return _instance;
        }

        #endregion

        protected abstract T BuildSingletonInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator,
            ParameterSet parameters);
        protected abstract T BuildSingletonInstance(InjectionContext context, InjectionOperator<T> injectionOperator,
            ParameterSet parameters);
    }
}