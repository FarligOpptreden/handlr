using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Collections;
using System.Dynamic;
using System.Reflection;
using System.ComponentModel;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Handlr.Framework
{
    public static class Converters 
    { 
        public static Dictionary<string, object> ToDictionary(this string data)
        {
            JToken result = JsonConvert.DeserializeObject<dynamic>(data, new JsonSerializerSettings()
                {
                    Culture = System.Globalization.CultureInfo.CurrentCulture,
                    NullValueHandling = NullValueHandling.Include,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                }) as JToken;
            Func<JToken, IEnumerable> dictionary = null;
            dictionary = new Func<JToken, IEnumerable>(obj =>
            {
                if (!typeof(JContainer).IsAssignableFrom(obj.GetType()))
                    return null;
                if (obj.GetType() == typeof(JArray))
                {
                    List<object> list = new List<object>();
                    foreach (JToken child in ((JArray)obj))
                    {
                        if (child.GetType() == typeof(JObject))
                            list.Add(dictionary(child));
                        if (child.GetType() == typeof(JValue))
                            list.Add((child as JValue).Value);
                    }
                    return list;
                }
                if (obj.GetType() == typeof(JObject))
                {
                    Dictionary<string, object> toReturn = new Dictionary<string, object>();
                    foreach (JProperty prop in ((JObject)obj).Properties())
                    {
                        JToken value = prop.Value;
                        if (value.Type == JTokenType.Array ||
                            value.Type == JTokenType.Object)
                            toReturn.Add(prop.Name, dictionary(value));
                        else
                            toReturn.Add(prop.Name, ((JValue)value).Value);
                    }
                    return toReturn;
                }
                return null;
            });
            var dic = dictionary(result);
            if (dic is IDictionary)
                dic = dic as Dictionary<string, object>;
            else
                dic = new Dictionary<string, object>()
                {
                    {"Data", dic }
                };
            return dic as Dictionary<string, object>;
        }

        public static Dictionary<string, object> ToDictionary(this DataTable data)
        {
            Dictionary<string, Type> definition = new Dictionary<string, Type>();
            foreach (DataColumn c in data.Columns)
            {
                definition.Add(c.ColumnName, c.DataType);
            }
            List<Dictionary<string, object>> serialized = new List<Dictionary<string, object>>();
            foreach (DataRowView row in new DataView(data))
            {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                foreach (KeyValuePair<string, Type> kvp in definition)
                {
                    obj.Add(kvp.Key, row[kvp.Key]);
                }
                serialized.Add(obj);
            }
            return new Dictionary<string, object>() { { "Data", serialized } };
        }

        public static Dictionary<string, object> ToDictionary(this DataRow data)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (DataColumn c in data.Table.Columns)
            {
                dic.Add(c.ColumnName, data[c]);
            }
            return dic;
        }

        public static Dictionary<string, object> ToDictionary(this XDocument data)
        {
            Func<XElement, Dictionary<string, object>, object> build = null;
            build = new Func<XElement, Dictionary<string, object>, object>((element, parent) =>
                {
                    foreach (XAttribute attribute in element.Attributes())
                    {
                        parent.Add(attribute.Name.LocalName, attribute.Value);
                    }
                    var results = from XElement node in element.Elements()
                                  group node by node.Name.LocalName into nodes
                                  select new
                                  {
                                      name = nodes.Key,
                                      count = nodes.Count(),
                                      nodes = nodes
                                  };
                    if (results.Count() == 0)
                    {
                        parent.Add("Value", element.Value);
                        return 1;
                    }
                    foreach (var node in results.Where(n => n.count == 1).Select(n => n.nodes.First()))
                    {
                        if (!node.HasElements && !node.HasAttributes)
                        {
                            parent.Add(node.Name.LocalName, node.Value);
                            continue;
                        }
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        build(node, dic);
                        parent.Add(node.Name.LocalName, dic);
                    }
                    foreach (var node in results.Where(n => n.count > 1))
                    {
                        List<object> children = new List<object>();
                        foreach (var child in node.nodes)
                        {
                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            build(child, dic);
                            children.Add(dic);
                        }
                        parent.Add(element.Name.LocalName, children);
                    }
                    return 1;
                });
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            build(data.Root, dictionary);
            return dictionary;
        }

        public static Dictionary<string, object> ToDictionary(this ExpandoObject data)
        {
            var dic = data as IDictionary<string, object>;
            Dictionary<string, object> toReturn = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> kvp in dic)
            {
                if (kvp.Value is ExpandoObject)
                    toReturn.Add(kvp.Key, ((ExpandoObject)kvp.Value).ToDictionary());
                else
                    toReturn.Add(kvp.Key, kvp.Value);
            }
            return toReturn;
        }

        public static Dictionary<string, object> ToDictionary(this object data)
        {
            string result = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
                {
                    Culture = System.Globalization.CultureInfo.CurrentCulture,
                    NullValueHandling = NullValueHandling.Include,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                });
            return result.ToDictionary();
        }

        public static IDictionary<string, object> ToObjectDictionary(this object obj)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor property in properties)
            {
                result.Add(property.Name, property.GetValue(obj));
            }
            return result;
        }

        public static dynamic ToDynamic(this string data)
        {
            return data.ToDictionary().ToDynamic();
        }

        public static dynamic ToDynamic(this DataTable data)
        {
            return data.ToDictionary().ToDynamic();
        }

        public static dynamic ToDynamic(this DataRow data)
        {
            return data.ToDictionary().ToDynamic();
        }

        public static dynamic ToDynamic(this XDocument data)
        {
            return data.ToDictionary().ToDynamic();
        }

        public static dynamic ToDynamic(this IDictionary<string, object> data)
        {
            var expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;
            foreach (KeyValuePair<string, object> kvp in data)
            {
                if (kvp.Value is IDictionary<string, object>)
                    dictionary.Add(kvp.Key, ((IDictionary<string, object>)kvp.Value).ToDynamic());
                else if (kvp.Value is ICollection)
                {
                    var list = new List<object>();
                    foreach (var item in (ICollection)kvp.Value)
                    {
                        if (item is IDictionary<string, object>)
                            list.Add(((IDictionary<string, object>)item).ToDynamic());
                        else
                            list.Add(item);
                    }
                    dictionary.Add(kvp.Key, list);
                }
                else if (kvp.Value is DBNull)
                    dictionary.Add(new KeyValuePair<string, object>(kvp.Key, null));
                else
                    dictionary.Add(kvp);
            }
            return (dynamic)expando;
        }

        public static XDocument ToXml(this string data, string rootNode = null)
        {
            return XDocument.Parse(data);
        }

        public static XDocument ToXml(this DataTable data, string rootNode = null)
        {
            return data.ToDictionary().ToXml(rootNode);
        }

        public static XDocument ToXml(this DataRow data, string rootNode = null)
        {
            return data.ToDictionary().ToXml(rootNode);
        }

        public static XDocument ToXml(this ExpandoObject data, string rootNode = null)
        {
            return (data as IDictionary<string, object>).ToXml(rootNode);
        }

        public static XDocument ToXml(this IDictionary<string, object> data, string rootNode = null)
        {
            XDocument doc = new XDocument();
            Func<IDictionary<string, object>, List<XElement>> build = null;
            build = new Func<IDictionary<string, object>, List<XElement>>(dic =>
                 {
                     List<XElement> elements = new List<XElement>();
                     foreach (KeyValuePair<string, object> kvp in dic)
                     {
                         XElement element = new XElement(kvp.Key);
                         if (kvp.Value is IDictionary<string, object>)
                             element.Add(build((IDictionary<string, object>)kvp.Value));
                         else if (kvp.Value is ICollection)
                         {
                             foreach (var item in (ICollection)kvp.Value)
                             {
                                 if (item is IDictionary<string, object>)
                                     element.Add(new XElement("Item", build((IDictionary<string, object>)item)));
                                 else
                                     element.Add(new XElement("Item", item));
                             }
                         }
                         else
                             element.SetValue(kvp.Value != null ? kvp.Value : "");
                         elements.Add(element);
                     }
                     return elements;
                 });
            XElement root = new XElement(!string.IsNullOrEmpty(rootNode) ? rootNode : "Data", build(data));
            doc.AddFirst(root);
            return doc;
        }

        public static string ToJson(this DataTable data)
        {
            return data.ToDictionary().ToJson();
        }

        public static string ToJson(this DataRow data)
        {
            return data.ToDictionary().ToJson();
        }

        public static string ToJson(this XDocument data)
        {
            return data.ToDictionary().ToJson();
        }

        public static string ToJson(this ExpandoObject data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
            {
                Culture = System.Globalization.CultureInfo.CurrentCulture,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Include
            });
        }

        public static string ToJson(this IDictionary<string, object> data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
            {
                Culture = System.Globalization.CultureInfo.CurrentCulture,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Include
            });
        }

        public static string ToJson(this IList<object> data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
            {
                Culture = System.Globalization.CultureInfo.CurrentCulture,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Include
            });
        }

        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
            {
                Culture = System.Globalization.CultureInfo.CurrentCulture,
                NullValueHandling = NullValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            });
        }

        public static T ToObject<T>(this string data)
        {
            return (T)data.ToObject(typeof(T));
        }

        public static object ToObject(this string data, Type type)
        {
            if (type.IsPrimitive())
                return Convert.ChangeType(data, type);
            if (type == typeof(XDocument))
                return data.ToXml();
            return JsonConvert.DeserializeObject(data, type, new JsonSerializerSettings()
            {
                Culture = System.Globalization.CultureInfo.CurrentCulture,
                NullValueHandling = NullValueHandling.Include,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            });
        }

        public static T To<T>(this object obj)
            where T : new()
        {
            T n = new T();
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (obj.HasProperty(prop.Name))
                {
                    var objProp = obj.GetType().GetProperty(prop.Name);
                    prop.SetValue(n, objProp.GetValue(obj, null), null);
                }
            }
            return n;
        }
    }
}
