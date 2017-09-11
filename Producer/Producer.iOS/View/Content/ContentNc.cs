using System;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using UserNotifications;

using Producer.Auth;
using Producer.Domain;
using Producer.Shared;

namespace Producer.iOS
{
	public partial class ContentNc : UINavigationController
	{

		public ContentNc (IntPtr handle) : base (handle) { }


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.AddStatusBarView (Colors.ThemeDark);

			ClientAuthManager.Shared.AthorizationChanged += handleClientAuthChanged;

			if (Settings.FunctionsUrl != null && Settings.DocumentDbUrl != null)
			{
				Task.Run (async () =>
				{
					Log.Debug (ProducerClient.Shared.User?.ToString ());

					await ContentClient.Shared.GetAllAvContent ();

					await AssetPersistenceManager.Shared.RestorePersistenceManagerAsync (ContentClient.Shared.AvContent [UserRoles.General]);

					BeginInvokeOnMainThread (() => UNUserNotificationCenter.Current.RequestAuthorization (UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, authorizationRequestHandler));
				});
			}
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (Settings.FunctionsUrl == null || Settings.DocumentDbUrl == null)
			{
				showSettinsAlert ();
			}
		}


		void authorizationRequestHandler (bool authorized, NSError error)
		{
			if (authorized)
			{
				Log.Debug ($"RegisterForRemoteNotifications");

				BeginInvokeOnMainThread (() => UIApplication.SharedApplication.RegisterForRemoteNotifications ());
			}
			else
			{
				Log.Debug ($"{error}");
			}
		}


		void handleClientAuthChanged (object s, ClientAuthDetails e)
		{
			Log.Debug ($"Authenticated: {e}");

			Task.Run (async () =>
			{
				if (e == null)
				{
					ProducerClient.Shared.ResetUser ();
				}
				else
				{
					await ProducerClient.Shared.AuthenticateUser (e.Token, e.AuthCode);
				}

				BeginInvokeOnMainThread (() => UNUserNotificationCenter.Current.RequestAuthorization (UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, authorizationRequestHandler));

				await ContentClient.Shared.GetAllAvContent ();
			});
		}


		public bool SetupComposeVc (NSUrl url)
		{
			// TODO: Display some type of message for non-producers or figure out how to kill the document type registration

			var canCompose = ProducerClient.Shared.UserRole.CanWrite () && (url?.IsFileUrl ?? false);

			if (canCompose)
			{
				var composeVc = TopViewController as ComposeVc;

				if (composeVc == null)
				{
					var produceTvc = TopViewController as ProduceTvc;

					if (produceTvc == null && TopViewController is ContentTvc contentTvc)
					{
						produceTvc = Storyboard.Instantiate<ProduceTvc> ();

						contentTvc.ShowViewController (produceTvc, contentTvc);
					}

					composeVc = Storyboard.Instantiate<ComposeVc> ();

					produceTvc.ShowViewController (composeVc, produceTvc);
				}

				composeVc.SetData (url);
			}

			return canCompose;
		}


		void showSettinsAlert ()
		{
			var alertController = UIAlertController.Create ("Configure App", "You must add your Azure information to Settings before using the app.", UIAlertControllerStyle.Alert);

			alertController.AddAction (UIAlertAction.Create ("Open Settings", UIAlertActionStyle.Default, handleAlertControllerActionOpenSettings));

			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, handleAlertControllerActionDismiss));

			PresentViewController (alertController, true, null);
		}


		void handleAlertControllerActionDismiss (UIAlertAction obj) => DismissViewController (true, null);


		void handleAlertControllerActionOpenSettings (UIAlertAction obj)
		{
			var settingsString = UIApplication.OpenSettingsUrlString;

			if (!string.IsNullOrEmpty (settingsString))
			{
				var settingsUrl = NSUrl.FromString (settingsString);

				UIApplication.SharedApplication.OpenUrl (settingsUrl, new UIApplicationOpenUrlOptions (),
					(bool opened) => Log.Debug (opened ? "Opening app settings" : "Failed to open app settings"));
			}
		}


		public override UIStatusBarStyle PreferredStatusBarStyle () => UIStatusBarStyle.LightContent;


		public override void WillTransitionToTraitCollection (UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
		{
			this.UpdateStatusBarView (traitCollection);
		}
	}
}
