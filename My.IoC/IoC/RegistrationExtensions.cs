
using System;
using System.Collections.Generic;
using My.Foundation;
using My.IoC.Configuration.FluentApi;
using My.IoC.Core;
using My.Helpers;
using My.IoC.Injection.Func;
using My.IoC.Lifetimes;

namespace My.IoC
{
    /// <summary>
    /// Provides a set of static extension methods to register/unregister services and plugins.
    /// </summary>
    public static class RegistrationExtensions
    {
        #region Register using instance

        /// <summary>
        /// Register an instance as singleton (if no other <see cref="Lifetime{T}"/> style specified) and 
        /// use its concrete type, the base type from which it derives, or one of the interfaces it implements as 
        /// the retrieval key.
        /// </summary>
        /// <typeparam name="TConcrete">The concrete type of the instance, the base type from which the instance 
        /// derives, or one of the interfaces the instance implements.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>The method configurator</returns>
        public static ICommonConfigurationApi Register<TConcrete>(this IObjectRegistrar registrar, TConcrete instance)
        {
            Requires.NotNull(registrar, "registrar");
            Requires.NotNull(instance, "instance");
            Requires.IsPublicAccessibleType(typeof(TConcrete), "TConcrete");

            return DoRegister<TConcrete, TConcrete>(registrar, instance);
        }

        /// <summary>
        /// Register an instance as singleton (if no other <see cref="Lifetime{T}"/> style specified) and
        /// use its concrete type, the base type from which it derives, or one of the interfaces it implements as
        /// the retrieval key.
        /// </summary>
        /// <typeparam name="TContract">The contract type of the instance, the base type from which the instance
        /// derives, or one of the interfaces the instance implements.</typeparam>
        /// <typeparam name="TConcrete">The concrete type of the instance.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// The method configurator
        /// </returns>
        public static ICommonConfigurationApi Register<TContract, TConcrete>(this IObjectRegistrar registrar, TConcrete instance)
            where TConcrete : TContract
        {
            Requires.NotNull(registrar, "registrar");
            Requires.NotNull(instance, "instance");
            Requires.IsPublicAccessibleType(typeof(TContract), "TContract");
            Requires.IsPublicAccessibleType(typeof(TConcrete), "TConcrete");

            return DoRegister<TContract, TConcrete>(registrar, instance);
        }

        static ICommonConfigurationApi DoRegister<TContract, TConcrete>(this IObjectRegistrar registrar, TConcrete instance)
            where TConcrete : TContract
        {
            var configurator = new InstanceConfigurationApi<TContract>();
            configurator.CreateRegistrationProvider(registrar.Kernel, instance);
            var provider = configurator.GetRegistrationProvider();

            registrar.Register(provider);
            return configurator;
        }

        #endregion

        #region Register using delegate

        /// <summary>
        /// Register a service using the provided <paramref name="factory"/> and specify a implementation type, which might be the 
        /// concrete type of the returned object instance the <paramref name="factory"/> created, the base type from which it 
        /// derives, or one of the interface it implements, as the retrieval key.
        /// </summary>
        /// <typeparam name="TContract">The concrete type of the returned object instance which the <paramref name="factory"/> 
        /// created, the base type from which it derives, or one of  the interface it implements.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <param name="factory">The factory.</param>
        /// <returns>The method configurator</returns>
        public static ICommonConfigurationApi Register<TContract>(this IObjectRegistrar registrar, Func<IResolutionContext, TContract> factory)
        {
            Requires.NotNull(registrar, "registrar");
            Requires.NotNull(factory, "factory");
            Requires.IsPublicAccessibleType(typeof(TContract), "TContract");

            var configurator = new FuncConfigurationApi<TContract>();
            configurator.CreateRegistrationProvider(registrar.Kernel, factory, typeof(TContract));
            var provider = configurator.GetRegistrationProvider();

            registrar.Register(provider);
            return configurator;
        }

        /// <summary>
        /// Register a service using the provided <paramref name="factory"/> and specify a base type of the returned object instance
        /// which the <paramref name="factory"/> created or one of the interfaces it implements as the retrieval key.
        /// </summary>
        /// <typeparam name="TContract">The base type from which the returned object instance derives, or one of the interface it implements.</typeparam>
        /// <typeparam name="TConcrete">The concrete type of the returned object instance.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <param name="factory">The factory used to create a object instance.</param>
        /// <returns>
        /// The method configurator
        /// </returns>
        public static ICommonConfigurationApi Register<TContract, TConcrete>(this IObjectRegistrar registrar, Func<IResolutionContext, TConcrete> factory)
            where TConcrete : TContract
        {
            Requires.NotNull(registrar, "registrar");
            Requires.NotNull(factory, "factory");
            Requires.IsPublicAccessibleType(typeof(TContract), "TContract");
            Requires.IsPublicAccessibleType(typeof(TConcrete), "TConcrete");

            var configurator = new FuncConfigurationApi<TContract>();
            Func<IResolutionContext, TContract> newFunc = (context) => factory.Invoke(context);
            configurator.CreateRegistrationProvider(registrar.Kernel, newFunc, typeof(TConcrete));
            var provider = configurator.GetRegistrationProvider();

            registrar.Register(provider);
            return configurator;
        } 

        #endregion

        #region Register using type

        /// <summary>
        /// Register a service using the provided <paramref name="concreteType"/> and specify itself as the retrieval key.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="concreteType">The concrete type that will be used to create a object instance.</param>
        /// <returns>
        /// The method configurator
        /// </returns>
        public static ITypeConfigurationApi Register(this IObjectRegistrar registrar, Type concreteType)
        {
            return RegisterWithType(registrar, concreteType, concreteType);
        }

        /// <summary>
        /// Register a service using the provided <paramref name="concreteType"/> and specify the 
        /// <paramref name="contractType"/>, which might be a base type from which the 
        /// <paramref name="concreteType"/> derives or one of the interfaces it implements, as the 
        /// retrieval key. 
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="contractType">The base type from which the <paramref name="concreteType"/> derives, or one of the 
        /// interface it implements.</param>
        /// <param name="concreteType">The concrete type that will be used to create a object instance.</param>
        /// <returns>
        /// The method configurator
        /// </returns>
        public static ITypeConfigurationApi Register(this IObjectRegistrar registrar, Type contractType, Type concreteType)
        {
            return RegisterWithType(registrar, contractType, concreteType);
        }

        /// <summary>
        /// Register a service using the provided <typeparamref name="TConcrete"/> and specify itself as the
        /// retrieval key.
        /// </summary>
        /// <typeparam name="TConcrete">The concrete type that will be used to create a object instance.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <returns>
        /// The method configurator
        /// </returns>
        public static ITypeConfigurationApi Register<TConcrete>(this IObjectRegistrar registrar)
            where TConcrete : class
        {
            return RegisterWithTypeGeneric<TConcrete>(registrar, typeof(TConcrete));
        }

        /// <summary>
        /// Register a service using the provided <typeparamref name="TConcrete"/> and specify the
        /// <typeparamref name="TContract"/>, which might be a base type from which the
        /// <typeparamref name="TConcrete"/> derives or one of the interfaces it implements, as the
        /// retrieval key.
        /// </summary>
        /// <typeparam name="TContract">The base type from which the <typeparamref name="TConcrete"/> 
        /// derives, or one of the interface it implements.</typeparam>
        /// <typeparam name="TConcrete">The concrete type that will be used to create a object instance.</typeparam>
        /// <param name="registrar">The registrar.</param>
        /// <returns>
        /// The method configurator
        /// </returns>
        public static ITypeConfigurationApi Register<TContract, TConcrete>(this IObjectRegistrar registrar)
            where TContract : class
            where TConcrete : TContract
        {
            return RegisterWithTypeGeneric<TContract>(registrar, typeof(TConcrete));
        }

        public static ITypeConfigurationApi Register<TContract>(this IObjectRegistrar registrar, Type concreteType)
            where TContract : class
        {
            return RegisterWithTypeGeneric<TContract>(registrar, concreteType);
        }

        static ITypeConfigurationApi RegisterWithType(this IObjectRegistrar registrar, Type contractType, Type concreteType)
        {
            Requires.NotNull(contractType, "contractType");
            Requires.NotNull(concreteType, "concreteType");
            Requires.IsPublicAccessibleType(contractType, "contractType");
            Requires.IsPublicAccessibleType(concreteType, "concreteType");

            Requires.IsNotOpenGenericType(concreteType, "concreteType");
            Requires.IsConcreteType(concreteType, "concreteType");
            Requires.IsAutowirableType(contractType, "contractType");
            Requires.IsAssignableFrom(contractType, concreteType);

            var configurator = new ReflectionOrEmitConfigurationApi();
            configurator.CreateRegistrationProvider(registrar.Kernel, contractType, concreteType);
            var provider = configurator.GetRegistrationProvider();

            registrar.Register(provider);
            return configurator;
        }

        static ITypeConfigurationApi RegisterWithTypeGeneric<TContract>(this IObjectRegistrar registrar, Type concreteType)
            where TContract : class 
        {
            Requires.NotNull(concreteType, "concreteType");
            Requires.IsPublicAccessibleType(typeof(TContract), "TContract");
            Requires.IsPublicAccessibleType(concreteType, "concreteType");

            Requires.IsNotOpenGenericType(concreteType, "concreteType");
            Requires.IsConcreteType(concreteType, "concreteType");

            var contractType = typeof (TContract);
            Requires.IsAutowirableType(contractType, "contractType");
            Requires.IsAssignableFrom(contractType, concreteType);

            var configurator = new ReflectionOrEmitConfigurationApi();
            configurator.CreateRegistrationProvider<TContract>(registrar.Kernel, concreteType);
            var provider = configurator.GetRegistrationProvider();

            registrar.Register(provider);
            return configurator;
        }

        #endregion

        public static void RegisterModule(this IManualObjectRegistrar registrar, IRegistrationModule module)
        {
            module.Register(registrar);
            registrar.CommitRegistrations(); // TODO: Make sure we don't commit registration duplicatedly
        }

        #region Unregister

        /// <summary>
        /// Unregisters a service from the container using the <see cref="IObjectRegistration"/> associated with it, 
        /// so that it can't be used to serve the later requests. The service will be unregistered anyway, whether 
        /// it is depended on by any other components or not.
        /// But if there are components that depends on it, then the dependencies of those components will be updated 
        /// to allow them to rebind to other components at the next time they are requested.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="registration">The registration.</param>
        /// <returns></returns>
        public static void Unregister(this IObjectRegistrar registrar, IObjectRegistration registration)
        {
            Requires.NotNull(registrar, "registrar");
            registrar.Kernel.Unregister(registration);
        }

        /// <summary>
        /// Unregisters multiple components from the container using the <see cref="IObjectRegistration"/>s associated with them,
        /// so that it can't be used to serve the later requests. The components will be unregistered anyway, whether they are 
        /// depended on by any other components or not.
        /// But if there are components that depends on them, then the dependencies of those components will be updated 
        /// to allow to them rebind to other components at the next time they are requested.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="registrations">The registrations.</param>
        /// <returns></returns>
        public static void Unregister(this IObjectRegistrar registrar, IEnumerable<IObjectRegistration> registrations)
        {
            Requires.NotNull(registrar, "registrar");
            registrar.Kernel.Unregister(registrations);
        }

        /// <summary>
        /// Unregisters multiple components from the container using the <see cref="IObjectRegistration"/>s associated with them,
        /// so that it can't be used to serve the later requests. The components will be unregistered anyway, whether they are 
        /// depended on by any other components or not.
        /// But if there are components that depends on them, then the dependencies of those components will be updated 
        /// to allow to them rebind to other components at the next time they are requested.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="registrations">The registrations.</param>
        /// <returns></returns>
        public static void Unregister(this IObjectRegistrar registrar, params IObjectRegistration[] registrations)
        {
            Requires.NotNull(registrar, "registrar");
            registrar.Kernel.Unregister(registrations);
        }

        #endregion
    }
}
