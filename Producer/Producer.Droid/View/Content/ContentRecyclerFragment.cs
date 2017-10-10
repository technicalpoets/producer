using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Views;
using Android.OS;
using Android.Content;
using Producer.Auth;
using Producer.Domain;
using Producer.Shared;
using Producer.Droid.Providers;
using System.Linq;

namespace Producer.Droid
{
	public class ContentRecyclerFragment : ContentRecyclerFragmentBase, ITabFragment  //, SearchView.IOnQueryTextListener
	{
		#region ITabFragment Members


		public string Title => "Content";


		public int Icon => Resource.Drawable.ic_tabbar_resources;


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


		protected override void UpdateContent ()
		{
			if (Assets?.Count == 0 && ContentClient.Shared.AvContent.Count > 0)
			{
				var content = ContentClient.Shared.AvContent [UserRoles.General]
										   .Where (m => m.HasId && m.HasRemoteAssetUri)
										   .Select (s => AssetPersistenceManager.Shared.GetMusicAsset (s))
										   .ToList ();

				Activity.RunOnUiThread (() => ContentAdapter.SetItems (content));
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

				Activity.RunOnUiThread (() => ContentAdapter.SetItems (content));
			}

			Log.Debug ("Load Content");
		}


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


		#region PlaybackManager Handlers


		//void handlePlaybackManagerCurrentItemChanged (object sender, IMediaManager player)
		//{
		//	Log.Debug ($"handlePlaybackManagerCurrentItemChanged {sender}");

		//	//var playbackManager = sender as AssetPlaybackManager;

		//	//if (playerViewController != null && player.CurrentItem != null && playbackManager?.CurrentAsset.Music.ContentType == AvContentTypes.Video)
		//	//{
		//	//	playerViewController.Player = player;
		//	//}
		//}


		#endregion
	}
}