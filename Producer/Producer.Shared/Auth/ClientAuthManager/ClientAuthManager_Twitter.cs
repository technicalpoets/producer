namespace Producer.Auth
{
	public partial class ClientAuthManager
	{
#if __IOS__

#if NC_AUTH_TWITTER

		void logoutAuthProviderTwitter () { }
#else

		void logoutAuthProviderTwitter () { }
#endif


#elif __ANDROID__

#if NC_AUTH_TWITTER

		void logoutAuthProviderTwitter () { }
#else

		void logoutAuthProviderTwitter () { }
#endif

#endif

	}
}
