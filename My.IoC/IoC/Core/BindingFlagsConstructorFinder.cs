using System;
using System.Reflection;
using System.Collections.Generic;
using My.Helpers;
using My.IoC.Helpers;

namespace My.IoC.Core
{
    /// <summary>
    /// Finds constructors based on their binding flags.
    /// </summary>
    public class BindingFlagsConstructorFinder : IConstructorFinder
    {
        readonly BindingFlags _bindingFlags;

        /// <summary>
        /// Create an instance matching constructors with the supplied binding flags.
        /// </summary>
        /// <param name="bindingFlags">Binding flags to match.</param>
        public BindingFlagsConstructorFinder(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        /// <summary>
        /// Finds suitable constructors on the target type.
        /// </summary>
        /// <param name="targetType">Type to search for constructors.</param>
        /// <returns>A list of suitable constructors.</returns>
        public List<ConstructorInfo> FindConstructors(Type targetType)
        {
            var flags = BindingFlags.Instance | _bindingFlags;
            var constructors = targetType.GetConstructors(flags);
            Requires.EnsureTrue(constructors.Length > 0, 
                string.Format("Can not find any constructors for type [{0}] using the BindingFlags [{1}]!", 
                targetType.ToFullTypeName(), flags.ToString()));
            return new List<ConstructorInfo>(constructors);
        }
    }
}
