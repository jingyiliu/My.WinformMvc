using My.IoC.Configuration.Injection;
using My.IoC.Core;
using My.IoC.Lifetimes;

namespace My.IoC.Configuration.Provider
{
    class ReflectionOrEmitRegistrationProvider<T> : TypedRegistrationProvider
        where T : class 
    {
        IObjectRegistration _registration;
        InjectionConfigurationSet _configSet;
        Lifetime<T> _lifetime;

        public ReflectionOrEmitRegistrationProvider(Kernel kernel)
            :base(kernel)
        {
        }

        public override IObjectRegistration CreateObjectRegistration()
        {
            if (_registration != null)
                return _registration;

            var builder = CreateObjectBuilder();
            _registration = new ObjectRegistration<T>(builder);
            return _registration;
        }

        ObjectBuilder<T> CreateObjectBuilder()
        {
            var admin = CreateRegistrationAdmin();
            var description = CreateObjectDescription();
            _lifetime = LifetimeHelper.CreateLifetime<T>(LifetimeProvider);
            var configurationSet = CreateInjectionConfigurationSet(description, admin);
            return InjectionCondition == null
                ? new ObjectBuilder<T>(description, admin, _lifetime, configurationSet)
                : new ObjectBuilderWithCondition<T>(description, admin, _lifetime, configurationSet, InjectionCondition);
        }

        internal override InjectionConfigurationSet CreateInjectionConfigurationSet(ObjectDescription description, ObjectRelation admin)
        {
            if (_configSet != null)
                return _configSet;

            var constructorInfo = GetConstructorInfo();

            var ctorConfigItem = new ConstructorInjectionConfigurationItem(constructorInfo, ConfiguredParameters);
            var configGroup = new TypedInjectionConfigurationGroup(description, ctorConfigItem);
            var configSet = new InjectionConfigurationSet(description, admin, configGroup);

            if (MemberInjectionConfigurationItems != null)
            {
                foreach (var memberInjectionConfigurationItem in MemberInjectionConfigurationItems)
                    configGroup.AddMemberInjectionConfigurationItem(memberInjectionConfigurationItem);
            }

            IInjectionConfigurationInterpreter interpreter;
            if (ShouldUseLightweightCodeGeneration(Kernel))
                interpreter = new EmitInjectionConfigurationInterpreter(configGroup);
            else
                interpreter = new ReflectionInjectionConfigurationInterpreter(configGroup);
            configGroup.InjectionConfigurationInterpreter = interpreter;

            _configSet = configSet;
            return _configSet;
        }

        bool ShouldUseLightweightCodeGeneration(Kernel kernel)
        {
            switch (ActivatorKind)
            {
                case ActivatorKind.Dynamic:
                    return true;
                case ActivatorKind.Reflection:
                    return false;
                default:
                    return typeof(SingletonLifetime<T>).IsInstanceOfType(_lifetime)
                        ? false
                        : kernel.ContainerOption.UseLightweightCodeGeneration;
            }
        }
    }
}
