
using System;
using My.IoC.Condition;
using My.IoC.Configuration.Injection;
using My.IoC.Core;
using My.IoC.Lifetimes;

namespace My.IoC
{
    public abstract class ObjectBuilder
    {
        internal readonly ObjectRelation _relation;

        internal ObjectBuilder(ObjectRelation relation)
        {
            _relation = relation;
            relation.SetObjectBuilder(this);
        }

        internal ObjectRelation ObjectRelation
        {
            get { return _relation; }
        }

        internal bool Obsolete
        {
            get { return _relation.Obsolete; }
        }

        public abstract ObjectDescription ObjectDescription { get; }
        public abstract InjectionConfigurationSet InjectionConfigurationSet { get; }

        internal abstract InjectionOperator InjectionOperator { get; }
        internal abstract bool MatchCondition(IInjectionTargetInfo targetInfo);
        internal abstract void Release();

        internal abstract void BuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out object instance);
        internal abstract void BuildInstance(InjectionContext context, ParameterSet parameters, out object instance);

        internal ObjectBuilder<T> ToGeneric<T>()
        {
            var generic = this as ObjectBuilder<T>;
            if (generic == null)
                throw new InvalidOperationException("");
            return generic;
        }
    }

    public class ObjectBuilder<T> : ObjectBuilder
    {
        InjectionOperator<T> _injectionOperator;

        internal ObjectBuilder(ObjectDescription description, ObjectRelation admin, Lifetime<T> lifetime, InjectionConfigurationSet configurationSet)
            : base(admin)
        {
            _injectionOperator = new InjectionOperatorBuilder<T>(this, description, lifetime, configurationSet);
        }

        internal override InjectionOperator InjectionOperator
        {
            get { return _injectionOperator; }
        }

        public override InjectionConfigurationSet InjectionConfigurationSet
        {
            get
            {
                ThrowWhenInjectionOperatorIsNull();
                return _injectionOperator.InjectionConfigurationSet;
            }
        }

        public override ObjectDescription ObjectDescription
        {
            get
            {
                ThrowWhenInjectionOperatorIsNull();
                return _injectionOperator.ObjectDescription;
            }
        }

        void ThrowWhenInjectionOperatorIsNull()
        {
            if (_injectionOperator == null)
                throw new InvalidOperationException("");
        }

        internal override void Release()
        {
            _injectionOperator = null;
            _relation.Release();
        }

        internal override bool MatchCondition(IInjectionTargetInfo targetInfo)
        {
            // targetInfo == null: not requested by another ObjectBuilder
            // !ReferenceEquals(ObjectDescription, targetInfo.TargetDescription): not requested by itself, although it can be requested by another ObjectBuilder
            return targetInfo == null || !ReferenceEquals(ObjectDescription, targetInfo.TargetDescription);
        }

        #region For calling by user

        internal override void BuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out object instance)
        {
            T ins;
            _injectionOperator.BuildInstance(scope, parameters, out ins);
            instance = ins;
        }
        internal void BuildInstance(ISharingLifetimeScope scope, ParameterSet parameters, out T instance)
        {
            _injectionOperator.BuildInstance(scope, parameters, out instance);
        }

        #endregion

        #region For calling by container only

        internal void SetInjectionOperator(InjectionOperator<T> injectionOperator)
        {
            _injectionOperator = injectionOperator;
        }

        internal override void BuildInstance(InjectionContext context, ParameterSet parameters, out object instance)
        {
            T ins;
            _injectionOperator.BuildInstance(context, parameters, out ins);
            instance = ins;
        }
        internal void BuildInstance(InjectionContext context, ParameterSet parameters, out T instance)
        {
            _injectionOperator.BuildInstance(context, parameters, out instance);
        }

        #endregion
    }

    class ObjectBuilderWithCondition<T> : ObjectBuilder<T>
    {
        readonly IInjectionCondition _condition;

        internal ObjectBuilderWithCondition(ObjectDescription description, ObjectRelation admin, Lifetime<T> lifetime,
            InjectionConfigurationSet configurationSet, IInjectionCondition condition)
            : base(description, admin, lifetime, configurationSet)
        {
            _condition = condition;
        }

        internal override bool MatchCondition(IInjectionTargetInfo targetInfo)
        {
            return _condition.Match(targetInfo) && base.MatchCondition(targetInfo);
        }
    }
}
