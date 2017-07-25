using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using My.Exceptions;
using My.Foundation;
using My.Helpers;
using My.IoC.Core;
using My.IoC.Dependencies;
using My.IoC.Injection;
using My.IoC.Injection.Emit;
using My.IoC.Injection.Func;
using My.IoC.Injection.Instance;
using My.IoC.Injection.Reflection;

namespace My.IoC.Configuration.Injection
{
    public interface IInjectionConfigurationInterpreter
    {
        InjectionConfigurationGroup InjectionConfigurationGroup { get; }
        Injector<T> Parse<T>(Kernel kernel, ObjectDescription description, out List<DependencyProvider> groupDepProviders);
    }

    public class EmitInjectionConfigurationInterpreter : IInjectionConfigurationInterpreter
    {
        readonly TypedInjectionConfigurationGroup _configGroup;

        public EmitInjectionConfigurationInterpreter(TypedInjectionConfigurationGroup configGroup)
        {
            _configGroup = configGroup;
        }

        public InjectionConfigurationGroup InjectionConfigurationGroup
        {
            get { return _configGroup; }
        }

        public Injector<T> Parse<T>(Kernel kernel, ObjectDescription description, out List<DependencyProvider> groupDepProviders)
        {
            var ctorConfigItem = _configGroup.ConstructorInjectionConfigurationItem;
            var ctorDepProviders = ctorConfigItem.CreateDependencyProviders(kernel, description, true);

            InjectorEmitBody emitBody;
            Type injectorType;
            ConstructorInfo injectorTypeConstructor;
            Injector<T> injector;

            var ctorEmitBody = new ConstructorEmitBody(kernel.EmitInjectorManager, ctorConfigItem.Constructor, ctorDepProviders);
            var memberConfigItems = _configGroup.MemberInjectionConfigurationItems;
            if (memberConfigItems == null)
            {
                emitBody = new InjectorEmitBody(description.ContractType, ctorEmitBody, null);
                injectorType = BuildDynamicInjectorType(kernel, emitBody);
                injectorTypeConstructor = GetConstructorOfDynamicInjectorType(injectorType, ctorDepProviders, null);
                injector = CreateDynamicInjector<T>(injectorTypeConstructor, ctorDepProviders, null);
                groupDepProviders = ctorDepProviders == null ? null : new List<DependencyProvider>(ctorDepProviders);
                return injector;
            }

            List<DependencyProvider> allMemberDepProviders = null;
            var methodEmitBodySet = new MethodEmitBody[memberConfigItems.Count];
            for (int i = 0; i < memberConfigItems.Count; i++)
            {
                var memberConfigItem = memberConfigItems[i];
                var memberDepProviders = memberConfigItem.CreateDependencyProviders(kernel, description, true);
                if (memberDepProviders == null)
                {
                    if (memberConfigItem.MemberKind == MemberKind.Property)
                        throw new ImpossibleException();
                    methodEmitBodySet[i] = new MethodEmitBody(memberConfigItem.InjectionMethod, null);
                    continue;
                }

                if (allMemberDepProviders == null)
                    allMemberDepProviders = new List<DependencyProvider>();
                allMemberDepProviders.AddRange(memberDepProviders);

                var paramEmitBodySet = new ParameterEmitBody[memberDepProviders.Length];
                for (int j = 0; j < memberDepProviders.Length; j++)
                    paramEmitBodySet[j] = new ParameterEmitBody(memberDepProviders[j]);

                methodEmitBodySet[i] = new MethodEmitBody(memberConfigItem.InjectionMethod, paramEmitBodySet);
            }

            emitBody = new InjectorEmitBody(description.ContractType, ctorEmitBody, methodEmitBodySet);
            injectorType = BuildDynamicInjectorType(kernel, emitBody);
            injectorTypeConstructor = GetConstructorOfDynamicInjectorType(injectorType, ctorDepProviders, allMemberDepProviders);
            injector = CreateDynamicInjector<T>(injectorTypeConstructor, ctorDepProviders, allMemberDepProviders);

            groupDepProviders = ctorDepProviders == null ? null : new List<DependencyProvider>(ctorDepProviders);
            if (groupDepProviders == null)
            {
                if (allMemberDepProviders != null)
                    groupDepProviders = allMemberDepProviders;
            }
            else
            {
                if (allMemberDepProviders != null)
                    groupDepProviders.AddRange(allMemberDepProviders);
            }
            return injector;
        }

        static Type BuildDynamicInjectorType(Kernel kernel, InjectorEmitBody emitBody)
        {
            var injectorManager = kernel.EmitInjectorManager;
            return injectorManager.GetOrCreateInjectorType(emitBody);
        }

        static ConstructorInfo GetConstructorOfDynamicInjectorType(Type injectorType, DependencyProvider[] ctorDepProviders, List<DependencyProvider> memberDepProviders)
        {
            var parameterTypes = ctorDepProviders == null
                ? memberDepProviders == null ? Type.EmptyTypes : new[] { typeof(DependencyProvider[]) }
                : memberDepProviders == null ? new[] { typeof(DependencyProvider[]) } : new[] { typeof(DependencyProvider[]), typeof(DependencyProvider[]) };

            //var ctors = injectorType.GetConstructors();
            var constructor = injectorType.GetConstructor(parameterTypes);
            //var constructor = injectorType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, parameterTypes, null);
            if (constructor == null)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    Resources.CanNotFoundQualifiedConstructorForDynamicType, injectorType.ToFullTypeName()));
            return constructor;
        }

        static Injector<T> CreateDynamicInjector<T>(ConstructorInfo injectorTypeConstructor, DependencyProvider[] ctorDepProviders, List<DependencyProvider> memberDepProviders)
        {
            var objInjector = ctorDepProviders == null
                ? memberDepProviders == null ? injectorTypeConstructor.Invoke(null) : injectorTypeConstructor.Invoke(new object[] { memberDepProviders.ToArray() })
                : memberDepProviders == null ? injectorTypeConstructor.Invoke(new object[] { ctorDepProviders }) : injectorTypeConstructor.Invoke(new object[] { ctorDepProviders, memberDepProviders.ToArray() });

            var injector = objInjector as Injector<T>;
            if (injector == null)
                throw new ArgumentException();

            return injector;
        }
    }

    public class ReflectionInjectionConfigurationInterpreter : IInjectionConfigurationInterpreter
    {
        readonly TypedInjectionConfigurationGroup _configGroup;

        public ReflectionInjectionConfigurationInterpreter(TypedInjectionConfigurationGroup configGroup)
        {
            _configGroup = configGroup;
        }

        public InjectionConfigurationGroup InjectionConfigurationGroup
        {
            get { return _configGroup; }
        }

        public Injector<T> Parse<T>(Kernel kernel, ObjectDescription description, out List<DependencyProvider> groupDepProviders)
        {
            var ctorConfigItem = _configGroup.ConstructorInjectionConfigurationItem;
            var ctorDepProviders = ctorConfigItem.CreateDependencyProviders(kernel, description, false);
            groupDepProviders = ctorDepProviders == null ? null : new List<DependencyProvider>(ctorDepProviders);

            var memberConfigItems = _configGroup.MemberInjectionConfigurationItems;
            if (memberConfigItems == null)
                return new ReflectionConstructorInjector<T>(ctorConfigItem.Constructor, ctorDepProviders);

            var memberInjectors = new ReflectionMemberInjector[memberConfigItems.Count];
            for (int i = 0; i < memberConfigItems.Count; i++)
            {
                DependencyProvider[] memberDepProviders;
                var memberConfigItem = memberConfigItems[i];
                switch (memberConfigItem.MemberKind)
                {
                    case MemberKind.Property:
                        memberDepProviders = memberConfigItem.CreateDependencyProviders(kernel, description, false);
                        if (memberDepProviders == null || memberDepProviders.Length != 1) 
                            throw new ImpossibleException();

                        var memberDepProvider = memberDepProviders[0];
                        if (!memberDepProvider.HasDefaultValue)
                        {
                            if (groupDepProviders == null)
                                groupDepProviders = new List<DependencyProvider> { memberDepProvider };
                            else
                                groupDepProviders.Add(memberDepProvider);
                        }

                        memberInjectors[i] = new ReflectionPropertyInjector(memberConfigItem.InjectionMethod, memberDepProvider);
                        break;

                    case MemberKind.Method:
                        memberDepProviders = memberConfigItem.CreateDependencyProviders(kernel, description, false);
                        if (memberDepProviders != null)
                        {
                            if (groupDepProviders == null)
                                groupDepProviders = new List<DependencyProvider>(memberDepProviders.Length);
                            groupDepProviders.AddRange(memberDepProviders);
                        }

                        memberInjectors[i] = new ReflectionMethodInjector(memberConfigItem.InjectionMethod, memberDepProviders);
                        break;
                }
            }

            return new ReflectionConstructorMemberInjector<T>(ctorConfigItem.Constructor, ctorDepProviders, memberInjectors);
        }
    }

    public class InstanceInjectionConfigurationInterpreter : IInjectionConfigurationInterpreter
    {
        readonly InstanceInjectionConfigurationGroup _configGroup;

        public InstanceInjectionConfigurationInterpreter(InstanceInjectionConfigurationGroup configGroup)
        {
            _configGroup = configGroup;
        }

        public InjectionConfigurationGroup InjectionConfigurationGroup
        {
            get { return _configGroup; }
        }

        public Injector<T> Parse<T>(Kernel kernel, ObjectDescription description, out List<DependencyProvider> groupDepProviders)
        {
            groupDepProviders = null;
            return new InstanceInjector<T>((T)_configGroup.Instance);
        }
    }

    public class FuncInjectionConfigurationInterpreter : IInjectionConfigurationInterpreter
    {
        readonly FuncInjectionConfigurationGroup _configGroup;

        public FuncInjectionConfigurationInterpreter(FuncInjectionConfigurationGroup configGroup)
        {
            _configGroup = configGroup;
        }

        public InjectionConfigurationGroup InjectionConfigurationGroup
        {
            get { return _configGroup; }
        }

        public Injector<T> Parse<T>(Kernel kernel, ObjectDescription description, out List<DependencyProvider> groupDepProviders)
        {
            groupDepProviders = null;
            return new FuncInjector<T>((Func<IResolutionContext, T>)_configGroup.Factory);
        }
    }
}