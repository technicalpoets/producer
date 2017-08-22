namespace Producer.Auth
{
	public class ClientAuthDetails
	{
		public ClientAuthProviders ClientAuthProvider { get; set; }

		public string Name { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }

		public string Token { get; set; }

		public string AuthCode { get; set; }

		public string AvatarUrl { get; set; }
	}
}
