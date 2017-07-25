using System;
using System.Collections.Generic;
using System.Reflection;
using My.IoC.Core;
using My.IoC.Dependencies;
using My.IoC.Helpers;

namespace My.IoC.Injection.Reflection
{
    public class ReflectionConstructorInjector<T> : Injector<T>
    {
        protected readonly ConstructorInfo _constructor;
        readonly DependencyProvider[] _ctorDependencyProviders;

        public ReflectionConstructorInjector(ConstructorInfo constructor, DependencyProvider[] ctorDependencyProviders)
        {
            _constructor = constructor;
            _ctorDependencyProviders = ctorDependencyProviders;
        }

        public override void Execute(InjectionContext<T> context)
        {
            var parameters = MergeConstructorParameters(context);
            InjectInstanceIntoContext(context, (T)_constructor.Invoke(parameters));
        }

        #region ParameterMerger

        internal protected object[] MergeConstructorParameters(InjectionContext context)
        {
            object[] result = null;
            var myParams = context.Parameters;
            if (_ctorDependencyProviders != null)
            {
                if (myParams == null || myParams.Length == 0)
                {
                    result = new object[_ctorDependencyProviders.Length];
                    for (var i = 0; i < _ctorDependencyProviders.Length; i++)
                        _ctorDependencyProviders[i].CreateObject(context, out result[i]);
                }
                else
                {
                    var paramLength = myParams.Length - 1;
                    if (paramLength > _ctorDependencyProviders.Length - 1)
                    {
                        throw ParameterException.ParameterNumberExceeds(context,
                            _ctorDependencyProviders.Length, myParams.Length);
                    }

                    switch (myParams.ParameterKind)
                    {
                        case ParameterKind.Positional:
                            result = MergePositionalParameters(context, myParams);
                            break;
                        case ParameterKind.Named:
                            result = MergeNamedParameters(context, myParams);
                            break;
                    }
                }
            }
            else
            {
                if (myParams != null && myParams.Length > 0)
                    throw ParameterException.RedundantParametersProvided(context);
            }

            return result;
        }

        object[] MergePositionalParameters(InjectionContext context, ParameterSet myParams)
        {
            var paramLength = myParams.Length - 1;
            var result = new object[_ctorDependencyProviders.Length]; 
            for (var i = 0; i < _ctorDependencyProviders.Length; i++)
            {
                var depProvider = _ctorDependencyProviders[i];
                if (i > paramLength)
                {
                    if (depProvider.IsAutowirable)
                        depProvider.CreateObject(context, out result[i]);
                    else
                        throw ParameterException.NonautowirableParameterNotSpecified(context, depProvider, i);
                }
                else
                {
                    var providedParam = myParams[i];
                    if (providedParam.CanSupplyValueFor(depProvider))
                        result[i] = providedParam.ParameterValue;
                    else
                        depProvider.CreateObject(context, out result[i]);
                }
            }

            return result;
        }

        object[] MergeNamedParameters(InjectionContext context, ParameterSet myParams)
        {
            var result = new object[_ctorDependencyProviders.Length];
            for (var i = 0; i < _ctorDependencyProviders.Length; i++)
            {
                var depProvider = _ctorDependencyProviders[i];
                var namedParameter = GetNamedParameter(depProvider, myParams);
                if (namedParameter != null)
                    result[i] = namedParameter.ParameterValue;
                else
                    depProvider.CreateObject(context, out result[i]);
            }

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

        #endregion
    }

    public class ReflectionConstructorMemberInjector<T> : ReflectionConstructorInjector<T>
    {
        readonly ReflectionMemberInjector[] _memberInjectors;

        public ReflectionConstructorMemberInjector(ConstructorInfo constructor, DependencyProvider[] ctorDependencyProviders, ReflectionMemberInjector[] memberInjectors)
            : base(constructor, ctorDependencyProviders)
        {
            _memberInjectors = memberInjectors;
        }

        public override void Execute(InjectionContext<T> context)
        {
            var parameters = MergeConstructorParameters(context);

            object instance;
            try
            {
                instance = _constructor.Invoke(parameters);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            InjectInstanceIntoContext(context, (T)instance);

            for (int i = 0; i < _memberInjectors.Length; i++)
                _memberInjectors[i].Execute(instance, context);
        }
    }
}
