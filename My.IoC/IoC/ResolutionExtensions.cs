using System;
using System.Collections.Generic;
using My.Exceptions;
using My.IoC.Core;
using My.IoC.Exceptions;
using My.Helpers;
using System.Globalization;
using My.IoC.Registry;

namespace My.IoC
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ResolutionExtensions
    {
        #region Resolve Type

        /// <summary>
        /// Retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        public static TContract Resolve<TContract>(this IObjectResolver resolver)
        {
            return (TContract)ResolveWithType(resolver, typeof(TContract), null);
        }

        /// <summary>
        /// Retrieve a service associated with the <paramref name="contractType"/> then use that service
        /// to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns></returns>
        public static object Resolve(this IObjectResolver resolver, Type contractType)
        {
            return ResolveWithType(resolver, contractType, null);
        }

        #region PositionalParameter

        /// <summary>
        /// Retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static TContract Resolve<TContract>(this IObjectResolver resolver, params PositionalParameter[] overridenParameters)
        {
            return (TContract)ResolveWithType(resolver, typeof(TContract), new PositionalParameterSet(overridenParameters));
        }

        /// <summary>
        /// Retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static TContract Resolve<TContract>(this IObjectResolver resolver, IList<PositionalParameter> overridenParameters)
        {
            return (TContract)ResolveWithType(resolver, typeof(TContract), new PositionalParameterSet(overridenParameters));
        }

        /// <summary>
        /// Retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static TContract Resolve<TContract>(this IObjectResolver resolver, IEnumerable<PositionalParameter> overridenParameters)
        {
            return (TContract)ResolveWithType(resolver, typeof(TContract), new PositionalParameterSet(overridenParameters.ToList()));
        }

        /// <summary>
        /// Retrieve a service associated with the <paramref name="contractType"/> then use that service and 
        /// the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static object Resolve(this IObjectResolver resolver, Type contractType, params PositionalParameter[] overridenParameters)
        {
            return ResolveWithType(resolver, contractType, new PositionalParameterSet(overridenParameters));
        }

        /// <summary>
        /// Retrieve a service associated with the <paramref name="contractType"/> then use that service and 
        /// the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static object Resolve(this IObjectResolver resolver, Type contractType, IList<PositionalParameter> overridenParameters)
        {
            return ResolveWithType(resolver, contractType, new PositionalParameterSet(overridenParameters));
        }

        /// <summary>
        /// Retrieve a service associated with the <paramref name="contractType"/> then use that service and 
        /// the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static object Resolve(this IObjectResolver resolver, Type contractType, IEnumerable<PositionalParameter> overridenParameters)
        {
            return ResolveWithType(resolver, contractType, new PositionalParameterSet(overridenParameters.ToList()));
        }

        #endregion

        #region NamedParameter

        /// <summary>
        /// Retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static TContract Resolve<TContract>(this IObjectResolver resolver, params NamedParameter[] overridenParameters)
        {
            return (TContract)ResolveWithType(resolver, typeof(TContract), new NamedParameterSet(overridenParameters));
        }

        /// <summary>
        /// Retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static TContract Resolve<TContract>(this IObjectResolver resolver, IList<NamedParameter> overridenParameters)
        {
            return (TContract)ResolveWithType(resolver, typeof(TContract), new NamedParameterSet(overridenParameters));
        }

        /// <summary>
        /// Retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static TContract Resolve<TContract>(this IObjectResolver resolver, IEnumerable<NamedParameter> overridenParameters)
        {
            return (TContract)ResolveWithType(resolver, typeof(TContract), new NamedParameterSet(overridenParameters.ToList()));
        }

        /// <summary>
        /// Retrieve a service associated with the <paramref name="contractType"/> then use that service and 
        /// the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static object Resolve(this IObjectResolver resolver, Type contractType, params NamedParameter[] overridenParameters)
        {
            return ResolveWithType(resolver, contractType, new NamedParameterSet(overridenParameters));
        }

        /// <summary>
        /// Retrieve a service associated with the <paramref name="contractType"/> then use that service and 
        /// the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static object Resolve(this IObjectResolver resolver, Type contractType, IList<NamedParameter> overridenParameters)
        {
            return ResolveWithType(resolver, contractType, new NamedParameterSet(overridenParameters));
        }

        /// <summary>
        /// Retrieve a service associated with the <paramref name="contractType"/> then use that service and 
        /// the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <returns></returns>
        public static object Resolve(this IObjectResolver resolver, Type contractType, IEnumerable<NamedParameter> overridenParameters)
        {
            return ResolveWithType(resolver, contractType, new NamedParameterSet(overridenParameters.ToList()));
        }

        #endregion

        static object ResolveWithType(this IObjectResolver resolver, Type contractType, ParameterSet parameters)
        {
            Requires.NotNull(resolver, "resolver");
            Requires.NotNull(contractType, "contractType");

            ObjectBuilder builder;
            var state = resolver.Kernel.TryGet(contractType, out builder);

            switch (state)
            {
                case ObjectBuilderState.Normal:
                    return resolver.Resolve(builder, parameters);

                case ObjectBuilderState.Invalid:
                    throw new InvalidObjectBuilderException(GetInvalidObjectBuilderErrorMessage(contractType));

                case ObjectBuilderState.Unregistered:
                    builder = resolver.Kernel.AutoObjectRegistrar.GetObjectBuilder(contractType);
                    if (builder != null)
                        return resolver.Resolve(builder, parameters);
                    throw new ObjectBuilderNotFoundException(GetObjectBuilderNotFoundErrorMessage(contractType));

                default:
                    throw new ImpossibleException();
            }
        }

        #endregion

        #region TryResolve Type

        /// <summary>
        /// Tries to retrieve a service associated with the <paramref name="contractType"/> then use that service
        /// to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve(this IObjectResolver resolver, Type contractType, out object instance)
        {
            return TryResolveWithType(resolver, contractType, null, out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve<TContract>(this IObjectResolver resolver, out TContract instance)
        {
            return TryResolveWithType(resolver, null, out instance);
        }

        #region PositionalParameter

        /// <summary>
        /// Tries to retrieve a service associated with the <paramref name="contractType"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve(this IObjectResolver resolver, Type contractType, out object instance, params PositionalParameter[] overridenParameters)
        {
            return TryResolveWithType(resolver, contractType, new PositionalParameterSet(overridenParameters), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <paramref name="contractType"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve(this IObjectResolver resolver, Type contractType, out object instance, IList<PositionalParameter> overridenParameters)
        {
            return TryResolveWithType(resolver, contractType, new PositionalParameterSet(overridenParameters), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <paramref name="contractType"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve(this IObjectResolver resolver, Type contractType, out object instance, IEnumerable<PositionalParameter> overridenParameters)
        {
            return TryResolveWithType(resolver, contractType, new PositionalParameterSet(overridenParameters.ToList()), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve<TContract>(this IObjectResolver resolver, out TContract instance, params PositionalParameter[] overridenParameters)
        {
            return TryResolveWithType(resolver, new PositionalParameterSet(overridenParameters), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve<TContract>(this IObjectResolver resolver, out TContract instance, IList<PositionalParameter> overridenParameters)
        {
            return TryResolveWithType(resolver, new PositionalParameterSet(overridenParameters), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve<TContract>(this IObjectResolver resolver, out TContract instance, IEnumerable<PositionalParameter> overridenParameters)
        {
            return TryResolveWithType(resolver, new PositionalParameterSet(overridenParameters.ToList()), out instance);
        }

        #endregion

        #region NamedParameter

        /// <summary>
        /// Tries to retrieve a service associated with the <paramref name="contractType"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve(this IObjectResolver resolver, Type contractType, out object instance, params NamedParameter[] overridenParameters)
        {
            return TryResolveWithType(resolver, contractType, new NamedParameterSet(overridenParameters), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <paramref name="contractType"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve(this IObjectResolver resolver, Type contractType, out object instance, IList<NamedParameter> overridenParameters)
        {
            return TryResolveWithType(resolver, contractType, new NamedParameterSet(overridenParameters), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <paramref name="contractType"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve(this IObjectResolver resolver, Type contractType, out object instance, IEnumerable<NamedParameter> overridenParameters)
        {
            return TryResolveWithType(resolver, contractType, new NamedParameterSet(overridenParameters.ToList()), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve<TContract>(this IObjectResolver resolver, out TContract instance, params NamedParameter[] overridenParameters)
        {
            return TryResolveWithType(resolver, new NamedParameterSet(overridenParameters), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve<TContract>(this IObjectResolver resolver, out TContract instance, IList<NamedParameter> overridenParameters)
        {
            return TryResolveWithType(resolver, new NamedParameterSet(overridenParameters), out instance);
        }

        /// <summary>
        /// Tries to retrieve a service associated with the <typeparamref name="TContract"/> then use that service
        /// and the provided <paramref name="overridenParameters"/> to build a object instance.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="overridenParameters">The overriden parameters.</param>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public static Exception TryResolve<TContract>(this IObjectResolver resolver, out TContract instance, IEnumerable<NamedParameter> overridenParameters)
        {
            return TryResolveWithType(resolver, new NamedParameterSet(overridenParameters.ToList()), out instance);
        }

        #endregion

        static Exception TryResolveWithType<TContract>(this IObjectResolver resolver, ParameterSet parameters, out TContract instance)
        {
            object obj;
            var ex = TryResolveWithType(resolver, typeof(TContract), parameters, out obj);
            if (ex == null)
            {
                instance = (TContract)obj;
                return null;
            }
            instance = default(TContract);
            return ex;
        }

        static Exception TryResolveWithType(this IObjectResolver resolver, Type contractType, ParameterSet parameters, out object instance)
        {
            Requires.NotNull(resolver, "resolver");
            Requires.NotNull(contractType, "contractType");

            ObjectBuilder builder;
            var state = resolver.Kernel.TryGet(contractType, out builder);

            switch (state)
            {
                case ObjectBuilderState.Normal:
                    instance = resolver.Resolve(builder, parameters);
                    return null;

                case ObjectBuilderState.Invalid:
                    instance = null;
                    return new ObjectBuilderNotFoundException(GetObjectBuilderNotFoundErrorMessage(contractType));

                case ObjectBuilderState.Unregistered:
                    builder = resolver.Kernel.AutoObjectRegistrar.GetObjectBuilder(contractType);
                    if (builder == null)
                    {
                        instance = null;
                        return new ObjectBuilderNotFoundException(GetObjectBuilderNotFoundErrorMessage(contractType));
                    }
                    instance = resolver.Resolve(builder, parameters);
                    return null;

                default:
                    throw new ImpossibleException();
            }
        }

        #endregion

        #region ResolveAll Type

        /// <summary>
        /// Retrieve all service associated with the <typeparamref name="TContract"/> and use those components
        /// to build a object instances.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        public static IList<TContract> ResolveAll<TContract>(this IObjectResolver resolver)
        {
            Requires.NotNull(resolver, "resolver");
            IEnumerable<ObjectBuilder<TContract>> builders;
            if (!resolver.Kernel.TryGet(typeof(TContract), out builders))
                throw new ObjectBuilderNotFoundException(GetObjectBuilderNotFoundErrorMessage(typeof(TContract)));

            var instances = new List<TContract>();
            foreach (var builder in builders)
            {
                var instance = resolver.Resolve(builder, null);
                instances.Add(instance);
            }

            return instances;
        }

        /// <summary>
        /// Retrieve all service associated with the <paramref name="contractType"/> and use those components
        /// to build a object instances.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns></returns>
        public static IList<object> ResolveAll(this IObjectResolver resolver, Type contractType)
        {
            Requires.NotNull(resolver, "resolver");
            Requires.NotNull(contractType, "contractType");
            IEnumerable<ObjectBuilder> builders;
            if (!resolver.Kernel.TryGet(contractType, out builders))
                throw new ObjectBuilderNotFoundException(GetObjectBuilderNotFoundErrorMessage(contractType));

            var instances = new List<object>();
            foreach (var builder in builders)
            {
                var instance = resolver.Resolve(builder, null);
                instances.Add(instance);
            }

            return instances;
        }

        /// <summary>
        /// Tries to retrieve all service associated with the <typeparamref name="TContract"/> and use those components
        /// to build a object instances.
        /// </summary>
        /// <typeparam name="TContract">The type of the contract.</typeparam>
        /// <param name="resolver">The resolver.</param>
        /// <param name="instances">The instances.</param>
        /// <returns></returns>
        public static Exception TryResolveAll<TContract>(this IObjectResolver resolver, out IList<TContract> instances)
        {
            Requires.NotNull(resolver, "resolver");
            IEnumerable<ObjectBuilder<TContract>> builders;
            if (!resolver.Kernel.TryGet(typeof(TContract), out builders))
            {
                instances = null;
                return new ObjectBuilderNotFoundException(GetObjectBuilderNotFoundErrorMessage(typeof(TContract)));
            }

            var resolvedInstances = new List<TContract>();
            foreach (var builder in builders)
            {
                TContract instance;
                var ex = TryResolve(resolver, builder, null, out instance);
                if (ex != null)
                {
                    instances = null;
                    return ex;
                }
                resolvedInstances.Add(instance);
            }

            instances = resolvedInstances;
            return null;
        }

        /// <summary>
        /// Tries to retrieve all service associated with the <paramref name="contractType"/> and use those components
        /// to build a object instances.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="instances">The instances.</param>
        /// <returns></returns>
        public static Exception TryResolveAll(this IObjectResolver resolver, Type contractType, out IList<object> instances)
        {
            Requires.NotNull(resolver, "resolver");
            Requires.NotNull(contractType, "contractType");
            IEnumerable<ObjectBuilder> builders;
            if (!resolver.Kernel.TryGet(contractType, out builders))
            {
                instances = null;
                return new ObjectBuilderNotFoundException(GetObjectBuilderNotFoundErrorMessage(contractType));
            }

            var resolvedInstances = new List<object>();
            foreach (var builder in builders)
            {
                object instance;
                var ex = TryResolve(resolver, builder, null, out instance);
                if (ex != null)
                {
                    instances = null;
                    return ex;
                }
                resolvedInstances.Add(instance);
            }

            instances = resolvedInstances;
            return null;
        }

        #endregion

        #region Observer

        public static bool TryGetObserver(this IObjectResolver resolver, Type contractType, out IObjectObserver observer)
        {
            return resolver.Kernel.TryGet(contractType, out observer);
        }

        public static bool TryGetObserver<T>(this IObjectResolver resolver, out IObjectObserver<T> observer)
        {
            return resolver.Kernel.TryGet(typeof(T), out observer);
        }

        public static bool TryGetObserver(this IObjectResolver resolver, Type contractType, out IObjectCollectionObserver observer)
        {
            return resolver.Kernel.TryGet(contractType, out observer);
        }

        public static bool TryGetObserver<T>(this IObjectResolver resolver, out IObjectCollectionObserver<T> observer)
        {
            return resolver.Kernel.TryGet(typeof(T), out observer);
        }

        public static object Resolve(this IObjectResolver resolver, IObjectObserver observer)
        {
            var builder = observer.ObjectBuilder;
            if (builder == null)
                throw new ObsoleteObjectBuilderException(GetObsoleteObjectBuilderErrorMessage(observer.ContractType));
            return resolver.Resolve(builder, null);
        }

        public static T Resolve<T>(this IObjectResolver resolver, IObjectObserver<T> observer)
        {
            var builder = observer.ObjectBuilder;
            if (builder == null)
                throw new ObsoleteObjectBuilderException(GetObsoleteObjectBuilderErrorMessage(observer.ContractType));
            return resolver.Resolve(builder, null);
        }

        public static object[] ResolveAll(this IObjectResolver resolver, IObjectCollectionObserver observer)
        {
            var builders = observer.ObjectBuilders;
            if (builders == null || builders.Length == 0)
                throw new ObsoleteObjectBuilderException(GetObsoleteObjectBuilderErrorMessage(observer.ContractType));
            var result = new object[builders.Length];
            for (int i = 0; i < builders.Length; i++)
                result[i] = resolver.Resolve(builders[i], null);
            return result;
        }

        public static T[] ResolveAll<T>(this IObjectResolver resolver, IObjectCollectionObserver<T> observer)
        {
            var builders = observer.ObjectBuilders;
            if (builders == null)
                throw new ObsoleteObjectBuilderException(GetObsoleteObjectBuilderErrorMessage(observer.ContractType));
            var result = new T[builders.Length];
            for (int i = 0; i < builders.Length; i++)
                result[i] = resolver.Resolve(builders[i], null);
            return result;
        }

        public static Exception TryResolve(this IObjectResolver resolver, IObjectObserver observer, out object instance)
        {
            try
            {
                instance = Resolve(resolver, observer);
                return null;
            }
            catch (Exception ex)
            {
                instance = null;
                return ex;
            }
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectObserver<T> observer, out T instance)
        {
            try
            {
                instance = Resolve(resolver, observer);
                return null;
            }
            catch (Exception ex)
            {
                instance = default(T);
                return ex;
            }
        }

        public static Exception TryResolve(this IObjectResolver resolver, IObjectCollectionObserver observer, out object[] instances)
        {
            try
            {
                instances = ResolveAll(resolver, observer);
                return null;
            }
            catch (Exception ex)
            {
                instances = null;
                return ex;
            }
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectCollectionObserver<T> observer, out T[] instances)
        {
            try
            {
                instances = ResolveAll(resolver, observer);
                return null;
            }
            catch (Exception ex)
            {
                instances = null;
                return ex;
            }
        }

        #endregion

        #region IObjectRegistration

        public static object Resolve(this IObjectResolver resolver, IObjectRegistration registration)
        {
            return resolver.Resolve(registration.ObjectBuilder, null);
        }

        public static T Resolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration)
        {
            return resolver.Resolve(registration.ObjectBuilder, null);
        }

        #region Resolve PositionalParameter

        public static object Resolve(this IObjectResolver resolver, IObjectRegistration registration, params PositionalParameter[] overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new PositionalParameterSet(overridenParameters));
        }

        public static object Resolve(this IObjectResolver resolver, IObjectRegistration registration, IList<PositionalParameter> overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new PositionalParameterSet(overridenParameters));
        }

        public static object Resolve(this IObjectResolver resolver, IObjectRegistration registration, IEnumerable<PositionalParameter> overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new PositionalParameterSet(overridenParameters.ToList()));
        }

        public static T Resolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, params PositionalParameter[] overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new PositionalParameterSet(overridenParameters));
        }

        public static T Resolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, IList<PositionalParameter> overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new PositionalParameterSet(overridenParameters));
        }

        public static T Resolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, IEnumerable<PositionalParameter> overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new PositionalParameterSet(overridenParameters.ToList()));
        }

        #endregion

        #region Resolve NamedParameter

        public static object Resolve(this IObjectResolver resolver, IObjectRegistration registration, params NamedParameter[] overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new NamedParameterSet(overridenParameters));
        }

        public static object Resolve(this IObjectResolver resolver, IObjectRegistration registration, IList<NamedParameter> overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new NamedParameterSet(overridenParameters));
        }

        public static object Resolve(this IObjectResolver resolver, IObjectRegistration registration, IEnumerable<NamedParameter> overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new NamedParameterSet(overridenParameters.ToList()));
        }

        public static T Resolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, params NamedParameter[] overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new NamedParameterSet(overridenParameters));
        }

        public static T Resolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, IList<NamedParameter> overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new NamedParameterSet(overridenParameters));
        }

        public static T Resolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, IEnumerable<NamedParameter> overridenParameters)
        {
            return resolver.Resolve(registration.ObjectBuilder, new NamedParameterSet(overridenParameters.ToList()));
        }

        #endregion

        public static Exception TryResolve(this IObjectResolver resolver, IObjectRegistration registration, out object instance)
        {
            return TryResolveWithObjectRegistration(resolver, registration, null, out instance);
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, out T instance)
        {
            return TryResolveWithObjectRegistration(resolver, registration, null, out instance);
        }

        #region TryResolve PositionalParameter

        public static Exception TryResolve(this IObjectResolver resolver, IObjectRegistration registration, out object instance, params PositionalParameter[] overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new PositionalParameterSet(overridenParameters), out instance);
        }

        public static Exception TryResolve(this IObjectResolver resolver, IObjectRegistration registration, out object instance, IList<PositionalParameter> overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new PositionalParameterSet(overridenParameters), out instance);
        }

        public static Exception TryResolve(this IObjectResolver resolver, IObjectRegistration registration, out object instance, IEnumerable<PositionalParameter> overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new PositionalParameterSet(overridenParameters.ToList()), out instance);
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, out T instance, params PositionalParameter[] overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new PositionalParameterSet(overridenParameters), out instance);
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, out T instance, IList<PositionalParameter> overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new PositionalParameterSet(overridenParameters), out instance);
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, out T instance, IEnumerable<PositionalParameter> overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new PositionalParameterSet(overridenParameters.ToList()), out instance);
        }
        
        #endregion

        #region TryResolve NamedParameter

        public static Exception TryResolve(this IObjectResolver resolver, IObjectRegistration registration, out object instance, params NamedParameter[] overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new NamedParameterSet(overridenParameters), out instance);
        }

        public static Exception TryResolve(this IObjectResolver resolver, IObjectRegistration registration, out object instance, IList<NamedParameter> overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new NamedParameterSet(overridenParameters), out instance);
        }

        public static Exception TryResolve(this IObjectResolver resolver, IObjectRegistration registration, out object instance, IEnumerable<NamedParameter> overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new NamedParameterSet(overridenParameters.ToList()), out instance);
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, out T instance, params NamedParameter[] overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new NamedParameterSet(overridenParameters), out instance);
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, out T instance, IList<NamedParameter> overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new NamedParameterSet(overridenParameters), out instance);
        }

        public static Exception TryResolve<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, out T instance, IEnumerable<NamedParameter> overridenParameters)
        {
            return TryResolveWithObjectRegistration(resolver, registration, new NamedParameterSet(overridenParameters.ToList()), out instance);
        }

        #endregion

        static Exception TryResolveWithObjectRegistration(this IObjectResolver resolver, IObjectRegistration registration, ParameterSet parameters, out object instance)
        {
            try
            {
                instance = resolver.Resolve(registration.ObjectBuilder, parameters);
                return null;
            }
            catch (Exception ex)
            {
                instance = null;
                return ex;
            }
        }

        static Exception TryResolveWithObjectRegistration<T>(this IObjectResolver resolver, IObjectRegistration<T> registration, ParameterSet parameters, out T instance)
        {
            try
            {
                instance = resolver.Resolve(registration.ObjectBuilder, parameters);
                return null;
            }
            catch (Exception ex)
            {
                instance = default(T);
                return ex;
            }
        }
        
        #endregion

        public static Exception TryResolve(IObjectResolver resolver, ObjectBuilder builder, ParameterSet parameters, out object instance)
        {
            try
            {
                instance = resolver.Resolve(builder, parameters);
            }
            catch (Exception ex)
            {
                instance = null;
                return ex;
            }
            return null;
        }

        public static Exception TryResolve<T>(IObjectResolver resolver, ObjectBuilder<T> builder, ParameterSet parameters, out T instance)
        {
            try
            {
                instance = resolver.Resolve(builder, parameters);
            }
            catch (Exception ex)
            {
                instance = default(T);
                return ex;
            }
            return null;
        }

        static string GetObjectBuilderNotFoundErrorMessage(Type contractType)
        {
            return string.Format(CultureInfo.InvariantCulture,
                Resources.NoObjectRegistrationsFoundForContractType, contractType.ToFullTypeName());
        }

        static string GetObsoleteObjectBuilderErrorMessage(Type contractType)
        {
            return string.Format(CultureInfo.InvariantCulture,
                Resources.NoObjectRegistrationsFoundForContractType, contractType.ToFullTypeName());
        }

        static string GetInvalidObjectBuilderErrorMessage(Type contractType)
        {
            return string.Format(CultureInfo.InvariantCulture,
                Resources.NoObjectRegistrationsFoundForContractType, contractType.ToFullTypeName());
        }
    }
}
