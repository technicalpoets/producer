using System;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using UserNotifications;

using SettingsStudio;

using NomadCode.UIExtensions;

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

			Task.Run (async () =>
			{
				await Bootstrap.InitializeDataStoreAsync ();

				await ContentClient.Shared.GetAllAvContent ();

				await AssetPersistenceManager.Shared.RestorePersistenceManagerAsync (ContentClient.Shared.AvContent [UserRoles.General]);

				BeginInvokeOnMainThread (() => UNUserNotificationCenter.Current.RequestAuthorization (UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, authorizationRequestHandler));
			});

			//if (!Settings.TestProducer)
			//{
			//	SetViewControllers (new UIViewController [] { ViewControllers [0], ViewControllers [1], ViewControllers [3], ViewControllers [4] }, false);
			//}
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

					if (produceTvc == null)
					{
						var contentTvc = TopViewController as ContentTvc;

						if (contentTvc != null)
						{
							produceTvc = Storyboard.Instantiate<ProduceTvc> ();

							contentTvc.ShowViewController (produceTvc, contentTvc);
						}
					}

					composeVc = Storyboard.Instantiate<ComposeVc> ();

					produceTvc.ShowViewController (composeVc, produceTvc);
				}

				composeVc.SetData (url);
			}

			return canCompose;
		}


		public override UIStatusBarStyle PreferredStatusBarStyle () => UIStatusBarStyle.LightContent;


		public override void WillTransitionToTraitCollection (UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
		{
			this.UpdateStatusBarView (traitCollection);
		}
	}
}
