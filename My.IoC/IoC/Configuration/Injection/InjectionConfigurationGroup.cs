using System;
using System.Collections.Generic;
using My.Foundation;
using My.Helpers;
using My.IoC.Activities;
using My.IoC.Core;
using My.IoC.Dependencies;
using My.IoC.Injection.Func;

namespace My.IoC.Configuration.Injection
{
    // Contains information for constructor (required) and member (property or method, optional) injection
    public abstract class InjectionConfigurationGroup
    {
        protected readonly ObjectDescription MyDescription;
        IInjectionConfigurationInterpreter _interpreter;

        protected InjectionConfigurationGroup(ObjectDescription description)
        {
            MyDescription = description;
        }

        #region IInjectionConfigurationGroup Members

        public ObjectDescription ObjectDescription
        {
            get { return MyDescription; }
        }

        public IInjectionConfigurationInterpreter InjectionConfigurationInterpreter
        {
            get { return _interpreter; }
            set 
            {
                Requires.NotNull(value, "InjectionConfigurationInterpreter");
                if (this != value.InjectionConfigurationGroup)
                    throw new InvalidOperationException();
                _interpreter = value;
            }
        }

        public virtual string Id
        {
            get { return string.Empty; }
        }

        public virtual string Description
        {
            get { return string.Empty; }
        }

        #region Member Configurations

        public virtual int MemberInjectionConfigurationItemCount
        {
            get { return 0; }
        }

        public virtual IMemberInjectionConfigurationItem GetMemberInjectionConfigurationItem(int index)
        {
            return null;
        }

        public virtual void AddMemberInjectionConfigurationItem(IMemberInjectionConfigurationItem item)
        {
        }
        
        #endregion

        public abstract bool MatchInjectionConfigurationSet(InjectionConfigurationSet configSet);

        public void ReplaceConcreteType(Type newConcreteType)
        {
            MyDescription.ReplaceConcreteType(newConcreteType);
        }

        internal InjectionActivity<T> CreateInjectionActivity<T>(Kernel kernel, out List<DependencyProvider> dependencyProviders)
        {
            var interpreter = InjectionConfigurationInterpreter;
            if (interpreter == null)
                throw new InvalidOperationException("No InjectionConfigurationInterpreter specified!");
            var injector = interpreter.Parse<T>(kernel, MyDescription, out dependencyProviders);
            return new InjectionActivity<T>(injector);
        }

        #endregion
    }

    public sealed class TypedInjectionConfigurationGroup : InjectionConfigurationGroup
    {
        readonly IConstructorInjectionConfigurationItem _ctorConfigItem;
        List<IMemberInjectionConfigurationItem> _memberConfigItems;

        public TypedInjectionConfigurationGroup(ObjectDescription description, IConstructorInjectionConfigurationItem ctorConfigItem)
            : base(description)
        {
            Requires.NotNull(ctorConfigItem, "ctorConfigItem");
            if (!ctorConfigItem.MatchInjectionConfigurationGroup(this))
                throw new InvalidOperationException();
            _ctorConfigItem = ctorConfigItem;
        }

        public IConstructorInjectionConfigurationItem ConstructorInjectionConfigurationItem
        {
            get { return _ctorConfigItem; }
        }

        public IList<IMemberInjectionConfigurationItem> MemberInjectionConfigurationItems
        {
            get { return _memberConfigItems; }
        }

        public override string Id
        {
            get { return "TypedInjectionConfigurationGroup"; }
        }

        public override bool MatchInjectionConfigurationSet(InjectionConfigurationSet configSet)
        {
            return MyDescription.ContractType.IsAssignableFrom(_ctorConfigItem.Constructor.DeclaringType);
        }

        #region IInjectionConfigurationGroup Members

        public override int MemberInjectionConfigurationItemCount
        {
            get { return _memberConfigItems == null ? 0 : _memberConfigItems.Count; }
        }

        public override IMemberInjectionConfigurationItem GetMemberInjectionConfigurationItem(int index)
        {
            ThrowWhenNoMemberInjectionConfigurationItemsExisted();
            return _memberConfigItems[index];
        }

        public override void AddMemberInjectionConfigurationItem(IMemberInjectionConfigurationItem item)
        {
            Requires.NotNull(item, "item");
            if (!item.MatchInjectionConfigurationGroup(this))
                return;

            if (_memberConfigItems == null)
                _memberConfigItems = new List<IMemberInjectionConfigurationItem>();
            if (!_memberConfigItems.Contains(item))
                _memberConfigItems.Add(item);
        }

        void ThrowWhenNoMemberInjectionConfigurationItemsExisted()
        {
            if (_memberConfigItems == null || _memberConfigItems.Count == 0)
                throw new InvalidOperationException("");
        }

        #endregion
    }

    public sealed class InstanceInjectionConfigurationGroup : InjectionConfigurationGroup
    {
        readonly object _instance;

        public InstanceInjectionConfigurationGroup(ObjectDescription description, object instance)
            : base(description)
        {
            Requires.NotNull(instance, "instance");
            _instance = instance;
        }

        public object Instance
        {
            get { return _instance; }
        }

        public override string Id
        {
            get { return "InstanceInjectionConfigurationGroup"; }
        }

        public override bool MatchInjectionConfigurationSet(InjectionConfigurationSet configSet)
        {
            return MyDescription.ContractType.IsInstanceOfType(_instance);
        }
    }

    public sealed class FuncInjectionConfigurationGroup : InjectionConfigurationGroup
    {
        readonly Delegate _factory;

        public FuncInjectionConfigurationGroup(ObjectDescription description, Delegate factory)
            : base(description)
        {
            Requires.NotNull(factory, "factory");
            _factory = factory;
        }

        public Delegate Factory
        {
            get { return _factory; }
        }

        public override string Id
        {
            get { return "FuncInjectionConfigurationGroup"; }
        }

        public override bool MatchInjectionConfigurationSet(InjectionConfigurationSet configSet)
        {
            //Func<IResolutionContext, T>
            var delegateType = _factory.GetType();
            if (!delegateType.IsGenericType)
                return false;

            var genDef = delegateType.GetGenericTypeDefinition();
            if (genDef != typeof(Func<,>))
                return false;

            var genParams = delegateType.GetGenericArguments();
            if (genParams.Length != 2)
                return false;

            return genParams[0] == typeof(IResolutionContext) && genParams[1] == MyDescription.ContractType;
        }
    }
}