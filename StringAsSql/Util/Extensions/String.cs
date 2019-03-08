using System.Linq;

namespace StringAsSql.Util {
    public static class StringExtensions {
        public static bool ContainsAny(this string s, params string[] testStrings) => testStrings.Any(x => s.Contains(x));
    }
}
