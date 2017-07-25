
using My.IoC.Core;

namespace My.IoC.Injection.Instance
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InstanceInjector<T> : Injector<T>
    {
        readonly T _instance;

        public InstanceInjector(T instance)
        {
            _instance = instance;
        }

        public override void Execute(InjectionContext<T> context)
        {
            InjectInstanceIntoContext(context, _instance);
        }
    }
}
