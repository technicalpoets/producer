using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Views;
using Android.OS;
using Android.Content;
using Producer.Domain;
using Producer.Shared;
using Producer.Droid.Providers;
using System;
using System.Linq;
using Com.Google.Android.Exoplayer2;
using Plugin.MediaManager.Abstractions;

namespace Producer.Droid
{
	public class ContentRecyclerFragment : RecyclerViewListFragment<MusicAsset, ContentViewHolder>, ITabFragment  //, SearchView.IOnQueryTextListener
	{
		#region ITabFragment Members


		public string Title => "Content";


		public int Icon => Resource.Drawable.ic_tabbar_resources;


		#endregion


		List<MusicAsset> allAssets = new List<MusicAsset> ();

		//List<MusicAsset> savedAssets = new List<MusicAsset> ();


		//List<AvContent> DisplayContent = new List<AvContent> ();

		ContentRecyclerAdapter adapter;


		public override void OnCreate (Bundle savedInstanceState)
		{
			ShowDividers = false;

			base.OnCreate (savedInstanceState);

			_ = RefreshContent ();

			AssetPersistenceManager.Shared.DidRestore += handlePersistanceManagerDidRestore;

			AssetPersistenceManager.Shared.AssetDownloadStateChanged += handlePersistanceManagerAssetDownloadStateChanged;

			AssetPersistenceManager.Shared.AssetDownloadProgressChanged += handlePersistanceManagerAssetDownloadProgressChanged;

			AssetPlaybackManager.Shared.CurrentItemChanged += handlePlaybackManagerCurrentItemChanged;
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

			return view;
		}


		public override void OnStart ()
		{
			base.OnStart ();

			//if (DisplayContent.Count == 0)
			//{
			//	Task.Run (async () =>
			//	{
			//		await DataClient.Shared.LoadPartnerList ();
			//		DisplayPartners.AddRange (DataClient.Shared.FilteredPartners);
			//	}).ContinueWith (t =>
			//	{
			//		if (!t.IsFaulted)
			//		{
			//			Activity.RunOnUiThread (Adapter.NotifyDataSetChanged);
			//		}
			//	});
			//}
		}


		#region implemented abstract members of RecyclerViewFragment


		protected override RecyclerViewAdapter<MusicAsset, ContentViewHolder> GetAdapter ()
		{
			adapter = new ContentRecyclerAdapter (allAssets);
			//adapter.Filter = new PartnerFilter (adapter);

			return adapter;
		}


		protected override void OnItemClick (View view, MusicAsset item)
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


		async Task RefreshContent ()
		{
			await ContentClient.Shared.GetAllAvContent ();

			await AssetPersistenceManager.Shared.RestorePersistenceManagerAsync (ContentClient.Shared.AvContent [UserRoles.General]);
		}


		void handleAvContentChanged (object sender, UserRoles e) => updateMusicAssets ();


		#region PersistanceManager Handlers


		void updateMusicAssets ()
		{
			if (allAssets?.Count == 0 && ContentClient.Shared.AvContent.Count > 0)
			{
				var content = ContentClient.Shared.AvContent [UserRoles.General].Where (m => m.HasId && m.HasRemoteAssetUri)
																			  .Select (s => AssetPersistenceManager.Shared.GetMusicAsset (s))
																			  .ToList ();

				adapter.SetItems (content);
			}
			else
			{
				//var newAssets = ContentClient.Shared.AvContent [UserRoles.General].Where (m => m.HasId && m.HasRemoteAssetUri && !allAssets.Any (ma => ma.Id == m.Id))
				//																  .Select (s => AssetPersistenceManager.Shared.GetMusicAsset (s));



				//allAssets.AddRange (newAssets);

				//allAssets.RemoveAll (ma => !ContentClient.Shared.AvContent [UserRoles.General].Any (a => a.Id == ma.Id));

				//allAssets.Sort ((x, y) => y.Music.Timestamp.CompareTo (x.Music.Timestamp));


				var content = ContentClient.Shared.AvContent [UserRoles.General].Where (m => m.HasId && m.HasRemoteAssetUri)
																			  .Select (s => AssetPersistenceManager.Shared.GetMusicAsset (s))
																			  .ToList ();

				adapter.SetItems (content);
			}

			Log.Debug ("Load Content");
		}


		void handlePersistanceManagerDidRestore (object sender, EventArgs e)
		{
			updateMusicAssets ();

			ContentClient.Shared.AvContentChanged += handleAvContentChanged;
		}


		void handlePersistanceManagerAssetDownloadStateChanged (object sender, MusicAssetDownloadStateChangeArgs e)
		{
			Log.Debug ($"handlePersistanceManagerAssetDownloadStateChanged: {e.Music.DisplayName} | {e.State}");

			Activity.RunOnUiThread(() =>
			{
				//var cell = TableView.VisibleCells.FirstOrDefault (c => c.TextLabel.Text == e.Music.DisplayName);

				//if (cell != null)
				//{
				//	TableView.ReloadRows (new NSIndexPath [] { TableView.IndexPathForCell (cell) }, UITableViewRowAnimation.Automatic);
				//}
			});
		}


		void handlePersistanceManagerAssetDownloadProgressChanged (object sender, MusicAssetDownloadProgressChangeArgs e)
		{
			Log.Debug ($"handlePersistanceManagerAssetDownloadProgressChanged: {e.Music.DisplayName} | {e.Progress}");

			Activity.RunOnUiThread (() =>
			{
				//var cell = TableView.VisibleCells.FirstOrDefault (c => c.TextLabel.Text == e.Music.DisplayName) as ContentMusicTvCell;

				//cell?.UpdateDownloadProgress ((nfloat) e.Progress);
			});
		}


		#endregion


		#region PlaybackManager Handlers


		void handlePlaybackManagerCurrentItemChanged (object sender, IMediaManager player)
		{
			Log.Debug ($"handlePlaybackManagerCurrentItemChanged {sender}");

			//var playbackManager = sender as AssetPlaybackManager;

			//if (playerViewController != null && player.CurrentItem != null && playbackManager?.CurrentAsset.Music.ContentType == AvContentTypes.Video)
			//{
			//	playerViewController.Player = player;
			//}
		}


		#endregion
	}
}