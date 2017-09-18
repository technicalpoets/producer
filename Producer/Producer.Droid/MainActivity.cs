using System.Threading.Tasks;

using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;

using Producer.Auth;
using Producer.Domain;
using Producer.Shared;

using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Producer.Droid
{
	[Activity (Label = "Producer", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : BaseActivity
	{
		TabFragmentPagerAdapter PagerAdapter;
		static IMenu _menu;


		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			Bootstrap.Run ();

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			var toolbar = FindViewById<Toolbar> (Resource.Id.main_toolbar);

			//Toolbar will now take on default Action Bar characteristics
			SetSupportActionBar (toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);

			//final Drawable upArrow = getResources ().getDrawable (R.drawable.abc_ic_ab_back_mtrl_am_alpha);
			//upArrow.setColorFilter (getResources ().getColor (android.R.color.white), PorterDuff.Mode.SRC_ATOP);
			//getSupportActionBar ().setHomeAsUpIndicator (upArrow);
			setupViewPager ();

			ClientAuthManager.Shared.AuthorizationChanged += handleClientAuthChanged;
			ProducerClient.Shared.CurrentUserChanged += handleCurrentUserChanged;
		}


		protected override void OnResume ()
		{
			base.OnResume ();
			checkCompose ();
		}


		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
				ClientAuthManager.Shared.AuthorizationChanged -= handleClientAuthChanged;
				ProducerClient.Shared.CurrentUserChanged -= handleCurrentUserChanged;
			}
			base.Dispose (disposing);
		}


		void handleClientAuthChanged (object sender, ClientAuthDetails e)
		{

			Log.Debug ($"Authenticated: {e}");

			Task.Run (async () =>
			{
				if (e == null)
				{
					ProducerClient.Shared.ResetUser ();
				}
				else
				{
					await ProducerClient.Shared.AuthenticateUser (e.Token, e.AuthCode);
				}

				//todo Activity.RunOnUiThread (() => UNUserNotificationCenter.Current.RequestAuthorization (UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound, authorizationRequestHandler));
				await ContentClient.Shared.GetAllAvContent ();
			});
		}


		void handleCurrentUserChanged (object sender, User e)
		{
			checkCompose ();
			Log.Debug ($"User: {e?.ToString ()}");
		}


		void checkCompose ()
		{
			// Check if signed-in user has write access
			if (_menu != null)
			{
				RunOnUiThread (() =>
				{
					var composeItem = _menu.FindItem (Resource.Id.action_compose);
					composeItem?.SetVisible (ProducerClient.Shared.UserRole.CanWrite ());
				});
			}
		}


		public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_settings:

					//Toast.MakeText (this, "Settings selected", ToastLength.Short).Show ();

					FragmentManager.BeginTransaction ()
					.Add (Resource.Id.fragment_container, new SettingsFragment ())
					.AddToBackStack (null)
					.Commit ();
					break;
				case Resource.Id.action_compose:
					return true;
				case Android.Resource.Id.Home:
					//Finish ();
					profileButtonClicked ();
					return true;
			}

			return base.OnOptionsItemSelected (item);
		}


		public override bool OnPrepareOptionsMenu (IMenu menu)
		{
			checkCompose ();
			return base.OnPrepareOptionsMenu (menu);
		}


		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			_menu = menu;
			MenuInflater.Inflate (Resource.Menu.menu_settings, menu);
			MenuInflater.Inflate (Resource.Menu.menu_compose, menu);
			return base.OnCreateOptionsMenu (menu);
		}


		void setupViewPager ()
		{
			PagerAdapter = new TabFragmentPagerAdapter (this, SupportFragmentManager);
			PagerAdapter.AddFragment (new ContentRecyclerFragment ());
			PagerAdapter.AddFragment (new ContentRecyclerFragment ());
			//PagerAdapter.AddFragment (new ResourcesFragment ());

			var viewPager = FindViewById<ViewPager> (Resource.Id.main_viewPager);
			viewPager.Adapter = PagerAdapter;

			var tabLayout = FindViewById<TabLayout> (Resource.Id.main_tabLayout);
			tabLayout.TabMode = TabLayout.ModeFixed;
			tabLayout.TabGravity = TabLayout.GravityFill;
			tabLayout.SetupWithViewPager (viewPager);

			PagerAdapter.FillTabLayout (tabLayout);
			//ClientAuthManager.Shared.InitializeAuthProviders (this);

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


		void profileButtonClicked ()
		{
			RunOnUiThread (() =>
			{
				ClientAuthManager.Shared.AuthActivityLayoutResId = Resource.Layout.Login;
				ClientAuthManager.Shared.GoogleWebClientResId = Resource.String.default_web_client_id;
				ClientAuthManager.Shared.GoogleButtonResId = Resource.Id.sign_in_button;

				if (ProducerClient.Shared.User == null)
				{
					StartActivity (typeof (LoginActivity));
				}
				else
				{
					StartActivity (typeof (UserActivity));
				}
			});
		}


	}
}