using Handlr.Framework.Web.Interfaces;
using System.Collections.Generic;

namespace Handlr.Framework.UI.Types
{
    public class Application
    {
        public IUser User { get; set; }
        public List<MenuItem> ActionItems { get; set; }
        public List<MenuItem> MenuItems { get; set; }
    }
}
