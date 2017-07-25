
using System;
using My.Helpers;
using My.IoC.Helpers;
using System.Reflection;
using System.Collections.Generic;

namespace My.IoC.Core
{
    public class DefaultConstructorSelector : IConstructorSelector
    {
        readonly IConstructorFinder _constructorFinder;
        readonly IConstructorSorter _constructorSorter;

        public DefaultConstructorSelector(IConstructorFinder constructorFinder, IConstructorSorter constructorSorter)
        {
            Requires.NotNull(constructorFinder, "constructorFinder");
            Requires.NotNull(constructorSorter, "constructorSorter");
            _constructorFinder = constructorFinder;
            _constructorSorter = constructorSorter;
        }

        public IConstructorFinder ConstructorFinder
        {
            get { return _constructorFinder; }
        }

        public IConstructorSorter ConstructorSorter
        {
            get { return _constructorSorter; }
        }

        public ConstructorInfo SelectConstructor(Type concreteType, ParameterSet configuredParameters)
        {
            var constructors = FindAndSortConstructors(concreteType);
            if (configuredParameters == null || configuredParameters.Length == 0)
                return GetConstructorWithoutParameters(constructors);

            switch (configuredParameters.ParameterKind)
            {
                case ParameterKind.Positional:
                    return GetConstructorWithPositionalParameters(constructors, configuredParameters);
                case ParameterKind.Named:
                    return GetConstructorWithNamedParameters(constructors, configuredParameters);
                default:
                    throw new NotImplementedException();
            }
        }

        List<ConstructorInfo> FindAndSortConstructors(Type concreteType)
        {
            var constructors = _constructorFinder.FindConstructors(concreteType);
            return _constructorSorter.SortConstructors(constructors);
        }

        static ConstructorInfo GetConstructorWithoutParameters(IList<ConstructorInfo> constructors)
        {
            for (var i = 0; i < constructors.Count; i++)
            {
                var constructor = constructors[i];
                var ctorParameters = constructor.GetParameters();
                var canSelect = true;
                foreach (var ctorParameter in ctorParameters)
                {
                    if (ctorParameter.ParameterType.IsAutowirable())
                        continue;
                    canSelect = false;
                    break;
                }
                if (canSelect)
                    return constructor;
            }
            return null;
        }

        static ConstructorInfo GetConstructorWithNamedParameters(IList<ConstructorInfo> constructors, ParameterSet namedParameters)
        {
            var namedParamLength = namedParameters.Length;
            for (var i = 0; i < constructors.Count; i++)
            {
                var constructor = constructors[i];
                var ctorParameters = constructor.GetParameters();
                if (namedParamLength > ctorParameters.Length)
                    continue;

                var canSelect = true;

                for (int j = 0; j < namedParameters.Length; j++)
                {
                    var namedParameter = namedParameters[j];
                    if (MatchNamedParameter(namedParameter, ctorParameters))
                        continue;
                    canSelect = false;
                    break;
                }

                if (!canSelect)
                    continue;

                foreach (var ctorParameter in ctorParameters)
                {
                    if (ctorParameter.ParameterType.IsAutowirable() || MatchNamedParameter(ctorParameter, namedParameters))
                        continue;
                    canSelect = false;
                    break;
                }

                if (canSelect)
                    return constructor;
            }

            return null;
        }

        static bool MatchNamedParameter(Parameter namedParameter, ParameterInfo[] ctorParameters)
        {
            foreach (var ctorParameter in ctorParameters)
            {
                if (namedParameter.Match(ctorParameter))
                    return true;
            }
            return false;
        }

        static bool MatchNamedParameter(ParameterInfo ctorParameter, IEnumerable<Parameter> namedParameters)
        {
            foreach (var namedParameter in namedParameters)
            {
                if (namedParameter.Match(ctorParameter))
                    return true;
            }
            return false;
        }

        static ConstructorInfo GetConstructorWithPositionalParameters(IList<ConstructorInfo> constructors, ParameterSet positionalParameters)
        {
            var positionalParamLength = positionalParameters.Length - 1;
            for (var i = 0; i < constructors.Count; i++)
            {
                var constructor = constructors[i];
                var ctorParameters = constructor.GetParameters();
                if (positionalParamLength > ctorParameters.Length - 1)
                    continue;

                var canSelect = true;
                for (var j = 0; j < ctorParameters.Length; j++)
                {
                    var ctorParameter = ctorParameters[j];
                    if (j > positionalParamLength)
                    {
                        if (ctorParameter.ParameterType.IsAutowirable())
                            continue;
                        canSelect = false;
                        break;
                    }

                    var positionalParameter = positionalParameters[j];
                    if (positionalParameter.Match(ctorParameter))
                        continue;
                    canSelect = false;
                    break;
                }

                if (canSelect)
                    return constructor;
            }
            return null;
        }
    }
}
