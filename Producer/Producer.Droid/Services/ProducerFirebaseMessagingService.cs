using Android.App;
using Firebase.Messaging;

namespace Producer.Droid.Services
{
	[Service]
	[IntentFilter (new [] { "com.google.firebase.MESSAGING_EVENT" })]
	public class ProducerFirebaseMessagingService : FirebaseMessagingService
	{
		public override void OnMessageReceived (RemoteMessage message)
		{
			Log.Debug ($"From: {message.From}");
			Log.Debug ($"Notification Message Body: {message.GetNotification ().Body}");
		}
	}
}