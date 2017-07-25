
using System;

namespace My.IoC.Exceptions
{
    /// <summary>
    /// Indicate that the <see cref="ObjectBuilder"/> is obsolete, or does not satisfy the condition.
    /// </summary>
    [Serializable]
    public class InvalidLifetimeScopeException : Exception
    {
        public InvalidLifetimeScopeException() { }
        public InvalidLifetimeScopeException(string message) : base(message) { }
        public InvalidLifetimeScopeException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Indicate that the <see cref="ObjectBuilder"/> is obsolete, or does not satisfy the condition.
    /// </summary>
    [Serializable]
    public class InvalidObjectBuilderException : Exception
    {
        public InvalidObjectBuilderException() { }
        public InvalidObjectBuilderException(string message) : base(message) { }
        public InvalidObjectBuilderException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Indicate that an <see cref="ObjectBuilder"/> can not be found for the given contract type.
    /// </summary>
    [Serializable]
    public class ObjectBuilderNotFoundException : Exception
    {
        public ObjectBuilderNotFoundException() { }
        public ObjectBuilderNotFoundException(string message) : base(message) { }
        public ObjectBuilderNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Indicate that the <see cref="ObjectBuilder"/> is obsolete.
    /// </summary>
    [Serializable]
    public class ObsoleteObjectBuilderException : Exception
    {
        public ObsoleteObjectBuilderException() { }
        public ObsoleteObjectBuilderException(string message) : base(message) { }
        public ObsoleteObjectBuilderException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Indicate that more than one <see cref="ObjectBuilder"/>s satisfied the request, while only one is needed.
    /// </summary>
    [Serializable]
    public class AmbiguousObjectBuilderException : Exception
    {
        public AmbiguousObjectBuilderException() { }
        public AmbiguousObjectBuilderException(string message) : base(message) { }
        public AmbiguousObjectBuilderException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Indicate that there is a cyclic dependency.
    /// </summary>
    [Serializable]
    public class CyclicDependencyException : Exception
    {
        public CyclicDependencyException() { }
        public CyclicDependencyException(string message) : base(message) { }
        public CyclicDependencyException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Indicate that a dependency can not be found.
    /// </summary>
    [Serializable]
    public class DependencyNotFoundException : Exception
    {
        public DependencyNotFoundException() { }
        public DependencyNotFoundException(string message) : base(message) { }
        public DependencyNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
