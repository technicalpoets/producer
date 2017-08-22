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
	[Register ("LoginVc")]
	partial class LoginVc
	{
		[Outlet]
		UIKit.UIStackView buttonStackView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint buttonStackViewHeightConstraint { get; set; }

		[Action ("cancelClicked:")]
		partial void cancelClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonStackView != null) {
				buttonStackView.Dispose ();
				buttonStackView = null;
			}

			if (buttonStackViewHeightConstraint != null) {
				buttonStackViewHeightConstraint.Dispose ();
				buttonStackViewHeightConstraint = null;
			}
		}
	}
}
