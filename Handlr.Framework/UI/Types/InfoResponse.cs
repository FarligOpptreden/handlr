using Newtonsoft.Json;
using System.Collections.Generic;

namespace Handlr.Framework.UI.Types
{
    public class InfoResponse<T> : ResponseBase
    {
        public InfoResponse(T data, string message, bool success)
        {
            Data = data;
            Message = message;
            Success = success;
            Extra = null;
        }

        public InfoResponse(T data, string message)
        {
            Data = data;
            Message = message;
            Success = true;
            Extra = null;
        }

        public InfoResponse(T data, bool success)
        {
            Data = data;
            Message = null;
            Success = success;
            Extra = null;
        }

        public InfoResponse(T data, Dictionary<string, object> extra)
        {
            Data = data;
            Message = null;
            Success = true;
            Extra = extra;
        }

        public InfoResponse(T data, string message, Dictionary<string, object> extra)
        {
            Data = data;
            Message = message;
            Success = true;
            Extra = extra;
        }

        public InfoResponse(T data, bool success, Dictionary<string, object> extra)
        {
            Data = data;
            Message = null;
            Success = success;
            Extra = extra;
        }

        [JsonConstructor]
        public InfoResponse(T data, string message, bool success, Dictionary<string, object> extra)
        {
            Data = data;
            Message = message;
            Success = success;
            Extra = extra;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; private set; }

        public bool Success { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Extra { get; private set; }
    }
}
