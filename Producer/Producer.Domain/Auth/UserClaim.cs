using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Producer.Auth
{
	public class UserClaim
	{
		[JsonProperty ("typ")]
		public string Type { get; set; }

		[JsonProperty ("val")]
		public string Value { get; set; }
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
