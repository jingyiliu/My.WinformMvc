
using System.Collections.Generic;
using My.IoC.Configuration;

namespace My.IoC.Core
{
    /// <summary>
    /// One RegistrationCommitter per thread.
    /// </summary>
    class RegistrationCommitter
    {
        List<IRegistrationProvider> _providers;

        public void AddRegistration(IRegistrationProvider provider)
        {
            if (_providers == null)
                _providers = new List<IRegistrationProvider>();
            _providers.Add(provider);
        }

        public void CommitRegistrations(Kernel kernel)
        {
            if (_providers == null || _providers.Count == 0)
                return;

            var providers = _providers;
            _providers = null; // Clear the list of IRegistrationProvider

            if (providers.Count == 1)
                kernel.Register(providers[0].CreateObjectRegistration());
            else
            {
                List<IObjectRegistration> registrations = null;
                foreach (var provider in providers)
                {
                    if (registrations == null)
                        registrations = new List<IObjectRegistration>();
                    registrations.Add(provider.CreateObjectRegistration());
                }
                kernel.Register(registrations);
            }
        }
    }
}
