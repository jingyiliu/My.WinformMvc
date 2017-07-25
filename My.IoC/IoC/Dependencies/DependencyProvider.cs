using System;
using System.Collections.Generic;
using System.Reflection;
using My.Exceptions;
using My.Foundation;
using My.Helpers;
using My.IoC.Condition;
using My.IoC.Core;
using My.IoC.Helpers;
using My.IoC.Injection.Func;

namespace My.IoC.Dependencies
{
    public abstract partial class DependencyProvider
    {
        #region Public Static Members

        public static DependencyProvider CreatePropertyProvider(ObjectDescription description, PropertyInfo property, bool strongTyped)
        {
            Requires.NotNull(description, "description");
            Requires.NotNull(property, "property");
            IInjectionTargetInfo injectionTargetInfo = new PropertyInjectionTargetInfo(property, description);
            return CreateAutowiredDependencyProvider(injectionTargetInfo, strongTyped);
        }

        public static DependencyProvider CreatePropertyProvider(PropertyInfo property, object propertyValue)
        {
            Requires.NotNull(propertyValue, "propertyValue");
            return new WeakConstantPropertyProvider(property, propertyValue);
        }

        public static DependencyProvider CreatePropertyProvider<TProperty>(PropertyInfo property, TProperty propertyValue)
        {
            Requires.NotNull(propertyValue, "propertyValue");
            return new StrongConstantPropertyProvider<TProperty>(property, propertyValue);
        }

        public static DependencyProvider CreatePropertyProvider<TProperty>(PropertyInfo property, Func<IResolutionContext, TProperty> factory)
        {
            Requires.NotNull(factory, "factory");
            return new FuncConstantPropertyProvider<TProperty>(property, factory);
        }

        public static DependencyProvider[] CreateParameterProviders(ObjectDescription description, MethodBase methodBase, ParameterSet configuredParameters, bool strongTyped)
        {
            Requires.NotNull(description, "description");
            Requires.NotNull(methodBase, "methodBase");
            return configuredParameters == null
                ? DoCreateParameterProviders(description, methodBase, strongTyped)
                : DoCreateParameterProviders(description, methodBase, configuredParameters, strongTyped);
        }

        static DependencyProvider[] DoCreateParameterProviders(ObjectDescription description, MethodBase methodBase, bool strongTyped)
        {
            var paramInfos = methodBase.GetParameters();
            var methodParameterCount = paramInfos.Length;
            if (methodParameterCount == 0)
                return null;

            var dependencies = new DependencyProvider[methodParameterCount];
            for (var i = 0; i < methodParameterCount; i++)
            {
                var paramInfo = paramInfos[i];
                if (paramInfo.ParameterType.IsAutowirable())
                    dependencies[i] = CreateAutowiredDependencyProvider(new ParameterInjectionTargetInfo(paramInfo, description), strongTyped);
                else
                    throw ParameterException.RequiredParameterNotProvided(methodBase, paramInfo);
            }

            return dependencies;
        }

        static DependencyProvider[] DoCreateParameterProviders(ObjectDescription description, MethodBase methodBase, ParameterSet parameters, bool strongTyped)
        {
            var paramLength = parameters.Length;
            if (paramLength == 0)
                throw new ArgumentException("parameters.Count == 0");

            var methodParameters = methodBase.GetParameters();
            var methodParameterCount = methodParameters.Length;
            if (methodParameterCount < paramLength)
                throw new ImpossibleException();

            var dependencies = new DependencyProvider[methodParameterCount];
            switch (parameters.ParameterKind)
            {
                case ParameterKind.Named:
                    for (var i = 0; i < methodParameterCount; i++)
                    {
                        var methodParameter = methodParameters[i];
                        var namedParameter = GetNamedParameter(methodParameter, parameters);
                        dependencies[i] = namedParameter != null
                            ? namedParameter.CreateDependencyProvider(description, methodParameter, strongTyped)
                            : CreateAutowiredDependencyProvider(new ParameterInjectionTargetInfo(methodParameter, description), strongTyped);
                    }
                    break;

                case ParameterKind.Positional:
                    for (var i = 0; i < methodParameterCount; i++)
                    {
                        dependencies[i] = i < paramLength
                            ? parameters[i].CreateDependencyProvider(description, methodParameters[i], strongTyped)
                            : CreateAutowiredDependencyProvider(new ParameterInjectionTargetInfo(methodParameters[i], description), strongTyped);
                    }
                    break;

                default:
                    throw new ImpossibleException();
            }

            return dependencies;
        }

        static Parameter GetNamedParameter(ParameterInfo paramInfo, IEnumerable<Parameter> namedParameters)
        {
            foreach (var namedParameter in namedParameters)
            {
                if (namedParameter.Match(paramInfo))
                    return namedParameter;
            }
            return null;
        }

        internal static DependencyProvider CreateConstantParameterProvider(object paramValue, ParameterInfo parameter, bool strongTyped)
        {
            if (!strongTyped)
                return new WeakConstantParameterProvider(parameter, paramValue);
            var strongDependencyProviderType =
                typeof(StrongConstantParameterProvider<>).MakeGenericType(parameter.ParameterType);

            try
            {
                return (DependencyProvider)Activator.CreateInstance(strongDependencyProviderType, parameter, paramValue);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        internal static DependencyProvider CreateWeakConstantParameterProvider(object paramValue, ParameterInfo parameter)
        {
            return new WeakConstantParameterProvider(parameter, paramValue);
        }

        internal static DependencyProvider CreateStrongConstantParameterProvider<TParam>(TParam paramValue, ParameterInfo parameter)
        {
            return new StrongConstantParameterProvider<TParam>(parameter, paramValue);
        }

        internal static DependencyProvider CreateAutowiredDependencyProvider(IInjectionTargetInfo injectionTargetInfo, bool strongTyped)
        {
            if (!strongTyped)
                return new WeakAutowiredDependencyProvider(injectionTargetInfo);
            var strongDependencyProviderType =
                typeof(StrongAutowiredDependencyProvider<>).MakeGenericType(injectionTargetInfo.TargetType);

            try
            {
                return (DependencyProvider)Activator.CreateInstance(strongDependencyProviderType, injectionTargetInfo);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        internal static DependencyProvider CreateAutowiredDependencyProvider<TParam>(IInjectionTargetInfo injectionTargetInfo)
        {
            return new StrongAutowiredDependencyProvider<TParam>(injectionTargetInfo);
        }

        internal static DependencyProvider CreateAutowiredDependencyProvider(IInjectionTargetInfo injectionTargetInfo)
        {
            return new WeakAutowiredDependencyProvider(injectionTargetInfo);
        }

        #endregion
    }

    public abstract partial class DependencyProvider : DependencyUpdater, IDisposable
    {
        internal protected abstract string TargetName { get; }
        internal protected abstract Type TargetType { get; }
        internal protected abstract IInjectionTargetInfo InjectionTargetInfo { get; }
        internal protected abstract void InjectObjectBuilders(Kernel kernel);
        public abstract void CreateObject(InjectionContext context, out object instance);

        public virtual void Dispose()
        {
        }
    }

    public abstract class DependencyProvider<TDependency> : DependencyProvider
    {
        public abstract void CreateObject(InjectionContext context, out TDependency instance);
    }
}
