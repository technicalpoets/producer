using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;

namespace Producer.Droid
{
	[Activity]
	public abstract class BaseActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			Window.AddFlags (WindowManagerFlags.DrawsSystemBarBackgrounds);

			base.OnCreate (savedInstanceState);
		}


		protected override void OnStart ()
		{
			base.OnStart ();

			//HockeyApp.Android.Tracking.StartUsage (this);
		}


		protected override void OnStop ()
		{
			base.OnStop ();

			//HockeyApp.Android.Tracking.StopUsage (this);
		}
	}
}