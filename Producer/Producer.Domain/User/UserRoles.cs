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

		public static UserRoles FromClaim (string claim)
		{
			if (!string.IsNullOrEmpty (claim))
			{
				if (claim == UserRoles.Admin.Claim ())
				{
					return UserRoles.Admin;
				}
				if (claim == UserRoles.Producer.Claim ())
				{
					return UserRoles.Producer;
				}
				if (claim == UserRoles.Insider.Claim ())
				{
					return UserRoles.Insider;
				}
			}

			return UserRoles.General;
		}
	}
}