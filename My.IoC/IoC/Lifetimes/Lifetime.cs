
using My.IoC.Core;

namespace My.IoC.Lifetimes
{
    public abstract class Lifetime<T>
    {
        protected static string _error =
            string.Format(
                "The type [{0}] is registered in scope lifetime, which needs a SharingScope to resolve it!",
                typeof(T).FullName);

        public abstract T BuildInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters);
        public abstract T BuildInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters);

        protected T DoBuildInstance(ISharingLifetimeScope scope, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            T instance;
            injectionOperator.DoBuildInstance(scope, parameters, out instance);
            return instance;
        }

        protected T DoBuildInstance(InjectionContext context, InjectionOperator<T> injectionOperator, ParameterSet parameters)
        {
            T instance;
            injectionOperator.DoBuildInstance(context, parameters, out instance);
            return instance;
        }
    }
}
