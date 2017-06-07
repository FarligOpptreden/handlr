using System.Collections.Generic;

namespace Handlr.Framework.Web.Interfaces
{
    public interface IConfig
    {
        Config.Types.Application Application { get; }
        Config.Types.Accounts Accounts { get; }
        Config.Types.Logging Logging { get; }
        Dictionary<string, string> ConnectionStrings { get; }
        Config.Types.Custom Custom { get; }
    }
}
