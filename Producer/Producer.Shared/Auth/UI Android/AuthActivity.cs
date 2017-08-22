//#define NC_AUTH_GOOGLE
//#define NC_AUTH_FACEBOOK
//#define NC_AUTH_MICROSOFT
//#define NC_AUTH_TWITTER

#if __ANDROID__

using Android.App;
using Android.Content;

using Android.OS;
using Android.Views;

using Android.Support.V4.App;

#if NC_AUTH_GOOGLE
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
#endif


namespace Producer.Auth
{
	[Activity (Label = "AuthActivity")]
	public class AuthActivity : FragmentActivity, View.IOnClickListener
#if NC_AUTH_GOOGLE
		, GoogleApiClient.IOnConnectionFailedListener//AppCompatActivity
#endif
	{
		const int RC_SIGN_IN = 9001;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (ClientAuthManager.Shared.AuthActivityLayoutResId);

			// use this format to help users set up the app
			//if (GetString (Resource.String.google_app_id) == "YOUR-APP-ID")
			//throw new System.Exception ("Invalid google-services.json file.  Make sure you've downloaded your own config file and added it to your app project with the 'GoogleServicesJson' build action.");
#if NC_AUTH_GOOGLE
			ClientAuthManager.Shared.InitializeAuthProviders (this);

			FindViewById<SignInButton> (ClientAuthManager.Shared.GoogleButtonResId).SetOnClickListener (this);
#endif
		}


		public void OnClick (View v)
		{
#if NC_AUTH_GOOGLE
			if (v.Id == ClientAuthManager.Shared.GoogleButtonResId)
			{
				signIn ();
			}
#endif
		}


		void signIn ()
		{
#if NC_AUTH_GOOGLE
			var signInIntent = ClientAuthManager.Shared.GetSignInIntent ();

			StartActivityForResult (signInIntent, RC_SIGN_IN);
#endif
		}


		protected override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (requestCode == RC_SIGN_IN)
			{
#if NC_AUTH_GOOGLE
				var result = Auth.GoogleSignInApi.GetSignInResultFromIntent (data);

				handleSignInResult (result);
#endif
			}
		}


#if NC_AUTH_GOOGLE
		void handleSignInResult (GoogleSignInResult result)
		{
			//Log.d (TAG, "handleSignInResult:" + result.isSuccess ());
			if (result.IsSuccess)
			{
				// Signed in successfully, show authenticated UI.
				GoogleSignInAccount user = result.SignInAccount;

				if (user != null)
				{
					Producer.Log.Debug ($"user.Account.Name: {user.Account.Name}");
					Producer.Log.Debug ($"acct.DisplayName: {user.DisplayName}");
					Producer.Log.Debug ($"acct.Email: {user.Email}");
					Producer.Log.Debug ($"acct.FamilyName: {user.FamilyName}");
					Producer.Log.Debug ($"acct.GivenName: {user.GivenName}");
					Producer.Log.Debug ($"acct.GrantedScopes: {string.Join (",", user.GrantedScopes)}");
					Producer.Log.Debug ($"acct.Id: {user.Id}");
					Producer.Log.Debug ($"acct.IdToken: {user.IdToken}");
					Producer.Log.Debug ($"acct.PhotoUrl: {user.PhotoUrl}");
					Producer.Log.Debug ($"acct.ServerAuthCode: {user.ServerAuthCode}");

					var details = new ClientAuthDetails
					{
						ClientAuthProvider = ClientAuthProviders.Google,
						Username = user.DisplayName,
						Email = user.Email,
						Token = user.IdToken,
						AuthCode = user.ServerAuthCode,
						AvatarUrl = user.PhotoUrl.ToString ()
					};

					ClientAuthManager.Shared.SetClientAuthDetails (details);

					Finish ();
				}
			}
			else
			{
				// Signed out, show unauthenticated UI.
				Producer.Log.Debug ($"Google SingIn failed with code:{result.Status}");
			}
		}


		public void OnConnectionFailed (ConnectionResult result)
		{
			Producer.Log.Debug ($"{result.ErrorMessage} code: {result.ErrorCode}");
		}
#endif
	}
}

#endif