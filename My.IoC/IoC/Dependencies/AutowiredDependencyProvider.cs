using System;
using System.Collections.Generic;
using My.IoC.Condition;
using My.IoC.Core;
using My.IoC.Dependencies.Resolution;
using My.IoC.Mapping;

namespace My.IoC.Dependencies
{
    public abstract partial class DependencyProvider
    {
        abstract class AutowiredDependencyProvider<T> : DependencyProvider<T>
        {
            protected DependencyResolver<T> MyResolver;

            protected AutowiredDependencyProvider(DependencyResolver<T> resolver)
            {
                MyResolver = resolver;
            }

            internal DependencyResolver<T> DependencyResolver
            {
                get { return MyResolver; }
            }

            #region DependencyProvider<T> Members

            internal protected override string TargetName
            {
                get { return MyResolver.InjectionTargetInfo.TargetName; }
            }

            internal protected override Type TargetType
            {
                get { return MyResolver.InjectionTargetInfo.TargetType; }
            }

            internal protected override bool IsAutowirable
            {
                get { return true; }
            }

            internal protected override bool HasDefaultValue
            {
                get { return false; }
            }

            // If there is cyclic dependency, this call might cause StackOverflowEx
            internal protected override bool Obsolete
            {
                get { return MyResolver.Obsolete; }
            }

            internal protected override IInjectionTargetInfo InjectionTargetInfo
            {
                get { return MyResolver.InjectionTargetInfo; }
            }

            internal protected override bool IsCollection
            {
                get { return MyResolver.IsCollection; }
            }

            internal protected override ObjectBuilder GetCurrentObjectBuilder()
            {
                return MyResolver.GetCurrentObjectBuilder();
            }

            internal protected override IEnumerable<ObjectBuilder> GetCurrentObjectBuilders()
            {
                return MyResolver.GetCurrentObjectBuilders();
            }

            internal protected override bool CanUpdateObjectBuilder(ObjectBuilder builder)
            {
                return MyResolver.CanUpdateObjectBuilder(builder);
            }

            internal protected override void UpdateObjectBuilder(ObjectBuilder builder)
            {
                MyResolver.UpdateObjectBuilder(builder);
            }

            public override void CreateObject(InjectionContext context, out object instance)
            {
                instance = MyResolver.Resolve(context);
            }

            public override void CreateObject(InjectionContext context, out T instance)
            {
                instance = MyResolver.Resolve(context);
            }

            public override void Dispose()
            {
                MyResolver.Dispose();
            }

            #endregion
        }

        sealed class WeakAutowiredDependencyProvider : AutowiredDependencyProvider<object>
        {
            public WeakAutowiredDependencyProvider(IInjectionTargetInfo injectionTargetInfo)
                : base(new WeakDependencyResolver(injectionTargetInfo))
            {
            }

            internal protected override void InjectObjectBuilders(Kernel kernel)
            {
                if (MyResolver.TryInjectObjectBuilders(kernel))
                    return;

                var targetType = MyResolver.InjectionTargetInfo.TargetType;
                IObjectMapper mapper;
                if (!kernel.ObjectMapperManager.TryGetMapper(targetType, out mapper))
                    throw DependencyProviderException.DependencyUnregistered(targetType);

                if (mapper.IsCollection)
                    MyResolver = new MappableWeakCollectionDependencyResolver(MyResolver.InjectionTargetInfo, mapper);
                else
                    MyResolver = new MappableWeakDependencyResolver(MyResolver.InjectionTargetInfo, mapper);

                if (MyResolver.TryInjectObjectBuilders(kernel))
                    return;

                MyResolver = new WeakDependencyResolver(MyResolver.InjectionTargetInfo);
                var builder = kernel.AutoObjectRegistrar.GetObjectBuilder(targetType, MyResolver.InjectionTargetInfo);
                if (builder == null)
                    throw DependencyProviderException.DependencyUnregistered(targetType);
                MyResolver.UpdateObjectBuilder(builder);
            }
        }

        sealed class StrongAutowiredDependencyProvider<T> : AutowiredDependencyProvider<T>
        {
            public StrongAutowiredDependencyProvider(IInjectionTargetInfo injectionTargetInfo)
                : base(new StrongDependencyResolver<T>(injectionTargetInfo))
            {
            }

            internal protected override void InjectObjectBuilders(Kernel kernel)
            {
                if (MyResolver.TryInjectObjectBuilders(kernel))
                    return;

                var targetType = MyResolver.InjectionTargetInfo.TargetType;
                IObjectMapper mapper;
                if (!kernel.ObjectMapperManager.TryGetMapper(targetType, out mapper))
                    throw DependencyProviderException.DependencyUnregistered(targetType);

                if (mapper.IsCollection)
                    MyResolver = new MappableStrongCollectionDependencyResolver<T>(MyResolver.InjectionTargetInfo, mapper);
                else
                    MyResolver = new MappableStrongDependencyResolver<T>(MyResolver.InjectionTargetInfo, mapper);

                if (MyResolver.TryInjectObjectBuilders(kernel))
                    return;

                MyResolver = new StrongDependencyResolver<T>(MyResolver.InjectionTargetInfo);
                var builder = kernel.AutoObjectRegistrar.GetObjectBuilder(targetType, MyResolver.InjectionTargetInfo);
                if (builder == null)
                    throw DependencyProviderException.DependencyUnregistered(targetType);
                MyResolver.UpdateObjectBuilder(builder);
            }
        }
    }
}