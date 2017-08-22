using System;

namespace Producer.Auth
{
	public static class ClientAuthDetailsExtensions
	{
#if NC_AUTH_GOOGLE

		public static ClientAuthDetails GetAuthDetails (this Google.SignIn.GoogleUser user, int avatarSize = 24)
		{
			return new ClientAuthDetails
			{
				ClientAuthProvider = ClientAuthProviders.Google,
				Name = user?.Profile?.Name,
				GivenName = user?.Profile?.GivenName,
				FamilyName = user?.Profile?.FamilyName,
				Username = user?.Profile?.Name,
				Email = user?.Profile?.Email,
				Token = user?.Authentication?.IdToken,
				AuthCode = user?.ServerAuthCode,
				AvatarUrl = user?.Profile.GetImageUrl ((nuint) avatarSize * (nuint) UIKit.UIScreen.MainScreen.Scale)?.ToString ()
			};
		}

#endif
	}
}
