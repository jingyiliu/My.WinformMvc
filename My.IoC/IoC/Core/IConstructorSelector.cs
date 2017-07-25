
using System;
using System.Reflection;

namespace My.IoC.Core
{
    /// <summary>
    /// Represents a class that selects the constructor to be used for creating a new object instance. 
    /// </summary>
    public interface IConstructorSelector
    {
        //int MaxConstructorParameterLength { get; }
        IConstructorFinder ConstructorFinder { get; }
        IConstructorSorter ConstructorSorter { get; }
        /// <summary>
        /// Selects an eligible constructor for injection from the specified <paramref name="concreteType"/>
        /// using the configured parameters.
        /// </summary>
        /// <param name="concreteType">The concrete type.</param>
        /// <param name="configuredParameters">The configured parameters. This parameter can be null.</param>
        /// <returns></returns>
        ConstructorInfo SelectConstructor(Type concreteType, ParameterSet configuredParameters);
    }
}
