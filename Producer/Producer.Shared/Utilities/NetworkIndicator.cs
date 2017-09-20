namespace Producer.Shared
{
	public static class NetworkIndicator
	{
		static int toggleCount;

		static readonly object _object = new object ();

		public static void ToggleVisibility (bool visible)
		{
			lock (_object)
			{
				if (!visible) toggleCount--;

				if (toggleCount <= 0)
				{
#if __IOS__
					UIKit.UIApplication.SharedApplication.BeginInvokeOnMainThread (() => UIKit.UIApplication.SharedApplication.NetworkActivityIndicatorVisible = visible);
#endif
					toggleCount = 0;
				}

				if (visible) toggleCount++;
			}
		}
	}
}
