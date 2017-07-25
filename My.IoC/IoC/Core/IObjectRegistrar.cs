
using My.IoC.Configuration;

namespace My.IoC.Core
{
    public interface IObjectRegistrar : IHasKernel
    {
        void Register(IRegistrationProvider provider);
    }

    public interface IManualObjectRegistrar : IObjectRegistrar
    {
        void CommitRegistrations();
    }
}
