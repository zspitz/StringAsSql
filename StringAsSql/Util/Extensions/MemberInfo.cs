using System;
using System.Linq;
using System.Reflection;

namespace StringAsSql.Util {
    public static class MemberInfoExtensions {
        public static bool HasAttribute<TAttribute>(this MemberInfo mi, bool inherit = false) where TAttribute : Attribute =>
            mi.GetCustomAttributes(typeof(TAttribute), inherit).Any();

        public static object GetValue(this MemberInfo mi, object instance) {
            // TODO handle indexers
            if (mi is PropertyInfo pi) { return pi.GetValue(instance); }
            if (mi is FieldInfo fi) { return fi.GetValue(instance); }
            throw new InvalidOperationException("Unable to get value from MemberInfo");
        }
    }
}
