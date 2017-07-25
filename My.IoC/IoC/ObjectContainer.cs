
using System;
using My.Foundation;
using My.IoC.Configuration;
using My.IoC.Core;
using My.IoC.Helpers;
using My.IoC.Lifetimes;
using My.IoC.Mapping;

namespace My.IoC
{
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public class ObjectContainer : Disposable, IObjectContainer
    {
        readonly Kernel _kernel;
        readonly ContainerLifetimeScope _containerScope;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContainer"/> class.
        /// </summary>
        /// <param name="useLightweightCodeGeneration">if set to <c>true</c>, use lightweight code generation to create object instances.
        /// Otherwise, use the <see cref="System.Reflection"/> instead.</param>
        public ObjectContainer(bool useLightweightCodeGeneration)
            : this(new ContainerOption(useLightweightCodeGeneration))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContainer"/> class.
        /// </summary>
        /// <param name="option">The option.</param>
        public ObjectContainer(ContainerOption option)
        {
            var autoRegistrar = new AutoObjectRegistrar(this, option.AutoRegistrationPolicies);
            _kernel = new Kernel(option, autoRegistrar);
            _containerScope = new ContainerLifetimeScope(_kernel);
            var threadId = SystemHelper.InitialThreadId; //force to initialize the static member

            AddDefaultMapperBuilders();
        }

        void AddDefaultMapperBuilders()
        {
            var listMapperBuilder = new ListMapperBuilder();
            var iEnumerableMapperBuilder = new IEnumerableMapperBuilder();
            var queueMapperBuilder = new QueueMapperBuilder();
            var stackMapperBuilder = new StackMapperBuilder();
            var iListMapperBuilder = new IListMapperBuilder();

            _kernel.ObjectMapperManager.Add(listMapperBuilder);
            _kernel.ObjectMapperManager.Add(iEnumerableMapperBuilder);
            _kernel.ObjectMapperManager.Add(queueMapperBuilder);
            _kernel.ObjectMapperManager.Add(stackMapperBuilder);
            _kernel.ObjectMapperManager.Add(iListMapperBuilder);
        }

        /// <summary>
        /// Gets the kernel, which exposes all the functionality needed to register/resolve/unregister
        /// services and plug-ins.
        /// </summary>
        public Kernel Kernel
        {
            get { return _kernel; }
        }

        /// <summary>
        /// Occurs after a new service has been registered. The observer can use this event to get notified when the desired
        /// services is available.
        /// </summary>
        /// <remarks>This event is mainly for notification.</remarks>
        public event System.Action<ObjectRegisteredEventArgs> ObjectRegistered
        {
            add { _kernel.ObjectBuilderRegistry.ObjectBuilderRegistered += value; }
            remove { _kernel.ObjectBuilderRegistry.ObjectBuilderRegistered -= value; }
        }

        /// <summary>
        /// Occurs after a new service has been registered. The observer can use this event to get notified when the desired
        /// services is available.
        /// </summary>
        /// <remarks>This event is mainly for notification.</remarks>
        public event System.Action<ObjectUnregisteringEventArgs> ObjectUnregistering
        {
            add { _kernel.ObjectBuilderRegistry.ObjectBuilderUnregistering += value; }
            remove { _kernel.ObjectBuilderRegistry.ObjectBuilderUnregistering -= value; }
        }

        /// <summary>
        /// Occurs when a service of specified type is requested which has not been registered explicitly. The observer
        /// can use this event to registered a service to satisfy the request.
        /// </summary>
        public event System.Action<ObjectRequestedEventArgs> ObjectRequested
        {
            add { _kernel.ObjectBuilderRegistry.ObjectBuilderRequested += value; }
            remove { _kernel.ObjectBuilderRegistry.ObjectBuilderRequested -= value; }
        }

        /// <summary>
        /// Creates a lifetime scope which can be used to deterministically dispose resolved instances. The object instances
        /// created via this scope will be disposed along with it.
        /// </summary>
        /// <returns>
        /// A new lifetime scope.
        /// </returns>
        public ILifetimeScope BeginLifetimeScope()
        {
            return _containerScope.BeginLifetimeScope();
        }

        #region IObjectRegistrar Members

        public void Register(IRegistrationProvider provider)
        {
            var commiter = _kernel.GetRegistrationCommitter();
            commiter.AddRegistration(provider);
        }

        public void CommitRegistrations()
        {
            var commiter = _kernel.GetRegistrationCommitter();
            commiter.CommitRegistrations(_kernel);
        }

        #endregion

        #region IObjectResolver Members

        public object Resolve(ObjectBuilder builder, ParameterSet parameters)
        {
            return _containerScope.Resolve(builder, parameters);
        }

        public T Resolve<T>(ObjectBuilder<T> builder, ParameterSet parameters)
        {
            return _containerScope.Resolve(builder, parameters);
        }

        #endregion

        /// <summary>
        /// Releases resources
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _containerScope.Dispose();
            //DisposeUnmanagedResources();
        }

#if DEBUG
        public void SaveDynamicAssembly()
        {
            _kernel.EmitInjectorManager.SaveDynamicAssembly();
        }
#endif
    }
}
