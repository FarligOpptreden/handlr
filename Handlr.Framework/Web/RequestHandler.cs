using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;
using Handlr.Framework.Web.Attributes;
using Handlr.Framework.Web.Types;
using System.Threading;

namespace Handlr.Framework.Web
{
    /// <summary>
    /// Processes the incoming request.
    /// </summary>
    public sealed class RequestHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            // Only process the request if the physical file doesn't exist on disk
            if (!File.Exists(context.Server.MapPath(context.Request.Url.AbsolutePath)))
            {
                // Load the config before handling the request
                string configPath = context.Server.MapPath(context.Request.ApplicationPath + "/handlr.config");
                if (!File.Exists(configPath))
                {
                    Error.Factory(context, null, null, Status.NotImplemented, string.Format("Handlr.config not found at {0}", configPath)).Handle();
                    return;
                }
                // Instantiate the base request handler and invoke it's Handle() method
                WebHandler handler = new WebHandler(context, new Config(XDocument.Load(configPath)));
                handler.Handle();
            }
        }

        /// <summary>
        /// Handles the incoming request by interpreting the executing assembly for resource identifiers mapped against AcceptUrls attributes.
        /// </summary>
        private class WebHandler : Handler
        {
            public WebHandler(HttpContext context, Config configuration)
                : base(context, configuration)
            {
                // If logging is enabled for all requests, perform async logging of the request with all parameters and the input stream
                if (configuration.Logging.LogAllRequests && !string.IsNullOrEmpty(configuration.Logging.Debug))
                    Logging.LogAsync(
                        string.Format(
                            "Incoming request for url \"{0}\" with consuming type \"{1}\", producing type \"{2}\" and input stream length of {3}.",
                            Request.RawUrl,
                            Request.ContentType,
                            Request.AcceptTypes.Flatten(";"),
                            Request.InputStream.Length),
                            configuration.Logging.Debug);
            }

            /// <summary>
            /// Handle the request.
            /// </summary>
            public void Handle()
            {
                try
                {
                    // Check if the handler was initialized before performing the handling logic.
                    if (!Initialized)
                        throw new Exception("The Handler hasn't been initialized yet. Call Initialize(HttpContext) before using the Handler.");

                    string method = UriSegments[UriSegments.Count - 1].ToLower();
                    object response = null;
                    Handler context = this;
                    // Find the controller class associated with the request based on the resource path (URL).
                    var controllerClass = FindControllerClass();

                    if (controllerClass == null || controllerClass.Controllers == null || controllerClass.Controllers.Count == 0)
                        throw new WebException(Status.NotFound, "Resource not found");

                    // Find the controller method in the controller class that matches the verb being used (GET, POST, PUT or DELETE).
                    List<MethodInfo> controllerMethods = controllerClass.VerbMatches(RequestMethod);
                    List<Method> acceptedVerbs = controllerClass.AcceptedVerbs();

                    if (controllerMethods == null || controllerMethods.Count == 0 || controllerMethods.FirstOrDefault() == null)
                        throw new WebException(Status.MethodNotAllowed, "Method not allowed.\n\nOnly the following method" + (acceptedVerbs.Count == 1 ? " is" : "s are") + " allowed: " + acceptedVerbs.Flatten(", "));

                    // Find the controller method in the controller class that matches the consuming (contentType header) and producing (accept header) data types.
                    controllerMethods = controllerClass.TypeMatches(controllerMethods, RequestType, ResponseType);
                    if (controllerMethods == null || controllerMethods.Count == 0)
                    {
                        List<ContentType> consumes = controllerClass.ConsumedTypes(RequestMethod);
                        List<ContentType> produces = controllerClass.ProducedTypes(RequestMethod);
                        string consumedTypes = "The following type" + (consumes.Count == 1 ? " is" : "s are") + " consumed: " + consumes.Flatten(", ");
                        string producedTypes = "The following type" + (produces.Count == 1 ? " is" : "s are") + " produced: " + produces.Flatten(", ");

                        throw new WebException(Status.BadRequest, "Consumed or produced content type not allowed.\n\n" + consumedTypes + "\n\n" + producedTypes);
                    }

                    // Find the class' associated nonce provider. If no nonce provider is specified, the default provider will be used.
                    NonceProvider nonceProvider = controllerClass.NonceProvider;
                    // Get the controller method to execute from the enumerable list
                    MethodInfo mInfo = controllerMethods.First();
                    // Create an instance of the controller class using the request context, none provider and configuration
                    context = Factory(controllerClass.ClassType, Context, nonceProvider, Configuration);
                    context.SetDebug();
                    // If the verb is HEAD, only respond that the resource exists without invoking it's body
                    if (RequestMethod == Method.Head)
                    {
                        string contentType = mInfo.GetCustomAttribute<Produces>() == null ?
                            mInfo.ReturnType == typeof(string) ? "text/html" : "application/json" :
                            AllTypes.StringFromContentTypes(mInfo.GetCustomAttribute<Produces>().Type);
                        WriteResponse(
                            content: "",
                            contentType: contentType,
                            statusCode: (int)Status.OK,
                            headers: mInfo.GetCustomAttribute<Consumes>() == null ? null : new Dictionary<string, string>()
                            {
                                { "Accept", AllTypes.StringFromContentTypes(mInfo.GetCustomAttribute<Consumes>().Type) }
                            });
                        return;
                    }
                    // If the verb is OPTIONS, respond with all possible information for the controller, including consuming and producing types, verbs, etc.
                    if (RequestMethod == Method.Options)
                    {
                        AcceptVerbs optionsV = Attribute.GetCustomAttribute(mInfo, typeof(AcceptVerbs)) as AcceptVerbs;
                        Secure optionsS = Attribute.GetCustomAttribute(mInfo, typeof(Secure)) as Secure;
                        AcceptUrls optionsP = Attribute.GetCustomAttribute(mInfo, typeof(AcceptUrls)) as AcceptUrls;
                        Produces optionsProd = Attribute.GetCustomAttribute(mInfo, typeof(Produces)) as Produces;
                        Consumes optionsCons = Attribute.GetCustomAttribute(mInfo, typeof(Consumes)) as Consumes;
                        string verbs = "";
                        if (optionsV != null && optionsV.Verbs != null && optionsV.Verbs.Length > 0)
                            verbs = optionsV.Verbs.Flatten(",");
                        else
                            verbs = "None";
                        dynamic rest = new ExpandoObject();
                        rest.Name = mInfo.Name;
                        rest.Secure = optionsS != null && optionsS.IsSecure;
                        if (optionsP != null)
                            rest.UrlPatterns = optionsP.Patterns;
                        if (optionsCons != null)
                            rest.Consumes = optionsCons.ToString();
                        if (optionsProd != null)
                            rest.Produces = optionsProd.ToString();
                        rest.Parameters = (from parameter in mInfo.GetParameters() select parameter.Name).ToArray();
                        WriteResponse(
                            content: rest.ToDictionary(),
                            contentType: "application/json",
                            statusCode: (int)Status.OK,
                            headers: new Dictionary<string, string>() {
                            { "Allow", verbs.ToUpper() }
                            });
                        return;
                    }
                    // Get the secure attribute of the controller to know whether security checks should be performed or not.
                    Secure s = Attribute.GetCustomAttribute(mInfo, typeof(Secure)) as Secure;
                    bool isAuthenticated = true;
                    // If the query string contains the parameters "userid" and "accesscode", it is assumed that the login process 
                    // has redirected back to the original resource (URL) and the sign-in process should commence.
                    if (Parameters.ContainsKey("userid") && Parameters.ContainsKey("accesscode") && Configuration.Accounts != null && !string.IsNullOrEmpty(Configuration.Accounts.Url))
                    {
                        if (context.PerformSignIn(Parameters["userid"], Parameters["accesscode"]))
                        {
                            string queryString = Parameters.Where(kvp => !(kvp.Key.Contains("userid") || kvp.Key.Contains("accesscode"))).Select(kvp => kvp.Key + "=" + kvp.Value).ToList().Flatten("&");
                            // Redirect to the original resource (URL) and break out of the controller.
                            Response.Redirect(Request.Url.AbsoluteUri.Replace(Request.Url.Query, "") + (!string.IsNullOrEmpty(queryString) ? "?" + queryString : ""));
                            return;
                        }
                    }
                    // If the secure flag is specified on the controller, check whether the use is authenticated or not.
                    if (s != null && s.IsSecure)
                        isAuthenticated = context.IsAuthenticated();
                    if (!isAuthenticated && AppId.HasValue && AppId != Guid.Empty && !string.IsNullOrEmpty(SecuriUrl))
                    {
                        // Redirect to the accounts management page (sign in page) and break out of the controller.
                        RedirectToAccountsManagement(s);
                        return;
                    }
                    // If the use is authenticated and a nonce check should be done, use the controller's nonce provider to generate or validate the nonce.
                    if (s != null && s.IsSecure && s.NonceCheck && isAuthenticated)
                    {
                        string nonce =
                            context.Request.Headers["nonce"] ??
                            context.Request.Form["nonce"] ??
                            context.Request.QueryString["nonce"];
                        // If the header, query string or post body does not contain a nonce value, generate one and return it to the caller.
                        if (string.IsNullOrEmpty(nonce))
                        {
                            nonce = context.GenerateNonce();
                            context.NonceProvider.StoreNonce(context.GetUserId(), nonce);
                            context.WriteResponse(
                                "{ \"nonce\": \"" + nonce + "\" }",
                                AllTypes.StringFromContentTypes(ContentType.Json),
                                (int)Status.Accepted
                                );
                            return;
                        }
                        // Validate and delete the nonce
                        bool validated = context.NonceProvider.ValidateNonce(context.GetUserId(), nonce);
                        context.NonceProvider.DeleteNonce(nonce);

                        if (!validated)
                            throw new WebException(Status.Forbidden, "The transaction could not be validated or has already been submitted");
                    }
                    // If the user is authenticated, or the controller is not secure, derive all path variables and invoke the controller method.
                    if (s == null || !s.IsSecure || isAuthenticated)
                    {
                        context.DerivePathVariables(mInfo);
                        // Invoke the controller method in the loaded assembly and get the response.
                        response = context.InvokeHandler(mInfo);
                    }
                    Minify m = mInfo.GetCustomAttribute<Minify>();
                    CacheOutput c = mInfo.GetCustomAttribute<CacheOutput>();
                    // If the environment is not in debug mode and the controller is set to cache output, set the cache flags on the response
                    if (!DEBUG && c != null && c.DoCache)
                    {
                        Response.Cache.SetExpires(DateTime.Now.AddDays(c.MaxDaysDuration));
                        Response.Cache.SetCacheability(HttpCacheability.Public);
                    }
                    bool doMinification = m == null || m.DoMinification;
                    if (response == null)
                    {
                        // No response was returned by the controller method, so end the request and break out of the controller.
                        Response.End();
                        return;
                    }
                    if (response.IsPrimitive())
                    {
                        // The response is a primitive type (i.e. string, int, date, bool, etc) so write out a string representation of it.
                        context.WriteResponse(
                            content: response.ToString(),
                            minify: doMinification
                            );
                        Response.End();
                        return;
                    }
                    if (response is IDictionary<string, object> || !response.IsPrimitive())
                    {
                        // The response is not a primitive type (i.e. object type) so write a JSON version of it.
                        context.WriteJsonResponse(response, minify: doMinification);
                        Response.End();
                        return;
                    }
                    // The response is not primitive or an object, so write a byte[] to the stream. This is typically for files or images.
                    context.WriteResponse(
                        content: response as byte[]
                    );
                    Response.End();
                }
                catch (ThreadAbortException)
                {
                    // The controller probably redirected to another call, so the calling thread was aborted.
                    return;
                }
                catch (WebException ex)
                {
                    // Handle any web exception that happened.
                    Error.Factory(Context, Configuration, ErrorLogPath, ex.Status, ex.Message).Handle();
                }
                catch (TargetInvocationException ex)
                {
                    // Test whether the dynamically called member was a WebException and handle it appropriately.
                    if (ex.InnerException != null && ex.InnerException is WebException)
                        Error.Factory(Context, Configuration, ErrorLogPath, (ex.InnerException as WebException).Status, ex.InnerException.Message).Handle();
                    else
                        Error.Factory(Context, Configuration, ErrorLogPath, Status.GeneralException, ex.InnerException.Message, ex.InnerException).Handle();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // The dynamic invocation could not be completed successfully, so print out all the type loader exceptions.
                    Error.Factory(Context, Configuration, ErrorLogPath, Status.GeneralException, ex.Message, ex).Handle();
                    if (ex.LoaderExceptions != null)
                        foreach (Exception e in ex.LoaderExceptions)
                            Error.Factory(Context, Configuration, ErrorLogPath, Status.GeneralException, e.Message, e).Handle();
                }
                catch (Exception ex)
                {
                    // Handle any other exception as an internal server error (500).
                    Error.Factory(Context, Configuration, ErrorLogPath, Status.GeneralException, ex.Message, ex).Handle();
                }
            }

            /// <summary>
            /// Redirects the current request to the configured accounts management resource (URL).
            /// </summary>
            /// <param name="s">The secure attribute configured for the controller method</param>
            private void RedirectToAccountsManagement(Secure s)
            {
                // Redirect to the configured sign-in resource (URL).
                Response.Redirect(SecuriUrl + "?app=" + AppId.ToString() + (s != null && !string.IsNullOrEmpty(s.RedirectUrl) ? "&redirect=" + HttpUtility.UrlEncode(s.RedirectUrl) : ""), true);
            }

            /// <summary>
            /// Finds the resource controller matching the incoming resource identifier (URL).
            /// </summary>
            /// <returns>A request controller describing all controller methods that might match the resource identifier (URL)</returns>
            private RequestController FindControllerClass()
            {
                // Find all classes that inherit from the base Handler class in the current application domain.
                var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t != GetType() && t.IsClass && typeof(Handler).IsAssignableFrom(t));
                foreach (var type in types)
                {
                    // Find all controller methods matching the incoming resource identifier (URL) in the specified type.
                    RequestController c = FindControllerMethods(type);
                    if (c != null)
                        return c;
                }
                return null;
            }

            /// <summary>
            /// Finds all resource controllers for the specified type.
            /// </summary>
            /// <param name="type">The type to find resource controllers on</param>
            /// <returns>A request controller describing all controller methods that might match the specified type</returns>
            private RequestController FindControllerMethods(Type type)
            {
                // Get all methods of the type.
                MethodInfo[] mInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                RequestController controller = null;
                foreach (var mi in mInfos)
                {
                    // Get the method's AcceptUrl attribute, which identifies it as a possible resource controller.
                    var p = mi.GetCustomAttribute<AcceptUrls>();
                    if (p == null)
                        continue;

                    bool matchPattern = new Func<bool>(() =>
                    {
                        foreach (string pattern in p.Patterns)
                        {
                            // Replace path variables (/resource/{path-var}) with a regular expression to match sanitised text.
                            string replacedPattern = Regex.Replace(pattern, "\\{[a-zA-Z0-9]+\\}", "[a-zA-Z0-9-_\\s]+", RegexOptions.IgnoreCase);
                            Regex r = new Regex(replacedPattern + "$");
                            if (r.IsMatch("/" + UriSegments.Flatten("/")))
                                return true;
                        }
                        return false;
                    })();
                    if (matchPattern)
                    {
                        // The regex pattern matches the incoming resource identifier (URL), so add it to the list of possible controller classes.
                        if (controller == null)
                            controller = new RequestController(type);
                        controller.Controllers.Add(mi);
                    }
                }
                return controller;
            }
        }

        /// <summary>
        /// Represents a resource controller to handle the incoming resource identifier (URL).
        /// </summary>
        private class RequestController
        {
            private List<MethodInfo> _Controllers = new List<MethodInfo>();

            /// <summary>
            /// Gets the class type of the resource controller.
            /// </summary>
            public Type ClassType { get; private set; }

            /// <summary>
            /// Gets all controller methods matching the resource controller.
            /// </summary>
            public List<MethodInfo> Controllers { get { return _Controllers; } }

            /// <summary>
            /// Gets the nonce provider for the resource controller.
            /// </summary>
            public NonceProvider NonceProvider
            {
                get
                {
                    var provider = ClassType.GetCustomAttribute<NonceProvider>();
                    if (provider == null)
                        provider = new NonceProvider(typeof(DefaultNonceProvider));
                    return provider;
                }
            }

            public RequestController(Type classType)
            {
                ClassType = classType;
            }

            /// <summary>
            /// Returns a list of verbs that the incoming resource identifier (URL) can operate against.
            /// </summary>
            /// <param name="controllerMethod">The method to compare against all possible types. Can be null.</param>
            /// <returns>A list of all verbs that the incoming resource (URL) can operate against, or the configured verbs for the specified method</returns>
            public List<Method> AcceptedVerbs(MethodInfo controllerMethod = null)
            {
                if (controllerMethod != null)
                    return controllerMethod.GetCustomAttribute<AcceptVerbs>() == null ? AllTypes.Get<Method>() : controllerMethod.GetCustomAttribute<AcceptVerbs>().Verbs.ToList();

                var verbs = from mi in Controllers
                            from verb in mi.GetCustomAttribute<AcceptVerbs>() == null ? AllTypes.Get<Method>() : mi.GetCustomAttribute<AcceptVerbs>().Verbs.ToList()
                            select verb;
                return verbs.Distinct().ToList();
            }

            /// <summary>
            /// Returns a list of content types that the incoming resource identifier (URL) can produce.
            /// </summary>
            /// <param name="controllerMethod">The method to compare against all possible types. Can be null.</param>
            /// <returns>A list of all content types that the incoming resource (URL) can produce, or the configured producing type for the specified method</returns>
            public List<ContentType> ProducedTypes(Method method)
            {
                var types = from mi in Controllers
                            where mi.GetCustomAttribute<AcceptVerbs>() == null || mi.GetCustomAttribute<AcceptVerbs>().Verbs.Contains(method)
                            select mi.GetCustomAttribute<Produces>() == null ? ContentType.Any : mi.GetCustomAttribute<Produces>().Type;
                return types.Distinct().ToList();
            }

            /// <summary>
            /// Returns a list of content types that the incoming resource identifier (URL) can consume.
            /// </summary>
            /// <param name="controllerMethod">The method to compare against all possible types. Can be null.</param>
            /// <returns>A list of all content types that the incoming resource (URL) can consume, or the configured consuming type for the specified method</returns>
            public List<ContentType> ConsumedTypes(Method method)
            {
                var types = from mi in Controllers
                            where mi.GetCustomAttribute<AcceptVerbs>() == null || mi.GetCustomAttribute<AcceptVerbs>().Verbs.Contains(method)
                            select mi.GetCustomAttribute<Consumes>() == null ? ContentType.Any : mi.GetCustomAttribute<Consumes>().Type;
                return types.Distinct().ToList();
            }

            /// <summary>
            /// Returns a list of all methods matching the specified verb.
            /// </summary>
            /// <param name="method">The verb to check</param>
            /// <returns>A list of methods matching the specified verb</returns>
            public List<MethodInfo> VerbMatches(Method method)
            {
                return VerbMatches(Controllers, method);
            }

            /// <summary>
            /// Returns a list of all methods matching the specified verb.
            /// </summary>
            /// <param name="controllerMethods">A list of methods to check for the specified verb</param>
            /// <param name="method">The verb to check</param>
            /// <returns>A list of methods matching the specified verb</returns>
            public List<MethodInfo> VerbMatches(List<MethodInfo> controllerMethods, Method method)
            {
                var match = from mi in controllerMethods
                            where mi.GetCustomAttribute<AcceptVerbs>() == null || mi.GetCustomAttribute<AcceptVerbs>().Verbs.Contains(method)
                            select mi;
                return match.ToList();
            }

            /// <summary>
            /// Returns a list of methods matching both the consuming and producing types.
            /// </summary>
            /// <param name="controllerMethods">A list of methods to check for consuming and producing types</param>
            /// <param name="consumes">The consuming type to check</param>
            /// <param name="produces">The producing type to check</param>
            /// <returns>A list of methods matching the consuming and producing types</returns>
            public List<MethodInfo> TypeMatches(List<MethodInfo> controllerMethods, ContentType consumes, ContentType produces)
            {
                var match = from mi in controllerMethods
                            select new
                            {
                                ControllerMethod = mi,
                                ConsumesPriority = mi.GetCustomAttribute<Consumes>() != null ? 1 : 2,
                                ProducesPriority = mi.GetCustomAttribute<Produces>() != null ? 1 : 2,
                                Consumes = mi.GetCustomAttribute<Consumes>() != null ? mi.GetCustomAttribute<Consumes>().Type : ContentType.Any,
                                Produces = mi.GetCustomAttribute<Produces>() != null ? mi.GetCustomAttribute<Produces>().Type : ContentType.Any
                            };
                match = from m in match
                        where
                            (m.Consumes == consumes || m.Consumes == ContentType.Any || consumes == ContentType.Any) &&
                            (m.Produces == produces || m.Produces == ContentType.Any || produces == ContentType.Any)
                        orderby m.ConsumesPriority, m.ProducesPriority
                        select m;
                var typeMatch = from m in match
                                select m.ControllerMethod;
                return typeMatch.ToList();
            }
        }
    }
}