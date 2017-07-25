using System;
using My.IoC.Configuration.Injection;
using My.IoC.Core;
using My.IoC.Lifetimes;
using My.Helpers;
using System.Globalization;

namespace My.IoC.Configuration.Provider
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class InstanceRegistrationProvider<T> : RegistrationProvider<T>
    {
        T _instance;
        InjectionConfigurationSet _configSet;

        public InstanceRegistrationProvider(Kernel kernel)
            :base(kernel)
        {
        }

        public T Instance
        {
            get { return _instance; }
            internal set
            {
                Requires.NotNull(value, "instance");
                _instance = value;
            }
        }

        protected override Lifetime<T> CreateLifetime()
        {
            return LifetimeProvider == null
                ? GetContainerLifetime()
                : DoCreateLifetime();
        }

        Lifetime<T> DoCreateLifetime()
        {
            var lifetime = LifetimeProvider.GetLifetime<T>();
            if (!(lifetime is SingletonLifetime<T>))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    Resources.InstanceCanOnlyBeRegisteredInSingletonLifetime,
                    typeof(SingletonLifetime<>).ToFullTypeName(),
                    lifetime.GetType().ToFullTypeName()));
            }
            return lifetime;
        }

        static Lifetime<T> GetContainerLifetime()
        {
            var provider = new ContainerLifetimeProvider();
            return provider.GetLifetime<T>();
        }

        internal override InjectionConfigurationSet CreateInjectionConfigurationSet(ObjectDescription description, ObjectRelation admin)
        {
            if (_configSet != null)
                return _configSet;

            var configGroup = new InstanceInjectionConfigurationGroup(description, _instance);
            var configSet = new InjectionConfigurationSet(description, admin, configGroup);
            var interpreter = new InstanceInjectionConfigurationInterpreter(configGroup);
            configGroup.InjectionConfigurationInterpreter = interpreter;
            _configSet = configSet;

            return _configSet;
        }
    }
}
