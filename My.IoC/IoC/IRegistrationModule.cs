using My.IoC.Core;

namespace My.IoC
{
    public interface IRegistrationModule
    {
        void Register(IObjectRegistrar registrar);
    }
}
