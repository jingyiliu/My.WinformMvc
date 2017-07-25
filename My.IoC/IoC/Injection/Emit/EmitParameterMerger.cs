using System;
using System.Collections.Generic;
using System.Globalization;
using My.IoC.Core;
using My.IoC.Dependencies;
using My.IoC.Helpers;

namespace My.IoC.Injection.Emit
{
    public abstract class EmitParameterMerger
    {
        protected static Exception ParameterNumberExceeds(InjectionContext context, int neededParamNumber, int providedParamNumber)
        {
            return ParameterException.ParameterNumberExceeds(context, neededParamNumber, providedParamNumber);
        }

        protected static void VerfiyConstructorParameterLength(DependencyProvider[] dependencyProviders, int requiredLength)
        {
            if (dependencyProviders == null || dependencyProviders.Length != requiredLength)
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, Resources.ParameterNumberIsWrong, requiredLength, dependencyProviders == null ? 0 : dependencyProviders.Length));
        }

        protected static T GetPositionalDependencyObject<T>(DependencyProvider<T> dependencyProvider, Parameter positionalParameter, InjectionContext context)
        {
            T result;
            if (positionalParameter.CanSupplyValueFor(dependencyProvider))
                result = (T) positionalParameter.ParameterValue;
            else
                dependencyProvider.CreateObject(context, out result);
            return result;
        }

        protected static T GetNamedDependencyObject<T>(DependencyProvider<T> dependencyProvider, ParameterSet namedParameters, InjectionContext context)
        {
            T result;
            var named0 = GetNamedParameter(dependencyProvider, namedParameters);
            if (named0 != null)
                result = (T) named0.ParameterValue;
            else
                dependencyProvider.CreateObject(context, out result);
            return result;
        }

        static Parameter GetNamedParameter(DependencyProvider dependencyProvider, IEnumerable<Parameter> namedParameters)
        {
            foreach (var namedParameter in namedParameters)
            {
                if (namedParameter.CanSupplyValueFor(dependencyProvider))
                    return namedParameter;
            }
            return null;
        }
    }
}