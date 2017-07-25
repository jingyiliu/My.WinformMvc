
using System;
using System.Collections.Generic;
using My.Helpers;
using My.IoC.Condition;
using My.IoC.Exceptions;
using My.IoC.Helpers;
using My.IoC.Observers;
using My.Threading;

namespace My.IoC.Registry
{
    enum ObjectBuilderState
    {
        Normal,
        Invalid,
        Unregistered
    }

    abstract class ObjectBuilderSet
    {
        public abstract bool IsValid { get; }
        public abstract int Count { get; }
        public abstract void Add(IObjectRegistration registration);
        public abstract bool Remove(IObjectRegistration registration);

        public abstract ObjectBuilderState TryGetFirstValid(IInjectionTargetInfo targetInfo, out ObjectBuilder builder);
        public abstract ObjectBuilderState TryGetFirstValid(out ObjectBuilder builder);

        public abstract List<ObjectBuilder> GetAllValid(IInjectionTargetInfo targetInfo);
        public abstract List<ObjectBuilder> GetAllValid();
        public abstract List<ObjectBuilder<T>> GetAllValid<T>();

        public abstract List<ObjectObserver> GetObserversForAdd(ObjectBuilder builder);
        public abstract List<ObjectObserver> GetObserversForRemove(ObjectBuilder builder);
        public abstract bool IsObserved { get; }
        public abstract void AddObserver(ObjectObserver observer);
        public abstract void RemoveObserver(ObjectObserver observer);
    }

    class ObjectBuilderGroup// : ObjectBuilderSet
    {
        ObjectBuilderSet _internalSet;

        #region IBuilderSet Members

        public bool IsValid
        {
            get { return _internalSet.IsValid; }
        }

        public int Count
        {
            get { return _internalSet.Count; }
        }

        public void Add(IObjectRegistration registration)
        {
            if (_internalSet == null)
                _internalSet = new SingleObjectBuilderSet();
            else if (_internalSet.Count == 1)
                _internalSet = _internalSet.IsObserved
                    ? new ObservedMultipleObjectBuilderSet((ObservedSingleObjectBuilderSet)_internalSet)
                    : new MultipleObjectBuilderSet((SingleObjectBuilderSet)_internalSet);
            _internalSet.Add(registration);
        }
        public bool Remove(IObjectRegistration registration)
        {
            return _internalSet.Remove(registration);
        }

        public ObjectBuilderState TryGetFirstValid(IInjectionTargetInfo targetInfo, out ObjectBuilder builder)
        {
            return _internalSet.TryGetFirstValid(targetInfo, out builder);
        }
        public ObjectBuilderState TryGetFirstValid(out ObjectBuilder builder)
        {
            return _internalSet.TryGetFirstValid(out builder);
        }

        public List<ObjectBuilder> GetAllValid(IInjectionTargetInfo targetInfo)
        {
            return _internalSet.GetAllValid(targetInfo);
        }
        public List<ObjectBuilder> GetAllValid()
        {
            return _internalSet.GetAllValid();
        }
        public List<ObjectBuilder<T>> GetAllValid<T>()
        {
            return _internalSet.GetAllValid<T>();
        }


        public bool IsObserved
        {
            get { return _internalSet.IsObserved; }
        }
        public void AddObserver(ObjectObserver observer)
        {
            if (!_internalSet.IsObserved)
                ChangeToObservedBuilderSet();
            _internalSet.AddObserver(observer);
        }
        void ChangeToObservedBuilderSet()
        {
            if (_internalSet.Count > 1)
                _internalSet = new ObservedMultipleObjectBuilderSet((MultipleObjectBuilderSet)_internalSet);
            else
                _internalSet = new ObservedSingleObjectBuilderSet((SingleObjectBuilderSet)_internalSet);
        }
        public void RemoveObserver(ObjectObserver observer)
        {
            _internalSet.RemoveObserver(observer);
        }
        public List<ObjectObserver> GetObserversForAdd(ObjectBuilder builder)
        {
            return _internalSet.GetObserversForAdd(builder);
        }
        public List<ObjectObserver> GetObserversForRemove(ObjectBuilder builder)
        {
            return _internalSet.GetObserversForRemove(builder);
        }

        #endregion
    }

    class MultipleObjectBuilderSet : ObjectBuilderSet
    {
        readonly List<IObjectRegistration> _candidates;

        public MultipleObjectBuilderSet(SingleObjectBuilderSet builderSet)
        {
            _candidates = new List<IObjectRegistration> { builderSet.MyRegistration };
        }

        protected MultipleObjectBuilderSet(MultipleObjectBuilderSet builderSet)
        {
            _candidates = builderSet._candidates;
        }

        #region IBuilderSet Members

        public override bool IsValid
        {
            get
            {
                foreach (var candidate in _candidates)
                {
                    if (!candidate.ObjectBuilder.Obsolete)
                        return true;
                }
                return false;
            }
        }

        public override int Count
        {
            get { return _candidates.Count; }
        }

        public override void Add(IObjectRegistration registration)
        {
            var newRanking = registration.ObjectDescription.Ranking;

            if (_candidates.Count == 0)
            {
                _candidates.Add(registration);
            }
            else
            {
                var targetPosition = 0;
                for (int i = _candidates.Count - 1; i >= 0; i--)
                {
                    var candidate = _candidates[i];
                    if (newRanking >= candidate.ObjectDescription.Ranking)
                    {
                        targetPosition = i;
                        break;
                    }
                }

                _candidates.Insert(targetPosition + 1, registration);
            }
        }
        public override bool Remove(IObjectRegistration registration)
        {
            return _candidates.Remove(registration);
        }

        public override ObjectBuilderState TryGetFirstValid(IInjectionTargetInfo targetInfo, out ObjectBuilder builder)
        {
            builder = null;
            if (_candidates.Count == 0)
                return ObjectBuilderState.Unregistered;

            foreach (var candidate in _candidates)
            {
                var candidateBuilder = candidate.ObjectBuilder;
                if (!candidateBuilder.MatchCondition(targetInfo) || candidateBuilder.Obsolete)
                    continue;

                if (builder == null)
                    builder = candidateBuilder;
                else
                    throw AmbiguousObjectBuilderException(true);
            }

            return builder != null ? ObjectBuilderState.Normal : ObjectBuilderState.Invalid;
        }
        public override ObjectBuilderState TryGetFirstValid(out ObjectBuilder builder)
        {
            builder = null;
            if (_candidates.Count == 0)
                return ObjectBuilderState.Unregistered;

            foreach (var candidate in _candidates)
            {
                var candidateBuilder = candidate.ObjectBuilder;
                if (!candidateBuilder.MatchCondition(null) || candidateBuilder.Obsolete)
                    continue;

                if (builder == null)
                    builder = candidateBuilder;
                else
                    throw AmbiguousObjectBuilderException(false);
            }

            return builder != null ? ObjectBuilderState.Normal : ObjectBuilderState.Invalid;
        }
        Exception AmbiguousObjectBuilderException(bool hasInfo)
        {
            return hasInfo
                ? new AmbiguousObjectBuilderException(
                    ExceptionFormatter.Format("More than one ObjectBuilders matches the request for the contract type [{0}] and the injection condition while only one is needed! Please make sure to invoke the IObjectContainer.ResolveAll/IObjectContainer.TryResolveAll methods, instead of IObjectContainer.Resolve/IObjectContainer.TryResolve methods!", _candidates[0].ObjectDescription.ContractType.ToTypeName()))
                : new AmbiguousObjectBuilderException(
                    ExceptionFormatter.Format("More than one ObjectBuilders matches the request for the contract type [{0}] while only one is needed! Please make sure to invoke the IObjectContainer.ResolveAll/IObjectContainer.TryResolveAll methods, instead of IObjectContainer.Resolve/IObjectContainer.TryResolve methods!", _candidates[0].ObjectDescription.ContractType.ToTypeName()));
        }

        public override List<ObjectBuilder> GetAllValid(IInjectionTargetInfo targetInfo)
        {
            List<ObjectBuilder> builders = null;
            foreach (var candidate in _candidates)
            {
                var candidateBuilder = candidate.ObjectBuilder;
                if (!candidateBuilder.MatchCondition(targetInfo) || candidateBuilder.Obsolete)
                    continue;

                if (builders == null)
                    builders = new List<ObjectBuilder>();
                builders.Add(candidateBuilder);
            }
            return builders;
        }
        public override List<ObjectBuilder> GetAllValid()
        {
            List<ObjectBuilder> builders = null;
            foreach (var candidate in _candidates)
            {
                var candidateBuilder = candidate.ObjectBuilder;
                if (!candidateBuilder.MatchCondition(null) || candidateBuilder.Obsolete)
                    continue;

                if (builders == null)
                    builders = new List<ObjectBuilder>();
                builders.Add(candidateBuilder);
            }
            return builders;
        }
        public override List<ObjectBuilder<T>> GetAllValid<T>()
        {
            List<ObjectBuilder<T>> builders = null;
            foreach (var candidate in _candidates)
            {
                var candidateBuilder = candidate.ObjectBuilder;
                if (!candidateBuilder.MatchCondition(null) || candidateBuilder.Obsolete)
                    continue;

                if (builders == null)
                    builders = new List<ObjectBuilder<T>>();
                builders.Add(candidateBuilder.ToGeneric<T>());
            }
            return builders;
        }

        public override bool IsObserved
        {
            get { return false; }
        }
        public override List<ObjectObserver> GetObserversForAdd(ObjectBuilder builder)
        {
            throw new NotImplementedException();
        }
        public override List<ObjectObserver> GetObserversForRemove(ObjectBuilder builder)
        {
            throw new NotImplementedException();
        }
        public override void AddObserver(ObjectObserver observer)
        {
            throw new NotImplementedException();
        }
        public override void RemoveObserver(ObjectObserver observer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class ObservedMultipleObjectBuilderSet : MultipleObjectBuilderSet
    {
        readonly ILock _lock;
        readonly List<ObjectObserver> _observers;

        public ObservedMultipleObjectBuilderSet(MultipleObjectBuilderSet builderSet)
            : base(builderSet)
        {
            if (SystemHelper.MultiProcessors)
                _lock = new SpinLockSlim();
            else
                _lock = new MonitorLock();

            _observers = new List<ObjectObserver>();
        }

        public ObservedMultipleObjectBuilderSet(ObservedSingleObjectBuilderSet builderSet)
            : base(builderSet)
        {
            _observers = builderSet.Observers;
        }

        public override bool IsObserved
        {
            get { return _observers.Count > 0; }
        }

        public override List<ObjectObserver> GetObserversForAdd(ObjectBuilder builder)
        {
            List<ObjectObserver> observers = null;
            _lock.Enter();
            try
            {
                foreach (var observer in _observers)
                {
                    if (!observer.CanAdd(builder))
                        continue;
                    if (observers == null)
                        observers = new List<ObjectObserver>();
                    observers.Add(observer);
                }
                return observers;
            }
            finally
            {
                _lock.Exit();
            }
        }

        public override List<ObjectObserver> GetObserversForRemove(ObjectBuilder builder)
        {
            List<ObjectObserver> observers = null;
            _lock.Enter();
            try
            {
                foreach (var observer in _observers)
                {
                    if (!observer.CanRemove(builder))
                        continue;
                    if (observers == null)
                        observers = new List<ObjectObserver>();
                    observers.Add(observer);
                }
                return observers;
            }
            finally
            {
                _lock.Exit();
            }
        }

        public override void AddObserver(ObjectObserver observer)
        {
            _lock.Enter();
            try
            {
                _observers.Add(observer);
            }
            finally
            {
                _lock.Exit();
            }
        }

        public override void RemoveObserver(ObjectObserver observer)
        {
            _lock.Enter();
            try
            {
                _observers.Remove(observer);
            }
            finally
            {
                _lock.Exit();
            }
        }
    }

    class SingleObjectBuilderSet : ObjectBuilderSet
    {
        internal IObjectRegistration MyRegistration;

        public override bool IsValid
        {
            get { return !MyRegistration.ObjectBuilder.Obsolete; }
        }
        public override int Count
        {
            get { return MyRegistration != null ? 1 : 0; }
        }
        public override void Add(IObjectRegistration registration)
        {
            if (MyRegistration != null)
                throw new InvalidOperationException("Can not add items to a SingleObjectBuilderSet when there is an item already!");
            MyRegistration = registration;
        }
        public override bool Remove(IObjectRegistration registration)
        {
            if (MyRegistration != registration)
                return false;
            MyRegistration = null;
            return true;
        }

        public override ObjectBuilderState TryGetFirstValid(IInjectionTargetInfo targetInfo, out ObjectBuilder builder)
        {
            builder = null;
            if (MyRegistration == null)
                return ObjectBuilderState.Unregistered;

            var myBuilder = MyRegistration.ObjectBuilder;
            if (!myBuilder.MatchCondition(targetInfo) || myBuilder.Obsolete)
                return ObjectBuilderState.Invalid;

            builder = myBuilder;
            return ObjectBuilderState.Normal;
        }
        public override ObjectBuilderState TryGetFirstValid(out ObjectBuilder builder)
        {
            builder = null;
            if (MyRegistration == null)
                return ObjectBuilderState.Unregistered;

            var myBuilder = MyRegistration.ObjectBuilder;
            if (!myBuilder.MatchCondition(null) || myBuilder.Obsolete)
                return ObjectBuilderState.Invalid;

            builder = myBuilder;
            return ObjectBuilderState.Normal;
        }

        public override List<ObjectBuilder> GetAllValid(IInjectionTargetInfo targetInfo)
        {
            var builder = MyRegistration.ObjectBuilder;
            return !builder.MatchCondition(targetInfo) || builder.Obsolete
                ? null
                : new List<ObjectBuilder> { builder };
        }
        public override List<ObjectBuilder> GetAllValid()
        {
            var builder = MyRegistration.ObjectBuilder;
            return !builder.MatchCondition(null) || builder.Obsolete
                ? null
                : new List<ObjectBuilder> { builder };
        }
        public override List<ObjectBuilder<T>> GetAllValid<T>()
        {
            var builder = MyRegistration.ObjectBuilder;
            return !builder.MatchCondition(null) || builder.Obsolete
                ? null
                : new List<ObjectBuilder<T>> { builder.ToGeneric<T>() };
        }


        public override bool IsObserved
        {
            get { return false; }
        }
        public override List<ObjectObserver> GetObserversForAdd(ObjectBuilder builder)
        {
            throw new NotImplementedException();
        }
        public override List<ObjectObserver> GetObserversForRemove(ObjectBuilder builder)
        {
            throw new NotImplementedException();
        }
        public override void AddObserver(ObjectObserver observer)
        {
            throw new NotImplementedException();
        }
        public override void RemoveObserver(ObjectObserver observer)
        {
            throw new NotImplementedException();
        }
    }

    class ObservedSingleObjectBuilderSet : SingleObjectBuilderSet
    {
        readonly ILock _lock;
        internal readonly List<ObjectObserver> Observers = new List<ObjectObserver>();

        public ObservedSingleObjectBuilderSet(SingleObjectBuilderSet builderSet)
        {
            if (SystemHelper.MultiProcessors)
                _lock = new SpinLockSlim();
            else
                _lock = new MonitorLock();

            MyRegistration = builderSet.MyRegistration;
        }

        public override bool IsObserved
        {
            get { return Observers.Count > 0; }
        }

        public override List<ObjectObserver> GetObserversForAdd(ObjectBuilder builder)
        {
            List<ObjectObserver> observers = null;
            _lock.Enter();
            try
            {
                foreach (var observer in Observers)
                {
                    if (!observer.CanAdd(builder))
                        continue;
                    if (observers == null)
                        observers = new List<ObjectObserver>();
                    observers.Add(observer);
                }
                return observers;
            }
            finally
            {
                _lock.Exit();
            }
        }

        public override List<ObjectObserver> GetObserversForRemove(ObjectBuilder builder)
        {
            List<ObjectObserver> observers = null;
            _lock.Enter();
            try
            {
                foreach (var observer in Observers)
                {
                    if (!observer.CanRemove(builder))
                        continue;
                    if (observers == null)
                        observers = new List<ObjectObserver>();
                    observers.Add(observer);
                }
                return observers;
            }
            finally
            {
                _lock.Exit();
            }
        }

        public override void AddObserver(ObjectObserver observer)
        {
            _lock.Enter();
            try
            {
                Observers.Add(observer);
            }
            finally
            {
                _lock.Exit();
            }
        }

        public override void RemoveObserver(ObjectObserver observer)
        {
            _lock.Enter();
            try
            {
                Observers.Remove(observer);
            }
            finally 
            {
                _lock.Exit();
            }
        }
    }
}
