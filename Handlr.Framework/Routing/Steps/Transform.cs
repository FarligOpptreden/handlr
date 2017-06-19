using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Loaders;
using Handlr.Framework.Web.Interfaces;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a step that transforms a given type to a specified type.
    /// </summary>
    public abstract class Transform : Base<TransformLoaderArguments>
    {
        public Transform(IController executionContext) : base(executionContext) { }

        public abstract override IFieldCache ExecuteStep(IFieldCache fieldCache);
    }
}
