using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StringAsSql.Util {
    public static class ObjectExtensions {
        public static List<(string key, object value)> MemberNamesValues(this object o) {
            var ret = new List<(string, object)>();
            var t = o.GetType();
            t.GetProperties().Where(x => x.GetIndexParameters().None()).Select(p => (p.Name, p.GetValue(o))).AddRangeTo(ret);
            t.GetFields().Select(f => (f.Name, f.GetValue(o))).AddRangeTo(ret);
            return ret;
        }
    }
}
