
using System.Collections.Generic;
using My.IoC.Condition;
using My.IoC.Helpers;
using My.IoC.Registry;
using My.Threading;

namespace My.IoC.Observers
{
    class ObjectCollectionObserver : ObjectObserver
    {
        readonly IInjectionTargetInfo _targetInfo;
        readonly List<ObjectBuilder> _builders;
        readonly ILock _lock;

        public ObjectCollectionObserver(ObjectBuilderGroup group, List<ObjectBuilder> builders, IInjectionTargetInfo targetInfo)
            : base(group)
        {
            if (SystemHelper.MultiProcessors)
                _lock = new SpinLockSlim();
            else
                _lock = new MonitorLock();

            _builders = builders;
            _targetInfo = targetInfo;
        }

        protected internal override bool CanAdd(ObjectBuilder builder)
        {
            return builder.MatchCondition(_targetInfo);
        }

        protected internal override void Add(ObjectBuilder builder)
        {
            _lock.Enter();
            try
            {
                if (_builders.Count == 0)
                {
                    _builders.Add(builder);
                    return;
                }

                var newRanking = builder.ObjectDescription.Ranking;
                if (newRanking < _builders[0].ObjectDescription.Ranking)
                {
                    _builders.Insert(0, builder);
                    return;
                }

                var targetPosition = 0;
                for (int i = _builders.Count - 1; i >= 0; i--)
                {
                    var existing = _builders[i];
                    if (newRanking >= existing.ObjectDescription.Ranking)
                    {
                        targetPosition = i + 1;
                        break;
                    }
                }
                _builders.Insert(targetPosition, builder);
            }
            finally
            {
                _lock.Exit();
            }
        }

        protected internal override bool CanRemove(ObjectBuilder builder)
        {
            return builder.MatchCondition(_targetInfo);
        }

        protected internal override void Remove(ObjectBuilder builder)
        {
            _lock.Enter();
            try
            {
                _builders.Remove(builder);
            }
            finally
            {
                _lock.Exit();
            }
        }

        public int Count
        {
            get { return _builders.Count; }
        }

        public IEnumerable<ObjectBuilder> ObjectBuilders
        {
            get
            {
                _lock.Enter();
                try
                {
                    foreach (var builder in _builders)
                        yield return builder;
                }
                finally
                {
                    _lock.Exit();
                }
            }
        }
    }
}
