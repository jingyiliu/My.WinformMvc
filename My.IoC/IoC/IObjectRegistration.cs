
using System;
using My.IoC.Core;

namespace My.IoC
{
    public interface IObjectRegistration
    {
        ObjectDescription ObjectDescription { get; }
        ObjectBuilder ObjectBuilder { get; }
        event Action<ObjectChangedEventArgs> Changed;
    }

    public interface IObjectRegistration<T> : IObjectRegistration
    {
        new ObjectBuilder<T> ObjectBuilder { get; }
    }
}
