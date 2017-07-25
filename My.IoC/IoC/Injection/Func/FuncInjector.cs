using My.IoC.Core;
using My.Foundation;

namespace My.IoC.Injection.Func
{
    public interface IResolutionContext : IObjectResolver
    {
        //void RegisterDependencies(params Type[] dependedTypes);
    }

    sealed class ResolutionContext : IResolutionContext
    {
        readonly InjectionContext _funcContext;

        internal ResolutionContext(InjectionContext funcContext)
        {
            _funcContext = funcContext;
        }

        public Kernel Kernel
        {
            get { return _funcContext.Kernel; }
        }

        //public void RegisterDependencies(params Type[] dependedTypes)
        //{
        //}

        public object Resolve(ObjectBuilder builder, ParameterSet parameters)
        {
            object instance;
            builder.BuildInstance(_funcContext, parameters, out instance);
            return instance;
        }

        public T Resolve<T>(ObjectBuilder<T> builder, ParameterSet parameters)
        {
            T instance;
            builder.BuildInstance(_funcContext, parameters, out instance);
            return instance;
        }
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FuncInjector<T> : Injector<T>
    {
        readonly Func<IResolutionContext, T> _factory;

        public FuncInjector(Func<IResolutionContext, T> factory)
        {
            _factory = factory;
        }

        public override void Execute(InjectionContext<T> context)
        {
            var rContext = new ResolutionContext(context);
            InjectInstanceIntoContext(context, _factory.Invoke(rContext));
        }
    }
}
