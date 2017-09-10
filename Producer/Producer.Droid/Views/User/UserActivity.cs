
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbar = Android.Support.V7.Widget.Toolbar;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Producer.Auth;

namespace Producer.Droid.Views.User
{
	[Activity (Label = "UserActivity")]
	public class UserActivity : BaseActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.User);
			// Create your application here
			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);
			SetSupportActionBar (toolbar);

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
	}
}
