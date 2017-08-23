using System;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using UserNotifications;

using SettingsStudio;

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

			Task.Run (async () =>
			{
				if (ProducerClient.Shared.AuthUser == null && ClientAuthManager.Shared.ClientAuthDetails != null)
				{
					var user = await ProducerClient.Shared.GetAuthUserConfig () ??
							   await ProducerClient.Shared.GetAuthUserConfig (ClientAuthManager.Shared.ClientAuthDetails.Token, ClientAuthManager.Shared.ClientAuthDetails.AuthCode);

					Log.Debug (user.ToString ());
				}

				Log.Debug ($"AuthUser = {ProducerClient.Shared.AuthUser}");

				await ContentClient.Shared.GetAllAvContent ();

				await AssetPersistenceManager.Shared.RestorePersistenceManagerAsync (ContentClient.Shared.AvContent [UserRoles.General]);

				BeginInvokeOnMainThread (() => UNUserNotificationCenter.Current.RequestAuthorization (UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, authorizationRequestHandler));
			});
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


		public bool SetupComposeVc (NSUrl url)
		{
			// TODO: Display some type of message for non-producers or figure out how to kill the document type registration

			var canCompose = Settings.TestProducer && (url?.IsFileUrl ?? false);

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


		void handleClientAuthChanged (object s, ClientAuthDetails e)
		{
			Log.Debug ($"Authenticated: {e}");
		}


		public override UIStatusBarStyle PreferredStatusBarStyle () => UIStatusBarStyle.LightContent;


		public override void WillTransitionToTraitCollection (UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
		{
			this.UpdateStatusBarView (traitCollection);
		}
	}
}
