
using System;
using My.IoC.Condition;
using My.IoC.Helpers;

namespace My.IoC.Core
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class AutoObjectRegistrar
    {
        readonly IManualObjectRegistrar _registrar;
        readonly IAutoRegistrationPolicy[] _registrationPolicies;

        internal AutoObjectRegistrar(IManualObjectRegistrar registrar, IAutoRegistrationPolicy[] registrationPolicies)
        {
            _registrar = registrar;
            _registrationPolicies = registrationPolicies;
        }

        public ObjectBuilder GetObjectBuilder(Type concreteType)
        {
            return DoGetObjectBuilder(concreteType, null);
        }

        public ObjectBuilder GetObjectBuilder(Type concreteType, IInjectionTargetInfo injectionTargetInfo)
        {
            return DoGetObjectBuilder(concreteType, injectionTargetInfo);
        }

        ObjectBuilder DoGetObjectBuilder(Type concreteType, IInjectionTargetInfo injectionTargetInfo)
        {
            if (_registrationPolicies == null || !concreteType.IsConcrete())
                return null;

            IAutoRegistrationPolicy matchPolicy = null;
            for (int i = 0; i < _registrationPolicies.Length; i++)
            {
                var autoRegistrationPolicy = _registrationPolicies[i];
                if (!autoRegistrationPolicy.ShouldRegister(concreteType))
                    continue;
                matchPolicy = autoRegistrationPolicy;
                break;
            }

            if (matchPolicy == null)
                return null;

            IObjectRegistration registration;
            _registrar.Register(concreteType).In(matchPolicy.GetLifetimeProvider()).Return(out registration);
            if (!registration.ObjectBuilder.MatchCondition(injectionTargetInfo))
                return null;

            _registrar.CommitRegistrations();
            return registration.ObjectBuilder;
        }
    }
}
