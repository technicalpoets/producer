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
	}

	public static class AuthUserConfigExtensions
	{
		public static AuthUserConfig GetAuthUserConfig(this GoogleAuthUser user, string sid)
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
