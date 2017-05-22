using System;
using System.Collections.Generic;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Web.Interfaces;

namespace Handlr.Framework.Routing
{
    public static class Utilities
    {
        /// <summary>
        /// A collection of useful functions to be shared across projects.
        /// </summary>
        private static IConfig _Config;

        /// <summary>
        /// Sets the current configuration.
        /// </summary>
        /// <param name="config">The configuration to set.</param>
        public static void Configure(IConfig config)
        {
            _Config = config;
        }

        /// <summary>
        /// Decrypts a connection string for connecting to a data source.
        /// </summary>
        /// <param name="controllerContext">The execution context to get the configuration from</param>
        /// <param name="key">The key of the connection string to decrypt</param>
        /// <returns>The decrypted connection string</returns>
        public static string DecryptConnectionString(IController controllerContext, string key)
        {
            return Cryptography.Decrypt(controllerContext.Configuration().ConnectionStrings[key], _Config.PassPhrase);
        }

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="cipherText">The string to decrypt</param>
        /// <returns>The decrypted string</returns>
        public static string DecryptString(string cipherText)
        {
            return Cryptography.Decrypt(cipherText, _Config.PassPhrase);
        }

        /// <summary>
        /// Gets a data member based on the member path.
        /// </summary>
        /// <param name="pathParts">The individual parts of the access path to the data member</param>
        /// <param name="dataMember">The object to retrieve the data member from</param>
        /// <returns>The value of the associated access path</returns>
        public static object GetDataMember(string[] pathParts, object dataMember)
        {
            if (pathParts.Length < 1)
                return dataMember;

            for (int i = 0; i < pathParts.Length; i++)
            {
                string part = pathParts[i];
                if (part.StartsWith("@"))
                    continue;
                if (part.Contains("[") && part.Contains("]"))
                {
                    string propertyString = part.Substring(0, part.IndexOf("["));
                    string indexerString = part.Substring(part.IndexOf("[") + 1, part.Length - part.IndexOf("[") - 2);
                    int indexer;

                    if (!int.TryParse(indexerString, out indexer))
                        throw new ParserException(string.Format("The indexer \"{0}\" is not a valid number.", indexerString));

                    try
                    {
                        dataMember = (dataMember as Dictionary<string, object>)[propertyString];
                        dataMember = (dataMember as List<object>)[indexer];
                    }
                    catch
                    {
                        throw new ParserException(string.Format("Could not access the indexed property \"{0}\" on the member \"{1}\".", indexer, pathParts));
                    }

                    continue;
                }
                if (i == 0)
                {
                    dataMember = (dataMember as Dictionary<string, object>)[part];
                    continue;
                }
                try
                {
                    dataMember = (dataMember as Dictionary<string, object>)[part];
                }
                catch (Exception ex)
                {
                    throw new ParserException(string.Format("The property \"{0}\" could not be accessed as a data member: {1}", pathParts.Flatten("."), ex.Message));
                }
            }
            return dataMember;
        }
    }
}
