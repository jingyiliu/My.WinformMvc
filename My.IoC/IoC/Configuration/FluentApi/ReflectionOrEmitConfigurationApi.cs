using System;
using System.Reflection;
using My.Exceptions;
using My.Foundation;
using My.Helpers;
using My.IoC.Configuration.Injection;
using My.IoC.Configuration.Provider;
using My.IoC.Core;
using System.Collections.Generic;
using My.IoC.Helpers;
using My.IoC.Injection.Func;

namespace My.IoC.Configuration.FluentApi
{
    class ReflectionOrEmitConfigurationApi : CommonConfigurationApi, ITypeConfigurationApi
    {
        TypedRegistrationProvider _provider;

        public IConstructorApi CreateRegistrationProvider(Kernel kernel, Type contractType, Type concreteType)
        {
            var registrationProviderType =
                typeof(ReflectionOrEmitRegistrationProvider<>).MakeGenericType(contractType);

            try
            {
                var objProvider = Activator.CreateInstance(registrationProviderType, kernel);
                _provider = objProvider as TypedRegistrationProvider;
                if (_provider == null)
                    throw new ImpossibleException();
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            _provider.ActivatorKind = ActivatorKind.Default;
            _provider.ContractType = contractType;
            _provider.ConcreteType = concreteType;

            SetRegistrationProvider(_provider);
            return this;
        }

        public IConstructorApi CreateRegistrationProvider<T>(Kernel kernel, Type concreteType)
            where T : class 
        {
            _provider = new ReflectionOrEmitRegistrationProvider<T>(kernel)
            {
                ConcreteType = concreteType,
                ContractType = typeof(T),
                ActivatorKind = ActivatorKind.Default
            };

            SetRegistrationProvider(_provider);
            return this;
        }

        #region IConstructorApi Members

        IMemberApi IConstructorApi.WithConstructor(IConstructorSelector ctorSelector)
        {
            _provider.ConstructorSelector = ctorSelector;
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(params PositionalParameter[] ctorParameters)
        {
            _provider.ConfiguredParameters = new PositionalParameterSet(ctorParameters);
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IEnumerable<PositionalParameter> ctorParameters)
        {
            _provider.ConfiguredParameters = new PositionalParameterSet(ctorParameters.ToList());
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IList<PositionalParameter> ctorParameters)
        {
            _provider.ConfiguredParameters = new PositionalParameterSet(ctorParameters);
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IConstructorSelector ctorSelector, params PositionalParameter[] ctorParameters)
        {
            _provider.ConstructorSelector = ctorSelector;
            _provider.ConfiguredParameters = new PositionalParameterSet(ctorParameters);
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IConstructorSelector ctorSelector, IEnumerable<PositionalParameter> ctorParameters)
        {
            _provider.ConstructorSelector = ctorSelector;
            _provider.ConfiguredParameters = new PositionalParameterSet(ctorParameters.ToList());
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IConstructorSelector ctorSelector, IList<PositionalParameter> ctorParameters)
        {
            _provider.ConstructorSelector = ctorSelector;
            _provider.ConfiguredParameters = new PositionalParameterSet(ctorParameters);
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(params NamedParameter[] ctorParameters)
        {
            _provider.ConfiguredParameters = new NamedParameterSet(ctorParameters);
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IEnumerable<NamedParameter> ctorParameters)
        {
            _provider.ConfiguredParameters = new NamedParameterSet(ctorParameters.ToList());
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IList<NamedParameter> ctorParameters)
        {
            _provider.ConfiguredParameters = new NamedParameterSet(ctorParameters);
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IConstructorSelector ctorSelector, params NamedParameter[] ctorParameters)
        {
            _provider.ConstructorSelector = ctorSelector;
            _provider.ConfiguredParameters = new NamedParameterSet(ctorParameters);
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IConstructorSelector ctorSelector, IEnumerable<NamedParameter> ctorParameters)
        {
            _provider.ConstructorSelector = ctorSelector;
            _provider.ConfiguredParameters = new NamedParameterSet(ctorParameters.ToList());
            return this;
        }

        IMemberApi IConstructorApi.WithConstructor(IConstructorSelector ctorSelector, IList<NamedParameter> ctorParameters)
        {
            _provider.ConstructorSelector = ctorSelector;
            _provider.ConfiguredParameters = new NamedParameterSet(ctorParameters);
            return this;
        }

        #endregion

        #region IMemberApi Members

        public IMemberApi WithPropertiesAutowired()
        {
            var properties = _provider.ConcreteType.GetProperties();
            if (properties.Length == 0)
                return this;
            foreach (var property in properties)
            {
                if (!property.PropertyType.IsAutowirable())
                    throw new InvalidOperationException("");
                _provider.AddMemberInjectionConfigurationItem(new AutowiredPropertyInjectionConfigurationItem(property));
            }
            return this;
        }

        public IMemberApi WithPropertyAutowired(string propertyName)
        {
            var property = GetPublicSettableProperty(propertyName);
            if (!property.PropertyType.IsAutowirable())
                throw new InvalidOperationException("");
            _provider.AddMemberInjectionConfigurationItem(new AutowiredPropertyInjectionConfigurationItem(property));
            return this;
        }

        public IMemberApi WithPropertyAutowired(PropertyInfo property)
        {
            Requires.NotNull(property, "property");
            if (!property.PropertyType.IsAutowirable())
                throw new InvalidOperationException("");
            if (!property.CanWrite)
                throw new ArgumentException("Can not inject into a non-settable property!");
            if (!property.GetSetMethod().IsPublic)
                throw new ArgumentException("Can not inject into a non-public settable property!");
            _provider.AddMemberInjectionConfigurationItem(new AutowiredPropertyInjectionConfigurationItem(property));
            return this;
        }

        public IMemberApi WithPropertyValue(string propertyName, object propertyValue)
        {
            Requires.NotNull(propertyValue, "propertyValue");
            var property = GetPublicSettableProperty(propertyName);
            _provider.AddMemberInjectionConfigurationItem(new WeakConstantPropertyInjectionConfigurationItem(property, propertyValue));
            return this;
        }

        public IMemberApi WithPropertyValue<TProperty>(string propertyName, TProperty propertyValue)
        {
            Requires.NotNull(propertyValue, "propertyValue");
            var property = GetPublicSettableProperty(propertyName);
            _provider.AddMemberInjectionConfigurationItem(new StrongConstantPropertyInjectionConfigurationItem<TProperty>(property, propertyValue));
            return this;
        }

        public IMemberApi WithPropertyValue<TProperty>(string propertyName, Func<IResolutionContext, TProperty> valueFactory)
        {
            Requires.NotNull(valueFactory, "valueFactory");
            var property = GetPublicSettableProperty(propertyName);
            _provider.AddMemberInjectionConfigurationItem(new FuncPropertyInjectionConfigurationItem<TProperty>(property, valueFactory));
            return this;
        }

        PropertyInfo GetPublicSettableProperty(string propertyName)
        {
            Requires.NotNullOrEmpty(propertyName, "propertyName");
            var property = _provider.ConcreteType.GetProperty(propertyName);
            if (property == null)
                throw new ArgumentException("Can not find a public property with the specified name!");
            if (!property.CanWrite)
                throw new ArgumentException("Can not inject into a non-settable property!");
            var propSetMethod = property.GetSetMethod();
            if (!propSetMethod.IsPublic)
                throw new ArgumentException("Can not inject into a non-public-settable property!");
            return property;
        }

        public IMemberApi WithMethod(string methodName)
        {
            Requires.NotNullOrEmpty(methodName, "methodName");
            var method = _provider.ConcreteType.GetMethod(methodName);
            if (method == null)
                throw new ArgumentException("");
            if (method.IsGenericMethod)
                throw new ArgumentException("Can not inject into a generic method!");
            _provider.AddMemberInjectionConfigurationItem(new MethodInjectionConfigurationItem(method));
            return this;
        }

        public IMemberApi WithMethod(MethodInfo method)
        {
            Requires.NotNull(method, "method");
            if (!method.IsPublic)
                throw new ArgumentException("Can not inject into a non-public method!");
            if (method.IsGenericMethod)
                throw new ArgumentException("Can not inject into a generic method!");
            _provider.AddMemberInjectionConfigurationItem(new MethodInjectionConfigurationItem(method));
            return this;
        }

        #endregion

        #region IInjectorKindApi Members

        ICommonConfigurationApi IActivatorKindApi.WithActivator(ActivatorKind kind)
        {
            _provider.ActivatorKind = kind;
            return this;
        }

        #endregion
    }
}
