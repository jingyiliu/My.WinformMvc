using My.Foundation;
using My.Helpers;
using My.IoC.Configuration.Injection;
using My.IoC.Core;
using My.IoC.Injection.Func;
using My.IoC.Lifetimes;

namespace My.IoC.Configuration.Provider
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class FuncRegistrationProvider<T> : RegistrationProvider<T>
    {
        Func<IResolutionContext, T> _factory;
        InjectionConfigurationSet _configSet;

        public FuncRegistrationProvider(Kernel kernel)
            :base(kernel)
        {
        }

        public Func<IResolutionContext, T> Factory
        {
            get { return _factory; }
            internal set
            {
                Requires.NotNull(value, "factory");
                _factory = value;
            }
        }

        internal override InjectionConfigurationSet CreateInjectionConfigurationSet(ObjectDescription description, ObjectRelation admin)
        {
            if (_configSet != null)
                return _configSet;

            var configGroup = new FuncInjectionConfigurationGroup(description, _factory);
            var configSet = new InjectionConfigurationSet(description, admin, configGroup);
            var interpreter = new FuncInjectionConfigurationInterpreter(configGroup);
            configGroup.InjectionConfigurationInterpreter = interpreter;
            _configSet = configSet;

            return _configSet;
        }

        protected override Lifetime<T> CreateLifetime()
        {
            return LifetimeHelper.CreateLifetime<T>(LifetimeProvider);
        }
    }
}
