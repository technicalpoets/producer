namespace Producer.Droid
{
	public class FavoritesRecyclerFragment : ContentRecyclerFragmentBase, ITabFragment
	{
		#region ITabFragment Members


		public string Title => "Favorites";


		public int Icon => Resource.Drawable.ic_tab_favorites;


		#endregion


		protected override void UpdateContent ()
		{

		}
	}
}