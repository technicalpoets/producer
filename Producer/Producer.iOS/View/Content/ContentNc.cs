using System;
using System.Threading.Tasks;

using Foundation;
using UIKit;

using Producer.Auth;
using Producer.Domain;
using Producer.Shared;

namespace Producer.iOS
{
	public partial class ContentNc : BaseNc
	{

		public ContentNc (IntPtr handle) : base (handle) { }


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ClientAuthManager.Shared.AuthorizationChanged += HandleClientAuthChanged;
		}


		void AuthorizationRequestHandler (bool authorized, NSError error)
		{
			if (authorized)
			{
				BeginInvokeOnMainThread (() => UIApplication.SharedApplication.RegisterForRemoteNotifications ());
			}
			else
			{
				Log.Error (error.LocalizedDescription);
			}
		}


		void HandleClientAuthChanged (object sender, ClientAuthDetails authDetails)
		{
			Task.Run (async () =>
			{
				if (authDetails == null)
				{
					ProducerClient.Shared.ResetUser ();
				}
				else
				{
					await ProducerClient.Shared.AuthenticateUser (authDetails.Token, authDetails.AuthCode);
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
	}
}
