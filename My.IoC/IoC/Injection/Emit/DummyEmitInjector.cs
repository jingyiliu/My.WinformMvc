
#if DEBUG

using My.IoC.Core;
using My.IoC.Dependencies;

namespace My.IoC.Injection.Emit
{
    public abstract class AbstractClass
    {
    }

    public class DerivedClass : AbstractClass
    {
    }

    class DummyEmitInjector2 : Injector<DerivedClass>
    {
        public override void Execute(InjectionContext<DerivedClass> context)
        {
            var cl = new DerivedClass();
            InjectInstanceIntoContext(context, cl);
        }
    }

    class DummyClass
    {
        private ContainerOption _containerOption;
        private IObjectContainer _objectContainer;

        public DummyClass(IObjectRegistration p1, ContainerOption p2, ILifetimeScope p3) { }

        public Parameter Parameter { get; set; }

        public void SetProperties(ContainerOption containerOption, IObjectContainer objectContainer)
        {
            _containerOption = containerOption;
            _objectContainer = objectContainer;
        }
    }

    class DummyEmitInjector : Injector<DummyClass>
    {
        readonly EmitParameterMerger<IObjectRegistration, ContainerOption, ILifetimeScope> _parameterMerger;
        readonly DependencyProvider<Parameter> _f0;
        readonly DependencyProvider<ContainerOption> _f1;
        readonly DependencyProvider<IObjectContainer> _f2;

        public DummyEmitInjector(DependencyProvider[] ctorDependencyProviders, DependencyProvider[] memberDependencyProviders)
        {
            _parameterMerger = new EmitParameterMerger<IObjectRegistration, ContainerOption, ILifetimeScope>(ctorDependencyProviders);
            _f0 = (DependencyProvider<Parameter>) memberDependencyProviders[0];
            _f1 = (DependencyProvider<ContainerOption>)memberDependencyProviders[1];
            _f2 = (DependencyProvider<IObjectContainer>)memberDependencyProviders[2];
        }

        public override void Execute(InjectionContext<DummyClass> context)
        {
            IObjectRegistration p1;
            ContainerOption p2;
            ILifetimeScope p3;
            _parameterMerger.Merge(context, out p1, out p2, out p3);
            var dummyClass = new DummyClass(p1, p2, p3);

            InjectInstanceIntoContext(context, dummyClass);

            Parameter parameter;
            _f0.CreateObject(context, out parameter);
            dummyClass.Parameter = parameter;

            ContainerOption option;
            _f1.CreateObject(context, out option);

            IObjectContainer container;
            _f2.CreateObject(context, out container);

            dummyClass.SetProperties(option, container);
        }
    }
}

#endif
