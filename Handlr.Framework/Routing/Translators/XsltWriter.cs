using Handlr.Framework;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    public class XsltWriter : XmlParser<RestFieldCache, RestFieldCache>
    {
        private RestFieldCache cache;

        public override RestFieldCache Translate(RestFieldCache input)
        {
            cache = new RestFieldCache();
            cache.AddRange(LoadAndParseTemplate(input));
            return cache;
        }

        public override string ToString()
        {
            return cache.ToJson();
        }
    }
}
