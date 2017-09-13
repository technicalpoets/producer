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
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!Settings.HasUrls)
			{
				this.ShowSettinsAlert ();
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


		public override UIStatusBarStyle PreferredStatusBarStyle () => UIStatusBarStyle.LightContent;


		public override void WillTransitionToTraitCollection (UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
		{
			this.UpdateStatusBarView (traitCollection);
		}
	}
}
