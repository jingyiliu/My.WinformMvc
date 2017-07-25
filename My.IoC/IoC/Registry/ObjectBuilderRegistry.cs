using System;
using System.Collections.Generic;
using My.Foundation;
using My.IoC.Core;
using My.IoC.Helpers;
using My.Threading;

namespace My.IoC.Registry
{
    sealed partial class ObjectBuilderRegistry : Disposable
    {
        readonly Kernel _kernel;
        readonly ILock _stateLock;
        readonly IReaderWriterLockSlim _operationLock;

#if WindowsPhone
        // key: the TypeHandle of the contract type
        // value: the builders registered with that type
        readonly Dictionary<RuntimeTypeHandle, ObjectBuilderList> _key2Builders = new Dictionary<int, ObjectBuilderList>();
        static RuntimeTypeHandle GetKey(Type contractType)
        {
            return contractType.TypeHandle;
        }

#else
        // key: the TypeHandle of the contract type
        // value: the builders registered with that type
        readonly Dictionary<Type, ObjectBuilderGroup> _key2Groups = new Dictionary<Type, ObjectBuilderGroup>();

        static Type GetKey(Type contractType)
        {
            return contractType;
        }
#endif

        public ObjectBuilderRegistry(Kernel kernel)
        {
            _kernel = kernel;
            if (SystemHelper.MultiProcessors)
            {
                _stateLock = new SpinLockSlim();
                _operationLock = new SpinReaderWriterLockSlim();
            }
            else
            {
                _stateLock = new MonitorLock();
                _operationLock = new OptimisticReaderWriterLock();
            }
        }

        public IEnumerable<ObjectBuilder> ObjectBuilders
        {
            get
            {
                _operationLock.EnterReadLock();
                try
                {
                    foreach (var group in _key2Groups.Values)
                    {
                        var builders = group.GetAllValid();
                        foreach (var builder in builders)
                            yield return builder;
                    }
                }
                finally
                {
                    _operationLock.ExitReadLock();
                }
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        foreach (var service2Group in _key2Builders)
        //            service2Group.Value.Dispose();
        //        _key2Builders.Clear();
        //    }
        //    //DisposeUnmanagedResources();
        //}
    }
}
