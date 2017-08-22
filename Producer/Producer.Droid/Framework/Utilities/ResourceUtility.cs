using Android.Content;

namespace Producer.Droid
{
	public static class ResourceUtility
	{
		public static int GetDrawableResourceIdByName (this Context context, string name)
		{
			return context.Resources.GetIdentifier (name, "drawable", context.PackageName);
		}
	}
}