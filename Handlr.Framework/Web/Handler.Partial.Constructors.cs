using System;
using System.Linq;
using System.Web;
using Handlr.Framework.Web.Types;
using System.Collections.Generic;

namespace Handlr.Framework.Web
{
    public abstract partial class Handler
    {
        protected Handler() { }

        protected Handler(HttpContext context, Config configuration)
        {
            Init(context, configuration);
        }

        /// <summary>
        /// Initializes the controller using the supplied HTTP context and configuration.
        /// </summary>
        /// <param name="context">The HTTP context under which the controller is being executed</param>
        /// <param name="configuration">The configuration of the web application derived from the /handlr.config file</param>
        public void Init(HttpContext context, Config configuration)
        {
            PreInit?.Invoke();
            Context = context;
            Request = context.Request;
            Response = context.Response;
            Server = context.Server;
            Uri = context.Request.Url;
            UriSegments = context.Request.AppRelativeCurrentExecutionFilePath.Split('/').Where(s => s != "~").Select(s => s.ToLower()).ToList();
            UriSegments.RemoveAll(uri => string.IsNullOrEmpty(uri));
            if (UriSegments.Count == 0)
                UriSegments.Add("home");
            ResponseType = AllTypes.ContentTypeFromStrings(Request.AcceptTypes);
            RequestType = AllTypes.ContentTypeFromStrings(Request.ContentType);
            RequestMethod = (Method)Enum.Parse(typeof(Method), Request.HttpMethod.ToLower().Substring(0, 1).ToUpper() + Request.HttpMethod.ToLower().Substring(1));
            Parameters = (from key in Request.QueryString.AllKeys select new { Key = key.ToLower(), Value = Request.QueryString[key] }).ToDictionary(o => o.Key, o => o.Value);
            Post = (from key in Request.Form.AllKeys select new { Key = key.ToLower(), Value = Request.Form[key] }).ToDictionary(o => o.Key, o => o.Value);
            PathVariables = new Dictionary<string, string>();
            ErrorLogPath = configuration.Logging.Error;
            SecuriUrl = configuration.Accounts != null ? configuration.Accounts.Url : null;
            PreConfig?.Invoke(configuration);
            Configuration = configuration;
            PostConfig?.Invoke(configuration);
            ViewModel = new ViewModel.Container(configuration, Request.ApplicationPath, context, this);
            Initialized = true;
            Guid appId = Guid.Empty;
            if (Guid.TryParse(configuration.Application.Id, out appId))
                AppId = appId;
            PostInit?.Invoke();
        }
    }
}