
using System;
using System.Collections.Generic;
using System.Reflection;
using My.Foundation;
using My.IoC.Condition;
using My.IoC.Core;
using My.IoC.Injection.Func;

namespace My.IoC.Dependencies
{
    public abstract partial class DependencyProvider
    {
        abstract class ConstantDependencyProvider : DependencyProvider
        {
            internal protected override bool HasDefaultValue
            {
                get { return true; }
            }

            internal protected override bool IsAutowirable
            {
                get { return false; }
            }

            internal protected override IInjectionTargetInfo InjectionTargetInfo
            {
                get { return null; }
            }

            internal protected override bool Obsolete
            {
                get { return false; }
            }

            internal protected override bool IsCollection
            {
                get { return false; }
            }

            internal protected override void InjectObjectBuilders(Kernel kernel)
            {
            }

            internal protected override ObjectBuilder GetCurrentObjectBuilder()
            {
                return null;
            }

            internal protected override IEnumerable<ObjectBuilder> GetCurrentObjectBuilders()
            {
                return null;
            }

            internal protected override bool CanUpdateObjectBuilder(ObjectBuilder builder)
            {
                return false;
            }

            internal protected override void UpdateObjectBuilder(ObjectBuilder builder)
            {
            }
        }

        abstract class ConstantDependencyProvider<T> : DependencyProvider<T>
        {
            internal protected override bool HasDefaultValue
            {
                get { return true; }
            }

            internal protected override bool IsAutowirable
            {
                get { return false; }
            }

            internal protected override IInjectionTargetInfo InjectionTargetInfo
            {
                get { return null; }
            }

            internal protected override bool Obsolete
            {
                get { return false; }
            }

            internal protected override bool IsCollection
            {
                get { return false; }
            }

            internal protected override void InjectObjectBuilders(Kernel kernel)
            {
            }

            internal protected override ObjectBuilder GetCurrentObjectBuilder()
            {
                return null;
            }

            internal protected override IEnumerable<ObjectBuilder> GetCurrentObjectBuilders()
            {
                return null;
            }

            internal protected override bool CanUpdateObjectBuilder(ObjectBuilder builder)
            {
                return false;
            }

            internal protected override void UpdateObjectBuilder(ObjectBuilder builder)
            {
            }
        }

        #region ParameterProvider

        sealed class WeakConstantParameterProvider : ConstantDependencyProvider
        {
            readonly object _value;
            readonly ParameterInfo _paramInfo;

            public WeakConstantParameterProvider(ParameterInfo paramInfo, object value)
            {
                _value = value;
                _paramInfo = paramInfo;
            }

            internal protected override string TargetName
            {
                get { return _paramInfo.Name; }
            }

            internal protected override Type TargetType
            {
                get { return _paramInfo.ParameterType; }
            }

            public override void CreateObject(InjectionContext context, out object instance)
            {
                instance = _value;
            }
        }

        sealed class StrongConstantParameterProvider<TParameter> : ConstantDependencyProvider<TParameter>
        {
            readonly TParameter _value;
            readonly ParameterInfo _paramInfo;

            public StrongConstantParameterProvider(ParameterInfo paramInfo, TParameter value)
            {
                _value = value;
                _paramInfo = paramInfo;
            }

            internal protected override string TargetName
            {
                get { return _paramInfo.Name; }
            }

            internal protected override Type TargetType
            {
                get { return _paramInfo.ParameterType; }
            }

            public override void CreateObject(InjectionContext context, out object instance)
            {
                instance = _value;
            }

            public override void CreateObject(InjectionContext context, out TParameter instance)
            {
                instance = _value;
            }
        }

        #endregion

        #region PropertyProvider

        sealed class StrongConstantPropertyProvider<TProperty> : ConstantDependencyProvider<TProperty>
        {
            readonly PropertyInfo _property;
            readonly TProperty _value;

            public StrongConstantPropertyProvider(PropertyInfo property, TProperty value)
            {
                _property = property;
                _value = value;
            }

            internal protected override string TargetName
            {
                get { return _property.Name; }
            }

            internal protected override Type TargetType
            {
                get { return _property.PropertyType; }
            }

            public override void CreateObject(InjectionContext context, out object instance)
            {
                instance = _value;
            }

            public override void CreateObject(InjectionContext context, out TProperty instance)
            {
                instance = _value;
            }
        }

        sealed class FuncConstantPropertyProvider<TProperty> : ConstantDependencyProvider<TProperty>
        {
            readonly PropertyInfo _property;
            readonly Func<IResolutionContext, TProperty> _factory;

            public FuncConstantPropertyProvider(PropertyInfo property, Func<IResolutionContext, TProperty> factory)
            {
                _property = property;
                _factory = factory;
            }

            internal protected override string TargetName
            {
                get { return _property.Name; }
            }

            internal protected override Type TargetType
            {
                get { return _property.PropertyType; }
            }

            public override void CreateObject(InjectionContext context, out object instance)
            {
                var rContext = new ResolutionContext(context);
                instance = _factory.Invoke(rContext);
            }

            public override void CreateObject(InjectionContext context, out TProperty instance)
            {
                var rContext = new ResolutionContext(context);
                instance = _factory.Invoke(rContext);
            }
        }

        sealed class WeakConstantPropertyProvider : ConstantDependencyProvider
        {
            readonly PropertyInfo _property;
            readonly object _value;

            public WeakConstantPropertyProvider(PropertyInfo property, object value)
            {
                _property = property;
                _value = value;
            }

            internal protected override string TargetName
            {
                get { return _property.Name; }
            }

            internal protected override Type TargetType
            {
                get { return _property.PropertyType; }
            }

            public override void CreateObject(InjectionContext context, out object instance)
            {
                instance = _value;
            }
        }

        #endregion
    }
}
