using System;
using My.IoC.Helpers;

namespace My.Helpers
{
    public static class TypeExtensions
    {
        public static bool IsPublicAccessible(this Type type)
        {
            if (!type.IsNested)
                return type.IsPublic;
            if (!type.IsNestedPublic)
                return false;

            var declaringType = type.DeclaringType;
            do
            {
                if (!declaringType.IsNested)
                    return declaringType.IsPublic;
                if (!declaringType.IsNestedPublic)
                    return false;
                declaringType = declaringType.DeclaringType;
            } while (true);
        }

        public static bool IsAssignableFromGeneric(this Type openGenericBaseType, Type openGenericSubType)
        {
            Requires.NotNull(openGenericBaseType, "openGenericBaseType");
            Requires.NotNull(openGenericSubType, "openGenericSubType");
            Requires.IsOpenGenericType(openGenericBaseType, "openGenericBaseType");
            Requires.IsOpenGenericType(openGenericSubType, "openGenericSubType");

            // The (openGenericBaseType == openGenericSubType) won't work for the generic types obtained by 
            // calculation [like Type.GetGenericArguments()], because type.FullName might return null.
            if (ReferenceEquals(openGenericBaseType.Module, openGenericSubType.Module) 
                && openGenericBaseType.MetadataToken == openGenericSubType.MetadataToken)
                return true;

            if (openGenericBaseType.IsInterface)
            {
                var interfaces = openGenericSubType.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == openGenericBaseType)
                        return true;
                }
            }
            else
            {
                var baseType = openGenericSubType.BaseType;
                while (baseType != null)
                {
                    if (baseType.IsGenericType && openGenericBaseType == baseType.GetGenericTypeDefinition())
                        return true;
                    baseType = baseType.BaseType;
                }
            }

            return false;
        }

        public static string ToTypeNameOnly(this Type type)
        {
            Requires.NotNull(type, "type");
            if (type.IsArray)
                return type.GetElementType().ToTypeNameOnly() + "[]";

            var typeName = type.Name;
            return !type.IsGenericType
                ? typeName
                : typeName.Substring(0, typeName.IndexOf('`'));
        }

        public static string ToTypeName(this Type type)
        {
            Requires.NotNull(type, "type");
            if (type.IsArray)
                return type.GetElementType().ToTypeName() + "[]";

            var typeName = (type.IsNested && !type.IsGenericParameter)
                ? type.DeclaringType.ToTypeName() + "+" + type.Name
                : type.Name;

            if (!type.IsGenericType)
                return typeName;

            typeName = typeName.Substring(0, typeName.IndexOf('`'));
            var genericArguments = type.GetGenericArguments();
            var argumentNames = new string[genericArguments.Length];

            for (var i = 0; i < genericArguments.Length; i++)
                argumentNames[i] = genericArguments[i].ToTypeName();

            return typeName + "<" + string.Join(", ", argumentNames) + ">";
        }

        public static string ToFullTypeName(this Type type)
        {
            Requires.NotNull(type, "type");
            if (type.IsArray)
                return type.GetElementType().ToFullTypeName() + "[]";

            var typeName = (type.IsNested && !type.IsGenericParameter)
                ? type.DeclaringType.ToFullTypeName() + "+" + type.Name
                : type.Namespace + "." + type.Name; // type.FullName might return null for the types obtained by calculation [like Type.GetGenericArguments()].

            if (!type.IsGenericType)
                return typeName;

            typeName = typeName.Substring(0, typeName.IndexOf('`'));
            var genericArguments = type.GetGenericArguments();
            var argumentNames = new string[genericArguments.Length];

            for (var i = 0; i < genericArguments.Length; i++)
                argumentNames[i] = genericArguments[i].ToFullTypeName();

            return typeName + "<" + string.Join(", ", argumentNames) + ">";
        }
    }
}
