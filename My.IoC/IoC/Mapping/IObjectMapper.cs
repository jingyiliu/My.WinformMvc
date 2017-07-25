
using System;
using System.Collections.Generic;
using My.IoC.Helpers;
using My.IoC.Registry;
using My.Threading;

namespace My.IoC.Mapping
{
    /// <summary>
    /// Performs the mapping operation to turn the requested serviceable service into a unserviceable result.
    /// </summary>
    public interface IObjectMapper
    {
        ILock Lock { get; }
        /// <summary>
        /// Gets the target type that is mapped to and can be used to request a <see cref="ObjectBuilder"/> 
        /// from the <see cref="ObjectBuilderRegistry"/>.
        /// </summary>
        Type InjectableType { get; }
        /// <summary>
        /// Gets a value indicating whether the <see cref="Result"/> is some kind of collection.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="Result"/> is some kind of collection; otherwise, <c>false</c>.
        /// </value>
        bool IsCollection { get; }
        /// <summary>
        /// Gets the result of mapping operation.
        /// </summary>
        object Result { get; }
        void Reset();
        /// <summary>
        /// Performs the mapping operation on the specified <paramref name="instance"/>, which is of 
        /// <see cref="InjectableType"/>, to turn it into the result.
        /// </summary>
        /// <param name="instance">The requested service object.</param>
        void Map(object instance);
    }

    abstract class CollectionMapperBase<TElement>
    {
        readonly ILock _lock;

        protected CollectionMapperBase()
        {
            if (SystemHelper.MultiProcessors)
                _lock = new SpinLockSlim();
            else
                _lock = new MonitorLock();
        }

        public ILock Lock
        {
            get { return _lock; }
        }
        public Type InjectableType
        {
            get { return typeof(TElement); }
        }
        public bool IsCollection
        {
            get { return true; }
        }
    }

    class ListMapper<TElement> : CollectionMapperBase<TElement>, IObjectMapper
    {
        protected List<TElement> ResultObject;

        public object Result
        {
            get { return ResultObject; }
        }

        public void Reset()
        {
            ResultObject = null;
        }

        public void Map(object instance)
        {
            if (ResultObject == null)
                ResultObject = new List<TElement>();
            ResultObject.Add((TElement)instance);
        }
    }

    class ArrayMapper<TElement> : ListMapper<TElement>, IObjectMapper
    {
        public new object Result
        {
            get { return ResultObject.ToArray(); }
        }
    }

    class QueueMapper<TElement> : CollectionMapperBase<TElement>, IObjectMapper
    {
        Queue<TElement> _result;

        public object Result
        {
            get { return _result; }
        }

        public void Reset()
        {
            _result = null;
        }

        public void Map(object instance)
        {
            if (_result == null)
                _result = new Queue<TElement>();
            _result.Enqueue((TElement)instance);
        }
    }

    class StackMapper<TElement> : CollectionMapperBase<TElement>, IObjectMapper
    {
        Stack<TElement> _result;

        public object Result
        {
            get { return _result; }
        }

        public void Reset()
        {
            _result = null;
        }

        public void Map(object instance)
        {
            if (_result == null)
                _result = new Stack<TElement>();
            _result.Push((TElement)instance);
        }
    }
}


