
using System;
using System.Collections.Generic;
using System.Reflection;

namespace My.IoC.Core
{
    /// <summary>
    /// Find suitable constructors from which to sort and select.
    /// </summary>
    public interface IConstructorFinder
    {
        /// <summary>
        /// Finds suitable constructors on the target type.
        /// </summary>
        /// <param name="targetType">Type to search for constructors.</param>
        /// <returns>A list of suitable constructors.</returns>
        List<ConstructorInfo> FindConstructors(Type targetType);
    }
}
