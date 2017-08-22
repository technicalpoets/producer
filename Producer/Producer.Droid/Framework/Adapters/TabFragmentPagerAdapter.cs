using System.Collections.Generic;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Producer.Droid
{
	/// <summary>
	/// Adapter where Fragments can be created/configured externally and added/managed here in the adapter.
	/// </summary>
	public class TabFragmentPagerAdapter : BaseFragmentPagerAdapter
	{
		Context context;
		List<Fragment> fragments = new List<Fragment> ();
		readonly List<string> titles = new List<string> ();
		readonly List<int> icons = new List<int> ();

		public TabFragmentPagerAdapter (Context context, FragmentManager manager) : base (manager)
		{
			this.context = context;
		}


		public void AddFragment (Fragment fragment, string title = "", int iconDrawable = -1)
		{
			fragments.Add (fragment);
			titles.Add (title);
			icons.Add (iconDrawable);
		}


		public void AddFragment<TFragment> (TFragment fragment)
			where TFragment : Fragment, ITabFragment
		{
			AddFragment (fragment, fragment.Title, fragment.Icon);
		}


		public override int Count
		{
			get
			{
				return fragments.Count;
			}
		}


		public override Fragment GetItem (int position)
		{
			return fragments [position];
		}


		public override Java.Lang.ICharSequence GetPageTitleFormatted (int position)
		{
			return new Java.Lang.String (titles [position]);
		}


		/// <summary>
		/// Inflates the tab view at the specified position.
		/// </summary>
		/// <returns>The tab view.</returns>
		/// <param name="position">Position.</param>
		public virtual View InflateTabView (int position)
		{
			//get the item view and set the text + icon/image
			var tabItemView = LayoutInflater.From (context).Inflate (Resource.Layout.StackedTabLayout, null);
			var tabText = tabItemView.FindViewById<TextView> (Resource.Id.tabText);
			var tabImage = tabItemView.FindViewById<ImageView> (Resource.Id.tabIcon);

			tabText.Text = titles [position];

			var icon = icons [position];

			if (icon > -1)
			{
				tabImage.SetBackgroundResource (icon);
			}

			if (position == 0)
			{
				tabItemView.Selected = true;
			}

			return tabItemView;
		}


		/// <summary>
		/// Fills the tab layout with the tab fragments that have been added to this adapter.
		/// </summary>
		/// <param name="tabLayout">Tab layout.</param>
		public virtual void FillTabLayout (TabLayout tabLayout)
		{
			for (var i = 0; i < Count; i++)
			{
				var tab = tabLayout.GetTabAt (i);

				tab.SetCustomView (InflateTabView (i));
			}
		}
	}
}