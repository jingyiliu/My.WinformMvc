
using System;
using My.Helpers;

namespace My.IoC.Condition
{
    public interface IInjectionCondition
    {
        bool Match(IInjectionTargetInfo targetInfo);
    }

    class TargetTypesInjectionCondition : IInjectionCondition
    {
        readonly Type[] _expectedTypes;

        public TargetTypesInjectionCondition(Type[] expectedTypes)
        {
            Requires.NotNull(expectedTypes, "expectedTypes");
            _expectedTypes = expectedTypes;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            if (targetInfo == null)
                return false;
            var targetType = targetInfo.TargetDescription.ConcreteType;
            foreach (var expectedType in _expectedTypes)
            {
                if (targetType == expectedType || expectedType.IsAssignableFrom(targetType))
                    return true;
            }
            return false;
        }

        #endregion
    }
    class TargetTypeInjectionCondition : IInjectionCondition
    {
        readonly Type _expectedType;

        public TargetTypeInjectionCondition(Type expectedType)
        {
            Requires.NotNull(expectedType, "expectedType");
            _expectedType = expectedType;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            return targetInfo != null && 
                (_expectedType == targetInfo.TargetDescription.ConcreteType
                || _expectedType.IsAssignableFrom(targetInfo.TargetDescription.ConcreteType));
        }

        #endregion
    }

    class ExactlyTargetTypesInjectionCondition : IInjectionCondition
    {
        readonly Type[] _expectedTypes;

        public ExactlyTargetTypesInjectionCondition(Type[] expectedTypes)
        {
            Requires.NotNull(expectedTypes, "expectedTypes");
            _expectedTypes = expectedTypes;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            if (targetInfo == null)
                return false;
            var targetType = targetInfo.TargetDescription.ConcreteType;
            foreach (var expectedType in _expectedTypes)
            {
                if (targetType == expectedType)
                    return true;
            }
            return false;
        }

        #endregion
    }
    class ExactlyTargetTypeInjectionCondition : IInjectionCondition
    {
        readonly Type _expectedType;

        public ExactlyTargetTypeInjectionCondition(Type expectedType)
        {
            Requires.NotNull(expectedType, "expectedType");
            _expectedType = expectedType;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            return targetInfo != null && _expectedType == targetInfo.TargetDescription.ConcreteType;
        }

        #endregion
    }

    class TargetAttributeInjectionCondition : IInjectionCondition
    {
        readonly Type _targetAttribType;

        public TargetAttributeInjectionCondition(Type targetAttribType)
        {
            Requires.NotNull(targetAttribType, "targetAttribType");
            _targetAttribType = targetAttribType;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            return targetInfo != null && targetInfo.TargetAttributeProvider.IsDefined(_targetAttribType, false);
        }

        #endregion
    }
    class TargetNameInjectionCondition : IInjectionCondition
    {
        readonly string _targetName;

        public TargetNameInjectionCondition(string targetName)
        {
            Requires.NotNull(targetName, "targetName");
            _targetName = targetName;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            return targetInfo != null && targetInfo.TargetName == _targetName;
        }

        #endregion
    }

    class MetadataPredicateInjectionCondition : IInjectionCondition
    {
        readonly Predicate<object> _metadataCondition;

        public MetadataPredicateInjectionCondition(Predicate<object> metadataCondition)
        {
            Requires.NotNull(metadataCondition, "metadataCondition");
            _metadataCondition = metadataCondition;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            return targetInfo != null && _metadataCondition.Invoke(targetInfo.TargetDescription.Metadata);
        }

        #endregion
    }
    class MetadataIsInjectionCondition : IInjectionCondition
    {
        readonly object _metadata;

        public MetadataIsInjectionCondition(object metadata)
        {
            Requires.NotNull(metadata, "metadata");
            _metadata = metadata;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            return targetInfo != null && _metadata.Equals(targetInfo.TargetDescription.Metadata);
        }

        #endregion
    }
    class PredicateInjectionCondition : IInjectionCondition
    {
        readonly Predicate<IInjectionTargetInfo> _condition;

        public PredicateInjectionCondition(Predicate<IInjectionTargetInfo> condition)
        {
            Requires.NotNull(condition, "condition");
            _condition = condition;
        }

        #region IConditionEvaluator Members

        public bool Match(IInjectionTargetInfo targetInfo)
        {
            return targetInfo != null && _condition.Invoke(targetInfo);
        }

        #endregion
    }
}