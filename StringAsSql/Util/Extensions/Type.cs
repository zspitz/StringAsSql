using System;
using System.Collections.Generic;

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
    }
}
