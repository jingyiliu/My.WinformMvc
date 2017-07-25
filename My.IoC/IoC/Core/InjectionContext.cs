
using System;
using My.IoC.Activities;

namespace My.IoC.Core
{
    /// <summary>
    /// Represents the execution context in which the build operations run.
    /// </summary>
    public abstract class InjectionContext
    {
        readonly InjectionContext _parentContext;
        readonly ISharingLifetimeScope _scope;
        readonly ObjectDescription _description;
        readonly ParameterSet _parameters;

        internal InjectionContext(InjectionContext context, ObjectDescription description, ParameterSet parameters)
        {
            _parentContext = context;
            _scope = context.LifetimeScope;

            _description = description;
            _parameters = parameters;
        }

        internal InjectionContext(ISharingLifetimeScope scope, ObjectDescription description, ParameterSet parameters)
        {
            _scope = scope;
            _description = description;
            _parameters = parameters;
        }

        public Kernel Kernel
        {
            get { return _scope.Kernel; }
        }

        public ISharingLifetimeScope LifetimeScope
        {
            get { return _scope; }
        }

        public InjectionContext ParentContext
        {
            get { return _parentContext; }
        }

        public ObjectDescription ObjectDescription
        {
            get { return _description; }
        }
        
        public ParameterSet Parameters
        {
            get { return _parameters; }
        }

        public abstract bool InstanceBuilt { get; }

        public abstract InjectionContext FindMatchingContext(ObjectDescription description);

        public InjectionContext ForceFindMatchingContext(ObjectDescription description)
        {
            return DoFindMatchingContext(description);
        }

        // Create a NonViralSharedInjectionContext to build the instance, but if the current context is already a ViralSharedInjectionContext,
        // then it will create a ViralSharedInjectionContext also
        public abstract TCurrent BuildInstance<TCurrent>(InjectionProcess<TCurrent> process,
            ObjectDescription description, ParameterSet parameters);

        // Create a ViralSharedInjectionContext to build the instance, no matter what kind of InjectionContext the current context is
        public TCurrent BuildAndShareInstance<TCurrent>(InjectionProcess<TCurrent> process, ObjectDescription description, ParameterSet parameters)
        {
            var myContext = new ViralSharedInjectionContext<TCurrent>(this, description, parameters);
            process.Execute(myContext);
            return myContext.Instance;
        }

        protected InjectionContext DoFindMatchingContext(ObjectDescription description)
        {
            var context = ParentContext;

            if (context == null)
                return null;
            if (ReferenceEquals(context.ObjectDescription, description))
                return context;

            context = context.ParentContext;
            if (context == null)
                return null;
            if (ReferenceEquals(context.ObjectDescription, description))
                return context;

            context = context.ParentContext;
            if (context == null)
                return null;
            if (ReferenceEquals(context.ObjectDescription, description))
                return context;

            context = context.ParentContext;
            while (context != null)
            {
                if (ReferenceEquals(context.ObjectDescription, description))
                    return context;
            }

            return null;
        }
    }

    sealed class FakeInjectionContext : InjectionContext
    {
        internal FakeInjectionContext(InjectionContext context, ObjectDescription description, ParameterSet parameters)
            : base(context, description, parameters)
        {
        }

        public override bool InstanceBuilt
        {
            get { throw new NotImplementedException(); }
        }

        public override InjectionContext FindMatchingContext(ObjectDescription description)
        {
            throw new NotImplementedException();
        }

        public override TCurrent BuildInstance<TCurrent>(InjectionProcess<TCurrent> process, ObjectDescription description, ParameterSet parameters)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class InjectionContext<T> : InjectionContext
    {
        bool _instanceBuilt;
        T _instance;

        internal InjectionContext(InjectionContext context, ObjectDescription description, ParameterSet parameters)
            : base(context, description, parameters)
        {
        }

        internal InjectionContext(ISharingLifetimeScope scope, ObjectDescription description, ParameterSet parameters)
            : base(scope, description, parameters)
        {
        }

        public override bool InstanceBuilt 
        {
            get { return _instanceBuilt; }
        }

        // The base instance that is built by the first construction injector.
        // The Property/Method injection will target this value.
        // For example: Instance.MyProperty = SomeValue.

        // A decorated instance after apply Aop/Decorator, if there is any.
        // The Property/Method injection required by other ObjectBuilders will use this value.
        // For example: InstanceBuiltWithOtherObjectBuilder.OtherProperty = DecoratedInstance.
        public T Instance
        {
            get { return _instance; }
            internal set
            {
                if (!_instanceBuilt)
                    _instanceBuilt = true;
                _instance = value;
            }
        }
    }

    sealed class NonViralSharedInjectionContext<T> : InjectionContext<T>
    {
        internal NonViralSharedInjectionContext(InjectionContext context, ObjectDescription description, ParameterSet parameters)
            : base(context, description, parameters)
        {
        }

        internal NonViralSharedInjectionContext(ISharingLifetimeScope scope, ObjectDescription description, ParameterSet parameters)
            : base(scope, description, parameters)
        {
        }

        public override InjectionContext FindMatchingContext(ObjectDescription description)
        {
            return null;
        }

        public override TCurrent BuildInstance<TCurrent>(InjectionProcess<TCurrent> process, ObjectDescription description, ParameterSet parameters)
        {
            var myContext = new NonViralSharedInjectionContext<TCurrent>(this, description, parameters);
            process.Execute(myContext);
            return myContext.Instance;
        }
    }

    sealed class ViralSharedInjectionContext<T> : InjectionContext<T>
    {
        internal ViralSharedInjectionContext(InjectionContext context, ObjectDescription description, ParameterSet parameters)
            : base(context, description, parameters)
        {
        }

        internal ViralSharedInjectionContext(ISharingLifetimeScope scope, ObjectDescription description, ParameterSet parameters)
            : base(scope, description, parameters)
        {
        }

        public override InjectionContext FindMatchingContext(ObjectDescription description)
        {
            return DoFindMatchingContext(description);
        }

        public override TCurrent BuildInstance<TCurrent>(InjectionProcess<TCurrent> process, ObjectDescription description, ParameterSet parameters)
        {
            var myContext = new ViralSharedInjectionContext<TCurrent>(this, description, parameters);
            process.Execute(myContext);
            return myContext.Instance;
        }
    }
}
