using System.Reflection;
using My.Helpers;
using My.IoC.Core;
using My.IoC.Dependencies;

namespace My.IoC.Configuration.Injection
{
    public interface IConstructorInjectionConfigurationItem
    {
        ConstructorInfo Constructor { get; }
        ParameterSet ParameterSet { get; }
        DependencyProvider[] CreateDependencyProviders(Kernel kernel, ObjectDescription description, bool strongTyped);
        bool MatchInjectionConfigurationGroup(InjectionConfigurationGroup configGroup);
    }

    public class ConstructorInjectionConfigurationItem : IConstructorInjectionConfigurationItem
    {
        readonly ConstructorInfo _constructor;
        readonly ParameterSet _parameters;

        public ConstructorInjectionConfigurationItem(ConstructorInfo constructor, ParameterSet parameters)
        {
            Requires.NotNull(constructor, "constructorInfo");
            _constructor = constructor;
            _parameters = parameters;
        }

        public ConstructorInfo Constructor
        {
            get { return _constructor; }
        }

        public ParameterSet ParameterSet
        {
            get { return _parameters; }
        }

        #region IConstructorInjectionConfigurationItem Members

        public DependencyProvider[] CreateDependencyProviders(Kernel kernel, ObjectDescription description, bool strongTyped)
        {
            var depProviders = DependencyProvider.CreateParameterProviders(description, _constructor, _parameters, strongTyped);
            if (depProviders == null)
                return null;

            foreach (var depProvider in depProviders)
                depProvider.InjectObjectBuilders(kernel);
            return depProviders;
        }

        public bool MatchInjectionConfigurationGroup(InjectionConfigurationGroup configGroup)
        {
            var description = configGroup.ObjectDescription;
            return description.ContractType.IsAssignableFrom(_constructor.DeclaringType);
        }

        #endregion
    }
}