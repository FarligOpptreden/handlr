using System;
using System.Collections.Generic;
using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Web.Interfaces;
using System.Reflection;

namespace Handlr.Framework.Routing
{
    public static class Utilities
    {
        /// <summary>
        /// A collection of useful functions to be shared across projects.
        /// </summary>
        private static Interfaces.IConfig _Config;

        /// <summary>
        /// Sets the current configuration.
        /// </summary>
        /// <param name="config">The configuration to set.</param>
        public static void Configure(Interfaces.IConfig config)
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

        private static object GetIndexedProperty(object dataMember, int indexer)
        {
            var memberType = dataMember.GetType();
            // Assume the data member is an array or list, so search for the indexer property and use it to grab the indexed object
            PropertyInfo indexProp = null;
            foreach (var prop in memberType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (prop.GetIndexParameters().Length > 0)
                {
                    indexProp = prop;
                    break;
                }
            }
            return indexProp.GetValue(dataMember, new object[] { indexer });
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
                if (dataMember == null)
                    return dataMember;

                string part = pathParts[i];

                // If the current part starts with "@" (i.e. "@input") continue to the next part, as this assumes
                // the data member is being access inside a translation and @input would refer to the field cache
                if (part.StartsWith("@"))
                    continue;

                // If the current part contains indexing accessors "[" and "]" (i.e. input[1]), interpret is as
                // an array or list to get the indexed object
                if (part.Contains("[") && part.Contains("]"))
                {
                    string propertyString = part.Substring(0, part.IndexOf("["));
                    string indexerString = part.Substring(part.IndexOf("[") + 1, part.Length - part.IndexOf("[") - 2);
                    int indexer;

                    if (!int.TryParse(indexerString, out indexer))
                        throw new ParserException(string.Format("The indexer \"{0}\" is not a valid number.", indexerString));

                    try
                    {
                        if (dataMember is Dictionary<string, object>)
                        {
                            // The data member is a dictionary, so cast it accordingly to grab the indexed object
                            dataMember = (dataMember as Dictionary<string, object>)[propertyString];
                            dataMember = GetIndexedProperty(dataMember, indexer);
                            continue;
                        }
                        // The data member is not a dictionary, so use reflection to get to the indexed object
                        var memberType = dataMember.GetType();
                        var property = memberType.GetProperty(propertyString, BindingFlags.Public | BindingFlags.Instance);
                        dataMember = property.GetValue(dataMember);
                        dataMember = GetIndexedProperty(dataMember, indexer);
                    }
                    catch
                    {
                        throw new ParserException(string.Format("Could not access the indexed property \"{0}\" on the member \"{1}\".", indexer, pathParts));
                    }

                    continue;
                }
                try
                {
                    // Access the data member on the contextual dataMember based on the part name
                    if (dataMember is Dictionary<string, object>)
                    {
                        // The data member is a dictionary, so cast it accordingly to grab it
                        if (!(dataMember as Dictionary<string, object>).ContainsKey(part))
                        {
                            dataMember = null;
                            continue;
                        }
                        dataMember = (dataMember as Dictionary<string, object>)[part];
                        continue;
                    }
                    // The data member is not a dictionary, so use reflection to grab it
                    var memberType = dataMember.GetType();
                    var property = memberType.GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
                    if (property == null)
                        continue;
                    dataMember = property.GetValue(dataMember);
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