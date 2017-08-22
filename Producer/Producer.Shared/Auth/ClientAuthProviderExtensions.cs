using System;
using System.Collections.Generic;

#if __IOS__
using UIKit;
#elif __ANDROID__
using Android.Graphics;
#endif

namespace Producer.Auth
{
	public static class ClientAuthProviderExtensions
	{
#if __IOS__

		public static ClientAuthProvider FromTag (nint tag) => providers [(int)tag];

		static List<ClientAuthProvider> providers = new List<ClientAuthProvider>
		{
			new ClientAuthProvider
			{
				ButtonBackgroundColor = UIColor.White,
				ButtonTextColor = UIColor.FromWhiteAlpha(0, 0.54f),
				Icon = UIImage.FromBundle("nc_clientauth_i_google"),
				ProviderId = "foo",
				ShortName = "Google",
				SignInLabel = "Sign in with Google"
			},
			new ClientAuthProvider
			{
				ButtonBackgroundColor = UIColor.FromRGBA (59.0f / 255.0f, 89.0f / 255.0f, 152.0f / 255.0f, 1.0f),
				ButtonTextColor = UIColor.White,
				Icon = UIImage.FromBundle("nc_clientauth_i_facebook"),
				ProviderId = "foo",
				ShortName = "Facebook",
				SignInLabel = "Sign in with Facebook"
			},
			new ClientAuthProvider
			{
				ButtonBackgroundColor = UIColor.White,
				ButtonTextColor = UIColor.FromWhiteAlpha(0, 0.54f),
				Icon = UIImage.FromBundle("nc_clientauth_i_microsoft"),
				ProviderId = "foo",
				ShortName = "Microsoft",
				SignInLabel = "Sign in with Microsoft"
			},
			new ClientAuthProvider
			{
				ButtonBackgroundColor = UIColor.FromRGBA(71.0f / 255.0f, 154.0f / 255.0f, 234.0f / 255.0f, 1.0f),
				ButtonTextColor = UIColor.White,
				Icon = UIImage.FromBundle("nc_clientauth_i_twitter"),
				ProviderId = "foo",
				ShortName = "Twitter",
				SignInLabel = "Sign in with Twitter"
			}

#elif __ANDROID__

		public static ClientAuthProvider FromTag (int tag) => providers [tag];

		static List<ClientAuthProvider> providers = new List<ClientAuthProvider>
		{
			new ClientAuthProvider
			{
				ButtonBackgroundColor = Color.White,
				ButtonTextColor = Color.Gray,//Color.White.FromWhiteAlpha(0, 0.54f),
				//Icon = UIImage.FromBundle("nc_clientauth_i_google"),
				ProviderId = "foo",
				ShortName = "Google",
				SignInLabel = "Sign in with Google"
			},
			new ClientAuthProvider
			{
				ButtonBackgroundColor = Color.Rgb(59 / 255, 89 / 255, 152 / 255), // Color.FromRGBA (59.0f / 255.0f, 89.0f / 255.0f, 152.0f / 255.0f, 1.0f),
				ButtonTextColor = Color.White,
				//Icon = UIImage.FromBundle("nc_clientauth_i_facebook"),
				ProviderId = "foo",
				ShortName = "Facebook",
				SignInLabel = "Sign in with Facebook"
			},
			new ClientAuthProvider
			{
				ButtonBackgroundColor = Color.White,
				ButtonTextColor = Color.Gray, //Color.FromWhiteAlpha(0, 0.54f),
				//Icon = UIImage.FromBundle("nc_clientauth_i_microsoft"),
				ProviderId = "foo",
				ShortName = "Microsoft",
				SignInLabel = "Sign in with Microsoft"
			},
			new ClientAuthProvider
			{
				ButtonBackgroundColor = Color.Rgb(71 / 255, 154 / 255, 234 / 255),
				ButtonTextColor = Color.White,
				//Icon = UIImage.FromBundle("nc_clientauth_i_twitter"),
				ProviderId = "foo",
				ShortName = "Twitter",
				SignInLabel = "Sign in with Twitter"
			}
#endif
	};
	}
}
