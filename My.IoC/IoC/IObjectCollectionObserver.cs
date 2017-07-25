using System;
using My.IoC.Core;

namespace My.IoC
{
    public enum ObjectCollectionChangeMode
    {
        Add,
        Remove
    }

    public class ObjectCollectionChangedEventArgs
    {
        readonly int _position;
        readonly ObjectCollectionChangeMode _changeMode;
        readonly ObjectBuilder _builder;

        public ObjectCollectionChangedEventArgs(ObjectCollectionChangeMode changeMode, int position, ObjectBuilder objectBuilder)
        {
            _changeMode = changeMode;
            _position = position;
            _builder = objectBuilder;
        }

        public ObjectCollectionChangeMode ChangeMode
        {
            get { return _changeMode; }
        }

        public int Position
        {
            get { return _position; }
        }

        public ObjectBuilder ObjectBuilder
        {
            get { return _builder; }
        }

        public ObjectDescription ObjectDescription
        {
            get { return _builder.ObjectDescription; }
        }

        /// <summary>
        /// Gets the contract type, which represents the base type of the object instance to be built
        /// by the associated service or an interface it implements, this value can be used as a part 
        /// of the retrieval key to retrieve the associated service from the container.
        /// </summary>
        public Type ContractType
        {
            get { return _builder.ObjectDescription.ContractType; }
        }

        /// <summary>
        /// Gets the implementation type, which represents the concrete type of the object instance to be built
        /// by the associated service.
        /// </summary>
        public Type ConcreteType
        {
            get { return _builder.ObjectDescription.ConcreteType; }
        }

        /// <summary>
        /// Gets the ranking.
        /// The purpose of using ranking is to allow:
        ///     Selection: when a single service is requested but multiple components qualify, then the service with the lowest ranking will be selected.
        ///     Ordering: when multiple components must be used in a specified order.
        /// </summary>
        public int Ranking
        {
            get { return _builder.ObjectDescription.Ranking; }
        }

        /// <summary>
        /// Gets the metadata. This value could be null if no metadata were registered.
        /// </summary>
        public object Metadata
        {
            get { return _builder.ObjectDescription.Metadata; }
        }
    }

    public interface IObjectCollectionObserver : IDisposable
    {
        int Count { get; }
        Type ContractType { get; }
        ObjectBuilder[] ObjectBuilders { get; }
        event Action<ObjectCollectionChangedEventArgs> Changed;
    }

    public interface IObjectCollectionObserver<T> : IObjectCollectionObserver
    {
        new ObjectBuilder<T>[] ObjectBuilders { get; }
    }
}
