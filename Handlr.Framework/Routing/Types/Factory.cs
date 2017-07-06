using Handlr.Framework.Routing.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Handlr.Framework.Routing.Types
{
    public static class Factory
    {
        public static IFieldCache Build(XElement configuration, params object[] input)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            string tagType = configuration.Attribute("type") != null ? configuration.Attribute("type").Value : null;
            var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where typeof(IFieldCache).IsAssignableFrom(type)
                        where type.FullName.ToLower() == tagType.ToLower()
                        select type;

            if (types.FirstOrDefault() == null)
                throw new ArgumentException("An instance of \"" + tagType + "\" could not be instantiated.", "configuration");

            var cacheType = types.FirstOrDefault();
            var cacheInstance = Activator.CreateInstance(cacheType, input);
            return cacheInstance as IFieldCache;
        }
    }
}
