
using System;
using System.Reflection;
using My.Helpers;
using My.IoC.Condition;
using My.IoC.Core;
using My.IoC.Dependencies;
using My.IoC.Helpers;

namespace My.IoC
{
    public abstract class PositionalParameter : Parameter
    {
    }

    public abstract class NamedParameter : Parameter
    {
    }

    public abstract partial class Parameter
    {
        #region Public Static Members

        /// <summary>
        /// Indicates that the value of current parameter will be determined by the container according to
        /// its position automatically.
        /// </summary>
        public static readonly PositionalParameter Auto = new AutoParameter();

        public static PositionalParameter Positional<TParam>()
        {
            return new AutowiredPositionalParameter<TParam>();
        }

        public static PositionalParameter Positional(Type paramType)
        {
            return new AutowiredPositionalParameter(paramType);
        }

        public static PositionalParameter Positional<TParam>(TParam paramValue)
        {
            return new ConstantPositionalParameter<TParam>(paramValue);
        }

        // This could be useful when creating a Parameter using the result read from configuration files
        public static PositionalParameter Positional(object paramValue)
        {
            return new ConstantPositionalParameter(paramValue);
        }

        public static NamedParameter Named<TParam>(string paramName, TParam paramValue)
        {
            return new ConstantNamedParameter<TParam>(paramName, paramValue);
        }

        // This could be useful when creating a Parameter using the result read from configuration files
        public static NamedParameter Named(string paramName, object paramValue)
        {
            return new ConstantNamedParameter(paramName, paramValue);
        }

        #endregion

        #region Abstrace Members

        public abstract Type ParameterType { get; }
        public abstract object ParameterValue { get; }
        /// <summary>
        /// Matches the specified injectionInfo.
        /// </summary>
        /// <param name="paramInfo">The parameter injectionInfo.</param>
        /// <returns></returns>
        /// <remarks>
        /// Can we be used to represent the provided parameter at the time of registration.
        /// </remarks>
        public abstract bool Match(ParameterInfo paramInfo);
        /// <summary>
        /// Determines whether this parameter can supply value for the injection target represented by the specified injectionInfo.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>
        ///   <c>true</c> if this parameter can supply value for the injection target represented by the specified injectionInfo; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Should we use the parameter value provided with this instance at the time of resolution, when the injection target already
        /// has default parameter value out there?
        /// </remarks>
        public abstract bool CanSupplyValueFor(DependencyProvider provider);
        internal protected abstract DependencyProvider CreateDependencyProvider(ObjectDescription description, ParameterInfo paramInfo, bool strongTyped);

        #endregion
    }

    public abstract partial class Parameter
    {
        #region PositionalParameter

        sealed class AutoParameter : PositionalParameter
        {
            public AutoParameter() { }

            #region Parameter Members

            public override Type ParameterType
            {
                get { throw new NotImplementedException(); }
            }

            public override object ParameterValue
            {
                get { throw new NotImplementedException(); }
            }

            public override bool Match(ParameterInfo paramInfo)
            {
                Requires.NotNull(paramInfo, "paramInfo");
                return paramInfo.ParameterType.IsAutowirable();
            }

            public override bool CanSupplyValueFor(DependencyProvider provider)
            {
                return false;
            }

            internal protected override DependencyProvider CreateDependencyProvider(ObjectDescription description, ParameterInfo paramInfo, bool strongTyped)
            {
                return DependencyProvider.CreateAutowiredDependencyProvider(new ParameterInjectionTargetInfo(paramInfo, description), strongTyped);
            }

            #endregion
        }

        abstract class ConstantPositionalParameterBase<TParam> : PositionalParameter
        {
            protected readonly TParam MyParamValue;

            protected ConstantPositionalParameterBase(TParam paramValue)
            {
                Requires.True(!ReferenceEquals(paramValue, null), "paramValue");
                MyParamValue = paramValue;
            }

            public override object ParameterValue
            {
                get { return MyParamValue; }
            }

            public override Type ParameterType
            {
                get { return MyParamValue.GetType(); }
            }

            public override bool Match(ParameterInfo paramInfo)
            {
                Requires.NotNull(paramInfo, "paramInfo");
                return paramInfo.ParameterType.IsAssignableFrom(ParameterType);
            }

            public override bool CanSupplyValueFor(DependencyProvider provider)
            {
                Requires.NotNull(provider, "provider");
                return provider.TargetType.IsAssignableFrom(ParameterType);
            }
        }

        sealed class ConstantPositionalParameter<TParam> : ConstantPositionalParameterBase<TParam>
        {
            public ConstantPositionalParameter(TParam paramValue)
                : base(paramValue)
            {
            }

            internal protected override DependencyProvider CreateDependencyProvider(ObjectDescription description, ParameterInfo paramInfo, bool strongTyped)
            {
                return strongTyped
                    ? DependencyProvider.CreateStrongConstantParameterProvider(MyParamValue, paramInfo)
                    : DependencyProvider.CreateWeakConstantParameterProvider(MyParamValue, paramInfo);
            }
        }

        sealed class ConstantPositionalParameter : ConstantPositionalParameterBase<object>
        {
            public ConstantPositionalParameter(object paramValue)
                : base(paramValue)
            {
            }

            internal protected override DependencyProvider CreateDependencyProvider(ObjectDescription description, ParameterInfo paramInfo, bool strongTyped)
            {
                return DependencyProvider.CreateConstantParameterProvider(MyParamValue, paramInfo, strongTyped);
            }
        }

        abstract class AutowiredPositionalParameterBase : PositionalParameter
        {
            public override object ParameterValue
            {
                get { return null; }
            }

            public override bool Match(ParameterInfo paramInfo)
            {
                Requires.NotNull(paramInfo, "paramInfo");
                return paramInfo.ParameterType.IsAssignableFrom(ParameterType);
            }

            public override bool CanSupplyValueFor(DependencyProvider provider)
            {
                Requires.NotNull(provider, "provider");
                return false;
            }
        }

        sealed class AutowiredPositionalParameter<TParam> : AutowiredPositionalParameterBase
        {
            public AutowiredPositionalParameter()
            {
                if (!typeof(TParam).IsAutowirable())
                    throw ParameterException.ParameterIsNotAutowirable(typeof(TParam));
            }

            public override Type ParameterType
            {
                get { return typeof(TParam); }
            }

            internal protected override DependencyProvider CreateDependencyProvider(ObjectDescription description, ParameterInfo paramInfo, bool strongTyped)
            {
                return strongTyped
                    ? DependencyProvider.CreateAutowiredDependencyProvider<TParam>(new ParameterInjectionTargetInfo(paramInfo, description))
                    : DependencyProvider.CreateAutowiredDependencyProvider(new ParameterInjectionTargetInfo(paramInfo, description));
            }
        }

        sealed class AutowiredPositionalParameter : AutowiredPositionalParameterBase
        {
            readonly Type _paramType;

            public AutowiredPositionalParameter(Type paramType)
            {
                Requires.NotNull(paramType, "paramType");
                if (!paramType.IsAutowirable())
                    throw ParameterException.ParameterIsNotAutowirable(paramType);
                _paramType = paramType;
            }

            public override Type ParameterType
            {
                get { return _paramType; }
            }

            internal protected override DependencyProvider CreateDependencyProvider(ObjectDescription description, ParameterInfo paramInfo, bool strongTyped)
            {
                return DependencyProvider.CreateAutowiredDependencyProvider(new ParameterInjectionTargetInfo(paramInfo, description), strongTyped);
            }
        }

        #endregion

        #region NamedParameter

        abstract class ConstantNamedParameterBase<TParam> : NamedParameter
        {
            readonly string _paramName;
            protected readonly TParam MyParamValue;

            protected ConstantNamedParameterBase(string paramName, TParam paramValue)
            {
                Requires.NotNullOrEmpty(paramName, "paramName");
                Requires.True(!ReferenceEquals(paramValue, null), "paramValue");
                _paramName = paramName;
                MyParamValue = paramValue;
            }

            public override object ParameterValue
            {
                get { return MyParamValue; }
            }

            public override Type ParameterType
            {
                get { return MyParamValue.GetType(); }
            }

            public override bool Match(ParameterInfo paramInfo)
            {
                Requires.NotNull(paramInfo, "paramInfo");
                return _paramName.Equals(paramInfo.Name) && paramInfo.ParameterType.IsAssignableFrom(ParameterType);
            }

            public override bool CanSupplyValueFor(DependencyProvider provider)
            {
                Requires.NotNull(provider, "provider");
                return _paramName.Equals(provider.TargetName) && provider.TargetType.IsAssignableFrom(ParameterType);
            }
        }

        sealed class ConstantNamedParameter<TParam> : ConstantNamedParameterBase<TParam>
        {
            public ConstantNamedParameter(string paramName, TParam paramValue)
                : base(paramName, paramValue)
            {
            }

            internal protected override DependencyProvider CreateDependencyProvider(ObjectDescription description, ParameterInfo paramInfo, bool strongTyped)
            {
                return strongTyped
                    ? DependencyProvider.CreateStrongConstantParameterProvider(MyParamValue, paramInfo)
                    : DependencyProvider.CreateWeakConstantParameterProvider(MyParamValue, paramInfo);
            }
        }

        sealed class ConstantNamedParameter : ConstantNamedParameterBase<object>
        {
            public ConstantNamedParameter(string paramName, object paramValue)
                : base(paramName, paramValue)
            {
            }

            internal protected override DependencyProvider CreateDependencyProvider(ObjectDescription description, ParameterInfo paramInfo, bool strongTyped)
            {
                return DependencyProvider.CreateConstantParameterProvider(MyParamValue, paramInfo, strongTyped);
            }
        }

        #endregion
    }
}
