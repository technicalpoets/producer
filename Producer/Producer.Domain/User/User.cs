using Producer.Auth;

namespace Producer.Domain
{
	public class User
	{
		public string Id { get; set; }

		public string Alias { get; set; }

		public string Email { get; set; }

		public string Username { get; set; }

		public string Name { get; set; }

		public string GivenName { get; set; }

		public string FamilyName { get; set; }

		public string AvatarUrl { get; set; }

		public UserRoles UserRole { get; set; }

		public string Locale { get; set; }

		public string Token { get; set; }

		public string AuthCode { get; set; }

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\nUser\n");
			sb.Append ("  Id".PadRight(13));
			sb.Append ($"{Id}\n");
			sb.Append ("  Alias".PadRight(13));
			sb.Append ($"{Alias}\n");
			sb.Append ("  Email".PadRight(13));
			sb.Append ($"{Email}\n");
			sb.Append ("  Username".PadRight(13));
			sb.Append ($"{Username}\n");
			sb.Append ("  Name".PadRight(13));
			sb.Append ($"{Name}\n");
			sb.Append ("  GivenName".PadRight(13));
			sb.Append ($"{GivenName}\n");
			sb.Append ("  FamilyName".PadRight(13));
			sb.Append ($"{FamilyName}\n");
			sb.Append ("  AvatarUrl".PadRight(13));
			sb.Append ($"{AvatarUrl}\n");
			sb.Append ("  UserRole".PadRight(13));
			sb.Append ($"{UserRole}\n");
			sb.Append ("  Locale".PadRight(13));
			sb.Append ($"{Locale}\n");
			sb.Append ("  Token".PadRight(13));
			sb.Append ($"{Token}\n");
			sb.Append ("  AuthCode".PadRight(13));
			sb.Append ($"{AuthCode}\n");
			return sb.ToString ();
		}


#if __MOBILE__
		public User (ClientAuthDetails providerDetails, AuthUserConfig serverDetails)
		{
			Id = serverDetails.Id;
			Alias = providerDetails.Email ?? serverDetails.Email;
			Email = providerDetails.Email ?? serverDetails.Email;
			Username = providerDetails.Email ?? serverDetails.Email;
			Name = providerDetails.Name;
			GivenName = providerDetails.GivenName ?? serverDetails.GivenName;
			FamilyName = providerDetails.FamilyName ?? serverDetails.SurName;
			AvatarUrl = providerDetails.AvatarUrl ?? serverDetails.Picture;
			Locale = serverDetails.Locale;
			Token = providerDetails.Token;
			AuthCode = providerDetails.AuthCode;
		}
#endif
	}
}