namespace Producer.Domain
{
	public class User
	{
		public string Alias { get; set; }

		public string Email { get; set; }

		public string Username { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string AvatarUrl { get; set; }

		public UserRoles UserRole { get; set; }
	}
}