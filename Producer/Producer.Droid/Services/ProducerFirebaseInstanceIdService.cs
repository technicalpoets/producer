using Android.App;
using Android.Content;
using Firebase.Iid;

namespace Producer.Droid.Services
{
	[Service]
	[IntentFilter (new [] { "com.google.firebase.INSTANCE_ID_EVENT" })]
	public class ProducerFirebaseInstanceIdService : FirebaseInstanceIdService
	{
		public override void OnTokenRefresh ()
		{
			var refreshedToken = FirebaseInstanceId.Instance.Token;
			Log.Debug ("Refreshed token: " + refreshedToken);

			StartService (new Intent (this, typeof (RegistrationIntentService)));
		}
	}
}