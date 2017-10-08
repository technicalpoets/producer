using System;
using System.Linq;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using UserNotifications;

using WindowsAzure.Messaging;

using Producer.Auth;
using Producer.Domain;
using Producer.Shared;

namespace Producer.iOS
{
	[Register (nameof (AppDelegate))]
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

			ProducerClient.Shared.CurrentUserChanged += (sender, e) => RegisterForNotifications ();

			return true;
		}


		public override void OnResignActivation (UIApplication application)
		{
			Log.Debug (string.Empty);
			// Sent when the application is about to move from active to inactive state. 
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Use this method to pause ongoing tasks, disable timers, and invalidate graphics rendering callbacks. 
			// Games should use this method to pause the game.
		}


		// Called instead of WillTerminate when the user quits if application supports background execution.
		public override void DidEnterBackground (UIApplication application)
		{
			Log.Debug (string.Empty);

			// Use this method to release shared resources, save user data, invalidate timers, and store enough 
			// application state information to restore your application to its current state in case it is terminated later.
			// If your application supports background execution, this method is called instead of WillTerminate when the user quits.
		}


		public override void WillEnterForeground (UIApplication application)
		{
			Log.Debug (string.Empty);

			// Called as part of the transition from the background to the active state; 
			// here you can undo many of the changes made on entering the background.
		}


		public override void OnActivated (UIApplication application)
		{
			Log.Debug (string.Empty);

			InitializeContent ();
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}


		public override void WillTerminate (UIApplication application)
		{
			Log.Debug (string.Empty);
			// Called when the application is about to terminate. Save data if appropriate. See also DidEnterBackground.
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

			Log.Debug ($"Registering with Azure Notification Hub '{Settings.NotificationsName}' with Tags ({string.Join (ConstantStrings.Comma, tagArray)})");

			var tags = new NSSet (tagArray);

			notificationHub.RegisterNativeAsync (deviceToken, tags, err =>
			{
				if (err != null)
				{
					Log.Error ($"{err.Description}");
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
			Log.Debug (string.Join (ConstantStrings.Comma, ProducerClient.Shared.UserRole.GetTagArray ()));

			Log.Debug (error.LocalizedDescription);
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
			Log.Debug ($"\n{userInfo.ToString ()}");

			// if (userInfo.TryGetValue (new NSString ("collectionId"), out NSObject nsObj) && nsObj is NSString nsStr) { }

			if (processingNotification)
			{
				Log.Debug ($"Already processing notificaiton. Returning...");
				completionHandler (UIBackgroundFetchResult.NewData);
				return;
			}


			processingNotification = true;

			Log.Debug ($"Get All AvContent Async...");

			try
			{
				await ContentClient.Shared.GetAllAvContent ();

				DeleteLocalUploads ();

				Log.Debug ($"Finished Getting Data.");

				completionHandler (UIBackgroundFetchResult.NewData);
			}
			catch (Exception ex)
			{
				Log.Error ($"FAILED TO GET NEW DATA {ex.Message}");

				completionHandler (UIBackgroundFetchResult.Failed);
			}
			finally
			{
				processingNotification = false;
			}
		}


		void RegisterForNotifications ()
		{
			BeginInvokeOnMainThread (() => UNUserNotificationCenter.Current.RequestAuthorization (UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, (authorized, error) =>
			{
				BeginInvokeOnMainThread (() => UIApplication.SharedApplication.RegisterForRemoteNotifications ());
			}));
		}


		public void ShowConfigAlert (string alertTitle = "Configure App", string alertMessage = "Enter the \"Site Name\" used when deploying the to Azure:")
		{
			var alertController = UIAlertController.Create (alertTitle, alertMessage, UIAlertControllerStyle.Alert);

			alertController.AddTextField (textField =>
			{
				textField.Placeholder = "Site Name";
				textField.ReturnKeyType = UIReturnKeyType.Done;
			});

			alertController.AddAction (UIAlertAction.Create ("Done", UIAlertActionStyle.Default, obj =>
			{
				var text = alertController.TextFields?.FirstOrDefault ()?.Text;

				if (string.IsNullOrEmpty (text))
				{
					ShowConfigAlert ();
				}
				else
				{
					Settings.AzureSiteName = text;

					Task.Run (async () =>
					{
						if (await ProducerClient.Shared.UpdateAppSettings ())
						{
							InitializeContent ();
						}
					});
				}
			}));

			if (Window.RootViewController is UINavigationController root)
			{
				if (root.TopViewController is UIAlertController alert)
				{

				}
				else
				{
					root.PresentViewController (alertController, true, null);
				}
			}

			//Window.RootViewController.PresentViewController (alertController, true, null);

			//var root = Window.RootViewController;

			//if (root is UINavigationController container)
			//{
			//	container.TopViewController.PresentViewController (alertController, true, null);
			//}
			//else
			//{
			//	root.PresentViewController (alertController, true, null);
			//}
		}


		void InitializeContent ()
		{
			if (!Settings.EndpointConfigured)
			{
				ShowConfigAlert ();
			}
			else if (!ContentClient.Shared.Initialized)
			{
				Task.Run (async () =>
				{
					Log.Debug (ProducerClient.Shared.User?.ToString ());

					await AssetPersistenceManager.Shared.RestorePersistenceManagerAsync (ContentClient.Shared.AvContent [UserRoles.General]);

					await ContentClient.Shared.GetAllAvContent ();

					RegisterForNotifications ();
				});
			}
		}


		void DeleteLocalUploads ()
		{
			var locals = ContentClient.Shared.AvContent? [UserRoles.Producer].Where (avc => avc.HasLocalInboxPath);

			foreach (var asset in locals)
			{
				Log.Debug ($"Deleting local asset at: {asset.LocalInboxPath}");

				NSFileManager.DefaultManager.Remove (asset.LocalInboxPath, out NSError error);

				if (error == null)
				{
					asset.LocalInboxPath = null;
				}
				else
				{
					if (error.Code == 4) // not found
					{
						asset.LocalInboxPath = null;
					}

					Log.Error ($"{error}\n{error.Description}");
				}
			}
		}
	}
}