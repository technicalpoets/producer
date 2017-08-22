using System;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using UserNotifications;

using SettingsStudio;

using Google.SignIn;

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
				await ContentClient.Shared.GetAllAvContent ();

				await AssetPersistenceManager.Shared.RestorePersistenceManagerAsync (ContentClient.Shared.AvContent [UserRoles.General]);

				BeginInvokeOnMainThread (() => UNUserNotificationCenter.Current.RequestAuthorization (UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, authorizationRequestHandler));
			});
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			//authenticate ();
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


		#region Auth

		void handleClientAuthChanged (object s, ClientAuthDetails e)
		{
			Log.Debug ($"Authenticated: {e}");
		}


		void authenticate ()
		{
			Task.Run (async () =>
			{
				try
				{
					//ProducerClient.Shared.ResetCurrentUser ();
					//ClientAuthManager.Shared.LogoutAuthProviders ();
					//throw new Exception ("stop and re-comment out lines");

					var details = ClientAuthManager.Shared.ClientAuthDetails;

					// try authenticating with an existing token
					if (ProducerClient.Shared.AuthUser == null && details != null)
					{
						var user = await ProducerClient.Shared.GetAuthUserConfig () ?? await ProducerClient.Shared.GetAuthUserConfig (details?.Token, details?.AuthCode);

						if (user != null)
						{
							//Log.Debug (user.ToString ());
							//await BotClient.Shared.ConnectSocketAsync (conversationId => AgenciesClient.Shared.GetConversationAsync (conversationId));
						}
						else
						{
							logout ();
						}
					}
					else // otherwise prompt the user to login
					{
						if (ProducerClient.Shared.AuthUser == null)
						{
							BeginInvokeOnMainThread (() => presentAuthController ());
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error (ex.Message);
					throw;
				}
			});
		}


		void presentAuthController ()
		{
			var authViewController = new AuthViewController ();

			if (authViewController != null)
			{
				var authNavController = new UINavigationController (authViewController);

				if (authNavController != null)
				{
					PresentViewController (authNavController, true, null);
				}
			}
		}


		void logout ()
		{
			try
			{
				SignIn.SharedInstance.SignOutUser ();

				authenticate ();
			}
			catch (Exception ex)
			{
				Log.Error (ex.Message);
				throw;
			}
		}

		#endregion


		public override UIStatusBarStyle PreferredStatusBarStyle () => UIStatusBarStyle.LightContent;


		public override void WillTransitionToTraitCollection (UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
		{
			this.UpdateStatusBarView (traitCollection);
		}
	}
}
