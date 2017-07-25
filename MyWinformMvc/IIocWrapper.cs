using System;
using System.Collections.Generic;
using My.IoC;
using My.IoC.Core;

namespace My.WinformMvc
{
    public interface IIocWrapper
    {
        void RegisterTypes(ICoordinator coordinator);
        object GetInstance(Type controllerType, out IDisposable lifetimeScope);
    }

    public class IoCWrapper : IIocWrapper
    {
        class RegistrationModlule : IRegistrationModule
        {
            IEnumerable<ViewControllerPair> _bindings;

            internal RegistrationModlule(IEnumerable<ViewControllerPair> bindings)
            {
                _bindings = bindings;
            }

            #region IRegistrationModule Members

            public void Register(IObjectRegistrar registrar)
            {
                foreach (var binding in _bindings)
                {
                    registrar.Register(binding.ControllerType)
                        .WithPropertyAutowired("Coordinator") // Inject the Coordinator
                        .Matadata(binding.PairName); // set the metadata

                    registrar.Register(binding.ViewContractType, binding.ViewConcreteType)
                        .WhenMetadataIs(binding.PairName); // can only be injected into Controller with the same metadata
                }

                _bindings = null;
            }

            #endregion
        }

        readonly IObjectContainer _container;

        public IoCWrapper(IObjectContainer container)
        {
            _container = container;
        }

        public void RegisterTypes(ICoordinator coordinator)
        {
            _container.Register<ICoordinator>(coordinator)
                .WhenInjectedInto(typeof(IController)) // can only be injected into IController
                .In(Lifetime.Container());

            var module = new RegistrationModlule(coordinator.PairManager.ViewControllerPairs);
            _container.RegisterModule(module);
        }

        public object GetInstance(Type controllerType, out IDisposable lifetimeScope)
        {
            var scope = _container.BeginLifetimeScope();
            object instance;
            var ex = scope.TryResolve(controllerType, out instance);
            if (ex != null)
                throw ex;
            lifetimeScope = scope;
            return instance;
        }
    }
}