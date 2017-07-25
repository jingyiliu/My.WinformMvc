using System;
using System.Collections.Generic;
using My.IoC.Core;

namespace My.IoC.Lifetimes
{
    class QueueLite<T>
    {
        T[] _array;
        int _head;
        int _size;
        int _tail;

        public QueueLite()
            : this(4)
        {
        }

        public QueueLite(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("");
            _array = new T[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        public QueueLite(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentOutOfRangeException("");

            _array = new T[4];
            _size = 0;
            using (var enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    Enqueue(enumerator.Current);
            }
        }

        public int Count
        {
            get { return _size; }
        }

        public void Enqueue(T item)
        {
            if (_size == _array.Length)
            {
                var capacity = (int)((_array.Length * 200L) / 100L);
                if (capacity < (_array.Length + 4))
                    capacity = _array.Length + 4;
                SetCapacity(capacity);
            }
            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length;
            _size++;
        }

        void SetCapacity(int capacity)
        {
            var destinationArray = new T[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, destinationArray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, destinationArray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, destinationArray, _array.Length - _head, _tail);
                }
            }
            _array = destinationArray;
            _head = 0;
            _tail = (_size == capacity) ? 0 : _size;
        }

        public T Dequeue()
        {
            if (_size == 0)
                throw new InvalidOperationException("Empty Queue!");
            var local = _array[_head];
            //_array[_head] = default(T);
            _head = (_head + 1) % _array.Length;
            _size--;
            return local;
        }

        public void Discard()
        {
            _array = null;
        }
    }

    public abstract class LifetimeScope : ISharingLifetimeScope
    {
        readonly object _syncRoot = new object();
        readonly Kernel _kernel;
        QueueLite<IDisposable> _disposables;

        protected LifetimeScope(Kernel kernel)
        {
            _kernel = kernel;
        }

        #region IObjectResolver Members

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public Kernel Kernel
        {
            get { return _kernel; }
        }

        public object Resolve(ObjectBuilder builder, ParameterSet parameters)
        {
            object instance;
            builder.BuildInstance(this, parameters, out instance);
            return instance;
        }

        public T Resolve<T>(ObjectBuilder<T> builder, ParameterSet parameters)
        {
            T instance;
            builder.BuildInstance(this, parameters, out instance);
            return instance;
        }

        #endregion

        #region ILifetimeScope Members

        public abstract ILifetimeScope BeginLifetimeScope();

        public void RegisterForDisposal(IDisposable disposable)
        {
            if (_disposables == null)
                _disposables = new QueueLite<IDisposable>();
            _disposables.Enqueue(disposable);
        }

        public void Dispose()
        {
            lock (_syncRoot)
            {
                MakeSureEndingRightLifetimeScope();
                DisposeAndNotify();
            }
        }

        void DisposeAndNotify()
        {
            if (_disposables == null || _disposables.Count == 0)
                return;

            // Make sure the Dispose method for every disposable instances is only called once.
            while (_disposables.Count > 0)
            {
                var disposable = _disposables.Dequeue();
                disposable.Dispose();
            }
            _disposables.Discard();
            _disposables = null;
            LifetimeScopeEnded.Invoke(GetLifetimeScopeEndedEventArgs());
        }

        protected virtual void MakeSureEndingRightLifetimeScope()
        {
        }

        public event Action<LifetimeScopeEndedEventArgs> LifetimeScopeEnded = (e) => { };

        protected abstract LifetimeScopeEndedEventArgs GetLifetimeScopeEndedEventArgs();

        #endregion

        #region ISharingLifetimeScope Members

        public abstract ISharingLifetimeScope ContainerScope { get; }

        public abstract ISharingLifetimeScope SharingScope { get; }

        public virtual object GetInstance(ObjectDescription description)
        {
            return null;
        }

        public virtual void SetInstance(ObjectDescription description, object instance)
        {
        }

        #endregion
    }

    sealed class ContainerLifetimeScope : LifetimeScope
    {
        public ContainerLifetimeScope(Kernel kernel)
            : base(kernel)
        {
        }

        protected override LifetimeScopeEndedEventArgs GetLifetimeScopeEndedEventArgs()
        {
            return new LifetimeScopeEndedEventArgs(this);
        }

        public override ILifetimeScope BeginLifetimeScope()
        {
            return new RootLifetimeScope(Kernel, this);
        }

        public override ISharingLifetimeScope ContainerScope
        {
            get { return this; }
        }

        public override ISharingLifetimeScope SharingScope
        {
            get { return null; }
        }
    }

    abstract class LifetimeScopeWithParent : LifetimeScope
    {
        protected readonly LifetimeScopeWithParent _parentScope;

        protected LifetimeScopeWithParent(Kernel kernel, LifetimeScopeWithParent parentScope)
            : base(kernel)
        {
            _parentScope = parentScope;
        }

        public LifetimeScopeWithParent ParentScope
        {
            get { return _parentScope; }
        }
    }

    sealed class RootLifetimeScope : LifetimeScopeWithParent
    {
        sealed class ChildLifetimeScope : LifetimeScopeWithParent
        {
            readonly RootLifetimeScope _rootScope;

            public ChildLifetimeScope(Kernel kernel, RootLifetimeScope rootScope)
                : base(kernel, rootScope)
            {
                _rootScope = rootScope;
            }

            private ChildLifetimeScope(Kernel kernel, RootLifetimeScope rootScope, ChildLifetimeScope parentScope)
                : base(kernel, parentScope)
            {
                _rootScope = rootScope;
            }

            public override ILifetimeScope BeginLifetimeScope()
            {
                _rootScope.CurrentParent = this;
                return new ChildLifetimeScope(Kernel, _rootScope, this);
            }

            public override ISharingLifetimeScope ContainerScope
            {
                get { return _rootScope.ContainerScope; }
            }

            public override ISharingLifetimeScope SharingScope
            {
                get { return this; }
            }

            public override object GetInstance(ObjectDescription description)
            {
                return _rootScope.DoGetInstance(description, this);
            }

            public override void SetInstance(ObjectDescription description, object instance)
            {
                _rootScope.DoSetInstance(description, this, instance);
            }

            protected override void MakeSureEndingRightLifetimeScope()
            {
                ThrowWhenEndingWrongLifetimeScope();
                _rootScope.CurrentParent = _parentScope.ParentScope;
            }

            void ThrowWhenEndingWrongLifetimeScope()
            {
                if (_parentScope != _rootScope.CurrentParent)
                    throw new InvalidOperationException("The lifetime scope is ending in wrong order!");
            }

            protected override LifetimeScopeEndedEventArgs GetLifetimeScopeEndedEventArgs()
            {
                return new LifetimeScopeEndedEventArgs(this);
            }
        }

        struct ScopedInstanceKey
        {
            readonly ObjectDescription _description;
            readonly LifetimeScopeWithParent _currentScope;

            public ScopedInstanceKey(ObjectDescription description, LifetimeScopeWithParent currentScope)
            {
                _description = description;
                _currentScope = currentScope;
            }

            public override int GetHashCode()
            {
                return _description.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                var key = (ScopedInstanceKey)obj;

                if (!_description.Equals(key._description))
                    return false;

                var currentScope = key._currentScope;
                if (currentScope != null && ReferenceEquals(currentScope, _currentScope))
                    return true;

                if (currentScope == null)
                    return false;
                currentScope = currentScope.ParentScope;
                if (ReferenceEquals(currentScope, _currentScope))
                    return true;

                if (currentScope == null)
                    return false;
                currentScope = currentScope.ParentScope;
                if (ReferenceEquals(currentScope, _currentScope))
                    return true;

                if (currentScope == null)
                    return false;
                currentScope = currentScope.ParentScope;
                if (ReferenceEquals(currentScope, _currentScope))
                    return true;

                currentScope = currentScope.ParentScope;
                if (currentScope == null)
                    return false;
                while (currentScope != null)
                {
                    if (ReferenceEquals(currentScope, _currentScope))
                        return true;
                    currentScope = currentScope.ParentScope;
                }

                return false;
            }
        }

        readonly ContainerLifetimeScope _containerScope;
        Dictionary<ScopedInstanceKey, object> _cachedInstances;

        public RootLifetimeScope(Kernel kernel, ContainerLifetimeScope containerScope)
            : base(kernel, null)
        {
            _containerScope = containerScope;
            CurrentParent = containerScope;
        }

        internal ILifetimeScope CurrentParent { get; set; }

        public override ILifetimeScope BeginLifetimeScope()
        {
            CurrentParent = this;
            return new ChildLifetimeScope(Kernel, this);
        }

        public override ISharingLifetimeScope ContainerScope
        {
            get { return _containerScope; }
        }

        public override ISharingLifetimeScope SharingScope
        {
            get { return this; }
        }

        public override object GetInstance(ObjectDescription description)
        {
            return DoGetInstance(description, this);
        }

        public override void SetInstance(ObjectDescription description, object instance)
        {
            DoSetInstance(description, this, instance);
        }

        internal object DoGetInstance(ObjectDescription description, LifetimeScopeWithParent currentScope)
        {
            if (_cachedInstances == null)
                return null;
            var key = new ScopedInstanceKey(description, currentScope);
            object instance;
            return _cachedInstances.TryGetValue(key, out instance) ? instance : null;
        }

        internal void DoSetInstance(ObjectDescription description, LifetimeScopeWithParent currentScope, object instance)
        {
            if (_cachedInstances == null)
                _cachedInstances = new Dictionary<ScopedInstanceKey, object>();
            var key = new ScopedInstanceKey(description, currentScope);
            _cachedInstances.Add(key, instance);
        }

        protected override LifetimeScopeEndedEventArgs GetLifetimeScopeEndedEventArgs()
        {
            return new LifetimeScopeEndedEventArgs(this);
        }
    }
}
