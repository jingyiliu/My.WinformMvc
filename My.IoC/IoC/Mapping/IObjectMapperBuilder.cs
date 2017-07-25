
using System;
using System.Reflection;
using My.Helpers;
using My.IoC.Registry;
using System.Collections.Generic;

namespace My.IoC.Mapping
{
    public interface IObjectMapperBuilder
    {
        /// <summary>
        /// A concrete type or an open generic type that indicate what kind of types can be mapped.
        /// This will be used as a key to retrieve the <see cref="IObjectMapperBuilder"/> itself.
        /// </summary>
        /// <remarks>
        /// For example, you might decide to map a List{ConcurrencyService} (which is not registered 
        /// to the <see cref="ObjectBuilderRegistry"/>) to a ConcurrencyService (which is registered on the 
        /// other hand), then the generic <code>List{}</code> will be used as the <see cref="BuilderType"/>
        /// here.
        /// </remarks>
        Type BuilderType { get; }
        IObjectMapper BuildMapper(Type sourceType);
    }

    class ArrayMapperBuilder : IObjectMapperBuilder
    {
        static readonly Type ArrayMapperType = typeof(ArrayMapper<>);

        public Type BuilderType
        {
            get { return null; }
        }

        public IObjectMapper BuildMapper(Type sourceType)
        {
            var elementType = sourceType.GetElementType();
            var mapperType = ArrayMapperType.MakeGenericType(elementType);

            try
            {
                var mapper = Activator.CreateInstance(mapperType) as IObjectMapper;
                if (mapper == null)
                    throw new ArgumentException();
                return mapper;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }

    abstract class CollectionMapperBuilderBase : IObjectMapperBuilder
    {
        protected abstract Type GenericMapperType { get; }
        public abstract Type BuilderType { get; }

        public IObjectMapper BuildMapper(Type sourceType)
        {
            Requires.HasOneGenericArgument(sourceType, "sourceType");
            var elementType = sourceType.GetGenericArguments()[0];
            var mapperType = GenericMapperType.MakeGenericType(elementType);

            try
            {
                var mapper = Activator.CreateInstance(mapperType) as IObjectMapper;
                if (mapper == null)
                    throw new ArgumentException();
                return mapper;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }

    class ListMapperBuilder : CollectionMapperBuilderBase
    {
        static readonly Type MyListMapperType = typeof(ListMapper<>);
        static readonly Type MyBuilderType = typeof(List<>);

        protected override Type GenericMapperType
        {
            get { return MyListMapperType; }
        }

        public override Type BuilderType
        {
            get { return MyBuilderType; }
        }
    }

    class IListMapperBuilder : CollectionMapperBuilderBase
    {
        static readonly Type IListMapperType = typeof(ListMapper<>);
        static readonly Type BuilderTypeInternal = typeof(IList<>);

        protected override Type GenericMapperType
        {
            get { return IListMapperType; }
        }

        public override Type BuilderType
        {
            get { return BuilderTypeInternal; }
        }
    }

    class IEnumerableMapperBuilder : CollectionMapperBuilderBase
    {
        static readonly Type IEnumerableMapperType = typeof(ListMapper<>);
        static readonly Type BuilderTypeInternal = typeof(IEnumerable<>);

        protected override Type GenericMapperType
        {
            get { return IEnumerableMapperType; }
        }

        public override Type BuilderType
        {
            get { return BuilderTypeInternal; }
        }
    }

    class QueueMapperBuilder : CollectionMapperBuilderBase
    {
        static readonly Type QueueMapperType = typeof(QueueMapper<>);
        static readonly Type BuilderTypeInternal = typeof(Queue<>);

        protected override Type GenericMapperType
        {
            get { return QueueMapperType; }
        }

        public override Type BuilderType
        {
            get { return BuilderTypeInternal; }
        }
    }

    class StackMapperBuilder : CollectionMapperBuilderBase
    {
        static readonly Type StackMapperType = typeof(StackMapper<>);
        static readonly Type BuilderTypeInternal = typeof(Stack<>);

        protected override Type GenericMapperType
        {
            get { return StackMapperType; }
        }

        public override Type BuilderType
        {
            get { return BuilderTypeInternal; }
        }
    }
}
