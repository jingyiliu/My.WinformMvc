using System;

namespace My.Exceptions
{
    [Serializable]
    public class ImpossibleException : Exception
    {
        public ImpossibleException() { }
        public ImpossibleException(string message) : base(message) { }
        public ImpossibleException(string message, Exception inner) : base(message, inner) { }
    }

    [Serializable]
    public abstract class ContractException : ApplicationException
    {
        protected ContractException() { }
        
        protected ContractException(string message) : base(message) { }
        
        protected ContractException(string message, Exception inner) : base(message, inner) { }
    }

    [Serializable]
    public sealed class PreconditionException : ContractException
    {
        public PreconditionException() { }
        
        public PreconditionException(string message) : base(message) { }
        
        public PreconditionException(string message, Exception inner) : base(message, inner) { }
    }

    [Serializable]
    public sealed class PostconditionException : ContractException
    {
        public PostconditionException() { }
        
        public PostconditionException(string message) : base(message) { }
        
        public PostconditionException(string message, Exception inner) : base(message, inner) { }
    }

    [Serializable]
    public sealed class AssertionException : ContractException
    {
        public AssertionException() { }

        public AssertionException(string message) : base(message) { }

        public AssertionException(string message, Exception inner) : base(message, inner) { }
    }
}
