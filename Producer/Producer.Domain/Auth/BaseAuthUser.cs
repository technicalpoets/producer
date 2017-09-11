using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Producer.Auth
{
	public abstract class BaseAuthUser
	{
		[JsonIgnore]
		public abstract string Provider { get; }

		[JsonProperty ("user_id")]
		public string UserId { get; set; }

		[JsonProperty ("id_token")]
		public string IdToken { get; set; }

		[JsonProperty ("provider_name")]
		public string ProviderName { get; set; }

		[JsonProperty ("user_claims", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<UserClaim> UserClaims { get; set; }
	}


	public class UserClaim
	{
		[JsonProperty ("typ")]
		public string Type { get; set; }

		[JsonProperty ("val")]
		public string Value { get; set; }

		public override string ToString ()
		{
			return string.Format ("[UserClaim: Type={0}, Value={1}]", Type, Value);
		}
	}


	public static class UserClaimExtensions
	{
		public static string StringValue (this IEnumerable<UserClaim> claims, string type)
		{
			return claims?.FirstOrDefault (c => string.Compare (c.Type, type, StringComparison.OrdinalIgnoreCase) == 0)?.Value;
		}

		public static bool BoolValue (this IEnumerable<UserClaim> claims, string type)
		{
			return string.Compare (claims?.FirstOrDefault (c => string.Compare (c.Type, type, StringComparison.OrdinalIgnoreCase) == 0)?.Value, "true", StringComparison.OrdinalIgnoreCase) == 0;
		}
	}
}
