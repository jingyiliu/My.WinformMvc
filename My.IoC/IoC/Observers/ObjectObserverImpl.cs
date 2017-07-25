
using System;
using My.IoC.Registry;

namespace My.IoC.Observers
{
    abstract class ObjectObserverImplBase<T> : ObjectObserver
        where T : ObjectBuilder
    {
        readonly Type _contractType;
        protected T MyBuilder;

        protected ObjectObserverImplBase(ObjectBuilderGroup group, Type contractType, T builder)
            : base(group)
        {
            _contractType = contractType;
            MyBuilder = builder;
        }

        public Type ContractType
        {
            get { return _contractType; }
        }

        public T ObjectBuilder
        {
            get { return MyBuilder; }
        }

        protected internal override bool CanAdd(ObjectBuilder builder)
        {
            return (MyBuilder == null || MyBuilder.Obsolete) && builder.MatchCondition(null);
        }

        protected internal override bool CanRemove(ObjectBuilder builder)
        {
            return ReferenceEquals(MyBuilder, builder);
        }

        protected internal override void Remove(ObjectBuilder builder)
        {
            MyBuilder = null;
            OnChanged(ObjectChangeMode.Deactivate, builder);
        }

        protected void OnChanged(ObjectChangeMode changeMode, ObjectBuilder builder)
        {
            var args = new ObjectChangedEventArgs(changeMode, builder);
            Changed.Invoke(args);
        }

        public event Action<ObjectChangedEventArgs> Changed = (e) => { };
    }

    class ObjectObserverImpl : ObjectObserverImplBase<ObjectBuilder>, IObjectObserver
    {
        public ObjectObserverImpl(ObjectBuilderGroup group, Type contractType, ObjectBuilder builder)
            : base(group, contractType, builder)
        {
        }

        protected internal override void Add(ObjectBuilder builder)
        {
            MyBuilder = builder;
            OnChanged(ObjectChangeMode.Activate, builder);
        }
    }

    class ObjectObserverImpl<T> : ObjectObserverImplBase<ObjectBuilder<T>>, IObjectObserver<T>
    {
        public ObjectObserverImpl(ObjectBuilderGroup group, Type contractType, ObjectBuilder<T> builder)
            : base(group, contractType, builder)
        {
        }

        ObjectBuilder IObjectObserver.ObjectBuilder
        {
            get { return base.ObjectBuilder; }
        }

        protected internal override void Add(ObjectBuilder builder)
        {
            MyBuilder = builder.ToGeneric<T>();
            OnChanged(ObjectChangeMode.Activate, builder);
        }
    }
}
