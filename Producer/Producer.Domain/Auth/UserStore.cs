namespace Producer.Domain
{
	public class UserStore : Entity
	{
		public string Email { get; set; }

		public UserRoles UserRole { get; set; }

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nUserStore\n");
			sb.Append ("  Id".PadRight (13));
			sb.Append ($"{Id}\n");
			sb.Append ("  ETag".PadRight (13));
			sb.Append ($"{ETag}\n");
			sb.Append ("  ResourceId".PadRight (13));
			sb.Append ($"{ResourceId}\n");
			sb.Append ("  SelfLink".PadRight (13));
			sb.Append ($"{SelfLink}\n");
			sb.Append ("  Timestamp".PadRight (13));
			sb.Append ($"{Timestamp}\n");
			sb.Append ("  Email".PadRight (13));
			sb.Append ($"{Email}\n");
			sb.Append ("  UserRole".PadRight (13));
			sb.Append ($"{UserRole}\n");
			return sb.ToString ();
		}
	}
}
