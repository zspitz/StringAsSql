using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using static System.Linq.Enumerable;
using static System.StringComparer;

namespace StringAsSql.Util {
    public static class DbDataRecordExtensions {
        public static string[] Fieldnames(this DbDataRecord row) => Range(0, row.FieldCount).Select(x => row.GetName(x)).ToArray();

        public static T To<T>(this DbDataRecord row, T typer) {
            List<PropertyInfo> props = null;
            return row.To(typer, ref props);
        }
        public static T To<T>(this DbDataRecord row) {
            List<PropertyInfo> props = null;
            return row.To<T>(ref props);
        }

        public static T To<T>(this DbDataRecord row, T typer, ref List<PropertyInfo> props) => row.To<T>(ref props);

        public static T To<T>(this DbDataRecord row, ref List<PropertyInfo> props) {
            if (typeof(T) == typeof(object)) {
                // TODO allow returning only some of the values
                // a List<PropertyInfo> is the wrong mechanism for this, as PropertyInfo isn't relevant for ExpandoObject properties
                // TODO handle specific dynamic types -- e.g. ExpandoObject, or specific inheritors of DynamicObject
                var dynamicRet = new ExpandoObject();
                IDictionary<string, object> dynamicDict = dynamicRet;
                row.Fieldnames().Select(fieldname => (fieldname, row.Get(typeof(object), fieldname))).AddRangeTo(dynamicDict);
                return (T)(object)dynamicRet;
            }

            if (props == null) {
                var fieldnames = row.Fieldnames();
                props = typeof(T).GetProperties().Where(x => {
                    if (x.Name.NotIn(fieldnames, OrdinalIgnoreCase)) { return false; }
                    if (x.GetIndexParameters().Any()) { return false; }
                    if (!x.CanWrite) { return false; }
                    return true;
                }).ToList();
            }
            T ret;
            if (typeof(T).UnderlyingIfNullable().IsPrimitive || typeof(T) == typeof(string)) {
                ret = (T)row.Get(typeof(T), 0);
            } else if (typeof(T).HasParameterlessConstructor()) {
                //order of properties is unimportant; values can be set after creation in whatever order the properties have come in
                ret = Activator.CreateInstance<T>();
                props.ForEach(prp => prp.SetValue(ret, row.Get(prp.PropertyType, prp.Name)));
            } else {
                //anonymous type -- order of properties is important; property values have to be passed in to Activator.CreateInstance
                var ctors = typeof(T).GetConstructors();
                if (ctors.Length ==0) { throw new InvalidOperationException($"Type {typeof(T)} has no constructors"); }
                if (ctors.Length > 1) { throw new InvalidOperationException($"Type {typeof(T)} has multiple constructors"); }
                var values = ctors.Single().GetParameters().Select(x => row.Get(x.ParameterType, x.Name)).ToArray();
                ret = (T)Activator.CreateInstance(typeof(T), values);
            }
            return ret;
        }

        public static object Get(this DbDataRecord row, Type t, string fieldname) => row.Get(t, row.GetOrdinal(fieldname));

        public static object Get(this DbDataRecord row, Type t, int fieldindex) {
            if (row.IsDBNull(fieldindex)) { return t.IsValueType ? Activator.CreateInstance(t) : null; }
            return row[fieldindex];
        }
    }
}
