
using System;
using System.Globalization;
using System.Reflection;
using My.Helpers;
using My.IoC.Core;
using My.IoC.Dependencies;

namespace My.IoC.Helpers
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    static class ParameterException
    {
        public static Exception ParameterIsNotAutowirable(Type paramType)
        {
            return new ArgumentException(
                string.Format(CultureInfo.InvariantCulture, Resources.ParameterIsNotAutowirableAndShouldExplicitlyProvideAValue, paramType.ToTypeName()));
        }

        public static Exception ParameterNumberExceeds(InjectionContext context, int neededParamNumber, int providedParamNumber)
        {
            var description = context.ObjectDescription;
            return new ArgumentException(
                ExceptionFormatter.Format(context, Resources.ParameterNumberExceeds,
                description.ConcreteType.ToFullTypeName(),
                neededParamNumber,
                providedParamNumber));
        }

        public static Exception RedundantParametersProvided(InjectionContext context)
        {
            var description = context.ObjectDescription;
            return new ArgumentException(
                        ExceptionFormatter.Format(context, Resources.RedundantParametersProvided,
                        description.ConcreteType.ToFullTypeName()));
        }

        public static Exception NonautowirableParameterNotSpecified(InjectionContext context, DependencyProvider dependencyProvider, int index)
        {
            var description = context.ObjectDescription;
            return new ArgumentException(
                            ExceptionFormatter.Format(context, 
                            Resources.NonautowirableParameterNotSpecified,
                            index,
                            description.ConcreteType.ToFullTypeName(),
                            dependencyProvider.TargetType));
        }

        public static Exception RequiredParameterNotProvided(MethodBase methodBase, ParameterInfo ctorParameter)
        {
            return new ArgumentException(string.Format(CultureInfo.InvariantCulture, Resources.RequiredParameterNotProvided,
                            methodBase.DeclaringType.ToFullTypeName(), ctorParameter.ParameterType.ToFullTypeName()));
        }
    }
}
