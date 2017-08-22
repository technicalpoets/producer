#if __IOS__

using UIKit;
using Foundation;

#if NC_AUTH_GOOGLE
using Google.SignIn;
#endif

#elif __ANDROID__

using Android.Support.V4.App;
using Android.Content;

#if NC_AUTH_GOOGLE
using GoogleAuth = Android.Gms.Auth.Api.Auth;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
#endif

#endif

namespace Producer.Auth
{
	public partial class ClientAuthManager
	{
#if __IOS__

#if NC_AUTH_GOOGLE

		void initializeAuthProviderGoogle ()
		{
			var googleServiceDictionary = NSDictionary.FromFile ("GoogleService-Info.plist");

			if (googleServiceDictionary == null)
			{
				throw new System.IO.FileNotFoundException ("Must be present to use Google Auth", "GoogleService-Info.plist");
			}

			SignIn.SharedInstance.ClientID = googleServiceDictionary ["CLIENT_ID"].ToString ();
			SignIn.SharedInstance.ServerClientID = googleServiceDictionary ["SERVER_CLIENT_ID"].ToString ();
		}

		bool openUrlGoogle (UIApplication app, NSUrl url, UIApplicationOpenUrlOptions openUrlOptions)
		{
			return SignIn.SharedInstance.HandleUrl (url, openUrlOptions.SourceApplication, openUrlOptions.Annotation);
		}

		void logoutAuthProviderGoogle ()
		{
			SignIn.SharedInstance.SignOutUser ();
		}

#else
		void initializeAuthProviderGoogle () { }

		bool openUrlGoogle (UIApplication app, NSUrl url, UIApplicationOpenUrlOptions openUrlOptions) => false;

		void logoutAuthProviderGoogle () { }
#endif


#elif __ANDROID__

#if NC_AUTH_GOOGLE

		GoogleApiClient googleApiClient;

		public Intent GetSignInIntent () => GoogleAuth.GoogleSignInApi.GetSignInIntent (googleApiClient);

		public int GoogleButtonResId { get; set; }

		public int GoogleWebClientResId { get; set; }

		void initializeAuthProviderGoogle<T> (T context)
			where T : FragmentActivity, GoogleApiClient.IOnConnectionFailedListener
		{
			var webClientId = context.GetString (GoogleWebClientResId);

			GoogleSignInOptions gso = new GoogleSignInOptions.Builder (GoogleSignInOptions.DefaultSignIn)
															 .RequestEmail ()
															 .RequestIdToken (webClientId)
															 .RequestServerAuthCode (webClientId)
															 .Build ();

			googleApiClient = new GoogleApiClient.Builder (context)
												 .EnableAutoManage (context, context)
												 .AddApi (GoogleAuth.GOOGLE_SIGN_IN_API, gso)
												 .Build ();
		}

		void logoutAuthProviderGoogle () { }
#else
		void initializeAuthProviderGoogle<T> (T context) where T : FragmentActivity { }

		void logoutAuthProviderGoogle () { }
#endif

#endif
	}
}
