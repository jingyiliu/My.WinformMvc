
using My.Foundation;
using My.IoC.Registry;

namespace My.IoC.Observers
{
    abstract class ObjectObserver : Disposable
    {
        readonly ObjectBuilderGroup _group;

        protected ObjectObserver(ObjectBuilderGroup group)
        {
            _group = group;
        }

        protected internal abstract bool CanAdd(ObjectBuilder builder);
        protected internal abstract void Add(ObjectBuilder builder);
        protected internal abstract bool CanRemove(ObjectBuilder builder);
        protected internal abstract void Remove(ObjectBuilder builder);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _group.RemoveObserver(this);
            //DisposeUnmanagedResources();
        }
    }
}
