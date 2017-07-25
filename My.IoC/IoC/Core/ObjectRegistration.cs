
using System;

namespace My.IoC.Core
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    sealed class ObjectRegistration<T> : IObjectRegistration<T>
    {
        readonly ObjectBuilder<T> _builder;

        public ObjectRegistration(ObjectBuilder<T> builder)
        {
            _builder = builder;
        }

        #region IObjectRegistration Members

        public ObjectBuilder ObjectBuilder
        {
            get { return _builder; }
        }

        public event Action<ObjectChangedEventArgs> Changed
        {
            add { _builder.ObjectRelation.Changed += value; }
            remove { _builder.ObjectRelation.Changed -= value; }
        }

        #endregion

        #region IObjectRegistration<T> Members

        ObjectBuilder<T> IObjectRegistration<T>.ObjectBuilder
        {
            get { return _builder; }
        }

        public ObjectDescription ObjectDescription
        {
            get { return _builder.ObjectDescription; }
        }

        #endregion
    }
}
