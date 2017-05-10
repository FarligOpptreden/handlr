using Handlr.Framework.Web.Types;
using System.Collections.Generic;

namespace Handlr.Framework.UI.Types
{
    public class Application
    {
        public User User { get; set; }
        public List<MenuItem> ActionItems { get; set; }
        public List<MenuItem> MenuItems { get; set; }
    }
}
