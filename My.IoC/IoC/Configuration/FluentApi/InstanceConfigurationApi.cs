using My.IoC.Configuration.Provider;
using My.IoC.Core;

namespace My.IoC.Configuration.FluentApi
{
    class InstanceConfigurationApi<T> : CommonConfigurationApi
    {
        InstanceRegistrationProvider<T> _provider;

        public ICommonConfigurationApi CreateRegistrationProvider(Kernel kernel, T instance)
        {
            _provider = new InstanceRegistrationProvider<T>(kernel)
            {
                Instance = instance,
                ContractType = typeof(T),
                ConcreteType = instance.GetType()
            };
            
            SetRegistrationProvider(_provider);
            return this;
        }
    }
}
