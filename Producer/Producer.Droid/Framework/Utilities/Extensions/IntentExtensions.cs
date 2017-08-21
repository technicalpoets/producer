using Android.Content;

namespace Producer.Droid
{
	public static class IntentExtensions
	{
		public static Intent PutIntentData (this Intent intent, IntentData intentData)
		{
			return intent.PutExtra (intentData.Key, string.Join (IntentData.Delimiter, intentData.Values));
		}


		public static IntentData GetIntentData (this Intent intent)
		{
			return IntentData.FromIntent (intent);
		}
	}
}