
using System;
using My.Helpers;
using My.IoC.Exceptions;
using My.IoC.Helpers;

namespace My.IoC.Dependencies
{
    static class DependencyProviderException
    {
        internal static Exception DependencyUnregistered(Type targetType)
        {
            return new DependencyNotFoundException(
                ExceptionFormatter.Format(Resources.ObjectBuilderRegisteredWithTypeCanNotBeFound, targetType.ToTypeName()));
        }
    }
}
