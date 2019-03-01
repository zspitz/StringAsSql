using System;
using System.Collections.Generic;
using System.Linq;

namespace StringAsSql.Util {
    public static class IEnumerableTExtensions {
        public static bool None<T>(this IEnumerable<T> src) => !src.Any();

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> src, Action<T> action) {
            foreach (var item in src) {
                action(item);
            }
            return src;
        }
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> src, Action<T, int> action) {
            var current = 0;
            foreach (var item in src) {
                action(item, current);
                current += 1;
            }
            return src;
        }
    }
}
