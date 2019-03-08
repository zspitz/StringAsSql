using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StringAsSql.Util {
    public static class TypeExtensions {
        private static Dictionary<Type, bool> hasParameterlessConstructors = new Dictionary<Type, bool>();
        public static bool HasParameterlessConstructor(this Type type) {
            if (!hasParameterlessConstructors.TryGetValue(type, out var ret)) {
                ret = type.GetConstructor(new Type[] { }) != null;
                hasParameterlessConstructors[type] = ret;
            }
            return ret;
        }

        public static Type UnderlyingIfNullable(this Type type) => Nullable.GetUnderlyingType(type) ?? type;

        public static bool IsTupleType(this Type type) {
            if (!type.IsGenericType) { return false; }
            var openType = type.GetGenericTypeDefinition();
            if (openType.In(
                typeof(ValueTuple<>),
                typeof(ValueTuple<,>),
                typeof(ValueTuple<,,>),
                typeof(ValueTuple<,,,>),
                typeof(ValueTuple<,,,,>),
                typeof(ValueTuple<,,,,,>),
                typeof(ValueTuple<,,,,,,>),
                typeof(Tuple<>),
                typeof(Tuple<,>),
                typeof(Tuple<,,>),
                typeof(Tuple<,,,>),
                typeof(Tuple<,,,,>),
                typeof(Tuple<,,,,,>),
                typeof(Tuple<,,,,,,>)
            )) {
                return true;
            }
            return (openType.In(typeof(ValueTuple<,,,,,,,>), typeof(Tuple<,,,,,,,>))
                && type.GetGenericArguments()[7].IsTupleType());
        }

        public static bool IsAnonymous(this Type type) =>
            type.HasAttribute<CompilerGeneratedAttribute>() && type.Name.Contains("Anonymous") && type.Name.ContainsAny("<>", "VB$");

    }
}
