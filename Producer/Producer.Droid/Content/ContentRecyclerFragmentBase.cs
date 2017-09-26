using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Producer.Domain;
using Producer.Droid.Providers;
using Producer.Shared;

namespace Producer.Droid
{
	public abstract class ContentRecyclerFragmentBase : RecyclerViewListFragment<MusicAsset, ContentViewHolder>
	{
		protected List<MusicAsset> Assets = new List<MusicAsset> ();

		protected ContentRecyclerAdapter ContentAdapter;

		protected static Task refreshTask;


		public override void OnCreate (Bundle savedInstanceState)
		{
			ShowDividers = false;

			base.OnCreate (savedInstanceState);

			loadContent ();

			//AssetPersistenceManager.Shared.DidRestore += handlePersistanceManagerDidRestore;

			AssetPersistenceManager.Shared.AssetDownloadStateChanged += handlePersistanceManagerAssetDownloadStateChanged;

			AssetPersistenceManager.Shared.AssetDownloadProgressChanged += handlePersistanceManagerAssetDownloadProgressChanged;

			//AssetPlaybackManager.Shared.CurrentItemChanged += handlePlaybackManagerCurrentItemChanged;
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


		#region implemented abstract members of RecyclerViewFragment


		protected override RecyclerViewAdapter<MusicAsset, ContentViewHolder> GetAdapter ()
		{
			ContentAdapter = new ContentRecyclerAdapter (Assets);
			//adapter.Filter = new PartnerFilter (adapter);

			return ContentAdapter;
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


		void loadContent ()
		{
			//only start a content refresh if there isn't on running already
			if (refreshTask == null || refreshTask.IsFaulted || refreshTask.IsCanceled)
			{
				Log.Debug ("Starting refreshTask");
				refreshTask = refreshContent ();
			}

			if (refreshTask.IsCompleted)
			{
				Log.Debug ("refreshTask complete; updating adapter(s)");
				onContentRefreshed ();
			}
			else //using Task continuation here rather than subscribing to AssetPersistenceManager.DidRestore
			{
				Log.Debug ("refreshTask in process, adding completion to update adapter(s)");
				refreshTask.ContinueWith (t => onContentRefreshed ());
			}
		}


		async Task refreshContent ()
		{
			await ContentClient.Shared.GetAllAvContent ();

			await AssetPersistenceManager.Shared.RestorePersistenceManagerAsync (ContentClient.Shared.AvContent [UserRoles.General]);
		}


		void onContentRefreshed ()
		{
			UpdateContent ();

			ContentClient.Shared.AvContentChanged += handleAvContentChanged;
		}


		void handleAvContentChanged (object sender, UserRoles e) => UpdateContent ();


		protected abstract void UpdateContent ();


		#region PersistanceManager Handlers


		void handlePersistanceManagerAssetDownloadStateChanged (object sender, MusicAssetDownloadStateChangeArgs e)
		{
			Log.Debug ($"handlePersistanceManagerAssetDownloadStateChanged: {e.Music.DisplayName} | {e.State}");

			Activity.RunOnUiThread (() =>
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
	}
}