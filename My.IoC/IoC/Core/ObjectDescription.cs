
using System;
using My.Helpers;

namespace My.IoC.Core
{
    /// <summary>
    /// Provides the basic infomation of a service registered in the container.
    /// </summary>
    public class ObjectDescription
    {
        static int _id = int.MinValue;
        readonly int _uniqueId;
        readonly Type _contractType;
        Type _concreteType;

        internal ObjectDescription(Type contractType, Type concreteType)
        {
            Requires.NotNull(contractType, "contractType");
            Requires.NotNull(concreteType, "concreteType");
            _contractType = contractType;
            _concreteType = concreteType;
            _uniqueId = _id++;
        }

        /// <summary>
        /// Gets the contract type, which represents the base type of the object instance to be built
        /// by the associated service or an interface it implements, this value can be used as a part 
        /// of the retrieval key to retrieve the associated service from the container.
        /// </summary>
        public Type ContractType
        {
            get { return _contractType; }
        }

        /// <summary>
        /// Gets the implementation type, which represents the concrete type of the object instance to be built
        /// by the associated service.
        /// </summary>
        /// <remarks>This value will be always used whether any other implementations has overrode it or not.</remarks>
        public Type ConcreteType
        {
            get { return _concreteType; }
        }

        internal void ReplaceConcreteType(Type newConcreteType)
        {
            Requires.NotNull(newConcreteType, "newConcreteType");
            if (!_contractType.IsAssignableFrom(newConcreteType))
                throw new InvalidOperationException();
            _concreteType = newConcreteType;
        }

        /// <summary>
        /// Gets the ranking.
        /// The purpose of using ranking is to allow:
        ///     Selection: when a single service is requested but multiple services qualify, then the service with the lowest ranking will be selected.
        ///     Ordering: when multiple services must be used in a specified order.
        /// </summary>
        public virtual int Ranking
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the metadata. For example, an Uid of this object builder.
        /// </summary>
        /// <remarks>This value could be null if no metadata were provided.</remarks>
        public virtual object Metadata
        {
            get { return null; }
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return _uniqueId;
        }
    }

    class ObjectDescriptionWithRanking : ObjectDescription
    {
        readonly int _ranking;

        public ObjectDescriptionWithRanking(Type contractType, Type concreteType, int ranking)
            : base(contractType, concreteType)
        {
            _ranking = ranking;
        }

        public override int Ranking
        {
            get { return _ranking; }
        }
    }

    class ObjectDescriptionWithMetadata : ObjectDescription
    {
        readonly object _metadata;

        public ObjectDescriptionWithMetadata(Type contractType, Type concreteType, object metadata)
            : base(contractType, concreteType)
        {
            _metadata = metadata;
        }

        /// <summary>
        /// Gets the metadata. This value could be null if no metadata were registered.
        /// </summary>
        public override object Metadata
        {
            get { return _metadata; }
        }
    }

    class ObjectDescriptionWithRankingAndMetadata : ObjectDescription
    {
        readonly int _ranking;
        readonly object _metadata;

        public ObjectDescriptionWithRankingAndMetadata(Type contractType, Type concreteType, int ranking, object metadata)
            : base(contractType, concreteType)
        {
            _ranking = ranking;
            _metadata = metadata;
        }

        public override int Ranking
        {
            get { return _ranking; }
        }

        public override object Metadata
        {
            get { return _metadata; }
        }
    }
}
