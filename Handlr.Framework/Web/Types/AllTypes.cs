using Handlr.Framework.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handlr.Framework.Web.Types
{
    public static class AllTypes
    {
        public static List<T> Get<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static string StringFromContentTypes(params ContentType[] types)
        {
            foreach (ContentType type in types)
            {
                MemberInfo[] mi = type.GetType().GetMember(type.ToString());
                if (mi == null || mi.Length == 0)
                    continue;
                Display d = mi[0].GetCustomAttribute<Display>();
                if (d != null && d.Values != null)
                    return d.Values.FirstOrDefault();
            }
            return "text/html";
        }

        public static ContentType ContentTypeFromStrings(params string[] types)
        {
            if (types == null || types.Length == 0)
                return ContentType.Any;
            foreach (string type in types)
            {
                if (string.IsNullOrEmpty(type))
                    return ContentType.Any;
                foreach (var e in Enum.GetValues(typeof(ContentType)))
                {
                    MemberInfo[] mi = e.GetType().GetMember(e.ToString());
                    if (mi == null || mi.Length == 0)
                        continue;
                    Display d = mi[0].GetCustomAttribute<Display>();
                    if (d != null && d.Values != null && d.Values.Contains(type.ToLower()))
                        return (ContentType)e;
                }
            }
            return ContentType.Other;
        }
    }
}
