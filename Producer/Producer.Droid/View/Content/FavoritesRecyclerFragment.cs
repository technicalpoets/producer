namespace Producer.Droid
{
	public class FavoritesRecyclerFragment : ContentRecyclerFragmentBase, ITabFragment
	{
		#region ITabFragment Members


		public string Title => "Favorites";


		public int Icon => Resource.Drawable.ic_segment_saved;


		#endregion


		protected override void UpdateContent ()
		{

		}
	}
}