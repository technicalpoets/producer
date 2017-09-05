using System;
using System.Text;

using Foundation;
using UIKit;
using UserNotifications;

using WindowsAzure.Messaging;

using Producer.Domain;
using Producer.Shared;
using Producer.Auth;

namespace Producer.iOS
{
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate, IUNUserNotificationCenterDelegate
	{

		static bool processingNotification;


		public override UIWindow Window { get; set; }


		public AppDelegate ()
		{
			Bootstrap.Run ();
		}


		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			AssetPersistenceManager.Shared.Setup ();

			// must assign delegate before app finishes launching.
			UNUserNotificationCenter.Current.Delegate = this;

			ClientAuthManager.Shared.InitializeAuthProviders (application, launchOptions);

			return true;
		}


		public override bool OpenUrl (UIApplication app, NSUrl url, NSDictionary options)
		{
			if (!ClientAuthManager.Shared.OpenUrl (app, url, options))
			{
				var contentNc = app.KeyWindow.RootViewController as ContentNc;

				var openUrl = contentNc?.SetupComposeVc (url) ?? false;

				return openUrl;
			}

			return true;
		}


		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			var tagArray = ProducerClient.Shared.UserRole.GetTagArray ();

			var notificationHub = new SBNotificationHub (Settings.NotificationsConnectionString, Settings.NotificationsName);

			Log.Debug ($"Registering with Azure Notification Hub {Settings.NotificationsName} with Tags [{string.Join (ConstantStrings.Comma, tagArray)}]");

			var tags = new NSSet (tagArray);

			notificationHub.RegisterNativeAsync (deviceToken, tags, err =>
			{
				if (err != null)
				{
					Log.Debug ($"Error: {err.Description}");
				}
				else
				{
					var token = deviceToken.ToString ().Replace (" ", string.Empty).Trim ('<', '>');

					Log.Debug ($"Successfully Registered for Notifications. (device token: {token})");
				}
			});
		}


		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			Log.Debug ($"FailedToRegisterForRemoteNotifications {error}");
		}


		// For a push notification to trigger a download operation, the notification’s payload must include
		// the content-available key with its value set to 1. When that key is present, the system wakes
		// the app in the background (or launches it into the background) and calls the app delegate’s 
		// application:didReceiveRemoteNotification:fetchCompletionHandler: method. Your implementation 
		// of that method should download the relevant content and integrate it into your app.
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
		public override async void DidReceiveRemoteNotification (UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
		{
#if DEBUG
			Log.Debug ($"\npayload:\n{userInfo.ToString ()}");

			var sb = new StringBuilder ("\nuserInfo:\n");

			for (int i = 0; i < userInfo.Keys.Length; i++)
			{
				sb.AppendLine ($"{userInfo.Keys [i]} : {userInfo.Values [i]}");
			}

			Log.Debug (sb.ToString ());
#endif

			if (userInfo.TryGetValue (new NSString ("collectionId"), out NSObject nsObj) && nsObj is NSString nsStr)
			{
				Log.Debug ($"collectionId = {nsStr}");
			}

			if (processingNotification)
			{
				Log.Debug ($"DidReceiveRemoteNotification: Already processing notificaiton. Returning");
				completionHandler (UIBackgroundFetchResult.NewData);
				return;
			}


			processingNotification = true;

			Log.Debug ($"DidReceiveRemoteNotification: Get All AvContent Async...");

			try
			{
				await ContentClient.Shared.GetAllAvContent ();

				Log.Debug ($"DidReceiveRemoteNotification: Finished Getting Data.");

				completionHandler (UIBackgroundFetchResult.NewData);
			}
			catch (Exception ex)
			{
				Log.Debug ($"DidReceiveRemoteNotification: ERROR: FAILED TO GET NEW DATA {ex.Message}");

				completionHandler (UIBackgroundFetchResult.Failed);
			}
			finally
			{
				processingNotification = false;
			}
		}
	}
}