
using System;
using System.Collections.Generic;
using System.Reflection;
using My.Exceptions;
using My.IoC.Configuration.Provider;

namespace My.IoC.Extensions.OpenGeneric
{
    /// <summary>Resolves a given open generic type.</summary>
    sealed class OpenGenericRequestHandler
    {
        readonly Dictionary<Type, OpenGenericRegistrationData> _openContract2OpenConcrete = new Dictionary<Type, OpenGenericRegistrationData>();

        public int OpenGenericTypeCount
        {
            get { return _openContract2OpenConcrete.Count; }
        }

        public void Add(OpenGenericRegistrationData registrationData)
        {
            // Throw when the OpenGenericContractType already exists
            _openContract2OpenConcrete.Add(registrationData.OpenGenericContractType, registrationData);
        }

        public void Remove(Type openGenericContractType)
        {
            _openContract2OpenConcrete.Remove(openGenericContractType);
        }

        public void OnObjectRequested(ObjectRequestedEventArgs args)
        {
            Type[] genericArgs;
            OpenGenericRegistrationData registrationData;
            if (!CanHandle(args, out registrationData, out genericArgs))
                return;
            RegisterNewService(args, registrationData, genericArgs);
        }

        bool CanHandle(ObjectRequestedEventArgs args, out OpenGenericRegistrationData registrationData, out Type[] genericArgs)
        {
            genericArgs = null;
            registrationData = null;

            var contractType = args.ContractType;
            if (!contractType.IsGenericType)
                return false;

            var openGenericContractType = contractType.GetGenericTypeDefinition();
            if (openGenericContractType == null)
                throw new ImpossibleException();

            if (!_openContract2OpenConcrete.TryGetValue(openGenericContractType, out registrationData))
                return false;

            // check the generic constraints???
            genericArgs = contractType.GetGenericArguments();
            return true;
        }

        static void RegisterNewService(ObjectRequestedEventArgs args, OpenGenericRegistrationData registrationData, Type[] genericArgs)
        {
            var provider = CreateRegistrationProvider(args, registrationData, genericArgs);
            args.Register(provider);
        }

        static RegistrationProvider CreateRegistrationProvider(ObjectRequestedEventArgs args, OpenGenericRegistrationData registrationData, Type[] genericArgs)
        {
            var registrationProviderType =
                typeof(ReflectionOrEmitRegistrationProvider<>).MakeGenericType(args.ContractType);

            TypedRegistrationProvider provider;
            try
            {
                var objProvider = Activator.CreateInstance(registrationProviderType, args.Kernel);
                provider = objProvider as TypedRegistrationProvider;
                if (provider == null)
                    throw new ImpossibleException();
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            provider.ActivatorKind = registrationData.ActivatorKind;
            provider.ContractType = registrationData.OpenGenericContractType.MakeGenericType(genericArgs);
            provider.ConcreteType = registrationData.OpenGenericConcreteType.MakeGenericType(genericArgs);

            if (registrationData.ConstructorSelector != null)
                provider.ConstructorSelector = registrationData.ConstructorSelector;
            if (registrationData.ConfiguredParameters != null)
                provider.ConfiguredParameters = registrationData.ConfiguredParameters;
            if (registrationData.LifetimeProvider != null)
                provider.LifetimeProvider = registrationData.LifetimeProvider;

            return provider;
        }
    }
}
