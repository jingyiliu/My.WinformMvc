using System;
using System.Reflection;
using My.IoC.Core;

namespace My.IoC.Condition
{
    public interface IInjectionTargetInfo
    {
        /// <summary>
        /// Gets the <see cref="ObjectDescription"/> which describes the object where the target 
        /// (<see cref="ParameterInfo"/> or <see cref="PropertyInfo"/>) declared.
        /// </summary>
        ObjectDescription TargetDescription { get; }
        /// <summary>
        /// Gets the name of the target.
        /// </summary>
        /// <remarks>This could be the <c>Name</c> of <see cref="ParameterInfo"/> or <see cref="PropertyInfo"/> 
        /// that this instance will be injected into.</remarks>
        string TargetName { get; }
        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <remarks>This could be the <c>ParameterType</c> of <see cref="ParameterInfo"/> or the 
        /// <c>PropertyType</c> of <see cref="PropertyInfo"/> that this instance will be injected 
        /// into.</remarks>
        Type TargetType { get; }
        /// <summary>
        /// Gets the attribute provider of the target (<see cref="ParameterInfo"/> or <see cref="PropertyInfo"/>).
        /// </summary>
        ICustomAttributeProvider TargetAttributeProvider { get; }
    }
}
