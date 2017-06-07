using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Handlr.Framework.Web
{
    public class Config : Interfaces.IConfig
    {
        public Config(XDocument config)
        {
            var applicationNode = config.Root.Elements().Where(e => e.Name.LocalName.ToLower() == "application").FirstOrDefault();
            var accountsNode = config.Root.Elements().Where(e => e.Name.LocalName.ToLower() == "accounts").FirstOrDefault();
            var logNode = config.Root.Elements().Where(e => e.Name.LocalName.ToLower() == "log").FirstOrDefault();
            var connectionStringNode = config.Root.Elements().Where(e => e.Name.LocalName.ToLower() == "datasources").FirstOrDefault();
            var customNode = config.Root.Elements().Where(e => e.Name.LocalName.ToLower() == "custom").FirstOrDefault();
            if (applicationNode != null)
                Application = new Types.Application()
                {
                    Id = applicationNode.Attributes("id").Count() > 0 ? applicationNode.Attributes("id").FirstOrDefault().Value : null,
                    Name = applicationNode.Attributes("name").Count() > 0 ? applicationNode.Attributes("name").FirstOrDefault().Value : null
                };
            if (accountsNode != null)
                Accounts = new Types.Accounts()
                {
                    Url = accountsNode.Attributes("url").Count() > 0 ? accountsNode.Attributes("url").FirstOrDefault().Value : null,
                    PassPhrase = accountsNode.Attributes("passPhrase").Count() > 0 ? accountsNode.Attributes("passPhrase").FirstOrDefault().Value : null
                };
            if (logNode != null)
                Logging = new Types.Logging()
                {
                    Error = logNode.Attributes("error").Count() > 0 ? logNode.Attributes("error").FirstOrDefault().Value : null,
                    Info = logNode.Attributes("info").Count() > 0 ? logNode.Attributes("info").FirstOrDefault().Value : null,
                    Debug = logNode.Attributes("debug").Count() > 0 ? logNode.Attributes("debug").FirstOrDefault().Value : null,
                    LogAllRequests = logNode.Attributes("logAllRequests").Count() > 0 ? bool.Parse(logNode.Attributes("logAllRequests").FirstOrDefault().Value) : false,
                    LogAllResponses = logNode.Attributes("logAllResponses").Count() > 0 ? bool.Parse(logNode.Attributes("logAllResponses").FirstOrDefault().Value) : false,
                    MaxFileSize = logNode.Attributes("maxFileSize").Count() > 0 ? int.Parse(logNode.Attributes("maxFileSize").FirstOrDefault().Value) : (long?)null
                };
            if (connectionStringNode != null)
            {
                ConnectionStrings = new Dictionary<string, string>();
                foreach (var connectionString in connectionStringNode.Elements().Where(e => e.Name.LocalName.ToLower() == "source"))
                {
                    ConnectionStrings.Add(connectionString.Attributes("name").FirstOrDefault().Value, connectionString.Attributes("connectionString").FirstOrDefault().Value);
                }
            }
            if (customNode != null)
                Custom = new Types.Custom(customNode);
        }

        public Types.Application Application { get; private set; }
        public Types.Accounts Accounts { get; private set; }
        public Types.Logging Logging { get; private set; }
        public Dictionary<string, string> ConnectionStrings { get; private set; }
        public Types.Custom Custom { get; private set; }

        public class Types
        {
            public class Application
            {
                public string Id { get; internal set; }
                public string Name { get; internal set; }
            }

            public class Accounts
            {
                public string Url { get; internal set; }
                public string PassPhrase { get; internal set; }
            }

            public class Logging
            {
                public bool LogAllRequests { get; internal set; }
                public bool LogAllResponses { get; internal set; }
                public string Debug { get; internal set; }
                public string Error { get; internal set; }
                public string Info { get; internal set; }
                public long? MaxFileSize { get; internal set; }
            }

            public class Custom
            {
                private XElement _CustomXml;

                public Custom(XElement customXml)
                {
                    _CustomXml = customXml;
                }

                public string this[string path]
                {
                    get
                    {
                        var result = _CustomXml.XPathEvaluateAll(path);
                        return result != null ? result.FirstOrDefault() : null;
                    }
                }
            }
        }
    }
}
