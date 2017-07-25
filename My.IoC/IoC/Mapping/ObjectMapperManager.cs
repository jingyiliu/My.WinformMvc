
using System;
using System.Collections.Generic;
using My.Exceptions;
using My.Helpers;
using My.IoC.Helpers;
using My.Threading;

namespace My.IoC.Mapping
{
    class ObjectMapperManager
    {
        readonly ILock _lock;
        readonly ArrayMapperBuilder _arrayMapperBuilder;
        readonly Dictionary<Type, IObjectMapperBuilder> _type2Builders;
        readonly Dictionary<Type, IObjectMapper> _type2Mappers;

        public ObjectMapperManager()
        {
            if (SystemHelper.MultiProcessors)
                _lock = new SpinLockSlim();
            else
                _lock = new MonitorLock();

            _arrayMapperBuilder = new ArrayMapperBuilder();
            _type2Builders = new Dictionary<Type, IObjectMapperBuilder>();
            _type2Mappers = new Dictionary<Type, IObjectMapper>();
        }

        // The builder.BuilderType looks like List<>/IList<> or some other types. 
        public void Add(IObjectMapperBuilder builder)
        {
            Requires.NotNull(builder, "builder");
            _lock.Enter();
            try
            {
                _type2Builders.Add(builder.BuilderType, builder);
            }
            finally
            {
                _lock.Exit();
            }
        }

        public bool Remove(IObjectMapperBuilder builder)
        {
            Requires.NotNull(builder, "builder");
            _lock.Enter();
            try
            {
                return _type2Builders.Remove(builder.BuilderType);
            }
            finally
            {
                _lock.Exit();
            }
        }

        // The sourceType looks like List<MyCustomType>/IList<MyCustomType>, or some other types. 
        // The builder.BuilderType looks like List<>/IList<>, or some other types. 
        public bool TryGetMapper(Type sourceType, out IObjectMapper mapper)
        {
            Requires.NotNull(sourceType, "sourceType");

            if (_type2Mappers.TryGetValue(sourceType, out mapper))
                return true;

            if (sourceType.IsArray)
            {
                mapper = _arrayMapperBuilder.BuildMapper(sourceType);
                _lock.Enter();
                try
                {
                    _type2Mappers.Add(sourceType, mapper);
                }
                finally
                {
                    _lock.Exit();
                }
                return true;
            }

            var builderType = sourceType.IsGenericType
                ? sourceType.GetGenericTypeDefinition()
                : sourceType;

            if (builderType == null)
                throw new ImpossibleException();

            _lock.Enter();
            try
            {
                IObjectMapperBuilder builder;
                if (!_type2Builders.TryGetValue(builderType, out builder))
                    return false;
                mapper = builder.BuildMapper(sourceType);
                _type2Mappers.Add(sourceType, mapper);
                return true;
            }
            finally
            {
                _lock.Exit();
            }
        }

        // The sourceType looks like List<MyCustomType>/IList<MyCustomType>, or some other types. 
        // The builder.BuilderType looks like List<>/IList<>, or some other types. 
        public bool CanMap(Type sourceType)
        {
            Requires.NotNull(sourceType, "sourceType");

            if (sourceType.IsArray)
                return true;

            _lock.Enter();
            try
            {
                if (_type2Mappers.ContainsKey(sourceType))
                    return true;
                var builderType = sourceType.IsGenericType
                    ? sourceType.GetGenericTypeDefinition()
                    : sourceType;
                if (builderType == null)
                    throw new ImpossibleException();
                return _type2Builders.ContainsKey(builderType);
            }
            finally
            {
                _lock.Exit();
            }
        }
    }
}
