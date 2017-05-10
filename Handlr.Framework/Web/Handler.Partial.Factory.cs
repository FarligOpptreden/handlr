using Handlr.Framework.Web.Attributes;
using Handlr.Framework.Web.Types;
using System;
using System.Dynamic;
using System.Reflection;
using System.Web;

namespace Handlr.Framework.Web
{
    public abstract partial class Handler
    {
        public static Handler Factory(Type type, HttpContext context, NonceProvider nonceProvider, dynamic configuration)
        {
            ConstructorInfo ci = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(HttpContext), typeof(ExpandoObject) }, null);
            if (ci == null)
            {
                ci = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { }, null);
                Handler handler = (Handler)ci.Invoke(null);
                handler.PostInit += () =>
                {
                    if (nonceProvider == null || nonceProvider.ProviderType == null)
                        nonceProvider = new NonceProvider(typeof(DefaultNonceProvider));
                    handler.NonceProvider = nonceProvider.CreateInstance(handler);
                };
                handler.Init(context, configuration);
                return handler;
            }
            else
            {
                return (Handler)ci.Invoke(new object[] { context, configuration });
            }
        }
    }
}
