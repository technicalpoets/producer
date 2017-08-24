using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AVFoundation;
using AVKit;
using Foundation;
using UIKit;

using SettingsStudio;

using Producer.Domain;
using Producer.Shared;

namespace Producer.iOS
{
	public partial class ContentTvc : UITableViewController
	{

		bool saved => segmentControl.SelectedSegment == 1;

		MusicAsset activeAsset;

		List<MusicAsset> allAssets = new List<MusicAsset> ();

		List<MusicAsset> savedAssets = new List<MusicAsset> ();


		AVPlayerViewController playerViewController;


		public ContentTvc (IntPtr handle) : base (handle) { }


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationItem.RightBarButtonItem = ProducerClient.Shared.UserRole.CanWrite () ? composeButton : null;

			NavigationController.SetToolbarHidden (true, false);

			ProducerClient.Shared.CurrentUserChanged += handleCurrentUserChanged;

			//AssetPlaybackManager.Shared.ReadyToPlay += handlePlaybackManagerReadyToPlay;

			AssetPersistenceManager.Shared.DidRestore += handlePersistanceManagerDidRestore;

			AssetPlaybackManager.Shared.CurrentItemChanged += handlePlaybackManagerCurrentItemChanged;

			AssetPersistenceManager.Shared.AssetDownloadStateChanged += handlePersistanceManagerAssetDownloadStateChanged;

			AssetPersistenceManager.Shared.AssetDownloadProgressChanged += handlePersistanceManagerAssetDownloadProgressChanged;
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (NavigationController.ToolbarHidden && AssetPlaybackManager.Shared.IsPlaying)
			{
				NavigationController.SetToolbarHidden (false, false);
			}

			// dismissing the AVPlayerViewController.
			if (playerViewController != null)
			{
				AssetPlaybackManager.Shared.TogglePlayback (null);
				playerViewController.Player = null;
				playerViewController = null;
			}
		}


		void handleCurrentUserChanged (object sender, User e)
		{
			NavigationItem.RightBarButtonItem = ProducerClient.Shared.UserRole.CanWrite () ? composeButton : null;
		}


		nfloat yCache;

		[Export ("scrollViewDidScroll:")]
		public void Scrolled (UIScrollView scrollView)
		{
			bool hide;

			var y = scrollView.ContentOffset.Y;

			hide = y > 0 && yCache > 0 && y - yCache > 0;

			if (hide && !NavigationController.ToolbarHidden && y - yCache > 12)
			{
				NavigationController.SetToolbarHidden (true, true);
			}
			else if (!hide && NavigationController.ToolbarHidden && y - yCache < -12)
			{
				if (AssetPlaybackManager.Shared.IsPlaying)
				{
					NavigationController.SetToolbarHidden (false, true);
				}
			}

			yCache = y;
		}


		partial void profileButtonClicked (NSObject sender)
		{
			if (ProducerClient.Shared.User == null)
			{
				var loginNc = Storyboard.Instantiate<LoginNc> ();

				PresentViewController (loginNc, true, null);
			}
			else
			{
				var userNc = Storyboard.Instantiate<UserNc> ();

				PresentViewController (userNc, true, null);
			}
		}


		void handleAvContentChanged (object sender, UserRoles e) => updateMusicAssets ();


		#region UITableViewDataSource & UITableViewDelegate


		public override nint RowsInSection (UITableView tableView, nint section) => saved ? savedAssets.Count : allAssets.Count;


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.Dequeue<ContentMusicTvCell> (indexPath);

			var asset = saved ? savedAssets [indexPath.Row] : allAssets [indexPath.Row];

			cell.Tag = indexPath.Row;

			cell.SetData (asset.Music, AssetPersistenceManager.Shared.DownloadState (asset));

			return cell;
		}

		NSIndexPath indexPathCache;


		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt (indexPath) as ContentMusicTvCell;

			tableView.DeselectRow (indexPath, true);


			var asset = saved ? savedAssets [indexPath.Row] : allAssets [indexPath.Row];

			if (asset != null)
			{
				if (asset.Music.ContentType == AvContentTypes.Video)
				{
					playerViewController = new AVPlayerViewController ();

					PresentViewController (playerViewController, true, null);
				}

				BeginInvokeOnMainThread (() =>
				{
					var playing = AssetPlaybackManager.Shared.TogglePlayback (asset);

					if (indexPathCache != null && indexPathCache != indexPath)
					{
						if (tableView.IndexPathsForVisibleRows.Contains (indexPathCache))
						{
							var oldCell = tableView.CellAt (indexPathCache) as ContentMusicTvCell;

							oldCell?.SetPlaying (false);
						}
					}

					indexPathCache = indexPath;

					if (asset.Music.ContentType == AvContentTypes.Audio)
					{
						if (NavigationController.ToolbarHidden)
						{
							NavigationController.SetToolbarHidden (false, true);
						}

						cell?.SetPlaying (playing);

						var items = ToolbarItems;

						items [0] = playing ? pauseButton : playButton;

						//items [2].Title = asset.Music.DisplayName;
						titleButton.Title = asset.Music.DisplayName;

						SetToolbarItems (items, true);
					}
					else if (!NavigationController.ToolbarHidden)
					{
						NavigationController.SetToolbarHidden (true, false);
					}
				});
			}
		}


		public override UITableViewRowAction [] EditActionsForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var defaultAction = UITableViewRowAction.Create (UITableViewRowActionStyle.Normal, Strings.EmptyActionTitle, handleTableActionSave);
			defaultAction.BackgroundColor = UIColor.FromPatternImage (Images.TableActionSave);

			var moreAction = UITableViewRowAction.Create (UITableViewRowActionStyle.Normal, Strings.EmptyActionTitle, handleTableActionMore);
			moreAction.BackgroundColor = UIColor.FromPatternImage (Images.TableActionMore);

			return new UITableViewRowAction [] { defaultAction, moreAction };
		}


		partial void accessoryButtonClicked (NSObject sender)
		{
			var button = sender as UIButton;

			if (button?.Tag >= 0 && allAssets.Count > button.Tag)
			{
				var asset = saved ? savedAssets [(int) button.Tag] : allAssets [(int) button.Tag];

				if (asset != null)
				{
					//AssetPersistenceManager.Shared.MockDownloadAsset (asset);
					AssetPersistenceManager.Shared.DownloadAsset (asset);
				}
			}
		}


		partial void refreshValueChanged (NSObject sender)
		{
			Task.Run (async () =>
			{
				await ContentClient.Shared.GetAllAvContent ();

				BeginInvokeOnMainThread (() =>
				{
					TableView.ReloadData ();

					var refreshControl = sender as UIRefreshControl;

					refreshControl?.EndRefreshing ();
				});
			});
		}


		partial void segmentControlValueChanged (NSObject sender)
		{
			TableView.ReloadData ();

			if (TableView.NumberOfRowsInSection (0) > 0)
			{
				var topIndex = NSIndexPath.FromRowSection (0, 0);

				TableView.ScrollToRow (topIndex, UITableViewScrollPosition.Top, false);
			}

			if (Editing)
			{
				SetEditing (false, false);
			}

			NavigationItem.RightBarButtonItem = saved ? EditButtonItem : Settings.TestProducer ? composeButton : null;
		}


		partial void togglePlayClicked (NSObject sender)
		{
			if (indexPathCache != null)
			{
				RowSelected (TableView, indexPathCache);
			}
		}

		#endregion


		#region TableCell Action Handlers


		void handleTableActionSave (UITableViewRowAction action, NSIndexPath indexPath)
		{
			var asset = saved ? savedAssets [indexPath.Row] : allAssets [indexPath.Row];

			Log.Debug ($"Save: {asset?.Music?.DisplayName}");
		}


		void handleTableActionMore (UITableViewRowAction action, NSIndexPath indexPath)
		{
			activeAsset = saved ? savedAssets [indexPath.Row] : allAssets [indexPath.Row];

			Log.Debug ($"More: {activeAsset?.Music?.DisplayName}");

			var alertController = UIAlertController.Create (activeAsset.Music.DisplayName, null, UIAlertControllerStyle.ActionSheet);

			var downloadState = AssetPersistenceManager.Shared.DownloadState (activeAsset);

			alertController.AddAction (UIAlertAction.Create ("Share", UIAlertActionStyle.Default, handleAlertControllerActionShare));

			switch (downloadState)
			{
				case MusicAssetDownloadState.NotDownloaded:
					alertController.AddAction (UIAlertAction.Create ("Download", UIAlertActionStyle.Default, handleAlertControllerActionDownload));
					break;
				case MusicAssetDownloadState.Downloading:
					alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Destructive, handleAlertControllerActionCancel));
					break;
				case MusicAssetDownloadState.Downloaded:
					alertController.AddAction (UIAlertAction.Create ("Delete", UIAlertActionStyle.Destructive, handleAlertControllerActionDelete));
					break;
			}

			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, handleAlertControllerActionDismiss));

			PresentViewController (alertController, true, null);
		}


		void handleAlertControllerActionDismiss (UIAlertAction obj) => DismissViewController (true, null);


		void handleAlertControllerActionShare (UIAlertAction obj) { }


		void handleAlertControllerActionDownload (UIAlertAction obj)
		{
			if (activeAsset != null)
			{
				Log.Debug ($"Starting Download | {activeAsset.Music.DisplayName}");
				AssetPersistenceManager.Shared.DownloadAsset (activeAsset);
			}
		}


		void handleAlertControllerActionCancel (UIAlertAction obj)
		{
			if (activeAsset != null)
			{
				Log.Debug ($"Cancelling Download | {activeAsset.Music.DisplayName}");
				AssetPersistenceManager.Shared.CancelDownload (activeAsset);
			}
		}


		void handleAlertControllerActionDelete (UIAlertAction obj)
		{
			if (activeAsset != null)
			{
				Log.Debug ($"Deleting Download | {activeAsset.Music.DisplayName}");
				AssetPersistenceManager.Shared.DeleteAsset (activeAsset);
			}
		}


		#endregion


		#region PersistanceManager Handlers


		void updateMusicAssets ()
		{
			if (allAssets?.Count == 0 && ContentClient.Shared.AvContent.Count > 0)
			{
				allAssets = ContentClient.Shared.AvContent [UserRoles.General].Where (m => m.HasId && m.HasRemoteAssetUri)
																			  .Select (s => AssetPersistenceManager.Shared.GetMusicAsset (s))
																			  .ToList ();
			}
			else
			{
				var newAssets = ContentClient.Shared.AvContent [UserRoles.General].Where (m => m.HasId && m.HasRemoteAssetUri && !allAssets.Any (ma => ma.Id == m.Id))
																				  .Select (s => AssetPersistenceManager.Shared.GetMusicAsset (s));

				allAssets.AddRange (newAssets);

				allAssets.RemoveAll (ma => !ContentClient.Shared.AvContent [UserRoles.General].Any (a => a.Id == ma.Id));

				allAssets.Sort ((x, y) => y.Music.Timestamp.CompareTo (x.Music.Timestamp));
			}

			Log.Debug ("Load Content");

			BeginInvokeOnMainThread (() => { TableView.ReloadData (); });
		}


		void handlePersistanceManagerDidRestore (object sender, EventArgs e)
		{
			updateMusicAssets ();

			ContentClient.Shared.AvContentChanged += handleAvContentChanged;
		}


		void handlePersistanceManagerAssetDownloadStateChanged (object sender, MusicAssetDownloadStateChangeArgs e)
		{
			Log.Debug ($"handlePersistanceManagerAssetDownloadStateChanged: {e.Music.DisplayName} | {e.State}");

			BeginInvokeOnMainThread (() =>
			{
				var cell = TableView.VisibleCells.FirstOrDefault (c => c.TextLabel.Text == e.Music.DisplayName);

				if (cell != null)
				{
					TableView.ReloadRows (new NSIndexPath [] { TableView.IndexPathForCell (cell) }, UITableViewRowAnimation.Automatic);
				}
			});
		}


		void handlePersistanceManagerAssetDownloadProgressChanged (object sender, MusicAssetDownloadProgressChangeArgs e)
		{
			Log.Debug ($"handlePersistanceManagerAssetDownloadProgressChanged: {e.Music.DisplayName} | {e.Progress}");

			BeginInvokeOnMainThread (() =>
			{
				var cell = TableView.VisibleCells.FirstOrDefault (c => c.TextLabel.Text == e.Music.DisplayName) as ContentMusicTvCell;

				cell?.UpdateDownloadProgress ((nfloat) e.Progress);
			});
		}


		#endregion


		#region PlaybackManager Handlers


		void handlePlaybackManagerCurrentItemChanged (object sender, AVPlayer player)
		{
			Log.Debug ($"handlePlaybackManagerCurrentItemChanged {sender}");

			var playbackManager = sender as AssetPlaybackManager;

			if (playerViewController != null && player.CurrentItem != null && playbackManager?.CurrentAsset.Music.ContentType == AvContentTypes.Video)
			{
				playerViewController.Player = player;
			}
		}


		#endregion


		public override UIStatusBarStyle PreferredStatusBarStyle () => UIStatusBarStyle.LightContent;
	}
}
