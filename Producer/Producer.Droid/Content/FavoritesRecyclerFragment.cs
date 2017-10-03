using System;
using Android.OS;
using Android.Views;

namespace Producer.Droid
{
	public class FavoritesRecyclerFragment : ContentRecyclerFragmentBase, ITabFragment
	{
		#region ITabFragment Members


		public string Title => "Favorites";


		public int Icon => Resource.Drawable.ic_tabbar_resources;


		#endregion


		protected override void UpdateContent ()
		{

		}
	}
}