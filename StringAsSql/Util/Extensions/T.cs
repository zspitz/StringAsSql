﻿using System.Collections.Generic;
using System.Linq;

namespace StringAsSql.Util {
    public static class TExtensions {
        public static bool In<T>(this T val, IEnumerable<T> vals) => vals.Contains(val);
        public static bool In<T>(this T val, params T[] vals) => vals.Contains(val);
        public static bool In(this char c, string s) => s.IndexOf(c) > -1;
        public static bool In<T>(this T val, HashSet<T> vals) => vals.Contains(val);

        public static bool NotIn<T>(this T val, IEnumerable<T> vals, IEqualityComparer<T> comparer = null) => !vals.Contains(val, comparer);
        public static bool NotIn<T>(this T val, params T[] vals) => !vals.Contains(val);
        public static bool NotIn(this char c, string s) => s.IndexOf(c) == -1;
        public static bool NotIn<T>(this T val, HashSet<T> vals) => !vals.Contains(val);
    }
}
