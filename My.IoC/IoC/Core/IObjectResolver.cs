
namespace My.IoC.Core
{
    /// <summary>
    /// Defines a set of methods used to register services into the service container.
    /// </summary>
    public interface IObjectResolver : IHasKernel
    {
        object Resolve(ObjectBuilder builder, ParameterSet parameters);
        T Resolve<T>(ObjectBuilder<T> builder, ParameterSet parameters);
    }
}
