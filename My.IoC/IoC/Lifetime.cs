using My.IoC.Exceptions;
using My.IoC.Lifetimes;

namespace My.IoC
{
    public class Lifetime
    {
        static readonly ILifetimeProvider ScopeLifetimeProvider = new ScopeLifetimeProvider();
        static readonly ILifetimeProvider TransientLifetimeProvider = new TransientLifetimeProvider();
        static readonly ILifetimeProvider ContainerLifetimeProvider = new ContainerLifetimeProvider();

        internal static void ThrowWhenMatchingScopeIsNull(ILifetimeScope matchingScope, string error)
        {
            if (matchingScope == null)
                throw new InvalidLifetimeScopeException(error);
        }

        public static ILifetimeProvider Scope()
        {
            return ScopeLifetimeProvider;
        }

        public static ILifetimeProvider Transient()
        {
            return TransientLifetimeProvider;
        }

        public static ILifetimeProvider Container()
        {
            return ContainerLifetimeProvider;
        }
    }

    //public static class LifetimeExtensions
    //{
    //    static readonly ILifetimeCreator ScopeLifetimeCreator = new ScopeLifetimeCreator();
    //    static readonly ILifetimeCreator RequestLifetimeCreator = new RequestLifetimeCreator();
    //    static readonly ILifetimeCreator ContainerLifetimeCreator = new ContainerLifetimeCreator();

    //    public static ILifetimeCreator Scope(this Lifetime lifetime)
    //    {
    //        return ScopeLifetimeCreator;
    //    }

    //    public static ILifetimeCreator Request(this Lifetime lifetime)
    //    {
    //        return RequestLifetimeCreator;
    //    }

    //    public static ILifetimeCreator Container(this Lifetime lifetime)
    //    {
    //        return ContainerLifetimeCreator;
    //    }
    //}
}
