namespace Producer.Domain
{
	public enum UserRoles
	{
		General,
		Insider,
		Producer,
		Admin
	}

	public static class UserRolesExtensions
	{
		public static string Claim (this UserRoles role) => role.ToString ().ToLower ();
	}
}