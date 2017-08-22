namespace Producer.Auth
{
	public class ClientAuthProvider
	{
		public string ProviderId { get; set; }

		public string ShortName { get; set; }

		public string SignInLabel { get; set; }
#if __IOS__
		public UIKit.UIImage Icon { get; set; }

		public UIKit.UIColor ButtonBackgroundColor { get; set; }

		public UIKit.UIColor ButtonTextColor { get; set; }
#elif __ANDROID__
		public Android.Graphics.Bitmap Icon { get; set; }

		public Android.Graphics.Color ButtonBackgroundColor { get; set; }

		public Android.Graphics.Color ButtonTextColor { get; set; }
#endif
	}
}
