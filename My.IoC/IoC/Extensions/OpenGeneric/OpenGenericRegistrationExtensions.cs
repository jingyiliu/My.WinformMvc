using System;
using My.Helpers;

namespace My.IoC.Extensions.OpenGeneric
{
    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for registration of open generic types in the <see cref="IObjectContainer"/>.
    /// </summary>
    public static class OpenGenericRegistrationExtensions
    {
        /// <summary>
        /// Register a service using the provided <paramref name="openGenericConcreteType"/>.
        /// The registrar will make a close type of <paramref name="openGenericConcreteType"/>.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="openGenericConcreteType">The open generic concrete type.</param>
        /// <returns>
        /// The method chain
        /// </returns>
        public static IConstructorApi RegisterOpenGeneric(this IObjectContainer registrar, Type openGenericConcreteType)
        {
            Requires.NotNull(openGenericConcreteType, "openGenericConcreteType");
            Requires.IsOpenGenericType(openGenericConcreteType, "openGenericConcreteType");
            Requires.IsConcreteType(openGenericConcreteType, "openGenericConcreteType");
            return DoRegisterOpenGeneric(registrar, openGenericConcreteType, openGenericConcreteType);
        }

        /// <summary>
        /// Register a service using the provided <paramref name="openGenericContractType"/> and <paramref name="openGenericConcreteType"/>.
        /// The registrar will make a close type of <paramref name="openGenericConcreteType"/> when a close type of 
        /// <paramref name="openGenericContractType"/> is requested.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="openGenericContractType">The open generic contract type.</param>
        /// <param name="openGenericConcreteType">The open generic concrete type.</param>
        /// <returns>
        /// The method chain
        /// </returns>
        public static IConstructorApi RegisterOpenGeneric(this IObjectContainer registrar, Type openGenericContractType, Type openGenericConcreteType)
        {
            Requires.NotNull(openGenericContractType, "openGenericContractType");
            Requires.NotNull(openGenericConcreteType, "openGenericConcreteType");
            Requires.IsOpenGenericType(openGenericContractType, "openGenericContractType");
            Requires.IsOpenGenericType(openGenericConcreteType, "openGenericConcreteType");
            Requires.IsConcreteType(openGenericConcreteType, "openGenericConcreteType");
            Requires.IsAssignableFromGeneric(openGenericContractType, openGenericConcreteType);

            return DoRegisterOpenGeneric(registrar, openGenericContractType, openGenericConcreteType);
        }

        static IConstructorApi DoRegisterOpenGeneric(this IObjectContainer registrar, Type openGenericContractType, Type openGenericConcreteType)
        {
            Requires.NotNull(registrar, "registrar");

            var componentContainer = registrar.Kernel.ComponentContainer;
            OpenGenericRequestHandler handler;
            if (!componentContainer.TryGet(out handler))
            {
                handler = new OpenGenericRequestHandler();
                // Register a singleton OpenGenericRegistrar object into the InstanceCache.
                componentContainer.Add(handler);
                // Hook the BuilderRequested event. This event will be hooked only once, that is, only one 
                // OpenGenericRegistrar instance will be used to response to the event for an unregistered 
                // open type.
                registrar.ObjectRequested += handler.OnObjectRequested;
            }

            var chain = new OpenGenericConfigurationApi();
            chain.Register(openGenericContractType, openGenericConcreteType);
            handler.Add(chain.GetRegistrationData());
            return chain;
        }

        /// <summary>
        /// Unregisters the open generic registrations, so that they can't be used to serve the later requests again.
        /// </summary>
        /// <param name="registrar">The registrar.</param>
        /// <param name="openGenericContractType">Type of the open generic contract.</param>
        public static void UnregisterOpenGeneric(this IObjectContainer registrar, Type openGenericContractType)
        {
            Requires.NotNull(registrar, "registrar");
            Requires.NotNull(openGenericContractType, "openGenericContractType");
            Requires.IsOpenGenericType(openGenericContractType, "openGenericContractType");

            var instanceCache = registrar.Kernel.ComponentContainer;
            OpenGenericRequestHandler handler;
            if (!instanceCache.TryGet(out handler))
                return;

            handler.Remove(openGenericContractType);
            if (handler.OpenGenericTypeCount == 0)
            {
                registrar.ObjectRequested -= handler.OnObjectRequested;
                instanceCache.Remove(typeof (OpenGenericRequestHandler));
            }
        }
    }
}