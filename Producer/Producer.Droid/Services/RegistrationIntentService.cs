using System;
using Android.App;
using Android.Content;
using Firebase.Iid;
using Producer.Domain;
using Producer.Shared;
using WindowsAzure.Messaging;

namespace Producer.Droid.Services
{
	[Service]
	public class RegistrationIntentService : IntentService
	{
		NotificationHub hub;

		protected override void OnHandleIntent (Intent intent)
		{
			try
			{
				//can't do any of this unless we've got a valid hub configuration
				if (!Settings.NotificationsConfigured)
				{
					return;
				}

				var fcmToken = FirebaseInstanceId.Instance.Token;
				Log.Debug ($"FCM Registration Token: {fcmToken}");

				// Storing the registration id that indicates whether the generated token has been
				// sent to your server. If it is not stored, send the token to your server,
				// otherwise your server should have already received the token.

				var regId = Settings.RegistrationId;

				if (string.IsNullOrEmpty (regId))
				{
					Log.Debug ($"Attempting a new registration with NH using FCM token : {fcmToken}");

					registerWithHub (fcmToken);
				}

				// Check if the token may have been compromised and needs refreshing.
				else if (Settings.FcmToken != fcmToken)
				{
					Log.Debug ($"NH Registration refreshing with token : {fcmToken}");

					registerWithHub (fcmToken);
				}

				else //same token and we have a registration, we'll see if we need to update it
				{
					Log.Debug ($"Previously Registered Successfully - RegId : {regId}");

					registerWithHub (fcmToken, true);
				}
			}
			catch (Exception e)
			{
				Log.Error (e);
				// If an exception happens while fetching the new token or updating our registration data
				// on a third-party server, this ensures that we'll attempt the update at a later time.
			}
		}


		void registerWithHub (string fcmToken, bool checkTags = false)
		{
			var tagArray = ProducerClient.Shared.UserRole.GetTagArray ();
			var tags = string.Join (ConstantStrings.Comma, tagArray);
			var savedTags = Settings.NotificationTags;

			if (!checkTags || checkTags && (string.IsNullOrEmpty (savedTags) || savedTags != tags))
			{
				hub = new NotificationHub (Settings.NotificationsName, Settings.NotificationsConnectionString, this);

				Log.Debug ($"Registering with Azure Notification Hub '{Settings.NotificationsName}' with Tags ({tags})");

				// If you want to use tags...
				// Refer to : https://azure.microsoft.com/en-us/documentation/articles/notification-hubs-routing-tag-expressions/
				var regID = hub.Register (fcmToken, tags).RegistrationId;

				//var regID = hub.RegisterTemplate (fcmToken, nameof (PushTemplate), PushTemplate.Android, tags).RegistrationId;

				Settings.RegistrationId = regID;
				Settings.FcmToken = fcmToken;
				Settings.NotificationTags = tags;

				Log.Debug ($"Successfully Registered for Notifications (RegId: {regID} :: (device token: {fcmToken})");
			}
		}
	}
}