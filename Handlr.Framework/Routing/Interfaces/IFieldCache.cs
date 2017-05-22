﻿using System.Collections;

namespace Handlr.Framework.Routing.Interfaces
{
    /// <summary>
    /// An interface for building field caches that represent data passed between IStep implementations.
    /// </summary>
    public interface IFieldCache : IDictionary
    {
        bool Exists(string key);

        T GetValue<T>(string key);
    }
}
