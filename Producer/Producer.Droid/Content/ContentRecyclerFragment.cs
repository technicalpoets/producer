using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Views;
using Android.OS;
using Android.Content;
using Producer.Domain;
using Producer.Auth;
using System;
using Producer.Shared;
using System.Linq;

namespace Producer.Droid
{
	public class ContentRecyclerFragment : RecyclerViewListFragment<AvContent, ContentViewHolder>, ITabFragment//, SearchView.IOnQueryTextListener
	{
		#region ITabFragment Members


		public string Title => "Content";


		public int Icon => Resource.Drawable.ic_tabbar_resources;

		ContentRecyclerAdapter CRAdapter;


		#endregion

		List<AvContent> DisplayContent = new List<AvContent> ();


		public override void OnCreate (Bundle savedInstanceState)
		{
			ShowDividers = false;

			base.OnCreate (savedInstanceState);
		}


		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = base.OnCreateView (inflater, container, savedInstanceState);

			//color our scrollbar & popup according to the Tier
			//if (RecyclerView is FastScrollRecyclerView)
			//{
			//var color = Tier.GetColor (Activity);
			//var recycler = (FastScrollRecyclerView)RecyclerView;
			//recycler.SetThumbActiveColor (color);
			//recycler.SetTrackInactiveColor (color);
			//recycler.SetPopupBackgroundColor (color);
			//}

			updateMusicAssets ();
			ContentClient.Shared.AvContentChanged += handleAvContentChanged;
			return view;
		}


		void handleAvContentChanged (object sender, UserRoles e) => updateMusicAssets ();


		void updateMusicAssets ()
		{
			Log.Debug ("Load Content");

			if (DisplayContent?.Count == 0 && ContentClient.Shared.AvContent.Count > 0)
			{
				DisplayContent = ContentClient.Shared.AvContent [UserRoles.General].Where (m => m.HasId && m.HasRemoteAssetUri)
																			  .ToList ();
			}
			else
			{
				var newAssets = ContentClient.Shared.AvContent [UserRoles.General].Where (m => m.HasId && m.HasRemoteAssetUri && !DisplayContent.Any (ma => ma.Id == m.Id));

				DisplayContent.AddRange (newAssets);

				DisplayContent.RemoveAll (ma => !ContentClient.Shared.AvContent [UserRoles.General].Any (a => a.Id == ma.Id));
			}


			//DisplayContent?.Sort ((x, y) => y.Music.Timestamp.CompareTo (x.Music.Timestamp));
			if (CRAdapter != null)
			{
				//CRAdapter.SetFilterResults (DisplayContent);
				CRAdapter.SetItems (DisplayContent);
				//CRAdapter = new ContentRecyclerAdapter (DisplayContent);
				//RecyclerView.SwapAdapter (CRAdapter, false);

				//CRAdapter.SetItems (DisplayContent);
				Activity.RunOnUiThread (() => CRAdapter?.NotifyDataSetChanged ());
			}
		}



		#region implemented abstract members of RecyclerViewFragment


		protected override RecyclerViewAdapter<AvContent, ContentViewHolder> GetAdapter ()
		{
			CRAdapter = new ContentRecyclerAdapter (DisplayContent);
			//adapter.Filter = new PartnerFilter (adapter);

			return CRAdapter;
		}



		protected override void OnItemClick (View view, AvContent item)
		{
			//var partner = item;//DisplayPartners [position];
			//var partnerLogoImageView = view.FindViewById<AppCompatImageView> (Resource.Id.partner_logo);

			//var detailIntent = new Intent (Context, typeof (PartnerDetailActivity));
			////var intentData = IntentData.Partner.Create (partner.Id, partner.Name, partner.PartnerTier);
			//var intentData = IntentData.Create (partner.Id, partner.Name, partner.PartnerTier);
			//detailIntent.PutIntentData (intentData);

			//TransitionToActivity (detailIntent, partnerLogoImageView);
		}




		#endregion


		//#region SearchView.IOnQueryTextListener Members


		//public bool OnQueryTextChange (string query)
		//{
		//	//begins an async filtering operation
		//	((Android.Widget.IFilterable) Adapter).Filter.InvokeFilter (query);

		//	return true;
		//}


		//public bool OnQueryTextSubmit (string query)
		//{
		//	return false;
		//}


		//#endregion
	}
}