using System;
using System.Collections.Generic;

namespace Handlr.Framework.Routing.Attributes
{
    /// <summary>
    /// Represents a tag in a route definition.
    /// </summary>
    public class Tag : Attribute
    {
        /// <summary>
        /// Gets a list of tags that will be matched against the route definition.
        /// </summary>
        public List<string> Tags { get; private set; } = new List<string>();

        /// <summary>
        /// Creates a new Tag instance.
        /// </summary>
        /// <param name="tags">An array of tags to associated with the tag instance</param>
        public Tag(params string[] tags)
        {
            if (tags != null)
                Tags.AddRange(tags);
        }
    }
}
