// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Producer.iOS
{
	[Register ("UserTvc")]
	partial class UserTvc
	{
		[Outlet]
		UIKit.UIButton createAccountButton { get; set; }

		[Outlet]
		UIKit.UIView emptyView { get; set; }

		[Action ("cancelClicked:")]
		partial void cancelClicked (Foundation.NSObject sender);

		[Action ("createAccountButtonClicked:")]
		partial void createAccountButtonClicked (Foundation.NSObject sender);

		[Action ("loginButtonClicked:")]
		partial void loginButtonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (createAccountButton != null) {
				createAccountButton.Dispose ();
				createAccountButton = null;
			}

			if (emptyView != null) {
				emptyView.Dispose ();
				emptyView = null;
			}
		}
	}
}
