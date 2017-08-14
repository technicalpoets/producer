using System;

using Foundation;
using UIKit;

using Producer.Domain;

namespace Producer.iOS
{
	public partial class UserTvc : UITableViewController
	{

		User user = null;

		public UserTvc (IntPtr handle) : base (handle) { }


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (user != null)
			{
				emptyView.RemoveFromSuperview ();
			}
			else
			{
				setEmptyViewFrame ();
			}
		}


		public override UIStatusBarStyle PreferredStatusBarStyle () => UIStatusBarStyle.LightContent;


		public override void WillTransitionToTraitCollection (UITraitCollection traitCollection, IUIViewControllerTransitionCoordinator coordinator)
		{
			if (emptyView?.IsDescendantOfView (TableView) ?? false)
			{
				setEmptyViewFrame ();
			}
		}


		partial void loginButtonClicked (NSObject sender)
		{

		}


		partial void createAccountButtonClicked (NSObject sender)
		{

		}


		partial void cancelClicked (NSObject sender) => DismissViewController (true, null);


		void setEmptyViewFrame ()
		{
			var frame = emptyView.Frame;

			frame.Height = UIScreen.MainScreen.Bounds.Height;// - (TabBarController.TabBar.Frame.Height + 20);

			emptyView.Frame = frame;

			createAccountButton.Layer.CornerRadius = 4;
		}
	}
}
