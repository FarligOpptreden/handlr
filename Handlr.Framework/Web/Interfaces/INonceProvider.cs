namespace Handlr.Framework.Web.Interfaces
{
    /// <summary>
    /// Provides an interface for building a nonce provider to ensure all secure requests are unique.
    /// </summary>
    public interface INonceProvider
    {
        /// <summary>
        /// Gets or sets the controller context of the nonce provider.
        /// </summary>
        IController ControllerContext { get; set; }

        /// <summary>
        /// Store the newly issued nonce in the nonce store for the specified user ID.
        /// </summary>
        /// <param name="userId">The user ID to associate the nonce with</param>
        /// <param name="nonce">The newly issued nonce</param>
        void StoreNonce(string userId, string nonce);

        /// <summary>
        /// Validate the supplied nonce against the nonce issued during the previous request for the specified user ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        bool ValidateNonce(string userId, string nonce);

        /// <summary>
        /// Delete a nonce from the nonce store.
        /// </summary>
        /// <param name="nonce">The nonce to delete</param>
        void DeleteNonce(string nonce);
    }
}
