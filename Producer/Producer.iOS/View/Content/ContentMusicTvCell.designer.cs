// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Producer.iOS
{
	[Register ("ContentMusicTvCell")]
	partial class ContentMusicTvCell
	{
		[Outlet]
		UIKit.UIButton accessoryButton { get; set; }

		[Outlet]
		Producer.iOS.CircularProgressView accessoryProgressView { get; set; }

		[Outlet]
		UIKit.UIProgressView progressBar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (accessoryButton != null) {
				accessoryButton.Dispose ();
				accessoryButton = null;
			}

			if (progressBar != null) {
				progressBar.Dispose ();
				progressBar = null;
			}

			if (accessoryProgressView != null) {
				accessoryProgressView.Dispose ();
				accessoryProgressView = null;
			}
		}
	}
}
