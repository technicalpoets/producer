using System;
using Newtonsoft.Json;

namespace Producer.Domain
{
	public class UserStore : Entity
	{
		[JsonIgnore]
		public const int TokenDurationSeconds = 18000; // 5 hours

		[JsonIgnore]
		public const double TokenRefreshSeconds = 600; // 10 minutes

		[JsonIgnore]
		public const string DatabaseId = "Users";

		[JsonIgnore]
		public const string CollectionId = "Users";

		[JsonIgnore]
		public const string AnonymousId = "anonymous_user";


		public string Email { get; set; }

		public UserRoles UserRole { get; set; }

		public string Token { get; set; }

		[JsonConverter (typeof (UnixDateTimeConverter))]
		public DateTime TokenTimestamp { get; set; }


		[JsonIgnore]
		public bool HasToken => !string.IsNullOrEmpty (Token);

		[JsonIgnore]
		public DateTime TokenExpiration => HasToken ? TokenTimestamp.AddSeconds (TokenDurationSeconds) : DateTime.MinValue;

		[JsonIgnore]
		public bool RefreshToken => TokenExpiration == DateTime.MinValue || TokenExpiration.Subtract (DateTime.UtcNow).TotalSeconds < TokenRefreshSeconds;

		[JsonIgnore]
		public bool ValidToken => !RefreshToken;


		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nUserStore\n");
			sb.Append ("  Id".PadRight (20));
			sb.Append ($"{Id}\n");
			sb.Append ("  ETag".PadRight (20));
			sb.Append ($"{ETag}\n");
			sb.Append ("  ResourceId".PadRight (20));
			sb.Append ($"{ResourceId}\n");
			sb.Append ("  SelfLink".PadRight (20));
			sb.Append ($"{SelfLink}\n");
			sb.Append ("  Timestamp".PadRight (20));
			sb.Append ($"{Timestamp}\n");
			sb.Append ("  Email".PadRight (20));
			sb.Append ($"{Email}\n");
			sb.Append ("  UserRole".PadRight (20));
			sb.Append ($"{UserRole}\n");
			sb.Append ("  Token".PadRight (20));
			sb.Append ($"{Token}\n");
			sb.Append ("  TokenTimestamp".PadRight (20));
			sb.Append ($"{TokenTimestamp}\n");
			sb.Append ("  TokenExpiration".PadRight (20));
			sb.Append ($"{TokenExpiration}\n");
			sb.Append ("  RefreshToken".PadRight (20));
			sb.Append ($"{RefreshToken}\n");
			return sb.ToString ();
		}
	}
}
