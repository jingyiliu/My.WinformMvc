using System;
using System.Reflection;

namespace My.WinformMvc
{
    public interface IPairManager : IPairProvider
    {
        void RegisterAssembly(Assembly assembly);
        void RegisterController(Type controllerType);
        void RegisterView(Type viewType);
        void VerifyPairs();
    }
}
