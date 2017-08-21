using Android.App;
using Android.Widget;
using Android.OS;

namespace Producer.Droid
{
	[Activity (Label = "Producer", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Bootstrap.Run ();

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
		}
	}
}

