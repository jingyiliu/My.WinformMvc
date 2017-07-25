
using My.IoC.Core;

namespace My.IoC.Injection
{
    public abstract class Injector<T>
    {
        public abstract void Execute(InjectionContext<T> context);

        /// <summary>
        /// Injects the instance into context. This method should be call in the <see cref="Execute"/> 
        /// method immediately after the instance has been created.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="instance">The instance.</param>
        protected void InjectInstanceIntoContext(InjectionContext<T> context, T instance)
        {
            context.Instance = instance;
        }
    }
}
