using System.Collections.Generic;
namespace Producer.Domain
{
	public static class TagExtensions
	{
		const string userrole = "userrole";

		public static string GetExpressionString (this UserRoles role)
		{
			var sb = new System.Text.StringBuilder ($"({userrole}:0");

			if (role != UserRoles.General) // if it's not General
			{
				sb.Append ($" || {userrole}:1"); // it's at least Insider

				if (role != UserRoles.Insider) // if it's not Insider
				{
					sb.Append ($" || {userrole}:2"); // it's at least Producer

					if (role != UserRoles.Producer) // if it's not Producer
					{
						sb.Append ($" || {userrole}:3"); // must be Admin
					}
				}
			}

			sb.Append (")");

			return sb.ToString ();
		}

		public static string [] GetTagArray (this UserRoles role)
		{
			List<string> strings = new List<string> { $"{userrole}:0" };

			if (role != UserRoles.General) // if it's not General
			{
				strings.Add ($"{userrole}:1"); // it's at least Insider

				if (role != UserRoles.Insider) // if it's not Insider
				{
					strings.Add ($"{userrole}:2"); // it's at least Producer

					if (role != UserRoles.Producer) // if it's not Producer
					{
						strings.Add ($"{userrole}:3"); // must be Admin
					}
				}
			}

			return strings.ToArray ();
		}
	}
}
