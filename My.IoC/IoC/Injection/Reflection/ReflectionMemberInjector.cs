using System.Reflection;
using My.IoC.Core;
using My.IoC.Dependencies;
using My.Helpers;

namespace My.IoC.Injection.Reflection
{
    public abstract class ReflectionMemberInjector
    {
        public abstract void Execute(object target, InjectionContext context);
    }

    public class ReflectionPropertyInjector : ReflectionMemberInjector
    {
        readonly MethodInfo _propertySetMethod;
        readonly DependencyProvider _depProvider;

        public ReflectionPropertyInjector(MethodInfo propertySetMethod, DependencyProvider depProvider)
        {
            Requires.NotNull(propertySetMethod, "propertySetMethod");
            Requires.NotNull(depProvider, "depProvider");
            _propertySetMethod = propertySetMethod;
            _depProvider = depProvider;
        }

        public MethodInfo PropertySetMethod
        {
            get { return _propertySetMethod; }
        }

        public DependencyProvider DependencyProvider
        {
            get { return _depProvider; }
        }

        public override void Execute(object target, InjectionContext context)
        {
            object instance;
            _depProvider.CreateObject(context, out instance);

            try
            {
                _propertySetMethod.Invoke(target, new object[] { instance });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }

    public class ReflectionMethodInjector : ReflectionMemberInjector
    {
        readonly MethodInfo _method;
        readonly DependencyProvider[] _depProviders;

        public ReflectionMethodInjector(MethodInfo method, DependencyProvider[] depProviders)
        {
            Requires.NotNull(method, "method");
            _method = method;
            _depProviders = depProviders;
        }

        public MethodInfo Method
        {
            get { return _method; }
        }

        public DependencyProvider[] DependencyProviders
        {
            get { return _depProviders; }
        }

        public override void Execute(object target, InjectionContext context)
        {
            if (_depProviders == null)
            {
                _method.Invoke(target, null);
                return;
            }

            var parameters = new object[_depProviders.Length];
            for (int i = 0; i < _depProviders.Length; i++)
                _depProviders[i].CreateObject(context, out parameters[i]);
            
            try
            {
                _method.Invoke(target, parameters);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}