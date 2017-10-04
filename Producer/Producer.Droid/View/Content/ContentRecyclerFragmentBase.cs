using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Producer.Domain;
using Producer.Droid.Providers;
using Producer.Shared;
using ActionMode = Android.Support.V7.View.ActionMode;

namespace Producer.Droid
{
	public abstract class ContentRecyclerFragmentBase : RecyclerViewListFragment<MusicAsset, ContentViewHolder>, ActionMode.ICallback
	{
		protected List<MusicAsset> Assets = new List<MusicAsset> ();

		protected ContentRecyclerAdapter ContentAdapter;

		/// <summary>
		/// Shared Task used to synchronize data refresh
		/// </summary>
		protected static Task RefreshTask;

		ActionMode actionMode;


		public override void OnCreate (Bundle savedInstanceState)
		{
			//ShowDividers = false;
			EnableLongClick = true;

			base.OnCreate (savedInstanceState);

			//AssetPersistenceManager.Shared.DidRestore += handlePersistanceManagerDidRestore;

			AssetPersistenceManager.Shared.AssetDownloadStateChanged += handlePersistanceManagerAssetDownloadStateChanged;

			AssetPersistenceManager.Shared.AssetDownloadProgressChanged += handlePersistanceManagerAssetDownloadProgressChanged;

			//AssetPlaybackManager.Shared.CurrentItemChanged += handlePlaybackManagerCurrentItemChanged;
		}


		#region implemented abstract/virtual members of RecyclerViewFragment


		protected override RecyclerViewAdapter<MusicAsset, ContentViewHolder> CreateAdapter ()
		{
			ContentAdapter = new ContentRecyclerAdapter (Assets);
			//adapter.Filter = new PartnerFilter (adapter);

			return ContentAdapter;
		}


		protected override Task LoadData ()
		{
			//check if the shared RefreshTask has been set, since we're sharing data between fragments
			if (RefreshTask == null)
			{
				//TODO: determine if we need to lock() or otherwise thread sync

				RefreshTask = Task.Run (async () =>
				{
					await ContentClient.Shared.GetAllAvContent ();

					await AssetPersistenceManager.Shared.RestorePersistenceManagerAsync (ContentClient.Shared.AvContent [UserRoles.General]);
				});
			}

			return RefreshTask;
		}


		protected override void OnDataLoaded ()
		{
			base.OnDataLoaded ();

			UpdateContent ();

			ContentClient.Shared.AvContentChanged += handleAvContentChanged;
		}


		protected override void AttachEvents ()
		{
			//call base to attach basic click handler(s), etc.
			base.AttachEvents ();

			//then add our icon click
			ContentAdapter.SetIconClickHandler (OnItemIconClick);
		}


		protected override void OnItemClick (View view, MusicAsset item, int position)
		{
			//var partner = item;//DisplayPartners [position];
			//var partnerLogoImageView = view.FindViewById<AppCompatImageView> (Resource.Id.partner_logo);

			//var detailIntent = new Intent (Context, typeof (PartnerDetailActivity));
			////var intentData = IntentData.Partner.Create (partner.Id, partner.Name, partner.PartnerTier);
			//var intentData = IntentData.Create (partner.Id, partner.Name, partner.PartnerTier);
			//detailIntent.PutIntentData (intentData);

			//TransitionToActivity (detailIntent, partnerLogoImageView);

			//TODO: play media
		}


		protected override void OnItemLongClick (View view, MusicAsset item, int position)
		{
			enableActionMode (position);
		}


		protected void OnItemIconClick (View view, MusicAsset item, int position)
		{
			enableActionMode (position);
		}


		#endregion


		void handleAvContentChanged (object sender, UserRoles e) => UpdateContent ();


		protected abstract void UpdateContent ();


		void enableActionMode (int position)
		{
			if (actionMode == null)
			{
				actionMode = ((AppCompatActivity) Activity).StartSupportActionMode (this);
			}

			toggleSelection (position);
		}


		void toggleSelection (int position)
		{
			TypedAdapter.ToggleSelection (position);

			int count = TypedAdapter.SelectedItemCount;

			if (count == 0)
			{
				actionMode.Finish ();
			}
			else
			{
				actionMode.Title = $"{count} items";
				actionMode.Invalidate ();
			}
		}


		#region ActionMode.ICallback Members


		public bool OnCreateActionMode (ActionMode mode, IMenu menu)
		{
			mode.MenuInflater.Inflate (Resource.Menu.menu_action_content, menu);

			//disable pull to refresh if action mode is enabled
			SwipeRefreshLayout.Enabled = false;

			return true;
		}


		public bool OnPrepareActionMode (ActionMode mode, IMenu menu)
		{
			return false;
		}


		public bool OnActionItemClicked (ActionMode mode, IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.action_download:
					//download
					//TODO: download
					mode.Finish ();
					return true;

				case Resource.Id.action_favorite:
					//favorite
					//TODO: favorite
					mode.Finish ();
					return true;

				default:
					return false;
			}
		}


		public void OnDestroyActionMode (ActionMode mode)
		{
			TypedAdapter.ClearSelectedItems ();
			SwipeRefreshLayout.Enabled = true;
			actionMode = null;
		}


		#endregion


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