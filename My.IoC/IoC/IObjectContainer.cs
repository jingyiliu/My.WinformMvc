
using System;
using System.Collections.Generic;
using My.Helpers;
using My.IoC.Condition;
using My.IoC.Configuration;
using My.IoC.Core;

namespace My.IoC
{
    public interface IObjectContainer : IManualObjectRegistrar, IObjectResolver, IDisposable
    {
        /// <summary>
        /// Occurs when a object instance of specified type is requested which has not been registered explicitly. The observer 
        /// can use this event to registered a service to satisfy the request. 
        /// </summary>
        /// <remarks>
        /// This event provides support for various features, like generic type resolution, lazy registration/resolution...
        /// </remarks>
        event Action<ObjectRequestedEventArgs> ObjectRequested;
        /// <summary>
        /// Occurs after a new service has been registered. The observer can use this event to check whether the desired 
        /// services is available.
        /// </summary>
        /// <remarks>This event is mainly for notification.</remarks>
        event Action<ObjectRegisteredEventArgs> ObjectRegistered;
        event Action<ObjectUnregisteringEventArgs> ObjectUnregistering;

        /// <summary>
        /// Creates a lifetime scope which can be used to deterministically dispose resolved instances. The object instances 
        /// created via this scope will be disposed along with it.
        /// </summary>
        /// <returns>A new lifetime scope.</returns>
        ILifetimeScope BeginLifetimeScope();

#if DEBUG
        void SaveDynamicAssembly();
#endif
    }

    public class ObjectRegisteredEventArgs
    {
        readonly ObjectBuilder _builder;

        public ObjectRegisteredEventArgs(ObjectBuilder builder)
        {
            Requires.NotNull(builder, "builder");
            _builder = builder;
        }

        public ObjectBuilder ObjectBuilder
        {
            get { return _builder; }
        }

        public ObjectDescription ObjectDescription
        {
            get { return _builder.ObjectDescription; }
        }

        public object ObjectMetadata
        {
            get { return _builder.ObjectDescription.Metadata; }
        }

        public Type ContractType
        {
            get { return _builder.ObjectDescription.ContractType; }
        }

        public Type ConcreteType
        {
            get { return _builder.ObjectDescription.ConcreteType; }
        }

        public int Ranking
        {
            get { return _builder.ObjectDescription.Ranking; }
        }
    }

    public class ObjectUnregisteringEventArgs
    {
        readonly ObjectDescription _description;

        public ObjectUnregisteringEventArgs(ObjectDescription description)
        {
            Requires.NotNull(description, "description");
            _description = description;
        }

        public ObjectDescription ObjectDescription
        {
            get { return _description; }
        }

        public object ObjectMetadata
        {
            get { return _description.Metadata; }
        }

        public Type ContractType
        {
            get { return _description.ContractType; }
        }

        public Type ConcreteType
        {
            get { return _description.ConcreteType; }
        }

        public int Ranking
        {
            get { return _description.Ranking; }
        }
    }

    public class ObjectRequestedEventArgs : IObjectRegistrar
    {
        bool _handled = false;
        readonly Kernel _kernel;
        readonly Type _contractType;
        readonly IInjectionTargetInfo _targetInfo;
        List<IRegistrationProvider> _providers;

        public ObjectRequestedEventArgs(Kernel kernel, Type contractType, IInjectionTargetInfo targetInfo)
            : this(kernel, contractType)
        {
            _targetInfo = targetInfo;
        }

        ObjectRequestedEventArgs(Kernel kernel, Type contractType)
        {
            Requires.NotNull(kernel, "kernel");
            Requires.NotNull(contractType, "contractType");
            _kernel = kernel;
            _contractType = contractType;
        }

        public Kernel Kernel
        {
            get { return _kernel; }
        }

        public Type ContractType
        {
            get { return _contractType; }
        }

        public IInjectionTargetInfo InjectionTargetInfo
        {
            get { return _targetInfo; }
        }

        public bool Handled
        {
            get { return _handled; }
        }

        #region IObjectRegistrar Members

        public void Register(IRegistrationProvider provider)
        {
            if (_providers == null)
                _providers = new List<IRegistrationProvider>();
            _providers.Add(provider);
            _handled = true;
        }

        internal void CommitRegistrations()
        {
            if (!_handled || _providers == null || _providers.Count == 0)
                throw new InvalidOperationException("");

            if (_providers.Count == 1)
                _kernel.Register(_providers[0].CreateObjectRegistration());
            else
            {
                var registrations = new List<IObjectRegistration>(_providers.Count);
                foreach (var provider in _providers)
                    registrations.Add(provider.CreateObjectRegistration());
                _kernel.Register(registrations);
            }
        }

        #endregion
    }
}
