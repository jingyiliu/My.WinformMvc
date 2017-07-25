using System;
using System.Diagnostics;
using System.Globalization;
using My.Exceptions;
using My.IoC;

namespace My.Helpers
{
    public static partial class Requires
    {
        public static void True(bool value, string message)
        {
            if (!value)
                throw new PreconditionException(message);
        }

        public static void NotNull(object value, string paramName)
        {
            if (value == null)
                throw new PreconditionException(string.Format(CultureInfo.InvariantCulture,
                    Resources.ObjectCanNotBeNull, paramName));
        }

        public static void NotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new PreconditionException(string.Format(CultureInfo.InvariantCulture,
                    Resources.StringCanNotBeNullOrEmpty, paramName));
        }

        public static void NotNullOrWhiteSpace(string value, string paramName)
        {
            if (value.IsNullOrWhiteSpace())
                throw new PreconditionException(string.Format(CultureInfo.InvariantCulture,
                    Resources.StringCanNotBeNullOrEmpty, paramName));
        }

        [Conditional("DEBUG")]
        public static void Assert(bool value, string message)
        {
            if (!value)
                throw new AssertionException(message);
        }

        [Conditional("DEBUG")]
        public static void EnsureTrue(bool value, string message)
        {
            if (!value)
                throw new PostconditionException(message);
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNull(object value, string paramName)
        {
            if (value == null)
                throw new PostconditionException(string.Format("The [{0}] must make sure by itself that it is not null!", paramName));
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
                throw new PostconditionException(string.Format("The [{0}] must make sure by itself that it is not null or empty!", paramName));
        }

        [Conditional("DEBUG")]
        public static void EnsureNotNullOrWhiteSpace(string value, string paramName)
        {
            if (value.IsNullOrWhiteSpace())
                throw new PostconditionException(string.Format("The [{0}] must make sure by itself that it is not null or whitespace!", paramName));
        }

        public static void IsPublicAccessibleType(Type type, string paramName)
        {
            if (!type.IsPublicAccessible())
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    Resources.SuppliedTypeIsNotPublicType, type.ToFullTypeName()), paramName);
        }

        public static void IsConcreteType(Type type, string paramName)
        {
            if (!type.IsClass || type.IsAbstract)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, 
                    Resources.SuppliedTypeIsNotConcreteType, type.ToFullTypeName()), paramName);
        }

        public static void IsNotOpenGenericType(Type type, string paramName)
        {
            // We check for ContainsGenericParameters to see whether there is a Generic Parameter 
            // to find out if this type can be created.
            if (type.ContainsGenericParameters)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    Resources.SuppliedTypeIsOpenGeneric,
                    type.ToFullTypeName()), paramName);
            }
        }

        public static void IsOpenGenericType(Type type, string paramName)
        {
            if (!type.ContainsGenericParameters)
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                                Resources.SuppliedTypeIsNotOpenGeneric, type.ToFullTypeName()), paramName);
        }

        public static void IsAssignableFrom(Type baseType, Type subType)
        {
            if (baseType != subType && !baseType.IsAssignableFrom(subType))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    Resources.SuppliedTypeIsNotAssignableFromType,
                    baseType.ToFullTypeName(),
                    subType.ToFullTypeName()));
            }
        }

        public static void IsAssignableFromGeneric(Type openGenericBaseType, Type openGenericSubType)
        {
            if (openGenericBaseType.IsAssignableFromGeneric(openGenericSubType))
                return;
            
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                Resources.SuppliedTypeIsNotAssignableFromOpenGenericType,
                openGenericSubType.ToFullTypeName(),
                openGenericBaseType.ToFullTypeName()));
        }
    }
}
