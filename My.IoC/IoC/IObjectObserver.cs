using System;
using My.IoC.Core;

namespace My.IoC
{
    public enum ObjectChangeMode
    {
        Activate,
        Deactivate
    }

    public sealed class ObjectChangedEventArgs
    {
        readonly ObjectChangeMode _changeMode;
        readonly ObjectBuilder _builder;

        public ObjectChangedEventArgs(ObjectChangeMode changeMode, ObjectBuilder builder)
        {
            _changeMode = changeMode;
            _builder = builder;
        }

        public ObjectChangeMode ChangeMode
        {
            get { return _changeMode; }
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

    public interface IObjectObserver : IDisposable
    {
        Type ContractType { get; }
        ObjectBuilder ObjectBuilder { get; }
        event Action<ObjectChangedEventArgs> Changed;
    }

    public interface IObjectObserver<T> : IObjectObserver
    {
        new ObjectBuilder<T> ObjectBuilder { get; }
    }
}
