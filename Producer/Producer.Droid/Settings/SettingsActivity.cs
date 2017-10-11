using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace Producer.Droid
{
	[Activity (Label = "Settings", ParentActivity = typeof (MainActivity))]
	public class SettingsActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Settings);

			FragmentManager.BeginTransaction ()
						   .Add (Resource.Id.fragment_container, new SettingsFragment ())
						   .Commit ();
		}
	}
}