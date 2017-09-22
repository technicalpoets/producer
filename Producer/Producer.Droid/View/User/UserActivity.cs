using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Views;

using Producer.Auth;

using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Producer.Droid
{
	[Activity (Label = "UserActivity")]
	public class UserActivity : BaseActivity
#if NC_AUTH_GOOGLE
		//, GoogleApiClient.IOnConnectionFailedListener//AppCompatActivity
#endif
	{


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.User);
			// Create your application here

			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);
			SetSupportActionBar (toolbar);
#if NC_AUTH_GOOGLE
			//ClientAuthManager.Shared.InitializeAuthProviders (this);
#endif
		}


		public override bool OnCreateOptionsMenu (Android.Views.IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.menu_logout, menu);
			return base.OnCreateOptionsMenu (menu);
		}


		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_logout:
					ClientAuthManager.Shared.LogoutAuthProviders();
					Finish ();
					break;
			}
			return base.OnOptionsItemSelected (item);
		}


		public void OnConnectionFailed (ConnectionResult result)
		{
			Log.Debug ($"{result.ErrorMessage} code: {result.ErrorCode}");
		}
	}
}
