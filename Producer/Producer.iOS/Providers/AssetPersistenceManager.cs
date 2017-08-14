using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using AVFoundation;
using CoreMedia;
using Foundation;

using Producer.Domain;

namespace Producer.iOS
{
	public class AssetPersistenceManager : AVAssetDownloadDelegate
	{

		static AssetPersistenceManager _shared;
		public static AssetPersistenceManager Shared => _shared ?? (_shared = new AssetPersistenceManager ());


		public event EventHandler DidRestore;

		public event EventHandler<MusicAssetDownloadStateChangeArgs> AssetDownloadStateChanged;

		public event EventHandler<MusicAssetDownloadProgressChangeArgs> AssetDownloadProgressChanged;


		/// Internal Bool used to track if the AssetPersistenceManager finished restoring its state.
		bool didRestorePersistenceManager;

		/// The AVAssetDownloadURLSession to use for managing AVAssetDownloadTasks.
		AVAssetDownloadUrlSession assetDownloadURLSession;

		/// Internal map of AVAssetDownloadTask to its corresponding Asset.
		Dictionary<AVAssetDownloadTask, MusicAsset> activeDownloadsMap = new Dictionary<AVAssetDownloadTask, MusicAsset> ();

		/// Internal map of AVAssetDownloadTask to its resoled AVMediaSelection
		Dictionary<AVAssetDownloadTask, AVMediaSelection> mediaSelectionMap = new Dictionary<AVAssetDownloadTask, AVMediaSelection> ();

		/// The URL to the Library directory of the application's data container.
		NSUrl baseDownloadURL;


		public void Setup ()
		{
			var audioSession = AVAudioSession.SharedInstance ();

			var error = audioSession.SetCategory (AVAudioSessionCategory.Playback);

			if (error != null)
			{
				Log.Debug ($"{error.LocalizedDescription}");
			}
			else
			{
				var homeDirectory = NSHomeDirectoryNative ();

				baseDownloadURL = new NSUrl (homeDirectory);

				var backgroundConfiguration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration ("HLS-Identifier");

				assetDownloadURLSession = AVAssetDownloadUrlSession.CreateSession (backgroundConfiguration, this, NSOperationQueue.MainQueue);
			}
		}


		/// Restores the Application state by getting all the AVAssetDownloadTasks and restoring their Asset structs.
		public async Task RestorePersistenceManagerAsync (List<AvContent> music)
		{
			if (baseDownloadURL == null)
			{
				throw new InvalidOperationException ("must call Setup() before anything else");
			}

			if (didRestorePersistenceManager)
			{
				return;
			}

			didRestorePersistenceManager = true;

			// Grab all the tasks associated with the assetDownloadURLSession
			var tasks = await assetDownloadURLSession.GetAllTasksAsync ();

			// For each task, restore the state in the app by recreating Asset structs and reusing existing AVURLAsset objects.
			foreach (var task in tasks)
			{
				var assetDownloadTask = task as AVAssetDownloadTask;

				if (assetDownloadTask != null)
				{
					var song = music.FirstOrDefault (s => s.Id == assetDownloadTask.TaskDescription);

					if (song != null)
					{
						var asset = new MusicAsset (song, assetDownloadTask.UrlAsset);

						activeDownloadsMap [assetDownloadTask] = asset;
					}
				}
			}

			DidRestore?.Invoke (this, EventArgs.Empty);
		}


		/// Triggers the initial AVAssetDownloadTask for a given Asset.
		public void DownloadAsset (MusicAsset asset)
		{
			// For the initial download, we ask the URLSession for an AVAssetDownloadTask
			// with a minimum bitrate corresponding with one of the lower bitrate variants
			// in the asset.

			// TODO: workaround for bug https://bugzilla.xamarin.com/show_bug.cgi?id=44201 to get it to build in Mobile Center
			var taskOptions = new NSDictionary (new NSString ("AVAssetDownloadTaskMinimumRequiredMediaBitrateKey"), new NSNumber (265000));

			// should be this
			// var taskOptions = new AVAssetDownloadOptions { MinimumRequiredMediaBitrate = 265000 };

			var task = assetDownloadURLSession.GetAssetDownloadTask (asset.UrlAsset, asset.Id, null, taskOptions);

			// To better track the AVAssetDownloadTask we set the taskDescription to something unique for our sample.
			task.TaskDescription = asset.Id;

			activeDownloadsMap [task] = asset;

			task.Resume ();

			AssetDownloadStateChanged?.Invoke (this, new MusicAssetDownloadStateChangeArgs (asset.Music, MusicAssetDownloadState.Downloading));
		}


		/// Returns the Active, Local, or a new Asset
		public MusicAsset GetMusicAsset (AvContent music) => ActiveMusicAsset (music) ?? LocalMusicAsset (music) ?? new MusicAsset (music, new AVUrlAsset (new NSUrl (music.RemoteAssetUri)));


		/// Returns an Asset given a specific name if that Asset is asasociated with an active download.
		public MusicAsset ActiveMusicAsset (AvContent music)
		{
			foreach (var item in activeDownloadsMap)
			{
				if (item.Value.Id == music.Id)
				{
					return item.Value;
				}
			}

			return null;
		}


		/// Returns an Asset pointing to a file on disk if it exists.
		public MusicAsset LocalMusicAsset (AvContent music)
		{
			var localPath = localFilePath (music.Id);

			if (localPath != null)
			{
				return new MusicAsset (music, new AVUrlAsset (localPath));
			}

			return null;
		}


		/// Returns the current download state for a given Asset.
		public MusicAssetDownloadState DownloadState (MusicAsset asset)
		{
#if DEBUG
			if (mockDownloadAsset == asset)
			{
				return MusicAssetDownloadState.Downloading;
			}
#endif

			var localPath = localFilePath (asset.Id);

			// Check if there is a file URL stored for this asset.
			if (localPath != null)
			{
				// Check if the file exists on disk
				if (localPath.Path == baseDownloadURL.Path)
				{
					return MusicAssetDownloadState.NotDownloaded;
				}

				if (NSFileManager.DefaultManager.FileExists (localPath.Path))
				{
					return MusicAssetDownloadState.Downloaded;
				}
			}

			// Check if there are any active downloads in flight.
			foreach (var item in activeDownloadsMap)
			{
				if (item.Value.Id == asset.Id)
				{
					return MusicAssetDownloadState.Downloading;
				}
			}

			return MusicAssetDownloadState.NotDownloaded;
		}


		/// Deletes an Asset on disk if possible.
		public void DeleteAsset (MusicAsset asset)
		{
			var localPath = localFilePath (asset.Id);

			// Check if there is a file URL stored for this asset.
			if (localPath != null)
			{
				NSError error;

				NSFileManager.DefaultManager.Remove (localPath, out error);

				if (error != null)
				{
					Log.Debug ($"An error occured trying to delete the contents on disk for {asset.Id}: {error.LocalizedDescription}");
				}

				SettingsStudio.Settings.SetSetting (asset.Id, string.Empty);

				AssetDownloadStateChanged?.Invoke (this, new MusicAssetDownloadStateChangeArgs (asset.Music, MusicAssetDownloadState.NotDownloaded));
			}
		}


		/// Cancels an AVAssetDownloadTask given an Asset.
		public void CancelDownload (MusicAsset asset)
		{
			foreach (var item in activeDownloadsMap)
			{
				if (asset == item.Value)
				{
					item.Key.Cancel ();
				}
			}
		}


		/// <summary>
		/// This function demonstrates returns the next <c>AVMediaSelectionGroup</c> and
		/// <c>AVMediaSelectionOption</c> that should be downloaded if needed. This is done
		/// by querying an <c>AVURLAsset</c>'s <c>AVAssetCache</c> for its available <c>AVMediaSelection</c>
		/// and comparing it to the remote versions.
		/// </summary>
		/// <returns>The media selection.</returns>
		/// <param name="asset">Asset.</param>
		Tuple<AVMediaSelectionGroup, AVMediaSelectionOption> nextMediaSelection (AVUrlAsset asset)
		{
			//guard let assetCache = asset.assetCache else { return (nil, nil) }
			var assetCache = asset.Cache;

			if (assetCache == null)
			{
				return new Tuple<AVMediaSelectionGroup, AVMediaSelectionOption> (null, null);
			}

			var mediaCharacteristics = new [] { AVMediaCharacteristic.Audible, AVMediaCharacteristic.Legible };

			foreach (var mediaCharacteristic in mediaCharacteristics)
			{
				var mediaSelectionGroup = asset.MediaSelectionGroupForMediaCharacteristic (mediaCharacteristic);

				if (mediaSelectionGroup != null)
				{
					var savedOptions = assetCache.GetMediaSelectionOptions (mediaSelectionGroup);

					if (savedOptions.Length < mediaSelectionGroup.Options.Length)
					{
						// There are still media options left to download.
						foreach (var option in mediaSelectionGroup.Options)
						{
							if (!savedOptions.Contains (option))
							{
								// This option has not been download.
								return new Tuple<AVMediaSelectionGroup, AVMediaSelectionOption> (mediaSelectionGroup, option);
							}
						}
					}
				}
			}

			// At this point all media options have been downloaded.
			return new Tuple<AVMediaSelectionGroup, AVMediaSelectionOption> (null, null);
		}


		public override void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error)
		{
			// This is the ideal place to begin downloading additional media selections
			// once the asset itself has finished downloading.

			MusicAsset asset;

			var assetTask = task as AVAssetDownloadTask;

			if (assetTask == null || !activeDownloadsMap.TryGetValue (assetTask, out asset))
			{
				return;
			}

			Log.Debug ($"Removing {assetTask.TaskDescription} from activeDownloads");

			activeDownloadsMap.Remove (assetTask);

			if (error != null)
			{
				Log.Debug ($"DidCompleteWithError: {task?.TaskDescription} | {error.Domain} :: {error.LocalizedDescription}");

				if (error.Domain == NSError.NSUrlErrorDomain)
				{
					if (error.Code == (int)NSUrlError.Cancelled)
					{
						// This task was canceled, you should perform cleanup using the
						// URL saved from AVAssetDownloadDelegate.urlSession(_:assetDownloadTask:didFinishDownloadingTo:).

						DeleteAsset (asset);

					}
					else if (error.Code == (int)NSUrlError.Unknown)
					{
						Log.Debug ($"FATAL: Downloading HLS streams is not supported in the simulator.");
						//fatalError ("Downloading HLS streams is not supported in the simulator.")
					}
				}

				Log.Debug ($"FATAL: An unexpected error occured {error.Domain}");
				//fatalError ("An unexpected error occured \(error.domain)")
			}
			else
			{
				var change = new MusicAssetDownloadStateChangeArgs (asset.Music);

				var mediaSelectionPair = nextMediaSelection (assetTask.UrlAsset);

				if (mediaSelectionPair.Item1 != null)
				{
					// This task did complete sucessfully. At this point the application
					// can download additional media selections if needed.
					//
					// To download additional `AVMediaSelection`s, you should use the
					// `AVMediaSelection` reference saved in `AVAssetDownloadDelegate.urlSession(_:assetDownloadTask:didResolve:)`.

					AVMediaSelection originalMediaSelection;

					if (!mediaSelectionMap.TryGetValue (assetTask, out originalMediaSelection))
					{
						return;
					}

					// There are still media selections to download.
					//
					// Create a mutable copy of the AVMediaSelection reference saved in
					// `AVAssetDownloadDelegate.urlSession(_:assetDownloadTask:didResolve:)`.

					var mediaSelection = originalMediaSelection.MutableCopy () as AVMutableMediaSelection;

					// Select the AVMediaSelectionOption in the AVMediaSelectionGroup we found earlier.
					mediaSelection.SelectMediaOption (mediaSelectionPair.Item2, mediaSelectionPair.Item1);

					// Ask the `URLSession` to vend a new `AVAssetDownloadTask` using
					// the same `AVURLAsset` and assetTitle as before.
					//
					// This time, the application includes the specific `AVMediaSelection`
					// to download as well as a higher bitrate.


					// TODO: workaround for bug https://bugzilla.xamarin.com/show_bug.cgi?id=44201 to get it to build in Mobile Center
					var taskOptions = new NSDictionary (new NSString ("AVAssetDownloadTaskMinimumRequiredMediaBitrateKey"), new NSNumber (2000000), new NSString ("AVAssetDownloadTaskMediaSelectionKey"), mediaSelection);

					// should be this
					// var taskOptions = new AVAssetDownloadOptions { MinimumRequiredMediaBitrate = 2000000, MediaSelection = mediaSelection };

					var newTask = assetDownloadURLSession.GetAssetDownloadTask (assetTask.UrlAsset, asset.Id, null, taskOptions);

					if (newTask == null)
					{
						return;
					}

					activeDownloadsMap [newTask] = asset;

					newTask.Resume ();

					change.State = MusicAssetDownloadState.Downloading;

					Log.Debug ($"????????? ?????????? ??????????? {mediaSelectionPair.Item2.DisplayName}");

					//change.DisplayName = mediaSelectionPair.Item2.DisplayName;
				}
				else
				{
					// All additional media selections have been downloaded.
					change.State = MusicAssetDownloadState.Downloaded;
				}

				AssetDownloadStateChanged?.Invoke (this, change);
			}
		}


		public override void DidFinishDownloadingToUrl (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, NSUrl location)
		{
			//Log.Debug ($"DidFinishDownloadingToUrl: {assetDownloadTask?.TaskDescription} | {location}");

			// This delegate callback should only be used to save the location URL
			// somewhere in your application. Any additional work should be done in
			// `URLSessionTaskDelegate.urlSession(_:task:didCompleteWithError:)`.

			MusicAsset asset;

			if (activeDownloadsMap.TryGetValue (assetDownloadTask, out asset))
			{
				SettingsStudio.Settings.SetSetting (asset.Id, location.RelativePath);
			}
		}


		public override void DidLoadTimeRange (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, CMTimeRange timeRange, NSValue [] loadedTimeRanges, CMTimeRange timeRangeExpectedToLoad)
		{
			//Log.Debug ($"DidLoadTimeRange: {assetDownloadTask?.TaskDescription}");

			var asset = activeDownloadsMap [assetDownloadTask];

			if (asset != null)
			{
				var percentComplete = 0.0;

				// Iterate through the loaded time ranges
				foreach (var val in loadedTimeRanges)
				{
					// Unwrap the CMTimeRange from the NSValue
					var loadedTimeRange = val.CMTimeRangeValue;

					// Calculate the percentage of the total expected asset duration
					percentComplete += loadedTimeRange.Duration.Seconds / timeRangeExpectedToLoad.Duration.Seconds;
				}

				AssetDownloadProgressChanged?.Invoke (this, new MusicAssetDownloadProgressChangeArgs (asset.Music, percentComplete));
			}
		}


		public override void DidResolveMediaSelection (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, AVMediaSelection resolvedMediaSelection)
		{
			Log.Debug ($"DidResolveMediaSelection: {assetDownloadTask?.TaskDescription} | {resolvedMediaSelection.Asset}");

			// You should be sure to use this delegate callback to keep a reference
			// to `resolvedMediaSelection` so that in the future you can use it to
			// download additional media selections.

			mediaSelectionMap [assetDownloadTask] = resolvedMediaSelection;
		}


		#region Utilities

		NSUrl localFilePath (string id)
		{
			var relPath = SettingsStudio.Settings.StringForKey (id);

			if (!string.IsNullOrEmpty (relPath))
			{
				return NSUrl.CreateFileUrl (new string [] { NSHomeDirectoryNative () }).Append (relPath, false);
			}

			return null;
		}


		[System.Runtime.InteropServices.DllImport (ObjCRuntime.Constants.FoundationLibrary)]
		static extern IntPtr NSHomeDirectory ();

		string NSHomeDirectoryNative ()
		{
			return (NSString)ObjCRuntime.Runtime.GetNSObject (NSHomeDirectory ());
		}

#if DEBUG

		MusicAsset mockDownloadAsset;

		public void MockDownloadAsset (MusicAsset asset)
		{
			mockDownloadAsset = asset;

			AssetDownloadStateChanged?.Invoke (this, new MusicAssetDownloadStateChangeArgs (asset.Music, MusicAssetDownloadState.Downloading));

			var rand = new Random ();

			double progress = 0;

			Task.Run (async () =>
			{
				while (progress <= 1)
				{
					await Task.Delay (TimeSpan.FromSeconds (1));

					AssetDownloadProgressChanged?.Invoke (this, new MusicAssetDownloadProgressChangeArgs (asset.Music, progress));

					double inc = (double)rand.Next (1, 15) / 100;

					progress += inc;
				}

				await Task.Delay (TimeSpan.FromSeconds (1));

				mockDownloadAsset = null;

				AssetDownloadProgressChanged?.Invoke (this, new MusicAssetDownloadProgressChangeArgs (asset.Music, 1));

				AssetDownloadStateChanged?.Invoke (this, new MusicAssetDownloadStateChangeArgs (asset.Music, MusicAssetDownloadState.Downloaded));
			});
		}

#endif

		#endregion
	}
}
