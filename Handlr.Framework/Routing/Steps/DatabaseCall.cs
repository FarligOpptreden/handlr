using Handlr.Framework.Routing.Loaders;
using Handlr.Framework.Routing.Types;
using System;
using System.Collections.Generic;
using Handlr.Framework.Web.Interfaces;

namespace Handlr.Framework.Routing.Steps
{
    /// <summary>
    /// Represents a base class for building database call steps.
    /// </summary>
    public abstract class DatabaseCall : Base<DatabaseCallLoaderArguments>
    {
        public DatabaseCall(IController executionContext) : base(executionContext) { }

        /// <summary>
        /// Derives the parameters for the query from the parameters passed to the loader arguments.
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, object> DeriveParameters()
        {
            var parameters = new Dictionary<string, object>();
            foreach (var parameter in LoaderArguments.Parameters)
            {
                object value = ParseValue(parameter.Value.ToString());
                switch (parameter.Type)
                {
                    case DatabaseParameter.DataType.Bit:
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            value = parameter.Nullable ? null : (object)false;
                            break;
                        }
                        bool outBool;
                        if (!bool.TryParse(value.ToString(), out outBool))
                            throw new ArgumentException("Invalid bit parameter passed to stored procedure", parameter.Name);
                        value = outBool;
                        break;
                    case DatabaseParameter.DataType.DateTime:
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            value = parameter.Nullable ? null : (object)DateTime.Today;
                            break;
                        }
                        DateTime outDateTime;
                        if (!DateTime.TryParse(value.ToString(), out outDateTime))
                            throw new ArgumentException("Invalid datetime parameter passed to stored procedure", parameter.Name);
                        value = outDateTime;
                        break;
                    case DatabaseParameter.DataType.Float:
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            value = parameter.Nullable ? null : (object)0.00f;
                            break;
                        }
                        float outFloat;
                        if (!float.TryParse(value.ToString(), out outFloat))
                            throw new ArgumentException("Invalid float parameter passed to stored procedure", parameter.Name);
                        value = outFloat;
                        break;
                    case DatabaseParameter.DataType.Int:
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            value = parameter.Nullable ? null : (object)0;
                            break;
                        }
                        int outInt;
                        if (!int.TryParse(value.ToString(), out outInt))
                            throw new ArgumentException("Invalid int parameter passed to stored procedure", parameter.Name);
                        value = outInt;
                        break;
                    case DatabaseParameter.DataType.String:
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            value = parameter.Nullable ? null : "";
                            break;
                        }
                        value = value.ToString();
                        break;
                }
                parameters.Add(parameter.Name, value);
            }
            return parameters;
        }
    }
}
