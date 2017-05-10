using System;
using System.Collections.Generic;
using System.Web;
using Handlr.Framework.Web.Types;
using Handlr.Framework.Web.Interfaces;

namespace Handlr.Framework.Web
{
    public abstract partial class Handler
    {
        /// <summary>
        /// A flag indicating whether the controller is running in debug mode or not.
        /// </summary>
        public static bool DEBUG = false;

        /// <summary>
        /// Gets the configuration associated with the controller. The configuration is typically loaded from the /handlr.config file.
        /// </summary>
        public Config Configuration { get; private set; }

        /// <summary>
        /// Gets the HTTP context of the request.
        /// </summary>
        public HttpContext Context { get; private set; }

        /// <summary>
        /// Gets the request object associated with the HTTP context.
        /// </summary>
        public HttpRequest Request { get; private set; }

        /// <summary>
        /// Gets the response object associated with the HTTP context.
        /// </summary>
        public HttpResponse Response { get; private set; }

        /// <summary>
        /// Gets the server utility object associated with the HTTP context.
        /// </summary>
        public HttpServerUtility Server { get; private set; }

        /// <summary>
        /// Gets the view model associated with the controller.
        /// </summary>
        public ViewModel.Container ViewModel { get; private set; }

        /// <summary>
        /// Gets the nonce provider of the controller.
        /// </summary>
        public INonceProvider NonceProvider { get; private set; }

        /// <summary>
        /// Gets the URI that was used to access the controller.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the formatted segments of the URI, i.e. /home/resource/path would be 3 segments represented by [ "home", "resource", "path" ].
        /// </summary>
        public List<string> UriSegments { get; private set; }

        /// <summary>
        /// Gets a dictionary of all query string parameters sent to the controller.
        /// </summary>
        public Dictionary<string, string> Parameters { get; private set; }

        /// <summary>
        /// Gets a dictionary of all post variables sent to the controller.
        /// </summary>
        public Dictionary<string, string> Post { get; private set; }

        /// <summary>
        /// Gets a dictionary of all path variables derived from the URI, i.e. sending /home/root/123 to the configured path 
        /// /home/{resouce}/{path} would map {resource} to "root" and {path} to "123".
        /// </summary>
        public Dictionary<string, string> PathVariables { get; private set; }

        /// <summary>
        /// Gets the content type sent by the consumer.
        /// </summary>
        public ContentType RequestType { get; private set; }

        /// <summary>
        /// Gets the content type expected back by the consumer.
        /// </summary>
        public ContentType ResponseType { get; private set; }

        /// <summary>
        /// Gets the access verb use to invoke the controller.
        /// </summary>
        public Method RequestMethod { get; private set; }

        /// <summary>
        /// Gets a serialized representation of the input stream.
        /// </summary>
        public string InputStream { get; set; }

        /// <summary>
        /// Gets a list of all files posted to the resource.
        /// </summary>
        public List<HttpPostedFile> Files { get; set; }

        /// <summary>
        /// Gets the unique application ID associated with the controller. This is derived from the Configuration.
        /// </summary>
        public Guid? AppId { get; private set; }

        /// <summary>
        /// Gets the relative path to the web application that is part of the controller.
        /// </summary>
        public string AppPath { get; private set; }

        /// <summary>
        /// Gets the accounts (sign-in) URL of the web application. This is derived from the Configuration.
        /// </summary>
        protected string SecuriUrl { get; private set; }

        /// <summary>
        /// Gets the path where error files are logged to.
        /// </summary>
        protected string ErrorLogPath { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the controller has been initialized yet. It will only be set to "true" once .Initialize() is called on the controller.
        /// </summary>
        protected bool Initialized { get; private set; }

        /// <summary>
        /// Gets or sets the mime-type for the response.
        /// </summary>
        private string ResponseMimeType { get; set; }

        /// <summary>
        /// Executes before the controller is initialized.
        /// </summary>
        public event InitDelegate PreInit;

        /// <summary>
        /// Executes after the controller has been initialized.
        /// </summary>
        public event InitDelegate PostInit;

        /// <summary>
        /// Executes before the configuration is set on the controller.
        /// </summary>
        public event ConfigDelegate PreConfig;

        /// <summary>
        /// Executes after the configuration has been set on the controller.
        /// </summary>
        public event ConfigDelegate PostConfig;

        /// <summary>
        /// Executes before the controller method mapped to the request is handled.
        /// </summary>
        public event ParameterDelegate PreHandle;

        /// <summary>
        /// Executes after the controller method mapped to the request has been handled.
        /// </summary>
        public event ResponseDelegate PostHandle;

        /// <summary>
        /// Executes before the final response is sent to the consumer.
        /// </summary>
        public event ResponseDelegate PreResponse;
    }
}
