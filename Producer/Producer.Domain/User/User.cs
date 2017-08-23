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