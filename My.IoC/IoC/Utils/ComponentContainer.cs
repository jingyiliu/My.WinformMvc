
using System;
using System.Collections.Generic;
using My.Helpers;
using My.IoC.Extensions.OpenGeneric;
using My.IoC.Helpers;
using My.Threading;

namespace My.IoC.Utils
{
    /// <summary>
    /// Provides a convenient way to add, retrieve and remove custom components
    /// (such as <see cref="OpenGenericRequestHandler"/>).
    /// </summary>
    public class ComponentContainer
    {
        readonly ILock _lock;
        readonly Dictionary<Type, object> _type2Components;

        public ComponentContainer()
        {
            if (SystemHelper.MultiProcessors)
                _lock = new SpinLockSlim();
            else
                _lock = new MonitorLock();

            _type2Components = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Adds the specified component to the container.
        /// </summary>
        /// <param name="component">The component.</param>
        public void Add(object component)
        {
            Requires.NotNull(component, "component");
            _lock.Enter();
            try
            {
                _type2Components.Add(component.GetType(), component);
            }
            finally
            {
                _lock.Exit();
            }
        }

        /// <summary>
        /// Removes the component of specified type from the container.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns></returns>
        public bool Remove(Type componentType)
        {
            Requires.NotNull(componentType, "componentType");
            _lock.Enter();
            try
            {
                return _type2Components.Remove(componentType);
            }
            finally
            {
                _lock.Exit();
            }
        }

        /// <summary>
        /// Tries to get a component of specified type from the container.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <param name="component">The component.</param>
        /// <returns></returns>
        public bool TryGet(Type componentType, out object component)
        {
            Requires.NotNull(componentType, "componentType");
            _lock.Enter();
            try
            {
                return _type2Components.TryGetValue(componentType, out component);
            }
            finally
            {
                _lock.Exit();
            }
        }

        /// <summary>
        /// Tries to get a component of <typeparam name="TComponent"/> from the container.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <param name="component">The component.</param>
        /// <returns></returns>
        public bool TryGet<TComponent>(out TComponent component)
        {
            _lock.Enter();
            try
            {
                object cachedInstance;
                if (_type2Components.TryGetValue(typeof(TComponent), out cachedInstance))
                {
                    component = (TComponent)cachedInstance;
                    return true;
                }
            }
            finally
            {
                _lock.Exit();
            }

            component = default(TComponent);
            return false;
        }

        /// <summary>
        /// Determines whether the container contains a component of the specified type.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns>
        ///   <c>true</c> if the container contains an component of the specified type; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Type componentType)
        {
            Requires.NotNull(componentType, "componentType");
            _lock.Enter();
            try
            {
                return _type2Components.ContainsKey(componentType);
            }
            finally
            {
                _lock.Exit();
            }
        }
    }
}
