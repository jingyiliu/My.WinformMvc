
using System;
using My.IoC.Condition;
using My.IoC.Configuration.Injection;
using My.IoC.Core;
using My.IoC.Lifetimes;

namespace My.IoC.Configuration.FluentApi
{
    public interface ICommonConfigurationApi : IWhenApi
    {
    }



    // Configuration Api for Conditional Binding
    public interface IWhenApi : IInApi
    {
        //IInApi WhenHasNoTarget();
        IInApi WhenInjectedInto(params Type[] parentTypes);
        IInApi WhenInjectedInto(Type parentType);
        IInApi WhenInjectedInto<TParent>();

        IInApi WhenInjectedExactlyInto(params Type[] parentTypes);
        IInApi WhenInjectedExactlyInto(Type parentType);
        IInApi WhenInjectedExactlyInto<TParent>();

        IInApi WhenMatches(Predicate<IInjectionTargetInfo> condition);
        IInApi WhenMetadataMatches(Predicate<object> metadataCondition);
        IInApi WhenMetadataIs(object metadata);

        IInApi WhenTargetHasAttribute(Type attributeType);
        IInApi WhenTargetHasAttribute<TAttribute>() where TAttribute : Attribute;
        IInApi WhenTargetNamed(string name);
    }
    // Configuration Api for Lifetime
    public interface IInApi : IMatadataApi
    {
        IMatadataApi In(ILifetimeProvider lifetimeProvider);
    }
    public interface IMatadataApi : IRankApi
    {
        IRankApi Matadata(object metadata);
    }
    public interface IRankApi : IReturnApi
    {
        IReturnApi Ranking(int rank);
    }
    // Configuration Api for IObjectBuilderRegistration
    public interface IReturnApi : IApi
    {
        IApi Return(out IObjectRegistration registration);
        IApi Return<T>(out IObjectRegistration<T> registration);
    }
    public interface IApi
    {
        Kernel Kernel { get; }
        InjectionConfigurationSet InjectionConfigurationSet { get; }
    }
}
