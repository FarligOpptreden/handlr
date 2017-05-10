using System;

namespace Handlr.Framework.Web
{
    partial class Handler
    {
        /// <summary>
        /// The default pass phrase to use for encryption if one isn't configured in the handlr.config file.
        /// Using the default pass phrase is NOT recommended.
        /// </summary>
        private string passPhrase = "h4ndlR.p@55phra5E";

        /// <summary>
        /// Decrypts and deserializes a cookie with the specified key to the supplied type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the cookie to</typeparam>
        /// <param name="key">The key of the cookie to decrypt and deserialize</param>
        /// <returns>An object representing the decrypted and deserialized cookie</returns>
        protected virtual T GetCookieObject<T>(string key)
        {
            string value = null;
            if (string.IsNullOrEmpty(key))
                return default(T);
            for (int i = 0; i < Request.Cookies.AllKeys.Length; i++)
            {
                var cookie = Request.Cookies[i];
                if (cookie.Name.ToLower() == key.ToLower() && !string.IsNullOrEmpty(cookie.Value))
                {
                    value = cookie.Value;
                    break;
                }
            }
            if (string.IsNullOrEmpty(value))
                return default(T);
            try
            {
                value = Cryptography.Decrypt(value, Configuration.Accounts.PassPhrase ?? passPhrase);
            }
            catch
            {
                Response.Cookies[key].Value = "";
                return default(T);
            }
            try
            {
                return value.ToObject<T>();
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Serializes and encrypts the supplied object to a cookie with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize and encrypt</typeparam>
        /// <param name="key">The key of the cookie to create or update</param>
        /// <param name="obj">The object to serialize and encrypt</param>
        /// <param name="expires">A date on which the cookie expires, but cookies default to session only</param>
        protected virtual void SetCookieObject<T>(string key, T obj, DateTime? expires = null)
        {
            if (obj != null)
            {
                string objString = !obj.IsPrimitive() ? obj.ToJson() : obj.ToString();
                objString = Cryptography.Encrypt(objString, Configuration.Accounts.PassPhrase ?? passPhrase);
                Response.Cookies[key].Value = objString;
                Response.Cookies[key].Path = "/";
                if (expires.HasValue)
                    Response.Cookies[key].Expires = expires.Value;
            }
            else
            {
                Response.Cookies[key].Value = "";
            }
        }
    }
}
