using System;
using System.Collections.Generic;

namespace StringAsSql.Util {
    public static class IEnumerableTuple2Extensions {
        public static IEnumerable<(T1, T2)> ForEachT<T1, T2>(this IEnumerable<(T1, T2)> src, Action<T1, T2> action) =>
            src.ForEach(tuple => action(tuple.Item1, tuple.Item2));
    }
}
