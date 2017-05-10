using Handlr.Framework.Web.Attributes;
using Newtonsoft.Json;
using System;

namespace Handlr.Framework.Web.Types
{
    public class User
    {
        public User() { }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid ID { get; set; }

        [MapsTo("Email_Address")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EmailAddress { get; set; }

        [MapsTo("Given_Name")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GivenName { get; set; }

        [MapsTo("Family_Name")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FamilyName { get; set; }

        [MapsTo("Display_Name")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Photo { get; set; }

        [MapsTo("Provider_*")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Provider Provider { get; set; }
    }
}
