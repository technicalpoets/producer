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
	[Register ("ComposeVc")]
	partial class ComposeVc
	{
		[Outlet]
		UIKit.UIButton createButton { get; set; }

		[Outlet]
		UIKit.UITextField descriptionTextField { get; set; }

		[Outlet]
		UIKit.UITextField fileDisplayNameTextField { get; set; }

		[Outlet]
		UIKit.UITextField fileNameTextField { get; set; }

		[Outlet]
		UIKit.UITextField fileTypeTextField { get; set; }

		[Action ("createButtonClicked:")]
		partial void createButtonClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (createButton != null) {
				createButton.Dispose ();
				createButton = null;
			}

			if (fileDisplayNameTextField != null) {
				fileDisplayNameTextField.Dispose ();
				fileDisplayNameTextField = null;
			}

			if (fileNameTextField != null) {
				fileNameTextField.Dispose ();
				fileNameTextField = null;
			}

			if (fileTypeTextField != null) {
				fileTypeTextField.Dispose ();
				fileTypeTextField = null;
			}

			if (descriptionTextField != null) {
				descriptionTextField.Dispose ();
				descriptionTextField = null;
			}
		}
	}
}
