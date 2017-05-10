using Handlr.Framework.Web.Types;

namespace Handlr.Framework.Web.Interfaces
{
    public interface IController
    {
        User User();
        Config Configuration();
    }
}
