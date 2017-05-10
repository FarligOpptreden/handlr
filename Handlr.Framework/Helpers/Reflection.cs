using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Handlr.Framework
{
    public static class Reflection
    {
        public static bool HasProperty(this object obj, string property, bool caseSensitive = false)
        {
            if (obj is IList)
                return false;
            if (obj is ExpandoObject || obj is IDictionary)
                return ((IDictionary<string, object>)obj).ContainsKey(property);
            if (obj is object)
                return (caseSensitive ? obj.GetType().GetProperty(property) : obj.GetType().GetProperty(property, BindingFlags.IgnoreCase)) != null;
            return false;
        }

        public static object GetProperty(this object obj, string property, bool caseSensitive = false)
        {
            if (obj.HasProperty(property, caseSensitive))
            {
                var prop = caseSensitive ? obj.GetType().GetProperty(property) : obj.GetType().GetProperty(property, BindingFlags.IgnoreCase);
                return prop.GetValue(obj, null);
            }
            else
                return null;
        }

        public static byte[] GetBytes(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;
            byte[] b = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, b, 0, b.Length);
            return b;
        }

        public static string GetString(this byte[] b)
        {
            if (b == null || b.Length == 0)
                return null;
            char[] c = new char[b.Length / sizeof(char)];
            System.Buffer.BlockCopy(b, 0, c, 0, b.Length);
            return new string(c);
        }

        public static bool IsPrimitive(this object data)
        {
            return data.GetType().IsPrimitive();
        }

        public static bool IsPrimitive(this Type t)
        {
            return
                t.IsPrimitive ||
                t == typeof(decimal) ||
                t == typeof(string) ||
                t == typeof(DateTime) ||
                t == typeof(TimeSpan);
        }

        public static object DefaultValue(this Type t)
        {
            object defaultValue = null;
            try
            {
                defaultValue = t.IsValueType ? null : Activator.CreateInstance(t);
            }
            catch (MissingMethodException) { }
            return defaultValue;
        }
    }
}
