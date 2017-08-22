using Android.OS;
using Android.Widget;

namespace Producer.Droid
{
	public static class TextViewExtensions
	{
		public static void SetTextAppearanceSafely (this TextView textView, int resId)
		{
			if (Build.VERSION.SdkInt < BuildVersionCodes.M)
			{

#pragma warning disable CS0618 // Type or member is obsolete
				textView.SetTextAppearance (textView.Context, resId);
#pragma warning restore CS0618 // Type or member is obsolete

			}
			else
			{

#pragma warning disable XA0001 // Find issues with Android API usage
				textView.SetTextAppearance (resId);
#pragma warning restore XA0001 // Find issues with Android API usage

			}
		}
	}
}