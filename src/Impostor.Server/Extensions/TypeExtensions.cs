using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Impostor.Server
{
    internal static class TypeExtensions
    {
        /// <summary>
        ///     Get the friendly name for the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The friendly name.</returns>
        [SuppressMessage("ReSharper", "SA1503", Justification = "Readability")]
        public static string GetFriendlyName(this Type type)
        {
            if (type == null)
                return "null";
            if (type == typeof(int))
                return "int";
            if (type == typeof(short))
                return "short";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(long))
                return "long";
            if (type == typeof(float))
                return "float";
            if (type == typeof(double))
                return "double";
            if (type == typeof(decimal))
                return "decimal";
            if (type == typeof(string))
                return "string";
            if (type.IsGenericType)
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName).ToArray()) + ">";
            return type.Name;
        }

        /// <summary>
        ///     Get the friendly name for the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="showParameters">True if the parameters should be included in the name.</param>
        /// <returns>Friendly name of the method.</returns>
        public static string GetFriendlyName(this MethodBase method, bool showParameters = true)
        {
            var str = method.Name;

            if (method.DeclaringType != null)
            {
                str = method.DeclaringType.GetFriendlyName() + '.' + str;
            }

            if (showParameters)
            {
                var parameters = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.GetFriendlyName()));
                str += $"({parameters})";
            }

            return str;
        }
    }
}
