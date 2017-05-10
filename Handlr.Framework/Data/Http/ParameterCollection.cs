using System;
using System.Collections.Generic;
using System.Linq;

namespace Handlr.Framework.Data.Http
{
    public sealed class ParameterCollection : System.Data.Common.DbParameterCollection
    {
        private List<Parameter> parameters = new List<Parameter>();

        public override int Add(object value)
        {
            if (value.GetType() != typeof(Parameter))
                throw new Exception("Invalid parameter type. Please use Handlr.Data.Http.Parameter type only.");
            parameters.Add((Parameter)value);
            return parameters.IndexOf((Parameter)value);
        }

        public override void AddRange(Array values)
        {
            foreach (Parameter value in values)
            {
                if (value.GetType() != typeof(Parameter))
                    throw new Exception("Invalid parameter type. Please use Handlr.Data.Http.Parameter type only.");
                parameters.Add(value);
            }
        }

        public override void Clear()
        {
            parameters = new List<Parameter>();
        }

        public override bool Contains(object value)
        {
            return parameters.Where(p => p.ParameterName.ToLower() == ((Parameter)value).ParameterName.ToLower()).FirstOrDefault() != null;
        }

        public override bool Contains(string value)
        {
            return parameters.Where(p => p.ParameterName.ToLower() == value.ToLower()).FirstOrDefault() != null;
        }

        public override void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public override int Count
        {
            get 
            { 
                return parameters.Count; 
            }
        }

        public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        {
            return base.CreateObjRef(requestedType);
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        protected override System.Data.Common.DbParameter GetParameter(int index)
        {
            return parameters[index];
        }

        protected override System.Data.Common.DbParameter GetParameter(string parameterName)
        {
            return parameters.Where(p => p.ParameterName.ToLower() == parameterName.ToLower()).FirstOrDefault();
        }

        public override int IndexOf(object value)
        {
            if (value.GetType() != typeof(Parameter))
                throw new Exception("Invalid parameter type. Please use Handlr.Data.Http.Parameter type only.");
            int index = 0;
            foreach (Parameter p in parameters)
            {
                if (p.ParameterName.ToLower() == ((Parameter)value).ParameterName.ToLower())
                    return index;
                index++;
            }
            return -1;
        }

        public override int IndexOf(string parameterName)
        {
            int index = 0;
            foreach (Parameter p in parameters)
            {
                if (p.ParameterName.ToLower() == parameterName.ToLower())
                    return index;
                index++;
            }
            return -1;
        }

        public override void Insert(int index, object value)
        {
            if (value.GetType() != typeof(Parameter))
                throw new Exception("Invalid parameter type. Please use Handlr.Data.Http.Parameter type only.");
            parameters.Insert(index, (Parameter)value);
        }

        public override bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public override void Remove(object value)
        {
            if (value.GetType() != typeof(Parameter))
                throw new Exception("Invalid parameter type. Please use Handlr.Data.Http.Parameter type only.");
            foreach (Parameter p in parameters)
            {
                if (p.ParameterName.ToLower() == ((Parameter)value).ParameterName.ToLower())
                {
                    parameters.Remove(p);
                    return;
                }
            }
        }

        public override void RemoveAt(int index)
        {
            parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            foreach (Parameter p in parameters)
            {
                if (p.ParameterName.ToLower() == parameterName.ToLower())
                {
                    parameters.Remove(p);
                    return;
                }
            }
        }

        protected override void SetParameter(int index, System.Data.Common.DbParameter value)
        {
            if (value.GetType() != typeof(Parameter))
                throw new Exception("Invalid parameter type. Please use Handlr.Data.Http.Parameter type only.");
            parameters[index] = (Parameter)value;
        }

        protected override void SetParameter(string parameterName, System.Data.Common.DbParameter value)
        {
            if (value.GetType() != typeof(Parameter))
                throw new Exception("Invalid parameter type. Please use Handlr.Data.Http.Parameter type only.");
            int index = 0;
            foreach (Parameter p in parameters)
            {
                if (p.ParameterName.ToLower() == parameterName.ToLower())
                {
                    parameters[index] = (Parameter)value;
                    return;
                }
                index++;
            }
        }

        public override object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
    }
}
