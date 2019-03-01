using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StringAsSql.Util {
    public static class ObjectExtensions {
        private static bool getKeyValue(object o, Type matchAgainst, (string keyName, string valName) names, out (string key, object val) keyValue) {
            var type = o.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == matchAgainst && type.GetGenericArguments().First() == typeof(string)) {
                keyValue = (
                    type.GetMember(names.keyName).Single().GetValue(o) as string,
                    type.GetMember(names.valName).Single().GetValue(o)
                );
                return true;
            };
            keyValue = ("", null);
            return false;
        }

        public static List<(string key, object value)> ParseObject(this object obj) {
            if (obj is IEnumerable enumerable && !(obj is string)) {
                return enumerable.Cast<object>().Select((x, index) =>
                    getKeyValue(x, typeof(KeyValuePair<,>), ("Key", "Value"), out var kv) ? kv :
                    getKeyValue(x, typeof(ValueTuple<,>), ("Item1", "Item2"), out var kv1) ? kv1 :
                    ($"Parameter{index}", x)
                ).ToList();
            }

            var props = obj.GetType().GetProperties().Where(x => x.GetIndexParameters().None()).ToList();
            return props.Select(prp => (prp.Name, prp.GetValue(obj))).ToList();
        }
    }
}
