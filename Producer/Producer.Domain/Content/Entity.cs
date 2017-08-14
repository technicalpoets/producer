using System;

using Newtonsoft.Json;

namespace Producer.Domain
{
	public class Entity
	{
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }

        [JsonProperty("_rid")]
        public string ResourceId { get; set; }

        [JsonProperty("_self")]
        public string SelfLink { get; set; }

        [JsonProperty(PropertyName = "_ts")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Timestamp { get; set; }

		[JsonIgnore]
		public bool HasId => !string.IsNullOrEmpty (Id);

		[JsonIgnore]
		public bool HasResourceId => !string.IsNullOrEmpty (ResourceId);
    }    
}
