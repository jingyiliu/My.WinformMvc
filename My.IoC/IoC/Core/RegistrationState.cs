
using My.IoC.Registry;

namespace My.IoC.Core
{
    /// <summary>
    /// Indicates what state the associated <see cref="ObjectBuilder{T}"/> is in. 
    /// </summary>
    enum RegistrationState : byte
    {
        /// <summary>
        /// Indicating that the associated <see cref="ObjectBuilder{T}"/> has not been registered to the <see cref="ObjectBuilderRegistry"/>, or 
        /// has been unregistered from the <see cref="ObjectBuilderRegistry"/>, or has been deactivated because one of the dependencies has been 
        /// unregistered or deactivated. 
        /// This is the default state.
        /// When a <see cref="ObjectBuilder{T}"/> is in <b>Removing</b> state, it can not be used for building object instances,
        /// this way we can prevent the use of an obsolete <see cref="ObjectBuilder{T}"/>.
        /// </summary>
        Unregistered = 0,
        /// <summary>
        /// Indicating that the associated <see cref="ObjectBuilder{T}"/> is being unregistered from the <see cref="ObjectBuilderRegistry"/>. 
        /// This is a transient state.
        /// When a <see cref="ObjectBuilder{T}"/> is in <b>Removing</b> state, it still can be used for building object instances.
        /// An event will be triggered at this time, so if the <see cref="ObjectBuilder{T}"/> is being used by other Services or 3rd 
        /// parties, they will be notified and have a chance to make adjustments before an exception is thrown for using an 
        /// obsolete <see cref="ObjectBuilder{T}"/>.
        /// </summary>
        Unregistering,

        Registered,

        /// <summary>
        /// Indicating that the associated <see cref="ObjectBuilder{T}"/> has been registered to the <see cref="ObjectBuilderRegistry"/>, so it
        /// can be used to build object instances.
        /// </summary>
       
        /// <summary>
        /// A temporary state.
        /// </summary>
        Deactivating,
        /// <summary>
        /// Indicating that one of the dependency of the associated <see cref="ObjectBuilder{T}"/> (whose dependencies has been successfully resolved 
        /// before) has been unregistered, and it can not be used for building object instances temporarily. In this case, it will be changed 
        /// into <see cref="Deactivated"/> state until the next resolution. At that time, if the missing dependency (dependencies) has been 
        /// registered, it will be tried to build object instances again, and if the operation is successful, its state will be changed to 
        /// <see cref="Resolved"/>. Otherwise, it will be changed to <see cref="Failed"/> state, and an exception will be thrown.
        /// </summary>
        Deactivated,

        Activating,
        Activated,
        ///// <summary>
        ///// Indicating that one of the dependency of the associated <see cref="ObjectBuilder"/> (whose dependencies has been successfully resolved 
        ///// before) has been unregistered, but a new <see cref="ObjectBuilder"/> that meet its dependency requirement has been registered, so it might 
        ///// be able to build object instances again (If nothing goes wrong with the new <see cref="ObjectBuilder"/>).
        ///// </summary>
        //Activated,

//        /// <summary>
//        /// Indicating that the associated <see cref="ObjectBuilder"/> is being resolved.
//        /// This is a transient state.
//        /// </summary>
//        Resolving,
//        /// <summary>
//        /// Indicating that the dependencies of the associated <see cref="ObjectBuilder"/> have been resolved successfully (all dependencies has
//        /// been registered and there is no cyclic dependency problem), so it will not be resolved again.
//        /// Even when a <see cref="ObjectBuilder"/> is in <see cref="Resolved"/> state, there is no guarantee that it can be used to successfully 
//        /// build object instances all the time, because other Services it depends on or the constructor parameters passed at the time of 
//        /// resolution might change.
//        /// </summary>
//        Resolved,
//        /// <summary>
//        /// Indicating that the associated <see cref="ObjectBuilder"/> has a Missing Dependency or Cyclic Dependency problem at the first 
//        /// time of resolution. It means that a configuration error has happened. In this case, an exception will be thrown.
//        /// </summary>
//        Failed
    }
}
