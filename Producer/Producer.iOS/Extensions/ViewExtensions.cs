using System;
using Foundation;
using UIKit;
using System.Linq;

namespace Producer.iOS
{
	public static class ViewExtensions
	{
		const int statusBarViewTag = 987;

		public static void ConstrainToParentCenter (this UIView view, nfloat width = default (nfloat), nfloat height = default (nfloat))
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


		public static void ShowSettingsAlert (this UIViewController viewController, string alertTitle = "Configure App", string alertMessage = "You must add your Azure information to Settings before using the app.")
		{
			var alertController = UIAlertController.Create (alertTitle, alertMessage, UIAlertControllerStyle.Alert);

			alertController.AddAction (UIAlertAction.Create ("Open Settings", UIAlertActionStyle.Default, (obj) =>
			{
				var settingsString = UIApplication.OpenSettingsUrlString;

				if (!string.IsNullOrEmpty (settingsString))
				{
					var settingsUrl = NSUrl.FromString (settingsString);

					UIApplication.SharedApplication.OpenUrl (settingsUrl, new UIApplicationOpenUrlOptions (),
						(bool opened) => Log.Debug (opened ? "Opening app settings" : "Failed to open app settings"));

				}
			}));

			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, (obj) => viewController.DismissViewController (true, null)));

			viewController.PresentViewController (alertController, true, null);
		}
	}
}
