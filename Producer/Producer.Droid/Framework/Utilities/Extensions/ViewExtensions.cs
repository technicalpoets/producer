using Android.Views;

namespace Producer.Droid.Framework.Utilities.Extensions
{
	public static class ViewExtensions
	{
		public static void ResetYRotation (this View view)
		{
			if ((int) view.RotationY != 0)
			{
				view.RotationY = 0;
			}
		}
	}
}