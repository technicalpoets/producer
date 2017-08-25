namespace Producer.Domain
{
	public class UserStore
	{
		public string Id { get; set; }

		public string Email { get; set; }

		public UserRoles UserRole { get; set; }

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nAuthUserConfig\n");
			sb.Append ("  Id".PadRight (13));
			sb.Append ($"{Id}\n");
			sb.Append ("  Email".PadRight (13));
			sb.Append ($"{Email}\n");
			sb.Append ("  UserRole".PadRight (13));
			sb.Append ($"{UserRole}\n");
			return sb.ToString ();
		}

	}
}
