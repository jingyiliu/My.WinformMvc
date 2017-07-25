
using System;
using System.Collections.Generic;
using My.IoC.Helpers;
using My.IoC.Registry;
using My.Threading;

namespace My.IoC.Observers
{
    abstract class ObjectCollectionObserverImplBase<T> : ObjectObserver
        where T : ObjectBuilder
    {
        readonly Type _contractType;
        protected readonly List<T> MyBuilders;
        protected readonly ILock MyLock;

        protected ObjectCollectionObserverImplBase(ObjectBuilderGroup group, Type contractType, List<T> builders)
            : base(group)
        {
            if (SystemHelper.MultiProcessors)
                MyLock = new SpinLockSlim();
            else
                MyLock = new MonitorLock();

            // The caller will ensure that the builders can not be null.
            MyBuilders = builders;
            _contractType = contractType;
        }

        public int Count
        {
            get { return MyBuilders.Count; }
        }

        public Type ContractType
        {
            get { return _contractType; }
        }

        public T[] ObjectBuilders
        {
            get
            {
                MyLock.Enter();
                try
                {
                    return MyBuilders.ToArray();
                }
                finally
                {
                    MyLock.Exit();
                }
            }
        }

        protected int AddObjectBuilderCore(T builder)
        {
            MyLock.Enter();
            try
            {
                if (MyBuilders.Count == 0)
                {
                    MyBuilders.Add(builder);
                    return 0;
                }

                var newRanking = builder.ObjectDescription.Ranking;
                if (newRanking < MyBuilders[0].ObjectDescription.Ranking)
                {
                    MyBuilders.Insert(0, builder);
                    return 0;
                }

                var targetPosition = 0;
                for (int i = MyBuilders.Count - 1; i >= 0; i--)
                {
                    var existing = MyBuilders[i];
                    if (newRanking >= existing.ObjectDescription.Ranking)
                    {
                        targetPosition = i + 1;
                        break;
                    }
                }
                MyBuilders.Insert(targetPosition, builder);
                return targetPosition;
            }
            finally
            {
                MyLock.Exit();
            }
        }

        protected int RemoveObjectBuilderCore(T builder)
        {
            MyLock.Enter();
            try
            {
                var position = MyBuilders.IndexOf(builder);
                if (position < 0)
                    return -1;
                MyBuilders.RemoveAt(position);
                return position;
            }
            finally
            {
                MyLock.Exit();
            }
        }

        protected internal override bool CanAdd(ObjectBuilder builder)
        {
            return builder.MatchCondition(null);
        }

        protected internal override bool CanRemove(ObjectBuilder builder)
        {
            return builder.MatchCondition(null);
        }

        protected void OnChanged(ObjectCollectionChangeMode changeMode, int position, ObjectBuilder builder)
        {
            var args = new ObjectCollectionChangedEventArgs(changeMode, position, builder);
            //Must not hold the lock when this event is triggered
            Changed.Invoke(args);
        }

        public event Action<ObjectCollectionChangedEventArgs> Changed = (e) => { };
    }

    class ObjectCollectionObserverImpl : ObjectCollectionObserverImplBase<ObjectBuilder>, IObjectCollectionObserver
    {
        public ObjectCollectionObserverImpl(ObjectBuilderGroup group, Type contractType, List<ObjectBuilder> builders)
            : base(group, contractType, builders)
        {
        }

        protected internal override void Add(ObjectBuilder builder)
        {
            var position = AddObjectBuilderCore(builder);
            OnChanged(ObjectCollectionChangeMode.Add, position, builder);
        }

        protected internal override void Remove(ObjectBuilder builder)
        {
            var position = RemoveObjectBuilderCore(builder);
            if (position != -1)
                OnChanged(ObjectCollectionChangeMode.Remove, position, builder);
        }
    }

    class ObjectCollectionObserverImpl<T> : ObjectCollectionObserverImplBase<ObjectBuilder<T>>, IObjectCollectionObserver<T>
    {
        public ObjectCollectionObserverImpl(ObjectBuilderGroup group, Type contractType, List<ObjectBuilder<T>> builders)
            : base(group, contractType, builders)
        {
        }

        public new ObjectBuilder[] ObjectBuilders
        {
            get
            {
                MyLock.Enter();
                try
                {
                    var builders = new ObjectBuilder[MyBuilders.Count];
                    for (int i = 0; i < MyBuilders.Count; i++)
                        builders[i] = MyBuilders[i];
                    return builders;
                }
                finally
                {
                    MyLock.Exit();
                }
            }
        }

        protected internal override void Add(ObjectBuilder builder)
        {
            var position = AddObjectBuilderCore(builder.ToGeneric<T>());
            OnChanged(ObjectCollectionChangeMode.Add, position, builder);
        }

        protected internal override void Remove(ObjectBuilder builder)
        {
            var position = RemoveObjectBuilderCore(builder.ToGeneric<T>());
            if (position != -1)
                OnChanged(ObjectCollectionChangeMode.Remove, position, builder);
        } 
    }
}
