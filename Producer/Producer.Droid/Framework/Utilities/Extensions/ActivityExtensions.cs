using Android.App;
using Android.Gms.Common;
using Android.Widget;

namespace Producer.Droid
{
	public static class ActivityExtensions
	{
		const int PLAY_SERVICES_RESOLUTION_REQUEST = 9000;


		/// <summary>
		/// Check the device to make sure it has the Google Play Services APK. If it doesn't, display a dialog that 
		/// allows users to download the APK from the Google Play Store or enable it in the device's system settings.
		/// </summary>
		/// <returns><c>true</c>, if play services are available, <c>false</c> otherwise.</returns>
		/// <param name="context">Activity.</param>
		public static bool CheckPlayServicesAvailable (this Activity context)
		{
			int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable (context);

			if (resultCode != ConnectionResult.Success)
			{
				if (GoogleApiAvailability.Instance.IsUserResolvableError (resultCode))
				{
					GoogleApiAvailability.Instance.GetErrorDialog (context, resultCode, PLAY_SERVICES_RESOLUTION_REQUEST)
										 .Show ();
				}
				else
				{
					Toast.MakeText (context, "Google Play Services not available:\r\nThis device is not supported", ToastLength.Long)
						 .Show ();
				}

				return false;
			}

			return true;
		}
	}
}