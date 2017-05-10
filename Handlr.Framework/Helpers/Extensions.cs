using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Handlr.Framework
{
    public static class Extensions
    {
        public static string Flatten(this IEnumerable list, string separator)
        {
            string toReturn = "";
            foreach (var item in list)
            {
                if (list is IDictionary)
                    toReturn += ((KeyValuePair<string, string>)item).Key + ":" + ((KeyValuePair<string, string>)item).Value + separator;
                else
                    toReturn += item.ToString() + separator;
            }
            if (string.IsNullOrEmpty(toReturn))
                return null;
            return toReturn.Substring(0, toReturn.Length - separator.Length);
        }

        public static string ReplaceAll(this string s, Dictionary<string, string> toReplace)
        {
            foreach (KeyValuePair<string, string> kvp in toReplace)
            {
                s = s.Replace(kvp.Key, kvp.Value);
            }
            return s;
        }

        public static IEnumerable<string> XPathEvaluateAll(this XNode node, string expression)
        {
            foreach (XObject obj in (IEnumerable)node.XPathEvaluate(expression))
            {
                if (obj is XElement)
                {
                    if (((XElement)obj).HasElements)
                        yield return ((XElement)obj).ToString();
                    yield return ((XElement)obj).Value;
                }
                else if (obj is XAttribute)
                    yield return ((XAttribute)obj).Value;
            }
        }

        public static IDictionary AddRange(this IDictionary dictionary, IDictionary toAdd)
        {
            if (toAdd == null)
                return dictionary;
            foreach (var key in toAdd.Keys)
            {
                dictionary.Add(key, toAdd[key]);
            }
            return dictionary;
        }
    }
}
