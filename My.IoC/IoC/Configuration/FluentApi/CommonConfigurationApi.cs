
using System;
using My.IoC.Condition;
using My.IoC.Configuration.Injection;
using My.IoC.Configuration.Provider;
using My.IoC.Core;
using My.IoC.Lifetimes;

namespace My.IoC.Configuration.FluentApi
{
    public abstract class CommonConfigurationApi : ICommonConfigurationApi, IConfigurationApi
    {
        RegistrationProvider _provider;

        public IRegistrationProvider GetRegistrationProvider()
        {
            return _provider;
        }

        protected void SetRegistrationProvider(RegistrationProvider provider)
        {
            _provider = provider;
        }

        public Kernel Kernel
        {
            get { return _provider.Kernel; }
        }

        public InjectionConfigurationSet InjectionConfigurationSet
        {
            get { return _provider.InjectionConfigurationSet; }
        }

        #region IWhenApi Members

        IInApi IWhenApi.WhenMatches(Predicate<IInjectionTargetInfo> condition)
        {
            _provider.InjectionCondition = new PredicateInjectionCondition(condition);
            return this;
        }

        IInApi IWhenApi.WhenMetadataMatches(Predicate<object> metadataCondition)
        {
            _provider.InjectionCondition = new MetadataPredicateInjectionCondition(metadataCondition);
            return this;
        }

        IInApi IWhenApi.WhenMetadataIs(object metadata)
        {
            _provider.InjectionCondition = new MetadataIsInjectionCondition(metadata);
            return this;
        }

        IInApi IWhenApi.WhenInjectedExactlyInto(params Type[] parentTypes)
        {
            _provider.InjectionCondition = new ExactlyTargetTypesInjectionCondition(parentTypes);
            return this;
        }

        IInApi IWhenApi.WhenInjectedExactlyInto(Type parentType)
        {
            _provider.InjectionCondition = new ExactlyTargetTypeInjectionCondition(parentType);
            return this;
        }

        IInApi IWhenApi.WhenInjectedExactlyInto<TParent>()
        {
            _provider.InjectionCondition = new ExactlyTargetTypeInjectionCondition(typeof(TParent));
            return this;
        }

        IInApi IWhenApi.WhenInjectedInto(params Type[] parentTypes)
        {
            _provider.InjectionCondition = new TargetTypesInjectionCondition(parentTypes);
            return this;
        }

        IInApi IWhenApi.WhenInjectedInto(Type parentType)
        {
            _provider.InjectionCondition = new TargetTypeInjectionCondition(parentType);
            return this;
        }

        IInApi IWhenApi.WhenInjectedInto<TParent>()
        {
            _provider.InjectionCondition = new TargetTypeInjectionCondition(typeof(TParent));
            return this;
        }

        IInApi IWhenApi.WhenTargetHasAttribute(Type attributeType)
        {
            _provider.InjectionCondition = new TargetAttributeInjectionCondition(attributeType);
            return this;
        }

        IInApi IWhenApi.WhenTargetHasAttribute<TAttribute>()
        {
            _provider.InjectionCondition = new TargetAttributeInjectionCondition(typeof(TAttribute));
            return this;
        }

        IInApi IWhenApi.WhenTargetNamed(string name)
        {
            _provider.InjectionCondition = new TargetNameInjectionCondition(name);
            return this;
        }

        #endregion

        #region IInApi Members

        public IMatadataApi In(ILifetimeProvider lifetimeProvider)
        {
            _provider.LifetimeProvider = lifetimeProvider;
            return this;
        }

        #endregion

        #region IMatadataApi Members

        IRankApi IMatadataApi.Matadata(object metadata)
        {
            _provider.Metadata = metadata;
            return this;
        }

        #endregion

        #region IRankApi Members

        IReturnApi IRankApi.Ranking(int rank)
        {
            _provider.Ranking = rank;
            return this;
        }

        #endregion

        #region IReturnApi Members

        IApi IReturnApi.Return(out IObjectRegistration registration)
        {
            registration = _provider.CreateObjectRegistration();
            return this;
        }

        IApi IReturnApi.Return<T>(out IObjectRegistration<T> registration)
        {
            var reg = _provider.CreateObjectRegistration();
            registration = (IObjectRegistration<T>)reg;
            return this;
        }

        #endregion
    }
}
