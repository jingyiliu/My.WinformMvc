
using My.Helpers;
using My.IoC.Core;
using My.IoC.Injection;

namespace My.IoC.Activities
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InjectionActivity<T>
    {
        readonly Injector<T> _injector;

        internal InjectionActivity()
        {
        }

        public InjectionActivity(Injector<T> injector)
        {
            Requires.NotNull(injector, "injector");
            _injector = injector;
        }

        public virtual void Execute(InjectionContext<T> context)
        {
            _injector.Execute(context);
        }
    }
}
