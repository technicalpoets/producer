namespace Producer.Auth
{
	public partial class ClientAuthManager
	{
#if __IOS__

#if NC_AUTH_MICROSOFT

		void logoutAuthProviderMicrosoft () { }
#else

		void logoutAuthProviderMicrosoft () { }
#endif


#elif __ANDROID__

#if NC_AUTH_MICROSOFT

		void logoutAuthProviderMicrosoft () { }
#else

		void logoutAuthProviderMicrosoft () { }
#endif

#endif

	}
}
