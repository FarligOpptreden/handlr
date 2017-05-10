using System.Collections.Generic;

namespace Handlr.Framework.UI.Types
{
    public class MenuItem
    {
        public bool? Persist { get; set; }
        public string Key { get; set; }
        public string Display { get; set; }
        public string Class { get; set; }
        public string Icon { get; set; }
        public object Arguments { get; set; }
        public List<MenuItem> Children { get; set; }
    }
}
