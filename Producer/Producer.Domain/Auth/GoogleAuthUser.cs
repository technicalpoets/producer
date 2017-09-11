using System;
using System.Security.Claims;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Producer.Auth
{
	public class GoogleAuthUser : BaseAuthUser
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


		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nGoogleAuthUser\n");
			sb.Append ("  AccessToken".PadRight (31)); sb.Append ($"{AccessToken}\n");
			sb.Append ("  RefreshToken".PadRight (31)); sb.Append ($"{RefreshToken}\n");
			sb.Append ("  AccessTokenExpiration".PadRight (31)); sb.Append ($"{AccessTokenExpiration}\n");
			sb.Append ("  NameIdentifier".PadRight (31)); sb.Append ($"{NameIdentifier}\n");
			sb.Append ("  EmailAddress".PadRight (31)); sb.Append ($"{EmailAddress}\n");
			sb.Append ("  EmailVerified".PadRight (31)); sb.Append ($"{EmailVerified}\n");
			sb.Append ("  AtHash".PadRight (31)); sb.Append ($"{AtHash}\n");
			sb.Append ("  Issuer".PadRight (31)); sb.Append ($"{Issuer}\n");
			sb.Append ("  IssuedAt".PadRight (31)); sb.Append ($"{IssuedAt}\n");
			sb.Append ("  Expires".PadRight (31)); sb.Append ($"{Expires}\n");
			sb.Append ("  Name".PadRight (31)); sb.Append ($"{Name}\n");
			sb.Append ("  Picture".PadRight (31)); sb.Append ($"{Picture}\n");
			sb.Append ("  GivenName".PadRight (31)); sb.Append ($"{GivenName}\n");
			sb.Append ("  SurName".PadRight (31)); sb.Append ($"{SurName}\n");
			sb.Append ("  Locale".PadRight (31)); sb.Append ($"{Locale}\n");
			return sb.ToString ();
		}
	}
}
