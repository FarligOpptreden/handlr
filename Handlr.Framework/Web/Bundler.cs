using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Handlr.Framework.Web.Attributes;
using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Web
{
    public class Bundler : Handler
    {
        [AcceptUrls("/handlr/bundle/css/[a-zA-Z\\-]+")]
        [AcceptVerbs(Method.Get)]
        [CacheOutput]
        string Css()
        {
            SetContentType("text/css");
            string key = UriSegments[3];
            return Context.Cache["css/" + key] as string;
        }

        [AcceptUrls("/handlr/bundle/js/[a-zA-Z\\-]+")]
        [AcceptVerbs(Method.Get)]
        [CacheOutput]
        string Js()
        {
            SetContentType("text/javascript");
            string key = UriSegments[3];
            return Context.Cache["js/" + key] as string;
        }

        public static string Bundle(List<string> files, Type type, string key)
        {
            HttpContext ctx = HttpContext.Current;
            Func<List<string>, List<string>, bool> dependenciesChanged = (args, cache) =>
                {
                    if (cache == null)
                        return true;
                    foreach (string s in args)
                    {
                        bool found = cache.Where(c => c.ToLower().Contains(s.ToLower())).FirstOrDefault() != null;
                        if (!found)
                            return true;
                    }
                    return false;
                };
            string fullKey = type.ToString().ToLower() + "/" + key;
            if (DEBUG || ctx.Cache[fullKey] == null || ctx.Cache[fullKey + "-dependencies"]  == null || dependenciesChanged(files, (List<string>)ctx.Cache[key + "-dependencies"]))
            {
                ctx.Cache.Remove(fullKey);
                ctx.Cache.Remove(fullKey + "-dependencies");
                StringBuilder sb = new StringBuilder();
                List<string> dependencies = new List<string>();
                foreach (string file in files)
                {
                    string path = ctx.Server.MapPath(file);
                    if (IO.IsFile(path))
                    {
                        sb.Append(IO.ReadTextFile(path) + "\n\n");
                        dependencies.Add(path);
                    }
                }
                ctx.Cache.Add(
                    fullKey,
                    sb.ToString(),
                    new System.Web.Caching.CacheDependency(dependencies.ToArray()),
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.Default,
                    null);
                ctx.Cache.Add(
                    fullKey + "-dependencies",
                    dependencies,
                    null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.Default,
                    null);
            }
            return (ctx.Request.ApplicationPath == "/" ? "" : ctx.Request.ApplicationPath) + "/handlr/bundle/" + type.ToString().ToLower() + "/" + key;
        }

        public enum Type
        {
            Css,
            Js
        }
    }
}
