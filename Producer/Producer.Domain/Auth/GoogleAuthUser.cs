using System;
using System.Security.Claims;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Producer.Auth
{
	public class GoogleAuthUser : AuthUser
	{
		public override string Provider => "google";

		[JsonProperty ("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty ("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonProperty ("expires_on")]
		public DateTimeOffset? AccessTokenExpiration { get; set; }


		[JsonIgnore]
		public string NameIdentifier => UserClaims.StringValue (ClaimTypes.NameIdentifier);

		[JsonIgnore]
		public string EmailAddress => UserClaims.StringValue (ClaimTypes.Email);

		[JsonIgnore]
		public bool EmailVerified => UserClaims.BoolValue ("email_verified");

		[JsonIgnore]
		public string AtHash => UserClaims.StringValue ("at_hash");

		[JsonIgnore]
		public string Issuer => UserClaims.StringValue ("iss");

		[JsonIgnore]
		public string IssuedAt => UserClaims.StringValue ("iat");

		[JsonIgnore]
		public string Expires => UserClaims.StringValue ("exp");

		[JsonIgnore]
		public string Name => UserClaims.StringValue ("name");

		[JsonIgnore]
		public string Picture => UserClaims.StringValue ("picture");

		[JsonIgnore]
		public string GivenName => UserClaims.StringValue (ClaimTypes.GivenName);

		[JsonIgnore]
		public string SurName => UserClaims.StringValue (ClaimTypes.Surname);

		[JsonIgnore]
		public string Locale => UserClaims.StringValue ("locale");
	}
}
