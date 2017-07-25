
using System;
using System.Reflection;
using My.IoC.Core;

namespace My.IoC.Condition
{
    sealed class PropertyInjectionTargetInfo : InjectionTargetInfo, IInjectionTargetInfo
    {
        readonly PropertyInfo _propertyInfo;

        public PropertyInjectionTargetInfo(PropertyInfo propertyInfo, ObjectDescription description)
            : base(description)
        {
            _propertyInfo = propertyInfo;
        }

        #region ITargetInfo Members

        public string TargetName
        {
            get { return _propertyInfo.Name; }
        }

        public Type TargetType
        {
            get { return _propertyInfo.PropertyType; }
        }

        public ICustomAttributeProvider TargetAttributeProvider
        {
            get { return _propertyInfo; }
        }

        #endregion
    }

    sealed class ParameterInjectionTargetInfo : InjectionTargetInfo, IInjectionTargetInfo
    {
        readonly ParameterInfo _paramInfo;

        public ParameterInjectionTargetInfo(ParameterInfo paramInfo, ObjectDescription description)
            : base(description)
        {
            _paramInfo = paramInfo;
        }

        public string TargetName
        {
            get { return _paramInfo.Name; }
        }

        public Type TargetType
        {
            get { return _paramInfo.ParameterType; }
        }

        public ICustomAttributeProvider TargetAttributeProvider
        {
            get { return _paramInfo; }
        }
    }

    public abstract class InjectionTargetInfo
    {
        readonly ObjectDescription _description;

        protected InjectionTargetInfo(ObjectDescription description)
        {
            _description = description;
        }

        public ObjectDescription TargetDescription
        {
            get { return _description; }
        }
    }
}
