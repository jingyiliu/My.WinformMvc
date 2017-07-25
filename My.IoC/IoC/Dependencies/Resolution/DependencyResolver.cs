
using System;
using System.Collections.Generic;
using My.IoC.Condition;
using My.IoC.Core;
using My.IoC.Mapping;
using My.IoC.Observers;
using My.IoC.Registry;

namespace My.IoC.Dependencies.Resolution
{
    #region Base

    abstract class DependencyResolverBase : IDisposable
    {
        protected IInjectionTargetInfo MyTargetInfo;

        protected DependencyResolverBase(IInjectionTargetInfo targetInfo)
        {
            MyTargetInfo = targetInfo;
        }

        public IInjectionTargetInfo InjectionTargetInfo
        {
            get { return MyTargetInfo; }
        }

        public abstract bool IsMappable { get; }
        public abstract bool IsCollection { get; }

        public abstract bool TryInjectObjectBuilders(Kernel kernel);

        /// <summary>
        /// Gets the current object builder.
        /// </summary>
        /// <remarks>If the <see cref="IsCollection"/> is <b>false</b>, call this method</remarks>
        public abstract ObjectBuilder GetCurrentObjectBuilder();
        /// <summary>
        /// Gets the current object builders.
        /// </summary>
        /// <remarks>If the <see cref="IsCollection"/> is <b>true</b>, call this method</remarks>
        public abstract IEnumerable<ObjectBuilder> GetCurrentObjectBuilders();

        public abstract bool Obsolete { get; }

        public abstract bool CanUpdateObjectBuilder(ObjectBuilder builder);
        public abstract void UpdateObjectBuilder(ObjectBuilder builder);

        public virtual void Dispose()
        {
            MyTargetInfo = null;
        }
    }

    abstract class DependencyResolver<TDependency> : DependencyResolverBase
    {
        protected DependencyResolver(IInjectionTargetInfo targetInfo)
            : base(targetInfo)
        {
        }

        public abstract TDependency Resolve(InjectionContext context);
    }

    abstract class SingleDependencyResolver<TDependency> : DependencyResolver<TDependency>
    {
        // After injection (TryInjectObjectBuilders), This value will never be null, even the 
        // ObjectRegistration is unregistered.
        internal protected ObjectBuilder MyBuilder;

        protected SingleDependencyResolver(IInjectionTargetInfo targetInfo)
            : base(targetInfo)
        {
        }

        public override bool Obsolete
        {
            get { return MyBuilder == null || MyBuilder.Obsolete; }
        }

        public override bool IsCollection
        {
            get { return false; }
        }
        public override ObjectBuilder GetCurrentObjectBuilder()
        {
            return MyBuilder;
        }
        public override IEnumerable<ObjectBuilder> GetCurrentObjectBuilders()
        {
            throw new InvalidOperationException("");
        }

        public override bool CanUpdateObjectBuilder(ObjectBuilder builder)
        {
            return
                // the [MyBuilder] is obsolete due to one of its children unregistered or deactivated, and now it is activated again.
                ReferenceEquals(builder, MyBuilder)
                // the [MyBuilder] is obsolete, and now a new [builder] will replace it.
                || (MyBuilder.Obsolete
                && builder.ObjectDescription.ContractType == MyBuilder.ObjectDescription.ContractType
                && builder.MatchCondition(MyTargetInfo));
        }
        public override void UpdateObjectBuilder(ObjectBuilder builder)
        {
            MyBuilder = builder;
        }

        public override void Dispose()
        {
            MyBuilder = null;
            base.Dispose();
        }
    }

    abstract class CollectionDependencyResolver<TDependency> : DependencyResolver<TDependency>
    {
        internal ObjectCollectionObserver MyObserver;
        internal IObjectMapper MyMapper;

        protected CollectionDependencyResolver(IInjectionTargetInfo targetInfo, IObjectMapper mapper)
            : this(targetInfo, mapper, null)
        {
        }

        protected CollectionDependencyResolver(IInjectionTargetInfo targetInfo, IObjectMapper mapper, ObjectCollectionObserver observer)
            : base(targetInfo)
        {
            MyMapper = mapper;
            MyObserver = observer;
        }

        public override bool IsMappable
        {
            get { return true; }
        }

        public override bool Obsolete
        {
            get { return MyObserver == null || MyObserver.Count == 0; }
        }

        public override bool IsCollection
        {
            get { return true; }
        }
        public override ObjectBuilder GetCurrentObjectBuilder()
        {
            throw new InvalidOperationException("");
        }
        public override IEnumerable<ObjectBuilder> GetCurrentObjectBuilders()
        {
            return MyObserver.ObjectBuilders;
        }

        public override bool CanUpdateObjectBuilder(ObjectBuilder builder)
        {
            return true;
        }
        public override void UpdateObjectBuilder(ObjectBuilder builder)
        {
        }

        public override bool TryInjectObjectBuilders(Kernel kernel)
        {
            return kernel.TryGet(MyMapper.InjectableType, MyTargetInfo, out MyObserver);
        }

        protected object DoResolve(InjectionContext context)
        {
            object instance;
            MyMapper.Lock.Enter();
            try
            {
                foreach (var builder in MyObserver.ObjectBuilders)
                {
                    object tmpInstance;
                    builder.BuildInstance(context, null, out tmpInstance);
                    MyMapper.Map(tmpInstance);
                }
                instance = MyMapper.Result;
                MyMapper.Reset();
            }
            finally
            {
                MyMapper.Lock.Exit();
            }

            return instance;
        }

        public override void Dispose()
        {
            MyObserver.Dispose();
            MyObserver = null;
            MyMapper = null;
            base.Dispose();
        }
    }

    #endregion

    #region Weak

    class WeakDependencyResolver : SingleDependencyResolver<object>
    {
        public WeakDependencyResolver(IInjectionTargetInfo targetInfo)
            : base(targetInfo)
        { }

        public override bool IsMappable
        {
            get { return false; }
        }

        public override object Resolve(InjectionContext context)
        {
            object instance;
            MyBuilder.BuildInstance(context, null, out instance);
            return instance;
        }

        public override bool TryInjectObjectBuilders(Kernel kernel)
        {
            return kernel.TryGet(MyTargetInfo.TargetType, MyTargetInfo, out MyBuilder) == ObjectBuilderState.Normal;
        }
    }

    class MappableWeakDependencyResolver : SingleDependencyResolver<object>
    {
        internal readonly IObjectMapper MyMapper;

        public MappableWeakDependencyResolver(IInjectionTargetInfo targetInfo, IObjectMapper mapper)
            : base(targetInfo)
        {
            MyMapper = mapper;
        }

        public override bool IsMappable
        {
            get { return true; }
        }

        public override object Resolve(InjectionContext context)
        {
            object instance;
            MyBuilder.BuildInstance(context, null, out instance);
            return MapInstance(instance);
        }

        object MapInstance(object instance)
        {
            object obj;
            MyMapper.Lock.Enter();
            try
            {
                MyMapper.Map(instance);
                obj = MyMapper.Result;
                MyMapper.Reset();
            }
            finally
            {
                MyMapper.Lock.Exit();
            }
            return obj;
        }

        public override bool TryInjectObjectBuilders(Kernel kernel)
        {
            return kernel.TryGet(MyMapper.InjectableType, MyTargetInfo, out MyBuilder) == ObjectBuilderState.Normal;
        }
    }

    class MappableWeakCollectionDependencyResolver : CollectionDependencyResolver<object>
    {
        public MappableWeakCollectionDependencyResolver(IInjectionTargetInfo targetInfo, IObjectMapper mapper)
            : base(targetInfo, mapper)
        {
        }

        public override object Resolve(InjectionContext context)
        {
            return DoResolve(context);
        }
    }

    #endregion

    #region Strong

    class StrongDependencyResolver<TDependency> : DependencyResolver<TDependency>
    {
        ObjectBuilder<TDependency> _builder;

        public StrongDependencyResolver(IInjectionTargetInfo targetInfo)
            : base(targetInfo)
        {
        }

        public override bool IsMappable
        {
            get { return false; }
        }

        public override bool Obsolete
        {
            get { return _builder == null || _builder.Obsolete; }
        }

        public override bool IsCollection
        {
            get { return false; }
        }
        public override ObjectBuilder GetCurrentObjectBuilder()
        {
            return _builder;
        }
        public override IEnumerable<ObjectBuilder> GetCurrentObjectBuilders()
        {
            throw new InvalidOperationException("");
        }

        public override bool CanUpdateObjectBuilder(ObjectBuilder builder)
        {
            return
                // the [_builder] is obsolete due to one of its children unregistered or deactivated, and now it is activated again.
                ReferenceEquals(builder, _builder)
                // the [_builder] is obsolete, and now a new [builder] will replace it.
                || (_builder.Obsolete
                && builder.ObjectDescription.ContractType == _builder.ObjectDescription.ContractType
                && builder.MatchCondition(MyTargetInfo));
        }

        public override void UpdateObjectBuilder(ObjectBuilder builder)
        {
            if (!ReferenceEquals(builder, _builder))
                _builder = builder.ToGeneric<TDependency>();
        }

        public override bool TryInjectObjectBuilders(Kernel kernel)
        {
            ObjectBuilder builder;
            if (kernel.TryGet(MyTargetInfo.TargetType, MyTargetInfo, out builder) != ObjectBuilderState.Normal)
                return false;
            _builder = builder.ToGeneric<TDependency>();
            return true;
        }

        public override TDependency Resolve(InjectionContext context)
        {
            TDependency instance;
            _builder.BuildInstance(context, null, out instance);
            return instance;
        }

        public override void Dispose()
        {
            _builder = null;
            base.Dispose();
        }
    }

    class MappableStrongDependencyResolver<TDependency> : SingleDependencyResolver<TDependency>
    {
        readonly IObjectMapper _mapper;

        public MappableStrongDependencyResolver(IInjectionTargetInfo targetInfo, IObjectMapper mapper)
            : base(targetInfo)
        {
            _mapper = mapper;
        }

        public override bool IsMappable
        {
            get { return true; }
        }

        public override bool TryInjectObjectBuilders(Kernel kernel)
        {
            return kernel.TryGet(_mapper.InjectableType, MyTargetInfo, out MyBuilder) == ObjectBuilderState.Normal;
        }

        public override TDependency Resolve(InjectionContext context)
        {
            object instance;
            MyBuilder.BuildInstance(context, null, out instance);
            return MapInstance(instance);
        }

        TDependency MapInstance(object instance)
        {
            TDependency tmpInstance;
            _mapper.Lock.Enter();
            try
            {
                _mapper.Map(instance);
                tmpInstance = (TDependency)_mapper.Result;
                _mapper.Reset();
            }
            finally
            {
                _mapper.Lock.Exit();
            }
            return tmpInstance;
        }
    }

    class MappableStrongCollectionDependencyResolver<TDependency> : CollectionDependencyResolver<TDependency>
    {
        public MappableStrongCollectionDependencyResolver(IInjectionTargetInfo targetInfo, IObjectMapper mapper)
            : base(targetInfo, mapper)
        {
        }

        public override TDependency Resolve(InjectionContext context)
        {
            return (TDependency)DoResolve(context);
        }
    }
    
    #endregion
}
