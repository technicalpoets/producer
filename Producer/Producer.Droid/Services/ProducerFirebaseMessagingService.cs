using System;
using System.Linq;
using Android.App;
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
			//Log.Debug ($"From: {message.From}");
			//Log.Debug ($"Notification Message Body: {message.GetNotification ().Body}");


			// if (userInfo.TryGetValue (new NSString ("collectionId"), out NSObject nsObj) && nsObj is NSString nsStr) { }

			if (processingNotification)
			{
				Log.Debug ($"Already processing notificaiton. Returning...");
				return;
			}

			processingNotification = true;

			Log.Debug ($"Get All AvContent Async...");

			try
			{
				//refresh content - this will generate an AvContentChanged event the UI will pick up
				await ContentClient.Shared.GetAllAvContent ();

				DeleteLocalUploads ();

				Log.Debug ($"Finished Getting Data.");
			}
			catch (Exception ex)
			{
				Log.Error ($"FAILED TO GET NEW DATA :: {ex.Message}");
			}
			finally
			{
				processingNotification = false;
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