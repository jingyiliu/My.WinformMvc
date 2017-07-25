
using System;
using System.Collections.Generic;
using My.Helpers;
using My.IoC.Condition;
using My.IoC.Observers;

namespace My.IoC.Registry
{
    sealed partial class ObjectBuilderRegistry
    {
        #region TryGet

        public bool TryGet(Type injectable, IInjectionTargetInfo targetInfo, out ObjectCollectionObserver observer)
        {
            Requires.NotNull(injectable, "injectable");
            Requires.NotNull(targetInfo, "info");
            ObjectBuilderGroup group;
            List<ObjectBuilder> builders;
            if (!TryGetCore(injectable, targetInfo, out group, out builders))
            {
                observer = null;
                return false;
            }
            var obs = new ObjectCollectionObserver(group, builders, targetInfo);
            group.AddObserver(obs);
            observer = obs;
            return true;
        }
        public ObjectBuilderState TryGet(Type injectable, IInjectionTargetInfo targetInfo, out ObjectBuilder builder)
        {
            Requires.NotNull(injectable, "injectable");
            Requires.NotNull(targetInfo, "info");
            return TryGetCore(injectable, targetInfo, out builder);
        }

        public bool TryGet(Type contract, out IObjectObserver observer)
        {
            Requires.NotNull(contract, "contract");
            ObjectBuilderGroup group;
            ObjectBuilder builder;
            if (TryGetCore(contract, null, out group, out builder) != ObjectBuilderState.Normal)
            {
                observer = null;
                return false;
            }
            var obs = new ObjectObserverImpl(group, contract, builder);
            group.AddObserver(obs);
            observer = obs;
            return true;
        }
        public bool TryGet<T>(Type contract, out IObjectObserver<T> observer)
        {
            Requires.NotNull(contract, "contract");
            ObjectBuilderGroup group;
            ObjectBuilder builder;
            if (TryGetCore(contract, null, out group, out builder) != ObjectBuilderState.Normal)
            {
                observer = null;
                return false;
            }
            var obs = new ObjectObserverImpl<T>(group, contract, builder.ToGeneric<T>());
            group.AddObserver(obs);
            observer = obs;
            return true;
        }

        public bool TryGet(Type contract, out IObjectCollectionObserver observer)
        {
            Requires.NotNull(contract, "contract");
            ObjectBuilderGroup group;
            List<ObjectBuilder> builders;
            if (!TryGetCore(contract, null, out group, out builders))
            {
                observer = null;
                return false;
            }
            var obs = new ObjectCollectionObserverImpl(group, contract, builders);
            group.AddObserver(obs);
            observer = obs;
            return true;
        }
        public bool TryGet<T>(Type contract, out IObjectCollectionObserver<T> observer)
        {
            Requires.NotNull(contract, "contract");
            ObjectBuilderGroup group;
            List<ObjectBuilder<T>> builders;
            if (!TryGetCore(contract, out group, out builders))
            {
                observer = null;
                return false;
            }
            var obs = new ObjectCollectionObserverImpl<T>(group, contract, builders);
            group.AddObserver(obs);
            observer = obs;
            return true;
        }

        public ObjectBuilderState TryGet(Type contract, out ObjectBuilder builder)
        {
            Requires.NotNull(contract, "contract");
            return TryGetCore(contract, null, out builder);
        }

        public bool TryGet(Type contract, out IEnumerable<ObjectBuilder> builders)
        {
            Requires.NotNull(contract, "contract");
            ObjectBuilderGroup group;
            List<ObjectBuilder> builderList;
            if (!TryGetCore(contract, null, out group, out builderList))
            {
                builders = null;
                return false;
            }
            builders = builderList;
            return true;
        }
        public bool TryGet<T>(Type contract, out IEnumerable<ObjectBuilder<T>> builders)
        {
            Requires.NotNull(contract, "contract");
            ObjectBuilderGroup group;
            List<ObjectBuilder<T>> builderList;
            if (!TryGetCore(contract, out group, out builderList))
            {
                builders = null;
                return false;
            }
            builders = builderList;
            return true;
        }

        ObjectBuilderState TryGetCore(Type contract, IInjectionTargetInfo targetInfo, out ObjectBuilder builder)
        {
            ObjectBuilderGroup group;
            return TryGetCore(contract, targetInfo, out group, out builder);
        }
        ObjectBuilderState TryGetCore(Type contract, IInjectionTargetInfo targetInfo, out ObjectBuilderGroup group, out ObjectBuilder builder)
        {
            var result = ObjectBuilderState.Unregistered;
            var key = GetKey(contract);
            _operationLock.EnterReadLock();
            try
            {
                if (_key2Groups.TryGetValue(key, out group))
                {
                    result = targetInfo != null
                        ? group.TryGetFirstValid(targetInfo, out builder)
                        : group.TryGetFirstValid(out builder);
                    if (result == ObjectBuilderState.Normal)
                        return result;
                }
            }
            finally
            {
                _operationLock.ExitReadLock();
            }

            var args = new ObjectRequestedEventArgs(_kernel, contract, targetInfo);
            if (!HandleObjectBuilderRequest(args))
            {
                builder = null;
                return result;
            }

            _operationLock.EnterReadLock();
            try
            {
                if (group == null && !_key2Groups.TryGetValue(key, out group))
                {
                    builder = null;
                    return result;
                }
                return targetInfo != null
                    ? group.TryGetFirstValid(targetInfo, out builder)
                    : group.TryGetFirstValid(out builder);
            }
            finally
            {
                _operationLock.ExitReadLock();
            }
        }
        bool TryGetCore(Type contract, IInjectionTargetInfo targetInfo, out ObjectBuilderGroup group, out List<ObjectBuilder> builders)
        {
            var key = GetKey(contract);
            _operationLock.EnterReadLock();
            try
            {
                if (_key2Groups.TryGetValue(key, out group))
                {
                    builders = targetInfo != null
                        ? group.GetAllValid(targetInfo)
                        : group.GetAllValid();
                    if (builders != null)
                        return true;
                }
            }
            finally
            {
                _operationLock.ExitReadLock();
            }

            var args = new ObjectRequestedEventArgs(_kernel, contract, targetInfo);
            if (!HandleObjectBuilderRequest(args))
            {
                builders = null;
                return false;
            }

            _operationLock.EnterReadLock();
            try
            {
                if (group == null && !_key2Groups.TryGetValue(key, out group))
                {
                    builders = null;
                    return false;
                }
                builders = targetInfo != null
                    ? group.GetAllValid(targetInfo)
                    : group.GetAllValid();
                return builders != null;
            }
            finally
            {
                _operationLock.ExitReadLock();
            }
        }
        bool TryGetCore<T>(Type contract, out ObjectBuilderGroup group, out List<ObjectBuilder<T>> builders)
        {
            var key = GetKey(contract);
            _operationLock.EnterReadLock();
            try
            {
                if (_key2Groups.TryGetValue(key, out group))
                {
                    builders = group.GetAllValid<T>();
                    if (builders != null)
                        return true;
                }
            }
            finally
            {
                _operationLock.ExitReadLock();
            }

            var args = new ObjectRequestedEventArgs(_kernel, contract, null);
            if (!HandleObjectBuilderRequest(args))
            {
                builders = null;
                return false;
            }

            _operationLock.EnterReadLock();
            try
            {
                if (group == null && !_key2Groups.TryGetValue(key, out group))
                {
                    builders = null;
                    return false;
                }
                builders = group.GetAllValid<T>();
                return builders != null;
            }
            finally
            {
                _operationLock.ExitReadLock();
            }
        }
        bool HandleObjectBuilderRequest(ObjectRequestedEventArgs args)
        {
            ObjectBuilderRequested.Invoke(args);
            if (!args.Handled)
                return false;
            args.CommitRegistrations();
            return true;
        }

        #endregion

        #region Contains

        public bool Contains(Type contract)
        {
            Requires.NotNull(contract, "contract");
            _operationLock.EnterReadLock();
            try
            {
                ObjectBuilderGroup group;
                return _key2Groups.TryGetValue(GetKey(contract), out group)
                    ? group.IsValid
                    : false;
            }
            finally
            {
                _operationLock.ExitReadLock();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a object instance of specified type is requested which has not been registered explicitly. The observer 
        /// can use this event to registered a service to satisfy the request. 
        /// </summary>
        /// <remarks>
        /// This event provides support for various features, like generic type resolution, lazy registration/resolution...
        /// </remarks>
        public event Action<ObjectRequestedEventArgs> ObjectBuilderRequested = (e) => { };

        #endregion
    }
}
