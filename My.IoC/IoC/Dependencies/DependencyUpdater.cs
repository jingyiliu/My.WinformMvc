using System.Collections.Generic;

namespace My.IoC.Dependencies
{
    public abstract class DependencyUpdater
    {
        internal protected abstract bool IsAutowirable { get; }
        internal protected abstract bool HasDefaultValue { get; }

        internal protected abstract bool Obsolete { get; }
        internal protected abstract bool IsCollection { get; }

        /// <summary>
        /// Gets the current object builder.
        /// </summary>
        /// <remarks>If the <see cref="IsCollection"/> is <b>false</b>, call this method</remarks>
        internal protected abstract ObjectBuilder GetCurrentObjectBuilder();
        /// <summary>
        /// Gets the current object builders.
        /// </summary>
        /// <remarks>If the <see cref="IsCollection"/> is <b>true</b>, call this method</remarks>
        internal protected abstract IEnumerable<ObjectBuilder> GetCurrentObjectBuilders();

        internal protected abstract bool CanUpdateObjectBuilder(ObjectBuilder builder);
        internal protected abstract void UpdateObjectBuilder(ObjectBuilder builder);
    }
}