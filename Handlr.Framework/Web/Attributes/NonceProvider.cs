using Handlr.Framework.Web.Interfaces;
using System;

namespace Handlr.Framework.Web.Attributes
{
    public sealed class NonceProvider : Attribute
    {
        public NonceProvider(Type nonceProviderType)
        {
            if (!typeof(INonceProvider).IsAssignableFrom(nonceProviderType))
                throw new ArgumentException("The nonce provider type must implement the INonceProvider interface");
            ProviderType = nonceProviderType;
        }

        public Type ProviderType { get; private set; }

        public INonceProvider CreateInstance(IController controller)
        {
            INonceProvider provider = (INonceProvider)Activator.CreateInstance(ProviderType);
            provider.ControllerContext = controller;
            return provider;
        }
    }
}
