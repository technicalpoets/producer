using Producer.Domain;

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

		public UserRoles UserRole { get; set; }

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nAuthUserConfig\n");
			sb.Append ("  Id".PadRight (13));
			sb.Append ($"{Id}\n");
			sb.Append ("  Name".PadRight (13));
			sb.Append ($"{Name}\n");
			sb.Append ("  GivenName".PadRight (13));
			sb.Append ($"{GivenName}\n");
			sb.Append ("  SurName".PadRight (13));
			sb.Append ($"{SurName}\n");
			sb.Append ("  Email".PadRight (13));
			sb.Append ($"{Email}\n");
			sb.Append ("  Picture".PadRight (13));
			sb.Append ($"{Picture}\n");
			sb.Append ("  Locale".PadRight (13));
			sb.Append ($"{Locale}\n");
			sb.Append ("  UserRole".PadRight (13));
			sb.Append ($"{UserRole}\n");
			return sb.ToString ();
		}

#if __MOBILE__

		static string serviceName (string prop) => $"authconfig.{prop}";

		public static AuthUserConfig FromKeychain (Keychain keychain)
		{
			var config = new AuthUserConfig
			{
				Id = keychain.GetItemFromKeychain (serviceName (nameof (Id))).PrivateKey,
				Name = keychain.GetItemFromKeychain (serviceName (nameof (Name))).PrivateKey,
				GivenName = keychain.GetItemFromKeychain (serviceName (nameof (GivenName))).PrivateKey,
				SurName = keychain.GetItemFromKeychain (serviceName (nameof (SurName))).PrivateKey,
				Email = keychain.GetItemFromKeychain (serviceName (nameof (Email))).PrivateKey,
				Picture = keychain.GetItemFromKeychain (serviceName (nameof (Picture))).PrivateKey,
				Locale = keychain.GetItemFromKeychain (serviceName (nameof (Locale))).PrivateKey,
				UserRole = UserRolesExtensions.FromClaim (keychain.GetItemFromKeychain (serviceName (nameof (UserRole))).PrivateKey)
			};

			return config;
		}

		public void SaveToKeychain (Keychain keychain)
		{
			if (!string.IsNullOrEmpty (Id)) keychain.SaveItemToKeychain (serviceName (nameof (Id)), "authconfig", Id);
			if (!string.IsNullOrEmpty (Name)) keychain.SaveItemToKeychain (serviceName (nameof (Name)), "authconfig", Name);
			if (!string.IsNullOrEmpty (GivenName)) keychain.SaveItemToKeychain (serviceName (nameof (GivenName)), "authconfig", GivenName);
			if (!string.IsNullOrEmpty (SurName)) keychain.SaveItemToKeychain (serviceName (nameof (SurName)), "authconfig", SurName);
			if (!string.IsNullOrEmpty (Email)) keychain.SaveItemToKeychain (serviceName (nameof (Email)), "authconfig", Email);
			if (!string.IsNullOrEmpty (Picture)) keychain.SaveItemToKeychain (serviceName (nameof (Picture)), "authconfig", Picture);
			if (!string.IsNullOrEmpty (Locale)) keychain.SaveItemToKeychain (serviceName (nameof (Locale)), "authconfig", Locale);
			keychain.SaveItemToKeychain (serviceName (nameof (UserRole)), "authconfig", UserRole.Claim ());
		}

		public static void RemoveFromKeychain (Keychain keychain)
		{
			keychain.RemoveItemFromKeychain (serviceName (nameof (Id)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (Name)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (GivenName)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (SurName)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (Email)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (Picture)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (Locale)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (UserRole)));
		}
#endif
	}


	public static class AuthUserConfigExtensions
	{
		public static AuthUserConfig GetAuthUserConfig (this GoogleAuthUser user, string sid, UserRoles role)
		{
			return new AuthUserConfig
			{
				Id = sid,
				Name = user.Name,
				GivenName = user.GivenName,
				SurName = user.SurName,
				Email = user.EmailAddress,
				Picture = user.Picture,
				Locale = user.Locale,
				UserRole = role
			};
		}
	}
}
