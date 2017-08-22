namespace Producer.Auth
{
	public class AuthUserConfig
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string GivenName { get; set; }

		public string SurName { get; set; }

		public string Email { get; set; }

		public string Picture { get; set; }

		public string Locale { get; set; }

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\nAuthUserConfig\n");
			sb.Append ("  Id".PadRight (10));
			sb.Append ($"{Id}\n");
			sb.Append ("  Name".PadRight (10));
			sb.Append ($"{Name}\n");
			sb.Append ("  GivenName".PadRight (10));
			sb.Append ($"{GivenName}\n");
			sb.Append ("  SurName".PadRight (10));
			sb.Append ($"{SurName}\n");
			sb.Append ("  Email".PadRight (10));
			sb.Append ($"{Email}\n");
			sb.Append ("  Picture".PadRight (10));
			sb.Append ($"{Picture}\n");
			sb.Append ("  Locale".PadRight (10));
			sb.Append ($"{Locale}\n");
			return sb.ToString ();
		}
	}

	public static class AuthUserConfigExtensions
	{
		public static AuthUserConfig GetAuthUserConfig (this GoogleAuthUser user, string sid)
		{
			return new AuthUserConfig
			{
				Id = sid,
				Name = user.Name,
				GivenName = user.GivenName,
				SurName = user.SurName,
				Email = user.EmailAddress,
				Picture = user.Picture,
				Locale = user.Locale
			};
		}
	}
}
