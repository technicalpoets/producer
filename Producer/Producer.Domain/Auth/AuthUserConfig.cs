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
			return sb.ToString ();
		}

#if __MOBILE__

		static string serviceName (string prop) => $"authconfig.{prop}";

		public static AuthUserConfig FromKeychain ()
		{
			var keychain = new Keychain ();

			var config = new AuthUserConfig ();

			config.Id = keychain.GetItemFromKeychain (serviceName (nameof (Id))).PrivateKey;
			config.Name = keychain.GetItemFromKeychain (serviceName (nameof (Name))).PrivateKey;
			config.GivenName = keychain.GetItemFromKeychain (serviceName (nameof (GivenName))).PrivateKey;
			config.SurName = keychain.GetItemFromKeychain (serviceName (nameof (SurName))).PrivateKey;
			config.Email = keychain.GetItemFromKeychain (serviceName (nameof (Email))).PrivateKey;
			config.Picture = keychain.GetItemFromKeychain (serviceName (nameof (Picture))).PrivateKey;
			config.Locale = keychain.GetItemFromKeychain (serviceName (nameof (Locale))).PrivateKey;

			return config;
		}

		public void SaveToKeychain ()
		{
			var keychain = new Keychain ();

			if (!string.IsNullOrEmpty (Id)) keychain.SaveItemToKeychain (serviceName (nameof (Id)), "authconfig", Id);
			if (!string.IsNullOrEmpty (Name)) keychain.SaveItemToKeychain (serviceName (nameof (Name)), "authconfig", Name);
			if (!string.IsNullOrEmpty (GivenName)) keychain.SaveItemToKeychain (serviceName (nameof (GivenName)), "authconfig", GivenName);
			if (!string.IsNullOrEmpty (SurName)) keychain.SaveItemToKeychain (serviceName (nameof (SurName)), "authconfig", SurName);
			if (!string.IsNullOrEmpty (Email)) keychain.SaveItemToKeychain (serviceName (nameof (Email)), "authconfig", Email);
			if (!string.IsNullOrEmpty (Picture)) keychain.SaveItemToKeychain (serviceName (nameof (Picture)), "authconfig", Picture);
			if (!string.IsNullOrEmpty (Locale)) keychain.SaveItemToKeychain (serviceName (nameof (Locale)), "authconfig", Locale);
		}

		public static void RemoveFromKeychain ()
		{
			var keychain = new Keychain ();

			keychain.RemoveItemFromKeychain (serviceName (nameof (Id)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (Name)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (GivenName)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (SurName)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (Email)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (Picture)));
			keychain.RemoveItemFromKeychain (serviceName (nameof (Locale)));
		}
#endif
	}


	public static class AuthUserConfigExtensions
	{
		public static AuthUserConfig GetAuthUserConfig (this GoogleAuthUser user, string sid)
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
