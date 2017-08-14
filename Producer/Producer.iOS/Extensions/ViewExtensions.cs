using System;
using UIKit;

namespace Producer.iOS
{
	public static class ViewExtensions
	{
		const int statusBarViewTag = 987;

		public static void ConstrainToParentCenter (this UIView view, nfloat height = default (nfloat), nfloat width = default (nfloat))
		{
			if (view?.Superview == null) throw new InvalidOperationException ("Must add view to a superview before calling this method");

			view.TranslatesAutoresizingMaskIntoConstraints = false;

			var verticalFormat = string.Format ("V:[super]-(<=1)-[view{0}]", height == default (nfloat) ? string.Empty : $"({height})");

			var horizontalFormat = string.Format ("H:[super]-(<=1)-[view{0}]", width == default (nfloat) ? string.Empty : $"({width})");

			var viewsAndMetrics = new object [] { "super", view.Superview, "view", view };

			view.Superview.AddConstraints (NSLayoutConstraint.FromVisualFormat (verticalFormat, NSLayoutFormatOptions.AlignAllCenterX, viewsAndMetrics));

			view.Superview.AddConstraints (NSLayoutConstraint.FromVisualFormat (horizontalFormat, NSLayoutFormatOptions.AlignAllCenterY, viewsAndMetrics));
		}


		public static void AddStatusBarView (this UIViewController controller, UIColor backgroundColor)
		{
			var statusBarView = new UIView
			{
				Tag = statusBarViewTag,
				BackgroundColor = backgroundColor,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			controller.View.AddSubview (statusBarView);

			controller.View.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"H:|[statusBarView]|", 0, "statusBarView", statusBarView));
			controller.View.AddConstraints (NSLayoutConstraint.FromVisualFormat (@"V:|[statusBarView(20.0)]", 0, "statusBarView", statusBarView));
		}


		public static void UpdateStatusBarView (this UIViewController controller, UITraitCollection traitCollection)
		{
			var statusBarView = controller.View.ViewWithTag (statusBarViewTag);

			if (statusBarView != null)
			{
				statusBarView.Hidden = traitCollection.VerticalSizeClass == UIUserInterfaceSizeClass.Compact;
			}
		}
	}
}
