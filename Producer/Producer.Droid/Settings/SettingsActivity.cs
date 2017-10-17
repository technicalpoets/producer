using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Producer.Droid
{
	[Activity (Label = "Settings", ParentActivity = typeof (UserActivity))]
	public class SettingsActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Settings);

			//config toolbar
			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);
			var title = toolbar.FindViewById<TextView> (Resource.Id.toolbar_title);
			//toolbar.SetNavigationIcon (Resource.Drawable.ic_tab_profile);
			title.Text = "Settings";

			//Toolbar will now take on default Action Bar characteristics
			SetSupportActionBar (toolbar);
			SupportActionBar.SetDisplayShowTitleEnabled (false); //we'll use a custom title
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
		}
	}
}