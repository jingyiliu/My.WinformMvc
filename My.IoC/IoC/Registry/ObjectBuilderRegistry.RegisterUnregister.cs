
using System;
using System.Collections.Generic;
using My.Exceptions;
using My.Helpers;
using My.IoC.Core;

namespace My.IoC.Registry
{
    sealed partial class ObjectBuilderRegistry
    {
        #region Private types

        interface IOperation
        {
            ObjectRelation ObjectRelation { get; }
            void Perform();
        }

        //sealed class PlaceholderOperation : IOperation
        //{
        //    readonly RegistrationAdmin _admin;

        //    public PlaceholderOperation(RegistrationAdmin relation)
        //    {
        //        _admin = relation;
        //    }

        //    public RegistrationAdmin RegistrationAdmin
        //    {
        //        get { return _admin; }
        //    }

        //    public void Perform(object manualResetEvent)
        //    {
        //    }
        //}

        abstract class Operation : IOperation
        {
            protected readonly ObjectRelation MyAdmin;

            protected Operation(ObjectRelation relation)
            {
                MyAdmin = relation;
            }

            public ObjectRelation ObjectRelation
            {
                get { return MyAdmin; }
            }

            public void Perform()
            {
                DoPerform();
            }

            protected abstract void DoPerform();
        }

        class UnregistrationOperation : Operation
        {
            public UnregistrationOperation(ObjectRelation relation)
                : base(relation)
            {
            }

            protected override void DoPerform()
            {
                MyAdmin.Unregister();
                MyAdmin.RegistrationState = RegistrationState.Unregistered;
            }
        }

        class PartialUnregistrationOperation : Operation
        {
            public PartialUnregistrationOperation(ObjectRelation relation)
                : base(relation)
            {
            }

            protected override void DoPerform()
            {
                MyAdmin.PartialUnregister();
                MyAdmin.RegistrationState = RegistrationState.Unregistered;
            }
        }

        class DeactivationOperation : Operation
        {
            public DeactivationOperation(ObjectRelation relation)
                : base(relation)
            {
            }

            protected override void DoPerform()
            {
                MyAdmin.Deactivate();
                MyAdmin.RegistrationState = RegistrationState.Deactivated;
            }
        }

        class RegistrationOperation : Operation
        {
            public RegistrationOperation(ObjectRelation relation)
                : base(relation)
            {
            }

            protected override void DoPerform()
            {
                MyAdmin.Register();
            }
        }

        class ActivationOperation : Operation
        {
            public ActivationOperation(ObjectRelation relation)
                : base(relation)
            {
            }

            protected override void DoPerform()
            {
                MyAdmin.Activate();
                MyAdmin.RegistrationState = RegistrationState.Activated;
            }
        }

        #endregion

#if WindowsPhone
        readonly Dictionary<Type, List<ObjectBuilder>> _key2Listeners = new Dictionary<Type, List<ObjectBuilder>>();
#else
        readonly Dictionary<Type, List<ObjectBuilder>> _key2Listeners = new Dictionary<Type, List<ObjectBuilder>>();
#endif

        #region Register

        public void Register(IObjectRegistration registration)
        {
            Requires.NotNull(registration, "registration");

            var operations = new List<IOperation>();
            var listeners = new List<ObjectBuilder>();

            _operationLock.EnterWriteLock();
            try
            {
                AddRegistration(registration, ref operations);
                ActivateListeners(registration, listeners.Count, ref listeners, ref operations);
            }
            finally
            {
                _operationLock.ExitWriteLock();
            }

            NotifyObjectBuilderRegistered(new ObjectRegisteredEventArgs(registration.ObjectBuilder));
            PerformActivationOperations(operations);
        }

        public void Register(IEnumerable<IObjectRegistration> registrations)
        {
            Requires.NotNull(registrations, "registrations");

            var operations = new List<IOperation>();
            var listeners = new List<ObjectBuilder>();

            _operationLock.EnterWriteLock();
            try
            {
                foreach (var registration in registrations)
                {
                    if (registration == null)
                        throw new ArgumentException("");
                    AddRegistration(registration, ref operations);
                    ActivateListeners(registration, listeners.Count, ref listeners, ref operations);
                }
            }
            finally
            {
                _operationLock.ExitWriteLock();
            }

            foreach (var registration in registrations)
                NotifyObjectBuilderRegistered(new ObjectRegisteredEventArgs(registration.ObjectBuilder));
            PerformActivationOperations(operations);
        }

        void AddRegistration(IObjectRegistration registration, ref List<IOperation> operations)
        {
            var key = GetKey(registration.ObjectDescription.ContractType);
            ObjectBuilderGroup group;
            if (!_key2Groups.TryGetValue(key, out group))
            {
                group = new ObjectBuilderGroup();
                _key2Groups.Add(key, group);
            }
            group.Add(registration);

            var relation = registration.ObjectBuilder.ObjectRelation;
            relation.RegistrationState = RegistrationState.Registered;
            relation.ObjectBuilderGroup = group;
            operations.Add(new RegistrationOperation(relation));
        }

        void ActivateListeners(IObjectRegistration registration, int startIndex, ref List<ObjectBuilder> listeners, ref List<IOperation> operations)
        {
            var current = registration.ObjectBuilder;
            #region breadth-first algorithm

            ActivateAndRemoveListenersFor(current, ref listeners, ref operations);
            if (listeners.Count == startIndex)
                return;

            for (int i = startIndex; i < listeners.Count; i++)
                ActivateAndRemoveListenersFor(listeners[i], ref listeners, ref operations);

            #endregion
        }

        void ActivateAndRemoveListenersFor(ObjectBuilder current, ref List<ObjectBuilder> listeners, ref List<IOperation> operations)
        {
            var key = GetKey(current.ObjectDescription.ContractType);
            List<ObjectBuilder> existingListeners;
            if (!_key2Listeners.TryGetValue(key, out existingListeners))
                return;

            // Listeners that can not be updated
            List<ObjectBuilder> unqualifiedListeners = null;

            for (int i = 0; i < existingListeners.Count; i++)
            {
                var existingListener = existingListeners[i];
                // The newly registered ObjectBuilder might not match the condition
                // of the listener, so the listener must stay and wait for other ObjectBuilders.
                if (!existingListener.ObjectRelation.UpdateChild(current))
                {
                    if (unqualifiedListeners == null)
                        unqualifiedListeners = new List<ObjectBuilder>();
                    unqualifiedListeners.Add(existingListener);
                }
                else
                {
                    var listenerAdmin = existingListener.ObjectRelation;
                    // The listener might need other ObjectBuilders to activate,
                    // so we must check to see whether it is qualified to be activated. 
                    if (!listenerAdmin.NeedUpdate)
                        continue;

                    listeners.Add(existingListener);

                    switch (listenerAdmin.RegistrationState)
                    {
                        case RegistrationState.Unregistered:
                        case RegistrationState.Unregistering:
                        case RegistrationState.Activated:
                        case RegistrationState.Registered:
                        case RegistrationState.Activating:
                            throw new ImpossibleException();

                        //case RegistrationState.Activating:
                        //    break;

                        default:
                            //case RegistrationState.Deactivating:
                            //case RegistrationState.Deactivated:
                            listenerAdmin.RegistrationState = RegistrationState.Activating;
                            var operation = new ActivationOperation(listenerAdmin);
                            operations.Add(operation);
                            break;
                    }
                }
            }

            if (unqualifiedListeners == null)
                _key2Listeners.Remove(key);
            else
                _key2Listeners[key] = unqualifiedListeners;
        }

        static void PerformActivationOperations(List<IOperation> operations)
        {
            if (operations == null || operations.Count == 0)
                return;

            for (int i = 0; i < operations.Count; i++)
            {
                var operation = operations[i];
                operation.Perform();
            }
        }

        #endregion

        #region Unregister

        struct RootRegistration
        {
            public IObjectRegistration ObjectRegistration { get; set; }
            public RegistrationState OriginalState { get; set; }
        }

        public void Unregister(IObjectRegistration registration)
        {
            Requires.NotNull(registration, "registration");
            var relation = registration.ObjectBuilder.ObjectRelation;
            var rootReg = new RootRegistration()
            {
                ObjectRegistration = registration,
                OriginalState = relation.RegistrationState
            };
            // Change the registration state, this is to make sure that the ObjectBuilderUnregistering event is only triggered once
            _stateLock.Enter();
            try
            {
                switch (relation.RegistrationState)
                {
                    case RegistrationState.Unregistering:
                    case RegistrationState.Unregistered:
                        return;
                    default:
                        //case RegistrationState.Deactivating:
                        //case RegistrationState.Deactivated:
                        //case RegistrationState.Registered:
                        //case RegistrationState.Activating:
                        //case RegistrationState.Activated:
                        relation.RegistrationState = RegistrationState.Unregistering;
                        break;
                }
            }
            finally
            {
                _stateLock.Exit();
            }

            NotifyObjectBuilderUnregistering(new ObjectUnregisteringEventArgs(registration.ObjectDescription));

            var operations = new List<IOperation>();
            var parents = new List<ObjectBuilder>();

            _operationLock.EnterWriteLock();
            try
            {
                RemoveRootRegistration(rootReg, ref operations);
                DeactivateParentRegistrations(registration, parents.Count, ref parents, ref operations);
            }
            finally
            {
                _operationLock.ExitWriteLock();
            }

            // Must not trigger any events within the scope of a lock to allow the event listeners to operate 
            // (add/remove/get...) the service registry in the event handler. Otherwise, these operations might 
            // cause a dead lock problem.
            PerformDeactivationOperations(operations);
        }

        public void Unregister(IEnumerable<IObjectRegistration> registrations)
        {
            Requires.NotNull(registrations, "registrations");
            var qualifiedItems = new List<RootRegistration>();
            // Change the registration states, make sure the ObjectBuilderUnregistering is only triggered once for each registration
            _stateLock.Enter();
            try
            {
                foreach (var registration in registrations)
                {
                    if (registration == null)
                        throw new ArgumentException("");

                    var relation = registration.ObjectBuilder.ObjectRelation;
                    switch (relation.RegistrationState)
                    {
                        case RegistrationState.Unregistering:
                        case RegistrationState.Unregistered:
                            continue;
                        default:
                            var rootReg = new RootRegistration
                            {
                                ObjectRegistration = registration,
                                OriginalState = relation.RegistrationState
                            };
                            qualifiedItems.Add(rootReg);
                            relation.RegistrationState = RegistrationState.Unregistering;
                            break;
                    }
                }
            }
            finally
            {
                _stateLock.Exit();
            }

            if (qualifiedItems.Count == 0)
                return;

            foreach (var qualifiedItem in qualifiedItems)
                NotifyObjectBuilderUnregistering(new ObjectUnregisteringEventArgs(qualifiedItem.ObjectRegistration.ObjectDescription));

            var parents = new List<ObjectBuilder>();
            var operations = new List<IOperation>();

            _operationLock.EnterWriteLock();
            try
            {
                for (int i = 0; i < qualifiedItems.Count; i++)
                {
                    var qualifiedItem = qualifiedItems[i];
                    RemoveRootRegistration(qualifiedItem, ref operations);
                    DeactivateParentRegistrations(qualifiedItem.ObjectRegistration, parents.Count, ref parents, ref operations);
                }
            }
            finally
            {
                _operationLock.ExitWriteLock();
            }

            PerformDeactivationOperations(operations);
        }

        void RemoveRootRegistration(RootRegistration rootReg, ref List<IOperation> operations)
        {
            var registration = rootReg.ObjectRegistration;
            var builder = registration.ObjectBuilder;
            var relation = builder.ObjectRelation;
            var group = relation.ObjectBuilderGroup;

            switch (rootReg.OriginalState)
            {
                case RegistrationState.Unregistering:
                case RegistrationState.Unregistered:
                    throw new ImpossibleException();

                case RegistrationState.Deactivating:
                case RegistrationState.Deactivated:
                    if (!group.Remove(registration))
                    {
                        relation.RegistrationState = RegistrationState.Unregistered;
                        throw new InvalidOperationException();
                    }
                    RemoveFromListenerList(builder);
                    if (group.Count == 0 && !group.IsObserved)
                    {
                        var key = GetKey(builder.ObjectDescription.ContractType);
                        _key2Groups.Remove(key);
                    }
                    operations.Add(new PartialUnregistrationOperation(relation));
                    break;

                default:
                    //case RegistrationState.Registered:
                    //case RegistrationState.Activating:
                    //case RegistrationState.Activated:
                    if (!group.Remove(registration))
                    {
                        relation.RegistrationState = RegistrationState.Unregistered;
                        throw new InvalidOperationException();
                    }
                    //RemoveFromListenerList(builder);
                    if (group.Count == 0 && !group.IsObserved)
                    {
                        var key = GetKey(builder.ObjectDescription.ContractType);
                        _key2Groups.Remove(key);
                    }
                    operations.Add(new UnregistrationOperation(relation));
                    break;
            }
        }

        // Try to find if the [removing] exists in the listening list 
        // (because it has been deactivated due to one of its children 
        // being unregistered), which is listening for one or more of 
        // its children, and remove all of them if there is (because it 
        // won't need to listen for any children any more)
        void RemoveFromListenerList(ObjectBuilder removing)
        {
            var children = removing.ObjectRelation.Children;
            if (children == null || children.Length == 0)
                return;

            foreach (var child in children)
            {
                var childAdmin = child.ObjectRelation;
                if (!childAdmin.Obsolete)
                    continue;

                // If the child is not serviceable, then we'll assume that 
                // it is listening for an ObjectBuilder which can provide
                // the same contract service like the [removing].
                var key = GetKey(child.ObjectDescription.ContractType);
                List<ObjectBuilder> listeners;
                if (_key2Listeners.TryGetValue(key, out listeners))
                {
                    listeners.Remove(removing);
                    if (listeners.Count == 0)
                        _key2Listeners.Remove(key);
                }
            }
        }

        void DeactivateParentRegistrations(IObjectRegistration registration, int startIndex,
            ref List<ObjectBuilder> allParents, ref List<IOperation> operations)
        {
            #region breadth-first algorithm

            DeactivateParentsAndAddListeners(registration.ObjectBuilder, ref allParents, ref operations);
            if (allParents.Count == startIndex)
                return;

            for (int i = startIndex; i < allParents.Count; i++)
                DeactivateParentsAndAddListeners(allParents[i], ref allParents, ref operations);

            #endregion
        }

        void DeactivateParentsAndAddListeners(ObjectBuilder current, ref List<ObjectBuilder> allParents, ref List<IOperation> operations)
        {
            var parents = current.ObjectRelation.Parents;
            if (parents == null || parents.Length == 0)
                return;

            List<ObjectBuilder> listeners = null;
            for (int i = 0; i < parents.Length; i++)
            {
                var parent = parents[i];
                var parentAdmin = parent.ObjectRelation;

                // If the [parent] don't need any update (it might depends on a ObjectBuilderCollection), 
                // just ignore it.
                if (!parentAdmin.NeedUpdate)
                    continue;

                switch (parentAdmin.RegistrationState)
                {
                    case RegistrationState.Unregistered:
                    case RegistrationState.Unregistering:
                        // The parent [parent] has been marked as "Unregistered/Unregistering" already, so we use a placeholder to 
                        // take the position. 
                        //operations.Add(new PlaceholderOperation(parentAdmin));
                        break;

                    case RegistrationState.Deactivating:
                    case RegistrationState.Deactivated:
                        // The parent [parent] might listen for a different contract, so we must try to add it to the listener list
                        // even it is in the Deactivating/Deactivated state.
                        if (listeners == null)
                            listeners = new List<ObjectBuilder>();
                        listeners.Add(parent);
                        break;

                    default:
                        //case RegistrationState.Registered:
                        //case RegistrationState.Activating:
                        //case RegistrationState.Activated:

                        // Add the [parent] to the total-parent list, so we can use a breadth-first algorithm to
                        // find all parents.
                        allParents.Add(parent);

                        // Add a listener for the [parent].
                        if (listeners == null)
                            listeners = new List<ObjectBuilder>();
                        listeners.Add(parent);

                        // Add a DeactivationOperation for the parent [parent].
                        operations.Add(new DeactivationOperation(parentAdmin));
                        // Don't forget to set the RegistrationState to Deactivating.
                        parentAdmin.RegistrationState = RegistrationState.Deactivating;
                        break;
                }
            }

            if (listeners != null)
                AddListeners(current, listeners);
        }

        void AddListeners(ObjectBuilder deactivated, List<ObjectBuilder> listeners)
        {
            var key = GetKey(deactivated.ObjectDescription.ContractType);
            List<ObjectBuilder> existingListeners;
            if (!_key2Listeners.TryGetValue(key, out existingListeners))
            {
                _key2Listeners.Add(key, listeners);
            }
            else
            {
                existingListeners.AddRange(listeners);
            }
        }

        static void PerformDeactivationOperations(List<IOperation> operations)
        {
            if (operations == null || operations.Count == 0)
                return;

            for (int i = operations.Count - 1; i >= 0; i--)
            {
                var operation = operations[i];
                operation.Perform();
            }
        }

//        static void PerformOperations(List<IDeactivationOperation> operations)
//        {
//            if (operations == null || operations.Count == 0)
//                return;

//            var doneEvents = new WaitHandle[operations.Count];
//            for (int i = operations.Count - 1; i >= 0; i--)
//            {
//                var doneEvent = new ManualResetEvent(false);
//                doneEvents[i] = doneEvent;
//                var operation = operations[i];
//                ThreadPool.QueueUserWorkItem(operation.Perform, doneEvents[i]);
//#if NET20
//                // This is a workaround to one of the BUG in .net 2.0.
//                // See http://www.cnblogs.com/anhr/archive/2008/05/24/ThreadPool_BUG_in_DotNET_2_0_SP1.html
//                Thread.Sleep(1);
//#endif
//            }

//            WaitHandle.WaitAll(doneEvents);
//        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs after a new service has been registered. The observer can use this event to check whether the desired 
        /// builders is available.
        /// </summary>
        /// <remarks>This event is mainly for notification.</remarks>
        public event Action<ObjectRegisteredEventArgs> ObjectBuilderRegistered = (e) => { };

        void NotifyObjectBuilderRegistered(ObjectRegisteredEventArgs args)
        {
            ObjectBuilderRegistered.Invoke(args);
        }

        /// <summary>
        /// Occurs when a service is being unregistered. The observers should release the service when they
        /// receives this event.
        /// </summary>
        /// <remarks>This event is mainly for notification.</remarks>
        public event Action<ObjectUnregisteringEventArgs> ObjectBuilderUnregistering = (e) => { };

        void NotifyObjectBuilderUnregistering(ObjectUnregisteringEventArgs args)
        {
            ObjectBuilderUnregistering.Invoke(args);
        }

        #endregion
    }
}
