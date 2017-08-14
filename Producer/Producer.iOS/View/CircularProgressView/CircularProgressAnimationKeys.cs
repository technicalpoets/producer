using Foundation;

namespace Producer.iOS
{
	public static class CircularProgressAnimationKeys
	{
		public const string indeterminate = "indeterminateAnimation";

		public const string progress = "progress";

		public const string transformRotation = "transform.rotation";

		public const string completionBlock = "completionBlock";

		public static NSString nsCompletionBlock => new NSString (completionBlock);

		public const string toValue = "toValue";
	}
}
