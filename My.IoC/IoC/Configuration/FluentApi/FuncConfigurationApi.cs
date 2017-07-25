
using System;
using My.Foundation;
using My.IoC.Configuration.Provider;
using My.IoC.Core;
using My.IoC.Injection.Func;

namespace My.IoC.Configuration.FluentApi
{
    class FuncConfigurationApi<T> : CommonConfigurationApi
    {
        FuncRegistrationProvider<T> _provider;

        public ICommonConfigurationApi CreateRegistrationProvider(Kernel kernel, Func<IResolutionContext, T> factory, Type concreteType)
        {
            _provider = new FuncRegistrationProvider<T>(kernel)
            {
                Factory = factory,
                ContractType = typeof(T),
                ConcreteType = concreteType
            };

            SetRegistrationProvider(_provider);
            return this;
        }
    }
}
