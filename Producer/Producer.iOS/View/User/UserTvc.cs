using System;

using Foundation;
using UIKit;

using Producer.Auth;
using Producer.Domain;

namespace Producer.iOS
{
	public partial class UserTvc : UITableViewController
	{

		public UserTvc (IntPtr handle) : base (handle) { }


		public override UIStatusBarStyle PreferredStatusBarStyle () => UIStatusBarStyle.LightContent;


		partial void cancelClicked (NSObject sender) => DismissViewController (true, null);


		partial void logoutButtonClicked (NSObject sender) => DismissViewController (true, ClientAuthManager.Shared.LogoutAuthProviders);
	}
}
