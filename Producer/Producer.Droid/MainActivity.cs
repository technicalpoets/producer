using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Android.Support.V4.View;

namespace Producer.Droid
{
	[Activity (Label = "Producer", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : BaseActivity
	{
		StaticFragmentPagerAdapter PagerAdapter;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Bootstrap.Run ();

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			var toolbar = FindViewById<Toolbar> (Resource.Id.main_toolbar);
			//Toolbar will now take on default Action Bar characteristics
			SetSupportActionBar (toolbar);

			setupViewPager ();
		}


		void setupViewPager ()
		{
			PagerAdapter = new StaticFragmentPagerAdapter (this, SupportFragmentManager);
			//PagerAdapter.AddFragment (new PartnerRecyclerFragment ());
			//PagerAdapter.AddFragment (new TeamRecyclerFragment ());
			//PagerAdapter.AddFragment (new ResourcesFragment ());

			var viewPager = FindViewById<ViewPager> (Resource.Id.main_viewPager);
			viewPager.Adapter = PagerAdapter;

			var tabLayout = FindViewById<TabLayout> (Resource.Id.main_tabLayout);
			tabLayout.TabMode = TabLayout.ModeFixed;
			tabLayout.TabGravity = TabLayout.GravityFill;
			tabLayout.SetupWithViewPager (viewPager);

			for (var i = 0; i < PagerAdapter.Count; i++)
			{
				var tab = tabLayout.GetTabAt (i);

				tab.SetCustomView (PagerAdapter.GetTabView (i));
			}

			viewPager.PageSelected += (sender, e) =>
			{

				//Tier = (PartnerTiers)e.Position;

				////update the query listener
				//var fragment = PagerAdapter.GetFragmentAtPosition (e.Position);
				//queryListener = (SearchView.IOnQueryTextListener)fragment;

				//searchView?.SetOnQueryTextListener (queryListener);

				//UpdateColors (Tier);
			};
		}
	}
}