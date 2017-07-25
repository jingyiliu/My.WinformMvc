using System;
using System.Threading;
using My.Exceptions;
using My.Helpers;
using My.IoC.Activities;
using My.IoC.Configuration.Injection;
using My.IoC.Exceptions;
using My.IoC.Helpers;
using My.IoC.Lifetimes;

namespace My.IoC.Core
{
    public abstract class InjectionOperator
    {
        protected readonly ObjectDescription _description;

        internal InjectionOperator(ObjectDescription description)
        {
            _description = description;
        }

        /// <summary>
        /// Indicating whether this instance has ever been used to build instances successfully.
        /// </summary>
        internal abstract bool Resolved { get; }

        public ObjectDescription ObjectDescription
        {
            get { return _description; }
        }

        public virtual InjectionConfigurationSet InjectionConfigurationSet
        {
            get { return null; }
        }

        protected Exception CyclicDependencyException(InjectionContext currentContext)
        {
            var errMsg = ExceptionFormatter.Format(currentContext, Resources.CyclicDependencyFoundWhileBuildingInstanceForType, _description.ConcreteType.ToTypeName());
            return new CyclicDependencyException(errMsg);
        }
    }

    public abstract class InjectionOperator<T> : InjectionOperator
    {
        protected readonly ObjectBuilder<T> _builder;
        protected readonly Lifetime<T> _lifetime;

        internal InjectionOperator(ObjectBuilder<T> builder, ObjectDescription description, Lifetime<T> lifetime)
            : base(description)
        {
            _lifetime = lifetime;
            _builder = builder;
        }

        #region For calling by ObjectBuilder{T} only

        internal abstract void BuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance);
        internal abstract void BuildInstance(InjectionContext context, ParameterSet parameters, out T instance);

        #endregion

        #region For calling by Lifetime{T} only

        internal abstract void DoBuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance);
        internal abstract void DoBuildInstance(InjectionContext context, ParameterSet parameters, out T instance);

        #endregion
    }

    #region Unready ObjectBuilder

    abstract class UnreadyInjectionOperator<T> : InjectionOperator<T>
    {
        internal UnreadyInjectionOperator(ObjectBuilder<T> builder, ObjectDescription description, Lifetime<T> lifetime)
            : base(builder, description, lifetime)
        {
        }

        internal override void BuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance)
        {
            DoBuildInstance(scope, parameters, out instance);
        }

        internal override void BuildInstance(InjectionContext context, ParameterSet parameters, out T instance)
        {
            DoBuildInstance(context, parameters, out instance);
        }
    }

    // The purpose of this class:
    // 1. Check for missing dependency (InjectionConfigurationSet.CreateInjectionProcess) problem
    // 2. Check for cyclic dependency (InjectionContext.TryGetMatchingContext) problem
    // 3. Cache the DependencyProviders to the ObjectBuilderAdmin
    // 4. Create the NonSharedInjectionOperator/SharedInjectionOperator according to the return value of [TryGetMatchingContext] and replace itself
    // 5. Call the BuildInstance method of NonSharedInjectionOperator/SharedInjectionOperator to build the instance
    class InjectionOperatorBuilder<T> : UnreadyInjectionOperator<T>
    {
        readonly InjectionConfigurationSet _configurationSet;

        internal InjectionOperatorBuilder(ObjectBuilder<T> builder, ObjectDescription description, Lifetime<T> lifetime, InjectionConfigurationSet configurationSet)
            : base(builder, description, lifetime)
        {
            _configurationSet = configurationSet;
        }

        internal override bool Resolved
        {
            get { return false; }
        }

        public override InjectionConfigurationSet InjectionConfigurationSet
        {
            get { return _configurationSet; }
        }

        // A lock is only needed when the first time an instance is built 
        internal override void DoBuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance)
        {
            var injectionOperator = _builder.InjectionOperator;
            if (!ReferenceEquals(injectionOperator, this))
            {
                _builder.BuildInstance(scope, parameters, out instance);
                return;
            }

            Monitor.Enter(_builder.ObjectRelation.SyncRoot);
            injectionOperator = _builder.InjectionOperator;
            if (!ReferenceEquals(injectionOperator, this))
            {
                Monitor.Exit(_builder.ObjectRelation.SyncRoot);
                _builder.BuildInstance(scope, parameters, out instance);
                return;
            }

            try
            {
                var process = _configurationSet.CreateInjectionProcess<T>(scope.Kernel);
                var oneoff = new OneOffInjectionOperator<T>(_builder, _description, _lifetime, process);
                InjectionOperatorHelper.UpgradeToOneOffObjectBuilder(_builder, oneoff);
                oneoff.BuildInstance(scope, parameters, out instance);
            }
            finally
            {
                Monitor.Exit(_builder.ObjectRelation.SyncRoot);
            }
        }

        // A lock is only needed when the first time an instance is built 
        internal override void DoBuildInstance(InjectionContext context, ParameterSet parameters, out T instance)
        {
            var injectionOperator = _builder.InjectionOperator;
            if (!ReferenceEquals(injectionOperator, this))
            {
                _builder.BuildInstance(context, parameters, out instance);
                return;
            }

            Monitor.Enter(_builder.ObjectRelation.SyncRoot);
            injectionOperator = _builder.InjectionOperator;
            if (!ReferenceEquals(injectionOperator, this))
            {
                Monitor.Exit(_builder.ObjectRelation.SyncRoot);
                _builder.BuildInstance(context, parameters, out instance);
                return;
            }

            try
            {
                var process = _configurationSet.CreateInjectionProcess<T>(context.Kernel);
                var oneoff = new OneOffInjectionOperator<T>(_builder, _description, _lifetime, process);
                InjectionOperatorHelper.UpgradeToOneOffObjectBuilder(_builder, oneoff);
                oneoff.BuildInstance(context, parameters, out instance);
            }
            finally
            {
                Monitor.Exit(_builder.ObjectRelation.SyncRoot);
            }
        }
    }

    #endregion

    #region Ready ObjectBuilder

    abstract class ReadyInjectionOperator<T> : InjectionOperator<T>
    {
        internal ReadyInjectionOperator(ObjectBuilder<T> builder, ObjectDescription description, Lifetime<T> lifetime)
            : base(builder, description, lifetime)
        {
        }

        internal override void BuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance)
        {
            instance = _lifetime.BuildInstance(scope, this, parameters);
        }
        internal override void BuildInstance(InjectionContext context, ParameterSet parameters, out T instance)
        {
            instance = _lifetime.BuildInstance(context, this, parameters);
        }
    }

    class OneOffInjectionOperator<T> : ReadyInjectionOperator<T>
    {
        readonly InjectionProcess<T> _process;

        public OneOffInjectionOperator(ObjectBuilder<T> builder, ObjectDescription description, Lifetime<T> lifetime, InjectionProcess<T> process)
            : base(builder, description, lifetime)
        {
            _process = process;
        }

        internal override bool Resolved
        {
            get { return false; }
        }

        internal override void DoBuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance)
        {
            var context = new ViralSharedInjectionContext<T>(scope, _description, parameters);
            _process.Execute(context);
            instance = context.Instance;
            // replace the ObjectBuilder
            InjectionOperatorHelper.UpgradeToNonSharedObjectBuilder
                (_builder, new NonSharedInjectionOperator<T>(_builder, _description, _lifetime, _process));
        }

        internal override void DoBuildInstance(InjectionContext context, ParameterSet parameters, out T instance)
        {
            var matchingContext = context.ForceFindMatchingContext(_description);
            if (matchingContext != null)
            {
                if (!matchingContext.InstanceBuilt)
                {
                    ////// Replace the temporary one with this, so we won't create an OneOffObjectBuilder any more the next time.
                    ////InjectionOperatorHelper.UpgradeToOneOffObjectBuilder(MyBuilder, this);
                    //if (context.InstanceBuilt)
                    //{
                    //    var newContext = new ViralSharedInjectionContext<T>(context, MyDescription, parameters);
                    //    _process.Execute(newContext);

                    //    var myContext = matchingContext as InjectionContext<T>;
                    //    if (myContext == null)
                    //        throw new ImpossibleException();
                    //    instance = myContext.Instance = newContext.Instance;
                    //}
                    //else

                    // The current BuildOperation depends on a parent context that has the same ObjectDescription with itself,
                    // yet the parent context does not have a built instance either, that means a cyclic dependency problem.
                    throw CyclicDependencyException(new FakeInjectionContext(context, _description, parameters));
                }
                else
                {
                    var myContext = matchingContext as InjectionContext<T>;
                    if (myContext == null)
                        throw new ImpossibleException();
                    instance = myContext.Instance;
                    // replace the InjectionOperator
                    InjectionOperatorHelper.UpgradeToSharedObjectBuilder(_builder, new SharedInjectionOperator<T>(_builder, _description, _lifetime, _process));
                }
            }
            else
            {
                // This ObjectBuilder has not built its first instance yet, so we must use a ViralSharedInjectionContext, 
                // instead of a NonViralSharedInjectionContext, no matter what kind of InjectionContext the parent context is.
                instance = context.BuildAndShareInstance(_process, _description, parameters);
                // replace the InjectionOperator
                InjectionOperatorHelper.UpgradeToNonSharedObjectBuilder(_builder, new NonSharedInjectionOperator<T>(_builder, _description, _lifetime, _process));
            }
        }
    }

    // Use to build instance with no constructor-member dependency relationship (no constructor-constructor dependency, i.e, cyclic dependency)
    class NonSharedInjectionOperator<T> : ReadyInjectionOperator<T>
    {
        readonly InjectionProcess<T> _process;

        public NonSharedInjectionOperator(ObjectBuilder<T> builder, ObjectDescription description, Lifetime<T> lifetime, InjectionProcess<T> process)
            : base(builder, description, lifetime)
        {
            _process = process;
        }

        internal override bool Resolved
        {
            get { return true; }
        }

        internal override void DoBuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance)
        {
            var context = new NonViralSharedInjectionContext<T>(scope, _description, parameters);
            _process.Execute(context);
            instance = context.Instance;
        }

        internal override void DoBuildInstance(InjectionContext context, ParameterSet parameters, out T instance)
        {
            var matchingContext = context.FindMatchingContext(_description);
            if (matchingContext != null)
            {
                if (!matchingContext.InstanceBuilt)
                    // The current BuildOperation depends on a parent context that has the same ObjectDescription with itself,
                    // yet the parent context does not have a built instance either, that means a cyclic dependency problem.
                    throw CyclicDependencyException(new FakeInjectionContext(context, _description, parameters));

                var myContext = matchingContext as InjectionContext<T>;
                if (myContext == null)
                    throw new ImpossibleException();
                instance = myContext.Instance;

                // If we do find a matching context, then turn this into a shared one.
                InjectionOperatorHelper.UpgradeToSharedObjectBuilder(_builder, new SharedInjectionOperator<T>(_builder, _description, _lifetime, _process));
            }
            else
            {
                instance = context.BuildInstance(_process, _description, parameters);
            }
        }
    }

    // Use to build instance with constructor-member dependency relationship (no constructor-constructor dependency, i.e, cyclic dependency)
    class SharedInjectionOperator<T> : ReadyInjectionOperator<T>
    {
        readonly InjectionProcess<T> _process;

        public SharedInjectionOperator(ObjectBuilder<T> builder, ObjectDescription description, Lifetime<T> lifetime, InjectionProcess<T> process)
            : base(builder, description, lifetime)
        {
            _process = process;
        }

        internal override bool Resolved
        {
            get { return true; }
        }

        internal override void DoBuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance)
        {
            var context = new NonViralSharedInjectionContext<T>(scope, _description, parameters);
            _process.Execute(context);
            instance = context.Instance;
        }

        internal override void DoBuildInstance(InjectionContext context, ParameterSet parameters, out T instance)
        {
            var matchingContext = context.ForceFindMatchingContext(_description);
            if (matchingContext != null)
            {
                if (!matchingContext.InstanceBuilt)
                    // The current BuildOperation depends on a parent context that has the same ObjectDescription with itself,
                    // yet the parent context does not have a built instance either, that means a cyclic dependency problem.
                    throw CyclicDependencyException(new FakeInjectionContext(context, _description, parameters));

                var myContext = matchingContext as InjectionContext<T>;
                if (myContext == null)
                    throw new ImpossibleException();
                instance = myContext.Instance;
            }
            else
            {
                instance = context.BuildInstance(_process, _description, parameters);
            }
        }
    }

    #endregion

    static class InjectionOperatorHelper
    {
        public static void UpgradeToOneOffObjectBuilder<T>(ObjectBuilder<T> builder, OneOffInjectionOperator<T> oneoff)
        {
            builder.SetInjectionOperator(oneoff);
        }

        public static void UpgradeToNonSharedObjectBuilder<T>(ObjectBuilder<T> builder, NonSharedInjectionOperator<T> nonShared)
        {
            var operatorType = builder.InjectionOperator.GetType();
            if (operatorType == typeof(NonSharedInjectionOperator<T>) || operatorType == typeof(SharedInjectionOperator<T>))
                return;
            builder.SetInjectionOperator(nonShared);
        }

        public static void UpgradeToSharedObjectBuilder<T>(ObjectBuilder<T> builder, SharedInjectionOperator<T> shared)
        {
            var operatorType = builder.InjectionOperator.GetType();
            if (operatorType == typeof(SharedInjectionOperator<T>))
                return;
            builder.SetInjectionOperator(shared);
        }
    }
}