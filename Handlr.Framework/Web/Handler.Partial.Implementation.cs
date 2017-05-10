using System;
using System.Collections.Generic;
using System.Reflection;
using Handlr.Framework.Web.Interfaces;
using Handlr.Framework.Web.Types;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using Handlr.Framework.Web.Attributes;
using System.Linq;
using System.IO;
using System.Web;

namespace Handlr.Framework.Web
{
    public abstract partial class Handler : IHandler
    {
        public void WriteResponse(object content, string contentType = null, int statusCode = (int)Status.OK, Dictionary<string, string> headers = null, bool minify = true)
        {
            if (!Initialized)
                throw new Exception("The Handler hasn't been initialized yet. Call Initialize(HttpContext) before using the Handler.");
            if (PreResponse != null)
                content = PreResponse(content);
            if (content.GetType() == typeof(string) && minify)
                content = Minify(content as string);
            Response.StatusCode = statusCode;
            Response.ContentType = new Func<ContentType, string>(type =>
            {
                if (!string.IsNullOrEmpty(contentType) && string.IsNullOrEmpty(ResponseMimeType))
                    return contentType;
                if (type == ContentType.Other && !string.IsNullOrEmpty(ResponseMimeType))
                    return ResponseMimeType;
                return AllTypes.StringFromContentTypes(ResponseType);
            })(ResponseType);
            if (headers != null)
                foreach (KeyValuePair<string, string> header in headers)
                    Response.AddHeader(header.Key, header.Value);
            if (Configuration.Logging.LogAllResponses && !string.IsNullOrEmpty(Configuration.Logging.Debug))
                Logging.LogAsync(
                    string.Format(
                        "Outgoing response for url \"{0}\" with consuming type \"{1}\", producing type \"{2}\":\nRequest body:{3}\nResponse:\n{4}",
                        Request.RawUrl,
                        Request.ContentType,
                        Request.AcceptTypes.Flatten(";"),
                        InputStream,
                        content.GetType() == typeof(string) ? content : "byte[] of " + (content as byte[]).Length + " length"),
                    Configuration.Logging.Debug);
            if (content.GetType() == typeof(string))
                Response.Write(content as string);
            else
                Response.OutputStream.Write(content as byte[], 0, ((byte[])content).Length);
        }

        public void WriteJsonResponse(object content, bool minify = true)
        {
            object c = null;
            string mimeType = null;
            if (content != null && content is byte[])
            {
                c = content;
                mimeType = null;
            }
            else if (content != null)
            {
                c = content.ToJson();
                mimeType = "application/json";
            }
            WriteResponse(
                c == null ? "Bad Request" : c,
                c == null ? "text/html" : mimeType,
                c == null ? 403 : 200,
                minify: minify
                );
        }

        public void WriteJsonResponse(IDictionary<string, object> content, bool minify = true)
        {
            WriteJsonResponse(content as object, minify);
        }

        private object GetParameterValue(ParameterInfo pInfo, Dictionary<string, string> passedParams)
        {
            var value = passedParams.ContainsKey(pInfo.Name.ToLower()) && !string.IsNullOrEmpty(passedParams[pInfo.Name.ToLower()]) ? passedParams[pInfo.Name.ToLower()] : null;
            if (value != null && pInfo.ParameterType == typeof(string))
                return value.ToString();
            if (value != null && (pInfo.ParameterType == typeof(bool) || pInfo.ParameterType == typeof(bool?)))
                return bool.Parse(value);
            if (value != null && (pInfo.ParameterType == typeof(int) || pInfo.ParameterType == typeof(int?)))
                return int.Parse(value);
            if (value != null && (pInfo.ParameterType == typeof(float) || pInfo.ParameterType == typeof(float?)))
                return float.Parse(value);
            if (value != null && (pInfo.ParameterType == typeof(decimal) || pInfo.ParameterType == typeof(decimal?)))
                return decimal.Parse(value);
            if (value != null && (pInfo.ParameterType == typeof(DateTime) || pInfo.ParameterType == typeof(DateTime?)))
                return DateTime.Parse(value);
            if (HasInputStream())
            {
                try
                {
                    if (pInfo.ParameterType == typeof(XDocument))
                        return XDocument.Parse(LoadInputStream(true));
                    if (pInfo.ParameterType == typeof(XmlDocument))
                    {
                        var xml = new XmlDocument();
                        xml.LoadXml(LoadInputStream(true));
                        return xml;
                    }
                    return LoadInputStream().ToObject(pInfo.ParameterType);
                }
                catch { }
            }
            return pInfo.ParameterType.DefaultValue();
        }

        public object InvokeHandler(MethodInfo controllerMethod)
        {
            ParameterInfo[] paramInfo = controllerMethod.GetParameters();
            List<object> paramValues = new List<object>();
            Dictionary<string, string> passedParams = new Dictionary<string, string>();
            passedParams.AddRange(Parameters).AddRange(Post).AddRange(PathVariables);
            if (PreHandle != null)
                passedParams = PreHandle(passedParams);
            foreach (ParameterInfo pInfo in paramInfo)
                paramValues.Add(GetParameterValue(pInfo, passedParams));
            object response = controllerMethod.Invoke(this, paramValues.ToArray());
            if (PostHandle != null)
                response = PostHandle(response);
            return response;
        }

        public void DerivePathVariables(MethodInfo controllerMethod)
        {
            AcceptUrls urls = controllerMethod.GetCustomAttribute<AcceptUrls>();
            if (urls == null)
                return;
            foreach (string url in urls.Patterns.Where(p => Regex.IsMatch(p, "\\{.+\\}")))
            {
                string[] segments = url.Split('/').Where(u => u != "^" && u != "$" && !string.IsNullOrEmpty(u)).ToArray();
                if (segments.Length == UriSegments.Count)
                {
                    for (int i = 0; i < segments.Length; i++)
                    {
                        if (Regex.IsMatch(segments[i], "\\{.+\\}"))
                            PathVariables.Add(segments[i].Replace("{", "").Replace("}", ""), UriSegments[i]);
                    }
                    return;
                }
            }
        }

        public bool HasInputStream()
        {
            return Request.InputStream.Length > 0;
        }

        public string LoadInputStream(bool decodeStream = false)
        {
            if (Request.InputStream.Length > 0)
            {
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    Request.InputStream.Seek(0, SeekOrigin.Begin);
                    reader.DiscardBufferedData();
                    InputStream = reader.ReadToEnd();
                }
                if (decodeStream && !string.IsNullOrEmpty(InputStream))
                    InputStream = HttpUtility.UrlDecode(InputStream);
            }
            return InputStream;
        }

        public string Minify(string content)
        {
            content = Regex.Replace(content as string, @"^\/\/([a-zA-Z0-9\s\.,=\-_\(\)\\\/<>'""]+)\n", "");
            content = Regex.Replace(content as string, @"\s+\/\/([a-zA-Z0-9\s\.,=\-_\(\)\\\/<>'""]+)\n", "");
            content = Regex.Replace(content as string, @"\n+\/\/([a-zA-Z0-9\s\.,=\-_\(\)\\\/<>'""]+)\n", "");
            content = Regex.Replace(content as string, @"\s+", " ");
            content = Regex.Replace(content as string, @"\s*\n\s*", "\n");
            content = Regex.Replace(content as string, @"\s*\>\s*\<\s*", "><");
            content = Regex.Replace(content as string, @"\/\*(.*?)\*\/", "");
            content = Regex.Replace(content as string, @"<!--(.*?)-->", "");
            var firstEndBracketPosition = (content as string).IndexOf(">");
            if (firstEndBracketPosition >= 0)
            {
                content = (content as string).Remove(firstEndBracketPosition, 1);
                content = (content as string).Insert(firstEndBracketPosition, ">");
            }
            return content;
        }

        public string GenerateNonce()
        {
            return Guid.NewGuid().ToString().ToLower().Replace("-", "");
        }

        [Conditional("DEBUG")]
        public void SetDebug()
        {
            DEBUG = true;
        }
    }
}
