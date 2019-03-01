using System.Collections.Generic;
using System.Linq;

namespace StringAsSql.Util {
    public static class TExtensions {
        public static bool NotIn<T>(this T val, IEnumerable<T> vals, IEqualityComparer<T> comparer = null) => 
            !vals.Contains(val, comparer);
        public static bool NotIn<T>(this T val, params T[] vals) =>
            !vals.Contains(val);
    }
}
