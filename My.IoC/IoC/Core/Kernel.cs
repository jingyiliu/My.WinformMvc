
using System;
using System.Collections.Generic;
using My.IoC.Condition;
using My.IoC.Extensions.OpenGeneric;
using My.Helpers;
using My.IoC.Injection.Emit;
using My.IoC.Mapping;
using My.IoC.Observers;
using My.IoC.Registry;
using My.IoC.Utils;

namespace My.IoC.Core
{
    /// <summary>
    /// This is the core and facade of the <c>My.IoC</c>, which exposes all the functionality needed 
    /// to register/resolve/unregister services and plugins.
    /// </summary>
    public sealed class Kernel
    {
        readonly ContainerOption _containerOption;
        readonly ThreadedRegistrationCommitter _threadedCommitter;
        readonly ObjectBuilderRegistry _registry;
        readonly EmitInjectorManager _emitInjectorManager;
        readonly ComponentContainer _componentContainer;
        readonly ObjectMapperManager _objectMapperManager;
        readonly AutoObjectRegistrar _autoObjectRegistrar;

        internal Kernel(ContainerOption containerOption, AutoObjectRegistrar autoObjectRegistrar)
        {
            Requires.NotNull(containerOption, "containerOption");
            Requires.NotNull(autoObjectRegistrar, "autoObjectRegistrar");
            _containerOption = containerOption;
            _autoObjectRegistrar = autoObjectRegistrar;

            _emitInjectorManager = new EmitInjectorManager();
            _componentContainer = new ComponentContainer();
            _objectMapperManager = new ObjectMapperManager();
            _threadedCommitter = new ThreadedRegistrationCommitter();
            _registry = new ObjectBuilderRegistry(this);
        }

        /// <summary>
        /// Gets the configuration options for the <b>My.IoC</b>.
        /// </summary>
        public ContainerOption ContainerOption
        {
            get { return _containerOption; }
        }

        /// <summary>
        /// Provides a convenient way to add, retrieve and remove custom components
        /// (such as <see cref="OpenGenericRequestHandler"/>).
        /// </summary>
        public ComponentContainer ComponentContainer
        {
            get { return _componentContainer; }
        }

        /// <summary>
        /// Provides a convenient way to cache, retrieve and remove instances.
        /// </summary>
        internal ObjectMapperManager ObjectMapperManager
        {
            get { return _objectMapperManager; }
        }

        internal AutoObjectRegistrar AutoObjectRegistrar
        {
            get { return _autoObjectRegistrar; }
        }

        internal ObjectBuilderRegistry ObjectBuilderRegistry
        {
            get { return _registry; }
        }

        internal EmitInjectorManager EmitInjectorManager
        {
            get { return _emitInjectorManager; }
        }

        internal RegistrationCommitter GetRegistrationCommitter()
        {
            return _threadedCommitter.GetRegistrationCommitter();
        }

        public void Register(IObjectRegistration registration)
        {
            _registry.Register(registration);
        }
        public void Register(IEnumerable<IObjectRegistration> registrations)
        {
            _registry.Register(registrations);
        }
        public void Unregister(IObjectRegistration registration)
        {
            _registry.Unregister(registration);
        }
        public void Unregister(IEnumerable<IObjectRegistration> registrations)
        {
            _registry.Unregister(registrations);
        }

        #region Internal methods

        internal bool TryGet(Type injectable, IInjectionTargetInfo targetInfo, out ObjectCollectionObserver observer)
        {
            return _registry.TryGet(injectable, targetInfo, out observer);
        }
        internal ObjectBuilderState TryGet(Type injectable, IInjectionTargetInfo targetInfo, out ObjectBuilder builder)
        {
            return _registry.TryGet(injectable, targetInfo, out builder);
        }

        internal ObjectBuilderState TryGet(Type contract, out ObjectBuilder builder)
        {
            return _registry.TryGet(contract, out builder);
        }
        internal bool TryGet(Type contract, out IEnumerable<ObjectBuilder> builders)
        {
            return _registry.TryGet(contract, out builders);
        }
        internal bool TryGet<T>(Type contract, out IEnumerable<ObjectBuilder<T>> builders)
        {
            return _registry.TryGet(contract, out builders);
        }

        //static ObjectBuilderNotFoundException CreateObjectBuilderNotFoundException(Type contract, string id, IConditionInfo info)
        //{
        //    return new ObjectBuilderNotFoundException(GetErrorMessage(contract, id, info));
        //}
        //static string GetErrorMessage(Type contract, string id, IConditionInfo info)
        //{
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        return ExceptionFormatter.Format("Can not find an ObjectBuilder with contract type {0} and id {1}!",
        //            contract.ToFullTypeName(), id);
        //    }
        //    if (info != null)
        //    {
        //        return ExceptionFormatter.Format("Can not find an ObjectBuilder for injecting into the target {0} with contract type {1}!",
        //            info.TargetName, contract.ToFullTypeName());
        //    }

        //    return ExceptionFormatter.Format("Can not find an ObjectBuilder with contract type {0}!", contract.ToFullTypeName());
        //}

        #endregion

        public bool TryGet(Type contract, out IObjectObserver observer)
        {
            return _registry.TryGet(contract, out observer);
        }
        public bool TryGet<T>(Type contract, out IObjectObserver<T> observer)
        {
            return _registry.TryGet(contract, out observer);
        }
        public bool TryGet(Type contract, out IObjectCollectionObserver observer)
        {
            return _registry.TryGet(contract, out observer);
        }
        public bool TryGet<T>(Type contract, out IObjectCollectionObserver<T> observer)
        {
            return _registry.TryGet(contract, out observer);
        }

        public bool Contains(Type contract)
        {
            return _registry.Contains(contract);
        }

        public IEnumerable<ObjectBuilder> ObjectBuilders
        {
            get { return _registry.ObjectBuilders; }
        }
    }
}
