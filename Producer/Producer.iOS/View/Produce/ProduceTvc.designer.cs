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
	[Register ("ProduceTvc")]
	partial class ProduceTvc
	{
		[Outlet]
		UIKit.UISegmentedControl segmentControl { get; set; }

		[Action ("segmentControlValueChanged:")]
		partial void segmentControlValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (segmentControl != null) {
				segmentControl.Dispose ();
				segmentControl = null;
			}
		}
	}
}
