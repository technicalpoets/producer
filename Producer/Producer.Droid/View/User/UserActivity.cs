using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Views;
using Android.Widget;
using Producer.Auth;

using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Producer.Droid
{
	[Activity (Label = "Profile", ParentActivity = typeof (MainActivity))]
	public class UserActivity : BaseActivity
#if NC_AUTH_GOOGLE
	//, GoogleApiClient.IOnConnectionFailedListener//AppCompatActivity
#endif
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.User);

			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);
			var toolbarTitle = toolbar.FindViewById<TextView> (Resource.Id.toolbar_title);

			SetSupportActionBar (toolbar);
			SupportActionBar.SetDisplayShowTitleEnabled (false); //we'll use a custom title
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);

			toolbarTitle.Text = this.Title;

#if NC_AUTH_GOOGLE
			//ClientAuthManager.Shared.InitializeAuthProviders (this);
#endif
		}


		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.menu_profile, menu);
			return base.OnCreateOptionsMenu (menu);
		}


		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_settings:
					StartActivity (typeof (SettingsActivity));
					return true;
				case Resource.Id.action_logout:
					ClientAuthManager.Shared.LogoutAuthProviders ();
					Finish ();
					return true;
			}

			return base.OnOptionsItemSelected (item);
		}


		public void OnConnectionFailed (ConnectionResult result)
		{
			Log.Debug ($"{result.ErrorMessage} code: {result.ErrorCode}");
		}
	}
}