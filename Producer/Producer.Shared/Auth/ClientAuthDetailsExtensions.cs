using System;

namespace Producer.Auth
{
	public static class ClientAuthDetailsExtensions
	{
#if NC_AUTH_GOOGLE
#if __IOS__
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


#elif __ANDROID__
		public static ClientAuthDetails GetAuthDetails (this Android.Gms.Auth.Api.SignIn.GoogleSignInAccount user)
		{
			return new ClientAuthDetails
			{
				ClientAuthProvider = ClientAuthProviders.Google,
				Username = user.DisplayName,
				Email = user.Email,
				Token = user.IdToken,
				AuthCode = user.ServerAuthCode,
				AvatarUrl = user.PhotoUrl.ToString ()
			};
		}
#endif
#endif
	}
}
