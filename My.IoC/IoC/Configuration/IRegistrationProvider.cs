
using My.IoC.Configuration.Injection;
using My.IoC.Core;

namespace My.IoC.Configuration
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IRegistrationProvider
    {
        /// <summary>
        /// Gets the kernel.
        /// Multiple calls must return the same kernel.
        /// </summary>
        Kernel Kernel { get; }
        /// <summary>
        /// Gets the injection configuration set.
        /// Multiple calls must return the same injection configuration set.
        /// </summary>
        InjectionConfigurationSet InjectionConfigurationSet { get; }
        /// <summary>
        /// Gets the <see cref="IObjectRegistration"/>. 
        /// This method must return the same <see cref="IObjectRegistration"/> instance across multiple calls.
        /// </summary>
        /// <returns></returns>
        IObjectRegistration CreateObjectRegistration();
    }
}
