#if __IOS__

using UIKit;
using Foundation;

#if NC_AUTH_FACEBOOK
using Facebook.CoreKit;
#endif

#elif __ANDROID__

using Android.Support.V4.App;
using Android.Content;

#if NC_AUTH_FACEBOOK
#endif

#endif
namespace Producer.Auth
{
	public partial class ClientAuthManager
	{
#if __IOS__

#if NC_AUTH_FACEBOOK

		void initializeAuthProviderFacebook (UIApplication application, NSDictionary launchOptions)
		{
			ApplicationDelegate.SharedInstance.FinishedLaunching (application, launchOptions);
		}

		bool openUrlFacebook (UIApplication app, NSUrl url, UIApplicationOpenUrlOptions openUrlOptions)
		{
			return ApplicationDelegate.SharedInstance.OpenUrl (app, url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);
		}

		void logoutAuthProviderFacebook () { }
#else
		void initializeAuthProviderFacebook (UIApplication application, NSDictionary launchOptions) { }

		bool openUrlFacebook (UIApplication app, NSUrl url, UIApplicationOpenUrlOptions openUrlOptions) => false;

		void logoutAuthProviderFacebook () { }
#endif


#elif __ANDROID__

#if NC_AUTH_FACEBOOK

		void logoutAuthProviderFacebook () { }
#else

		void logoutAuthProviderFacebook () { }
#endif

#endif

	}
}
