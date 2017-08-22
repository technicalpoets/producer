using System.Collections.Generic;

using Newtonsoft.Json;

namespace Producer.Auth
{
	public abstract class AuthUser
	{
		[JsonIgnore]
		public abstract string Provider { get; }

		[JsonProperty("user_id")]
		public string UserId { get; set; }

		[JsonProperty("id_token")]
		public string IdToken { get; set; }

		[JsonProperty("provider_name")]
		public string ProviderName { get; set; }

		[JsonProperty("user_claims", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<UserClaim> UserClaims { get; set; }
	}
}
