using System;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class CacheOutput : Attribute
    {
        public CacheOutput(bool cache = true, int maxDaysDuration = 10)
        {
            DoCache = cache;
            MaxDaysDuration = maxDaysDuration;
        }

        public bool DoCache { get; private set; }
        public int MaxDaysDuration { get; private set; }
    }
}
