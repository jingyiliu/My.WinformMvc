using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using My.Helpers;
using My.IoC.Condition;
using My.IoC.Configuration.Injection;
using My.IoC.Core;
using My.IoC.Exceptions;
using My.IoC.Lifetimes;

namespace My.IoC.Configuration.Provider
{
    class LifetimeHelper
    {
        internal static Lifetime<T> CreateLifetime<T>(ILifetimeProvider provider)
        {
            if (provider == null)
            {
                provider = GetDefaultLifetimeProvider(typeof(T)); 
                return provider.GetLifetime<T>();
            }
            VerifyLifetime(provider, typeof(T));
            return provider.GetLifetime<T>();
        }

        static ILifetimeProvider GetDefaultLifetimeProvider(Type instanceType)
        {
            if (typeof(IDisposable).IsAssignableFrom(instanceType))
                return new ScopeLifetimeProvider();
            return new TransientLifetimeProvider();
        }

        static void VerifyLifetime(ILifetimeProvider provider, Type instanceType)
        {
            if (!typeof(IDisposable).IsAssignableFrom(instanceType))
                return;
            var lifetimeType = provider.GetType();
            if (lifetimeType != typeof(ScopeLifetimeProvider)
                && lifetimeType != typeof(ContainerLifetimeProvider))
                throw new InvalidLifetimeScopeException(string.Format("The type [{0}] implements the [System.IDisposable], which should be registered in scope or container life time!", instanceType.FullName));
        }
    }

    public abstract class RegistrationProvider : IRegistrationProvider
    {
        readonly Kernel _kernel;
        ILifetimeProvider _lifetimeProvider;
        IInjectionCondition _injectionCondition;
        object _metadata;
        Type _contractType;
        Type _concreteType;
        ObjectDescription _description;
        ObjectRelation _admin;
        InjectionConfigurationSet _injectionConfigSet;

        protected RegistrationProvider(Kernel kernel)
        {
            _kernel = kernel;
        }

        #region ObjectDescription

        public Kernel Kernel
        {
            get { return _kernel; }
        }

        public Type ContractType
        {
            get { return _contractType; }
            set
            {
                Requires.NotNull(value, "ContractType");
                _contractType = value;
            }
        }

        public Type ConcreteType
        {
            get { return _concreteType; }
            set
            {
                Requires.NotNull(value, "ConcreteType");
                _concreteType = value;
            }
        }

        public object Metadata
        {
            get { return _metadata; }
            set
            {
                Requires.NotNull(value, "Metadata");
                _metadata = value;
            }
        }

        public int Ranking { get; set; }

        protected ObjectDescription CreateObjectDescription()
        {
            _description = _description ?? DoCreateObjectDescription();
            return _description;
        }

        ObjectDescription DoCreateObjectDescription()
        {
            if (Ranking != 0)
            {
                if (Metadata == null)
                    return new ObjectDescriptionWithRanking(_contractType, _concreteType, Ranking);
                return new ObjectDescriptionWithRankingAndMetadata(_contractType, _concreteType, Ranking, Metadata);
            }
            if (Metadata == null)
                return new ObjectDescription(_contractType, _concreteType);
            return new ObjectDescriptionWithMetadata(_contractType, _concreteType, Metadata);
        } 

        #endregion

        public ILifetimeProvider LifetimeProvider
        {
            get { return _lifetimeProvider; }
            set
            {
                Requires.NotNull(value, "LifetimeProvider");
                _lifetimeProvider = value;
            }
        }

        public IInjectionCondition InjectionCondition
        {
            get { return _injectionCondition; }
            set
            {
                Requires.NotNull(value, "InjectionCondition");
                _injectionCondition = value;
            }
        }

        #region RegistrationAdmin

        internal ObjectRelation CreateRegistrationAdmin()
        {
            _admin = _admin ?? DoCreateRegistrationAdmin();
            return _admin;
        }
        internal abstract ObjectRelation DoCreateRegistrationAdmin();

        #endregion

        #region InjectionConfigurationSet

        public InjectionConfigurationSet InjectionConfigurationSet
        {
            get
            {
                _injectionConfigSet = _injectionConfigSet ?? CreateInjectionConfigurationSet(CreateObjectDescription(), CreateRegistrationAdmin());
                return _injectionConfigSet;
            }
        }
        internal abstract InjectionConfigurationSet CreateInjectionConfigurationSet(ObjectDescription description, ObjectRelation admin);
        
        #endregion

        public abstract IObjectRegistration CreateObjectRegistration();
    }

    public abstract class RegistrationProvider<T> : RegistrationProvider
    {
        IObjectRegistration _registration;

        protected RegistrationProvider(Kernel kernel)
            :base(kernel)
        {
        }

        public override IObjectRegistration CreateObjectRegistration()
        {
            if (_registration != null)
                return _registration;

            var builder = CreateObjectBuilder();
            _registration = new ObjectRegistration<T>(builder);
            return _registration;
        }

        internal override ObjectRelation DoCreateRegistrationAdmin()
        {
            return new ObjectRelation();
        }

        ObjectBuilder<T> CreateObjectBuilder()
        {
            var admin = CreateRegistrationAdmin();
            var description = CreateObjectDescription();
            var lifetime = CreateLifetime();
            var configurationSet = CreateInjectionConfigurationSet(description, admin);
            return InjectionCondition == null
                ? new ObjectBuilder<T>(description, admin, lifetime, configurationSet)
                : new ObjectBuilderWithCondition<T>(description, admin, lifetime, configurationSet, InjectionCondition);
        }

        protected abstract Lifetime<T> CreateLifetime();
    }

    public abstract class TypedRegistrationProvider : RegistrationProvider
    {
        IConstructorSelector _constructorSelector;
        ParameterSet _configuredParameters;
        List<IMemberInjectionConfigurationItem> _memberConfigItems;

        protected TypedRegistrationProvider(Kernel kernel)
            :base(kernel)
        {
        }

        internal override ObjectRelation DoCreateRegistrationAdmin()
        {
            return new ObjectRelationWithProviders();
        }

        public ActivatorKind ActivatorKind { get; internal set; }

        #region Constructor

        public IConstructorSelector ConstructorSelector
        {
            get { return _constructorSelector ?? Kernel.ContainerOption.ConstructorSelector; }
            set
            {
                Requires.NotNull(value, "ctorSelector");
                _constructorSelector = value;
            }
        }

        public ParameterSet ConfiguredParameters
        {
            get { return _configuredParameters; }
            set
            {
                Requires.NotNull(value, "configuredParameters");
                Requires.True(value.Length > 0, "The parameters length must be greater than 0!");
                _configuredParameters = value;
            }
        }

        protected ConstructorInfo GetConstructorInfo()
        {
            var constructor = ConstructorSelector.SelectConstructor(ConcreteType, _configuredParameters);
            if (constructor != null)
                return constructor;

            var errMsg = _configuredParameters == null
                ? GetParameterExceptionMessage(ConcreteType)
                : GetParameterExceptionMessage(ConcreteType, _configuredParameters);
            throw new ArgumentException(errMsg);
        }

        static string GetParameterExceptionMessage(Type targetType)
        {
            return string.Format(CultureInfo.InvariantCulture, ReflectionOrEmitRegistrationProvider.NoSuitableConstructorFound,
                    targetType.ToFullTypeName());
        }

        static string GetParameterExceptionMessage(Type targetType, ParameterSet configuredParameters)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(ReflectionOrEmitRegistrationProvider.CanNotGetConstructorWithProvidedParameters, targetType.ToTypeName());
            sb.AppendLine();
            for (var i = 0; i < configuredParameters.Length; i++)
            {
                var configuredParameter = configuredParameters[i];
                sb.Append(i + 1);
                sb.Append(") ");
                sb.AppendLine(ReferenceEquals(configuredParameter, PositionalParameter.Auto)
                    ? "PositionalParameter.Auto"
                    : configuredParameter.ParameterType.ToTypeName());
            }

            return sb.ToString();
        }

        #endregion

        public IEnumerable<IMemberInjectionConfigurationItem> MemberInjectionConfigurationItems
        {
            get { return _memberConfigItems; }
        }

        public void AddMemberInjectionConfigurationItem(IMemberInjectionConfigurationItem item)
        {
            if (_memberConfigItems == null)
                _memberConfigItems = new List<IMemberInjectionConfigurationItem>();
            _memberConfigItems.Add(item);
        }
    }
}
