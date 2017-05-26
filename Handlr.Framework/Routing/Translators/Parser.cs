using Handlr.Framework.Routing.Exceptions;
using Handlr.Framework.Routing.Interfaces;
using Handlr.Framework.Routing.Loaders;
using System.IO;

namespace Handlr.Framework.Routing.Translators
{
    /// <summary>
    /// Represents a base class for building translation parsers.
    /// </summary>
    public abstract class Parser : Base<TranslationLoaderArguments>
    {
        protected string _ListPre;
        protected string _ListPost;
        protected string _Concat;
        protected string _FieldTermination;

        public Parser(string listPre, string listPost, string concat, string fieldTermExpr)
        {
            _ListPre = listPre;
            _ListPost = listPost;
            _Concat = concat;
            _FieldTermination = fieldTermExpr;
        }

        /// <summary>
        /// Loads the template specified in the loader arguments.
        /// </summary>
        /// <returns>The template to apply during the translation</returns>
        protected string LoadTemplate()
        {
            string path = LoaderArguments.TemplatePath.Replace("/", "\\").Replace("{AbsolutePath}", LoaderArguments.AbsolutePath);
            FileInfo fi = new FileInfo(path);

            if (!fi.Exists)
                throw new ParserException(string.Format("The translation file \"{0}\" could not be loaded as the file does not exist", path));

            string template = null;
            using (StreamReader reader = new StreamReader(path))
            {
                template = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(template))
                throw new ParserException(string.Format("The translation file \"{0}\" is empty", path));

            return template;
        }

        /// <summary>
        /// Parses a "foreach" statement in the template and applies the template to each object in the list of data.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="inputData">The field cache containing all keys to apply</param>
        /// <param name="parsedValue">The parsed value</param>
        /// <returns></returns>
        protected abstract bool TryParseForLoop(string value, IFieldCache inputData, out string parsedValue);

        /// <summary>
        /// Parses member accessors in the template.
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <param name="inputData">The field cache containing all keys to apply</param>
        /// <param name="parsedValue">The parsed value</param>
        /// <returns></returns>
        protected abstract bool TryParseDataMembers(string value, IFieldCache inputData, out string parsedValue);
    }
}
