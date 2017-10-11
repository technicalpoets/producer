using System;
using System.Linq;
using Android.App;
using Android.OS;
using Firebase.Messaging;
using Producer.Domain;
using Producer.Shared;

namespace Producer.Droid.Services
{
	[Service]
	[IntentFilter (new [] { "com.google.firebase.MESSAGING_EVENT" })]
	public class ProducerFirebaseMessagingService : FirebaseMessagingService
	{
		bool processingNotification;

		public async override void OnMessageReceived (RemoteMessage message)
		{
			if (processingNotification)
			{
				Log.Debug ($"Already processing notification. Returning...");
				return;
			}

			processingNotification = true;
			Log.Debug ($"Processing new RemoteMessage :: sender: {message.From}");

			try
			{
				Log.Debug ($"Get All AvContent Async...");

				//refresh content - this will generate an AvContentChanged event the UI will pick up
				await ContentClient.Shared.GetAllAvContent ();

				DeleteLocalUploads ();

				Log.Debug ($"Finished Getting AvContent Data.");
			}
			catch (Exception ex)
			{
				Log.Error ($"FAILED TO GET NEW DATA :: {ex.Message}");
			}
			finally
			{
				processingNotification = false;
			}

			var notification = message.GetNotification ();

			if (notification != null)
			{
				//Log.Debug ("Sending ContentPublished notification");

				//if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
				//{
				//	this.GetChannelNotification (NotificationChannels.ContentPublished.Id, notification.Title, notification.Body)
				//		.SetLaunchActivity (typeof (MainActivity))
				//		.Send ();
				//}
				//else
				//{
				//	this.GetNotification (notification.Title, notification.Body)
				//		.SetLaunchActivity (typeof (MainActivity))
				//		.Send ();
				//}
			}
		}


		void DeleteLocalUploads ()
		{
			var locals = ContentClient.Shared.AvContent? [UserRoles.Producer].Where (avc => avc.HasLocalInboxPath);

			foreach (var asset in locals)
			{
				Log.Debug ($"Deleting local asset at: {asset.LocalInboxPath}");

				//NSFileManager.DefaultManager.Remove (asset.LocalInboxPath, out NSError error);

				//if (error == null)
				//{
				//	asset.LocalInboxPath = null;
				//}
				//else
				//{
				//	if (error.Code == 4) // not found
				//	{
				//		asset.LocalInboxPath = null;
				//	}

				//	Log.Error ($"{error}\n{error.Description}");
				//}
			}
		}
	}
}