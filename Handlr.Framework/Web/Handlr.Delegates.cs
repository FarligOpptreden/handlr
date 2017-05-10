using System.Collections.Generic;

namespace Handlr.Framework.Web
{
    public delegate void InitDelegate();
    public delegate void ConfigDelegate(dynamic configuration);
    public delegate Dictionary<string, string> ParameterDelegate(Dictionary<string, string> parameters);
    public delegate object ResponseDelegate(object response);
}
