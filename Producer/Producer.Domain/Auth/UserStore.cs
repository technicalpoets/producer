using System;
using Newtonsoft.Json;

namespace Producer.Domain
{
	public class UserStore : Entity
	{
		public string Email { get; set; }

		public UserRoles UserRole { get; set; }

		public string Token { get; set; }

		[JsonConverter (typeof (UnixDateTimeConverter))]
		public DateTime TokenTimestamp { get; set; }


		[JsonIgnore]
		public bool HasToken => !string.IsNullOrEmpty (Token);

		[JsonIgnore]
		public double TokenMinutes => HasToken ? TokenTimestamp.Subtract (DateTime.UtcNow).TotalMinutes : -1;


		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nUserStore\n");
			sb.Append ("  Id".PadRight (15));
			sb.Append ($"{Id}\n");
			sb.Append ("  ETag".PadRight (15));
			sb.Append ($"{ETag}\n");
			sb.Append ("  ResourceId".PadRight (15));
			sb.Append ($"{ResourceId}\n");
			sb.Append ("  SelfLink".PadRight (15));
			sb.Append ($"{SelfLink}\n");
			sb.Append ("  Timestamp".PadRight (15));
			sb.Append ($"{Timestamp}\n");
			sb.Append ("  Email".PadRight (15));
			sb.Append ($"{Email}\n");
			sb.Append ("  UserRole".PadRight (15));
			sb.Append ($"{UserRole}\n");
			sb.Append ("  Token".PadRight (15));
			sb.Append ($"{Token}\n");
			sb.Append ("  TokenTimestamp".PadRight (15));
			sb.Append ($"{TokenTimestamp}\n");
			sb.Append ("  TokenMinutes".PadRight (15));
			sb.Append ($"{TokenMinutes}\n");
			return sb.ToString ();
		}
	}
}
