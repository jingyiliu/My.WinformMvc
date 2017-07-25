
using System;
using System.Reflection;
using My.Foundation;
using My.Helpers;
using My.IoC.Core;
using My.IoC.Dependencies;
using My.IoC.Injection.Func;

namespace My.IoC.Configuration.Injection
{
    public enum MemberKind
    {
        Property,
        Method
    }

    public interface IMemberInjectionConfigurationItem : IEquatable<IMemberInjectionConfigurationItem>
    {
        MemberKind MemberKind { get; }
        int MemberMetadataToken { get; }
        // Only property/method injection is supported!
        MethodInfo InjectionMethod { get; }
        DependencyProvider[] CreateDependencyProviders(Kernel kernel, ObjectDescription description, bool strongTyped);
        bool MatchInjectionConfigurationGroup(InjectionConfigurationGroup configGroup);
    }

    public class AutowiredPropertyInjectionConfigurationItem : IMemberInjectionConfigurationItem
    {
        protected readonly PropertyInfo _property;

        public AutowiredPropertyInjectionConfigurationItem(PropertyInfo property)
        {
            Requires.NotNull(property, "property");
            _property = property;
        }

        #region IMemberInjectionConfigurationItem Members

        public MemberKind MemberKind
        {
            get { return MemberKind.Property; }
        }

        public int MemberMetadataToken
        {
            get { return _property.MetadataToken; }
        }

        public MethodInfo InjectionMethod
        {
            get { return _property.GetSetMethod(); }
        }

        public virtual DependencyProvider[] CreateDependencyProviders(Kernel kernel, ObjectDescription description, bool strongTyped)
        {
            var depProvider = DependencyProvider.CreatePropertyProvider(description, _property, strongTyped);
            depProvider.InjectObjectBuilders(kernel);
            return new DependencyProvider[] { depProvider };
        }

        public bool MatchInjectionConfigurationGroup(InjectionConfigurationGroup configGroup)
        {
            var description = configGroup.ObjectDescription;
            return description.ConcreteType == _property.DeclaringType // The property is defined in the ConcreteType directly
                || _property.DeclaringType.IsAssignableFrom(description.ConcreteType); // The property is defined in the base type of ConcreteType
        }

        #endregion

        #region IEquatable<IMemberInjectionConfigurationItem> Members

        public bool Equals(IMemberInjectionConfigurationItem other)
        {
            return MemberMetadataToken == other.MemberMetadataToken;
        }

        #endregion
    }

    public class WeakConstantPropertyInjectionConfigurationItem : AutowiredPropertyInjectionConfigurationItem
    {
        readonly object _propertyValue;

        public WeakConstantPropertyInjectionConfigurationItem(PropertyInfo property, object propertyValue)
            : base(property)
        {
            _propertyValue = propertyValue;
        }

        public override DependencyProvider[] CreateDependencyProviders(Kernel kernel, ObjectDescription description, bool strongTyped)
        {
            var depProvider = DependencyProvider.CreatePropertyProvider(_property, _propertyValue);
            depProvider.InjectObjectBuilders(kernel);
            return new DependencyProvider[] { depProvider };
        }
    }

    public class StrongConstantPropertyInjectionConfigurationItem<TProperty> : AutowiredPropertyInjectionConfigurationItem
    {
        readonly TProperty _propertyValue;

        public StrongConstantPropertyInjectionConfigurationItem(PropertyInfo property, TProperty propertyValue)
            : base(property)
        {
            _propertyValue = propertyValue;
        }

        public override DependencyProvider[] CreateDependencyProviders(Kernel kernel, ObjectDescription description, bool strongTyped)
        {
            var depProvider = DependencyProvider.CreatePropertyProvider(_property, _propertyValue);
            depProvider.InjectObjectBuilders(kernel);
            return new DependencyProvider[] { depProvider };
        }
    }

    public class FuncPropertyInjectionConfigurationItem<TProperty> : AutowiredPropertyInjectionConfigurationItem
    {
        readonly Func<IResolutionContext, TProperty> _valueFactory;

        public FuncPropertyInjectionConfigurationItem(PropertyInfo property, Func<IResolutionContext, TProperty> valueFactory)
            : base(property)
        {
            _valueFactory = valueFactory;
        }

        public override DependencyProvider[] CreateDependencyProviders(Kernel kernel, ObjectDescription description, bool strongTyped)
        {
            var depProvider = DependencyProvider.CreatePropertyProvider(_property, _valueFactory);
            depProvider.InjectObjectBuilders(kernel);
            return new DependencyProvider[] { depProvider };
        }
    }

    public class MethodInjectionConfigurationItem : IMemberInjectionConfigurationItem
    {
        readonly MethodInfo _method;

        public MethodInjectionConfigurationItem(MethodInfo method)
        {
            Requires.NotNull(method, "method");
            _method = method;
        }

        #region IMemberInjectionConfigurationItem Members

        public MemberKind MemberKind
        {
            get { return MemberKind.Method; }
        }

        public int MemberMetadataToken
        {
            get { return _method.MetadataToken; }
        }

        public MethodInfo InjectionMethod
        {
            get { return _method; }
        }

        public DependencyProvider[] CreateDependencyProviders(Kernel kernel, ObjectDescription description, bool strongTyped)
        {
            var depProviders = DependencyProvider.CreateParameterProviders(description, _method, null, strongTyped);
            if (depProviders == null)
                return null;
            foreach (var depProvider in depProviders)
                depProvider.InjectObjectBuilders(kernel);
            return depProviders;
        }

        public bool MatchInjectionConfigurationGroup(InjectionConfigurationGroup configGroup)
        {
            var description = configGroup.ObjectDescription;
            return description.ConcreteType == _method.DeclaringType // The method is defined in the ConcreteType directly
                || _method.DeclaringType.IsAssignableFrom(description.ConcreteType); // The method is defined in the base type of ConcreteType
        }

        #endregion

        #region IEquatable<IMemberInjectionConfigurationItem> Members

        public bool Equals(IMemberInjectionConfigurationItem other)
        {
            return MemberMetadataToken == other.MemberMetadataToken;
        }

        #endregion
    }
}