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
	[Register ("ContentTvc")]
	partial class ContentTvc
	{
		[Outlet]
		UIKit.UIBarButtonItem composeButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem nextButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem pauseButton { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem playButton { get; set; }

		[Outlet]
		UIKit.UISegmentedControl segmentControl { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem titleButton { get; set; }

		[Action ("accessoryButtonClicked:")]
		partial void accessoryButtonClicked (Foundation.NSObject sender);

		[Action ("refreshValueChanged:")]
		partial void refreshValueChanged (Foundation.NSObject sender);

		[Action ("segmentControlValueChanged:")]
		partial void segmentControlValueChanged (Foundation.NSObject sender);

		[Action ("togglePlayClicked:")]
		partial void togglePlayClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (nextButton != null) {
				nextButton.Dispose ();
				nextButton = null;
			}

			if (pauseButton != null) {
				pauseButton.Dispose ();
				pauseButton = null;
			}

			if (playButton != null) {
				playButton.Dispose ();
				playButton = null;
			}

			if (segmentControl != null) {
				segmentControl.Dispose ();
				segmentControl = null;
			}

			if (titleButton != null) {
				titleButton.Dispose ();
				titleButton = null;
			}

			if (composeButton != null) {
				composeButton.Dispose ();
				composeButton = null;
			}
		}
	}
}
