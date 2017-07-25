using System;

namespace My.IoC.Helpers
{
    static class TypeExtensions
    {
        static readonly Type TypeType = typeof(Type);
        static readonly Type StringType = typeof(string);

        /// <summary>
        /// Determines whether a type is autowirable. When a type is autowirable, 
        /// the container will inject it automatically.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal static bool IsAutowirable(this Type type)
        {
            return type != StringType
                && type != TypeType
                && (type.IsClass || type.IsInterface);
        }

        /// <summary>
        /// Determines whether a type is not autowirable. When a type is not autowirable, 
        /// it means that you have to provide a value for it (like System.String).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal static bool IsNotAutowirable(this Type type)
        {
            return !IsAutowirable(type);
        }

        internal static bool IsAbstractOrInterface(this Type type)
        {
            return type.IsInterface || type.IsAbstract;
        }

        internal static bool IsConcrete(this Type type)
        {
            return type.IsClass && !type.IsAbstract;
        }

        

        internal static RuntimeTypeHandle GetTypeHandle(this Type type)
        {
            return type.TypeHandle;
        }

        internal static int GetTypePointer(this Type type)
        {
            return type.TypeHandle.Value.ToInt32();
        }
    }
}
