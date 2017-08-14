using Newtonsoft.Json;

namespace Producer.Functions
{

	public class ApsPayload
	{
		[JsonProperty ("aps")]
		public Aps Aps { get; set; }

		public ApsPayload (Aps aps)
		{
			Aps = aps;
		}

		public ApsPayload (string title, string body, bool contentAvailable)
		{
			Aps = new Aps
			{
				Alert = new ApsAlert
				{
					Title = string.IsNullOrWhiteSpace (title) ? null : title,
					Body = string.IsNullOrWhiteSpace (body) ? null : body
				},
				ContentAvailable = contentAvailable ? 1 : 0
			};
		}

		public static ApsPayload Create (string title, string body, bool contentAvailable) => new ApsPayload (title, body, contentAvailable);

		public string Serialize () => JsonConvert.SerializeObject (this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
	}


	public class Aps
	{
		[JsonProperty ("alert")]
		public ApsAlert Alert { get; set; }

		[JsonProperty ("badge")]
		public int? Badge { get; set; }

		[JsonProperty ("sound")]
		public string Sound { get; set; }

		[JsonProperty ("content-available")]
		public int? ContentAvailable { get; set; }

		[JsonProperty ("category")]
		public string Category { get; set; }
	}


	public class ApsAlert
	{
		[JsonProperty ("title")]
		public string Title { get; set; }

		[JsonProperty ("body")]
		public string Body { get; set; }
	}
}
