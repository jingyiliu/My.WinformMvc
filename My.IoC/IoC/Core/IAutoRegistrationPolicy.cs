using System;
using My.IoC.Lifetimes;

namespace My.IoC.Core
{
    public interface IAutoRegistrationPolicy
    {
        bool ShouldRegister(Type concreteType);
        ILifetimeProvider GetLifetimeProvider();
    }

    //public class ControllerAutoRegistrationPolicy : IAutoRegistrationPolicy
    //{
    //    #region IAutoRegistrationRule Members

    //    public bool ShouldRegister(Type concreteType)
    //    {
    //        return typeof(Controller).IsAssignableFrom(concreteType);
    //    }

    //    public ILifetimeProvider GetLifetimeProvider()
    //    {
    //        return new TransientLifetimeProvider();
    //    }

    //    #endregion
    //}
}