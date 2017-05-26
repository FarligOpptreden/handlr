using System;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Types;

namespace Handlr.Framework.Routing.Translators
{
    public class XmlWriter : XmlParser
    {
        private GenericFieldCache cache;

        public override IFieldCache Translate(IFieldCache input)
        {
            cache = new GenericFieldCache();
            var doc = LoadAndParseTemplate(input);
            return cache;
        }

        public override string ToString()
        {
            return cache.ToJson();
        }
    }
}
