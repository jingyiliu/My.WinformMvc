
using System;
using System.Collections.Generic;
using System.Timers;
using My.IoC.Dependencies;
using My.IoC.Registry;

namespace My.IoC.Core
{
    public class ObjectRelation
    {
        readonly object _syncRoot = new object();
        protected ObjectBuilder MyBuilder;
        ObjectBuilderGroup _group;
        List<ObjectBuilder> _parents;

        internal object SyncRoot
        {
            get { return _syncRoot; }
        }

        /// <summary>
        /// Gets a value indicating whether the ObjectBuilder is obsolete.
        /// </summary>
        internal bool Obsolete
        {
            get
            {
                switch (RegistrationState)
                {
                    case RegistrationState.Registered:
                    case RegistrationState.Activated:
                    case RegistrationState.Activating:
                        return false;
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ObjectBuilder needs update.
        /// If the ObjectBuilder itself or any of its direct child (not its 
        /// child's child) is obsolete, then it needs an update.
        /// </summary>
        internal virtual bool NeedUpdate
        {
            get { return Obsolete; }
        }

        internal RegistrationState RegistrationState { get; set; }

        internal void SetObjectBuilder(ObjectBuilder builder)
        {
            MyBuilder = builder;
        }

        internal ObjectBuilderGroup ObjectBuilderGroup
        {
            get { return _group; }
            set
            {
                if (_group != null)
                    throw new InvalidOperationException("");
                _group = value;
            }
        }

        #region Relationship

        internal virtual void BuildDependencyRelationship(IEnumerable<DependencyProvider> dependencyProvider)
        {
        }

        #region Child

        protected virtual void RemoveFromChildren()
        {
        }

        /// <summary>
        /// Updates the child, return a boolean indicating whether any of the children has been updated.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        internal virtual bool UpdateChild(ObjectBuilder builder)
        {
            return false;
        }

        internal virtual ObjectBuilder[] Children
        {
            get { return null; }
        } 

        #endregion

        #region Parent

        internal void AddParent(ObjectBuilder parent)
        {
            // No need to lock again, because the caller will lock the operation with the _syncRoot
            if (_parents == null)
                _parents = new List<ObjectBuilder>();
            _parents.Add(parent);
        }

        internal void RemoveParent(ObjectBuilder parent)
        {
            lock (_syncRoot)
            {
                if (_parents == null)
                    throw new InvalidOperationException();
                _parents.Remove(parent);
            }
        }

        /// <summary>
        /// Gets the list of ObjectBuilders that directly depends on this,
        /// not including the ObjectBuilders that indirectly depends on this.
        /// </summary>
        internal ObjectBuilder[] Parents
        {
            get
            {
                lock (_syncRoot)
                    return _parents != null ? _parents.ToArray() : null;
            }
        }

        #endregion

        #endregion

        #region Activate/Deactivate/Unregister

        internal void Activate()
        {
            if (Changed != null)
                Changed.Invoke(new ObjectChangedEventArgs(ObjectChangeMode.Activate, MyBuilder));
            AddToObservers();
        }
        
        internal void Register()
        {
        	AddToObservers();
        }

        void AddToObservers()
        {
            if (!_group.IsObserved)
                return;

            // get the ObjectObserver/ObjectCollectionObserver that observing this ObjectBuilder
            var observers = _group.GetObserversForAdd(MyBuilder);
            if (observers == null)
                return;

            // add this ObjectBuilder to the ObjectObserver/ObjectCollectionObserver that 
            // observing it, and send notification
            foreach (var observer in observers)
            {
            	if (observer.Disposed)
                    continue;
                observer.Add(MyBuilder);
            }
        }

        internal void Deactivate()
        {
            RemoveFromObservers();
            if (Changed != null)
                Changed.Invoke(new ObjectChangedEventArgs(ObjectChangeMode.Deactivate, MyBuilder));
        }

        internal void Unregister()
        {
            RemoveFromObservers();
            PartialUnregister();
        }

        internal void PartialUnregister()
        {
            RemoveFromChildren();
            Release();
        }

        void RemoveFromObservers()
        {
            if (!_group.IsObserved)
                return;

            // get the ObjectObserver/ObjectCollectionObserver that observing this ObjectBuilder
            var observers = _group.GetObserversForRemove(MyBuilder);
            if (observers == null)
                return;

            // remove itself from the ObjectObserver/ObjectCollectionObserver that observing this ObjectBuilder, and notify them
            foreach (var observer in observers)
            {
                if (observer.Disposed)
                    continue;
                observer.Remove(MyBuilder);
            }
        }

        #endregion

        /// <summary>
        /// Release the InjectionOperator and all dependencies, so that the ObjectBuilder can not used to build instances any more. 
        /// This can be done immediately when the ObjectBuilder is unregistered, or after some delay (like 500ms), or never release 
        /// it, unless the users choose to release it explicitly.
        /// </summary>
        internal virtual void Release()
        {
        }

        internal event Action<ObjectChangedEventArgs> Changed;
    }

    class ObjectRelationWithProviders : ObjectRelation
    {
        DependencyProvider[] _dependencyProviders;

        /// <summary>
        /// Gets a value indicating whether the ObjectBuilder needs update.
        /// If the ObjectBuilder itself or any of its direct child (not its
        /// child's child) is obsolete, then it needs an update.
        /// </summary>
        internal override bool NeedUpdate
        {
            get
            {
                if (Obsolete)
                    return true;

                if (_dependencyProviders == null || _dependencyProviders.Length == 0)
                    return false;

                foreach (var dependencyProvider in _dependencyProviders)
                {
                    if (dependencyProvider.Obsolete)
                        return true;
                }

                return false;
            }
        }

        internal override ObjectBuilder[] Children
        {
            get
            {
                if (_dependencyProviders == null || _dependencyProviders.Length == 0)
                    return null;

                var builders = new List<ObjectBuilder>();
                foreach (var dependencyProvider in _dependencyProviders)
                {
                    if (dependencyProvider.IsCollection)
                    {
                        var myBuilders = dependencyProvider.GetCurrentObjectBuilders();
                        builders.AddRange(myBuilders);
                    }
                    else
                    {
                        var myBuilder = dependencyProvider.GetCurrentObjectBuilder();
                        builders.Add(myBuilder);
                    }
                }

                return builders.ToArray();
            }
        }

        /// <summary>
        /// Updates the child, return a boolean indicating whether any of the children has been updated.
        /// </summary>
        internal override bool UpdateChild(ObjectBuilder builder)
        {
            if (_dependencyProviders == null || _dependencyProviders.Length == 0)
                return false;

            var result = false;
            foreach (var dependencyProvider in _dependencyProviders)
            {
                if (!dependencyProvider.CanUpdateObjectBuilder(builder))
                    continue;
                dependencyProvider.UpdateObjectBuilder(builder);
                result = true;
            }

            return result;
        }

        protected override void RemoveFromChildren()
        {
            var dependencyProviders = _dependencyProviders;
            if (dependencyProviders == null)
                return;

            foreach (var dependencyProvider in dependencyProviders)
            {
                if (dependencyProvider.IsCollection)
                {
                    var builders = dependencyProvider.GetCurrentObjectBuilders();
                    foreach (var builder in builders)
                        builder.ObjectRelation.RemoveParent(MyBuilder);
                }
                else
                {
                    var builder = dependencyProvider.GetCurrentObjectBuilder();
                    builder.ObjectRelation.RemoveParent(MyBuilder);
                }
            }
        }

        internal override void BuildDependencyRelationship(IEnumerable<DependencyProvider> dependencyProviders)
        {
            if (dependencyProviders == null)
                return;

            var providers = new List<DependencyProvider>();
            foreach (var dependencyProvider in dependencyProviders)
            {
                if (dependencyProvider.HasDefaultValue || !dependencyProvider.IsAutowirable)
                    continue;

                providers.Add(dependencyProvider);
                if (dependencyProvider.IsCollection)
                {
                    var children = dependencyProvider.GetCurrentObjectBuilders();
                    foreach (var child in children)
                        child.ObjectRelation.AddParent(MyBuilder);
                }
                else
                {
                    var child = dependencyProvider.GetCurrentObjectBuilder();
                    child.ObjectRelation.AddParent(MyBuilder);
                }
            }

            _dependencyProviders = providers.ToArray();
        }

        internal override void Release()
        {
            var timer = new Timer
            {
                Interval = 500, // Delay 500ms to call the event handler
                AutoReset = false // Execute only once
            };

            timer.Elapsed += DoRelease;
            timer.Start();
        }

        void DoRelease(object sender, ElapsedEventArgs e)
        {
            var dependencyProviders = _dependencyProviders;
            if (dependencyProviders == null)
                return;
            foreach (var dependencyProvider in _dependencyProviders)
                dependencyProvider.Dispose();
            _dependencyProviders = null;
        }
    }
}
