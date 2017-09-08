using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Producer.Auth;
using System.Threading.Tasks;
using System;
using static Android.Gms.Common.Apis.GoogleApiClient;
using Android.Gms.Common;
using Producer.Shared;
using Producer.Droid.Views.User;

namespace Producer.Droid
{
	[Activity (Label = "Producer", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : BaseActivity, IOnConnectionFailedListener
	{
		TabFragmentPagerAdapter PagerAdapter;

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
		}

		public override bool OnOptionsItemSelected (Android.Views.IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					//Finish ();
					profileButtonClicked ();
					return true;

				default:
					return base.OnOptionsItemSelected (item);
			}
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

		async Task loginAsync ()
		{
			try
			{
				//BotClient.Shared.ResetCurrentUser();
				//ClientAuthManager.Shared.LogoutAuthProviders();

				//var details = ClientAuthManager.Shared.ClientAuthDetails;

				// try authenticating with an existing token


				if (false)
				{
					//var user = await ProducerClient.Shared.User  ?? await AgenciesClient.Shared.GetAuthUserConfigAsync (details?.Token, details?.AuthCode);

					//if (user != null)
					//{
					//	BotClient.Shared.CurrentUserId = user.Id;

					//	BotClient.Shared.CurrentUserName = details.Username;
					//	BotClient.Shared.CurrentUserEmail = details.Email;
					//	BotClient.Shared.SetAvatarUrl (user.Id, details.AvatarUrl);

					//	await BotClient.Shared.ConnectSocketAsync (conversationId => AgenciesClient.Shared.GetConversationAsync (conversationId));

					//	FaceClient.Shared.SubscriptionKey = await AgenciesClient.Shared.GetFaceApiTokenAsync ();
					//}
					//else
					//{
					//	//logoutAsync();
					//	BotClient.Shared.ResetCurrentUser ();
					//	ClientAuthManager.Shared.LogoutAuthProviders ();
					//}
				}
				else // otherwise prompt the user to login
				{
					RunOnUiThread (() =>
					{
						ClientAuthManager.Shared.AuthActivityLayoutResId = Resource.Layout.LoginActivityLayout;

						ClientAuthManager.Shared.GoogleWebClientResId = Resource.String.default_web_client_id;
						ClientAuthManager.Shared.GoogleButtonResId = Resource.Id.sign_in_button;

						StartActivity (typeof (AuthActivity));
					});
				}
			}
			catch (Exception ex)
			{
				Log.Error (ex.Message);
				throw;
			}
		}
		 
		void profileButtonClicked ()
		{
			if (ProducerClient.Shared.User == null)
			{
				RunOnUiThread (() =>
				{
					ClientAuthManager.Shared.AuthActivityLayoutResId = Resource.Layout.LoginActivityLayout;

					ClientAuthManager.Shared.GoogleWebClientResId = Resource.String.default_web_client_id;
					ClientAuthManager.Shared.GoogleButtonResId = Resource.Id.sign_in_button;

					StartActivity (typeof (AuthActivity));
				});
			}
			else
			{
				StartActivity (typeof (UserActivity));

			}
		}


		public void OnConnectionFailed (ConnectionResult result)
		{
			throw new NotImplementedException ();
		}
	}
}