#if __IOS__

using System;

using CoreGraphics;
using UIKit;

namespace Producer.Auth
{
	public class SignInButton : UIButton
	{
		//Corner radius of the button.
		static nfloat cornerRadius = 2.0f;

		//Opacity of the drop shadow of the button.
		static float dropShadowAlpha = 0.24f;

		//Radius of the drop shadow of the button.
		static nfloat dropShadowRadius = 2.0f;

		//Vertical offset of the drop shadow of the button.
		static nfloat dropShadowYOffset = 2.0f;

		//Button text font size.
		static nfloat fontSize = 15.0f;

		//UIColor textColor;
		//UIColor backgroundColor;
		//UIImage image;

		public SignInButton ()
		{
			commonInit ();
		}

		public SignInButton (IntPtr handle) : base (handle)
		{
			commonInit ();
		}


		void commonInit ()
		{
			var text = "";
			SetTitle (text, UIControlState.Normal);
			//BackgroundColor = backgroundColor;
			//SetTitleColor (textColor, UIControlState.Normal);
			//SetImage (image, UIControlState.Normal);
			TitleLabel.Font = UIFont.BoldSystemFontOfSize (fontSize);

			Layer.CornerRadius = cornerRadius;

			// Add a drop shadow.
			Layer.MasksToBounds = false;
			Layer.ShadowColor = UIColor.Black.CGColor;
			Layer.ShadowOpacity = dropShadowAlpha;
			Layer.ShadowRadius = dropShadowRadius;
			Layer.ShadowOffset = new CGSize (0, dropShadowYOffset);

			AdjustsImageWhenHighlighted = false;
		}


		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			//var authProvider = ClientAuthProvider.FromTag (Tag);

			//SetImage (authProvider.Icon, UIControlState.Normal);
			//SetTitle (authProvider.SignInLabel, UIControlState.Normal);
			//BackgroundColor = authProvider.ButtonBackgroundColor;
			//SetTitleColor (authProvider.ButtonTextColor, UIControlState.Normal);
		}


		public override void LayoutSubviews ()
		{
			var authProvider = ClientAuthProviderExtensions.FromTag (Tag);

			SetImage (authProvider.Icon, UIControlState.Normal);
			SetTitle (authProvider.SignInLabel, UIControlState.Normal);
			BackgroundColor = authProvider.ButtonBackgroundColor;
			SetTitleColor (authProvider.ButtonTextColor, UIControlState.Normal);

			base.LayoutSubviews ();

			CGRect imageRect = ImageView.Frame;
			imageRect.X = 8.0f;
			ImageView.Frame = imageRect;

			CGRect titleRect = TitleLabel.Frame;
			titleRect.X = 50.0f;
			TitleLabel.Frame = titleRect;
		}
	}
}

#endif