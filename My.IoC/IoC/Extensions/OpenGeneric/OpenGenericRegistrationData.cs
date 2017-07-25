
using System;
using My.Helpers;
using My.IoC.Configuration.FluentApi;
using My.IoC.Configuration.Provider;
using My.IoC.Core;
using My.IoC.Lifetimes;

namespace My.IoC.Extensions.OpenGeneric
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class OpenGenericRegistrationData
    {
        Type _openGenericContractType;
        Type _openGenericConcreteType;
        ParameterSet _configuredParameters;
        IConstructorSelector _constructorSelector;
        ILifetimeProvider _lifetimeProvider;

        public Type OpenGenericContractType
        {
            get { return _openGenericContractType; }
            set
            {
                Requires.NotNull(value, "OpenGenericContractType");
                _openGenericContractType = value;
            }
        }
        public Type OpenGenericConcreteType
        {
            get { return _openGenericConcreteType; }
            set
            {
                Requires.NotNull(value, "OpenGenericConcreteType");
                _openGenericConcreteType = value;
            }
        }

        public ParameterSet ConfiguredParameters
        {
            get { return _configuredParameters; }
            set
            {
                Requires.NotNull(value, "ConfiguredParameters");
                _configuredParameters = value;
            }
        }

        public IConstructorSelector ConstructorSelector 
        {
            get { return _constructorSelector; }
            set 
            { 
                Requires.NotNull(value, "ConstructorSelector");
                _constructorSelector = value;
            }
        }

        public ILifetimeProvider LifetimeProvider
        {
            get { return _lifetimeProvider; }
            set
            {
                Requires.NotNull(value, "LifetimeProvider");
                _lifetimeProvider = value;
            }
        }

        public ActivatorKind ActivatorKind { get; set; }
    }
}
