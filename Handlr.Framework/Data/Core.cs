using Handlr.Framework.Data.Interfaces;
using System;
using System.Reflection;

namespace Handlr.Framework.Data
{
    public static class Core
    {
        /// <summary>
        /// Initializes and returns a new instance of the given type of database connector.
        /// </summary>
        /// <typeparam name="T">The type of database connector to initialize</typeparam>
        /// <param name="connectionString">The connection string to the datasource</param>
        /// <returns>An initialized instance of the specified database connector type</returns>
        public static T Factory<T>(string connectionString, Func<T, T> startupDelegate, bool skipTest, string loggingPath, bool bubbleExceptions = false)
            where T : IConnector, IDisposable
        {
            T connector = (T)typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string) }, null).Invoke(new object[] { connectionString });
            connector.Initialize();
            if (!string.IsNullOrEmpty(loggingPath))
                connector.LoggingPath = loggingPath;
            if (startupDelegate != null)
                connector = startupDelegate(connector);
            connector.BubbleExceptions = bubbleExceptions;
            if (skipTest ? false : !connector.TestConnection())
            {
                Logging.Log(new Exception(string.Format("Could not establish connection to the datasource: {0}", connectionString)), connector.LoggingPath);
                connector.Dispose();
                connector = default(T);
                throw new Exception("Could not establish connection to the datasource");
            }
            return connector;
        }

        public static T Factory<T>(string connectionString, Func<T, T> startupDelegate, bool skipTest)
            where T : IConnector, IDisposable
        {
            return Factory(connectionString, startupDelegate, skipTest, null);
        }

        public static T Factory<T>(string connectionString, Func<T, T> startupDelegate)
            where T : IConnector, IDisposable
        {
            return Factory(connectionString, startupDelegate, false, null);
        }

        public static T Factory<T>(string connectionString)
            where T : IConnector, IDisposable
        {
            return Factory<T>(connectionString, null, false, null);
        }
    }
}
