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
		class TabConfig
		{
			public Fragment Fragment { get; set; }

			public string Title { get; set; }

			public int IconResource { get; set; }

			public bool ShowTitle { get; set; }
		}

		Context context;

		List<TabConfig> tabList = new List<TabConfig> ();


		public TabFragmentPagerAdapter (Context context, FragmentManager manager) : base (manager)
		{
			this.context = context;
		}


		public void AddFragment (Fragment fragment, string title = "", int iconDrawable = -1, bool showTitle = true)
		{
			tabList.Add (new TabConfig
			{
				Fragment = fragment,
				Title = title,
				IconResource = iconDrawable,
				ShowTitle = showTitle
			});
		}


		public void AddFragment<TFragment> (TFragment fragment, bool showTitle = true)
			where TFragment : Fragment, ITabFragment
		{
			AddFragment (fragment, fragment.Title, fragment.Icon, showTitle);
		}


		public override int Count
		{
			get
			{
				return tabList.Count;
			}
		}


		public override Fragment GetItem (int position)
		{
			return tabList [position].Fragment;
		}


		public ITabFragment GetTabFragment (int position)
		{
			return (ITabFragment) GetItem (position);
		}


		public override Java.Lang.ICharSequence GetPageTitleFormatted (int position)
		{
			return new Java.Lang.String (tabList [position].Title);
		}


		/// <summary>
		/// Inflates the tab view at the specified position.
		/// </summary>
		/// <returns>The tab view.</returns>
		/// <param name="position">Position.</param>
		/// <param name="tabViewResourceId">The resource ID of the Tab item layout to use.  Defaults to <c>Resource.Layout.StackedTabLayout</c>.</param>
		public virtual View InflateTabView (int position, int tabViewResourceId = Resource.Layout.StackedTabLayout)
		{
			//get the item view and set the text + icon/image
			var tabItemView = LayoutInflater.From (context).Inflate (tabViewResourceId, null);
			var tabText = tabItemView.FindViewById<TextView> (Resource.Id.tabText);
			var tabImage = tabItemView.FindViewById<ImageView> (Resource.Id.tabIcon);

			var tab = tabList [position];

			tabText.Text = tab.Title;

			if (!tab.ShowTitle)
			{
				tabText.Visibility = ViewStates.Gone;
			}

			var icon = tab.IconResource;

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