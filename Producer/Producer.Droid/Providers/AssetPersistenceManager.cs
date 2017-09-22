using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Media;
using Android.Support.V4.Media.Session;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Java.IO;
using Java.Lang;
using Producer.Domain;

namespace Producer.Droid.Providers
{
	public class AssetPersistenceManager
	{
		static AssetPersistenceManager _shared;
		public static AssetPersistenceManager Shared => _shared ?? (_shared = new AssetPersistenceManager ());

		public event EventHandler DidRestore;

		public event EventHandler<MusicAssetDownloadStateChangeArgs> AssetDownloadStateChanged;

		public event EventHandler<MusicAssetDownloadProgressChangeArgs> AssetDownloadProgressChanged;

		//HlsDownloader downloader;
		//Com.Google.Android.Exoplayer2.


		/// Internal Bool used to track if the AssetPersistenceManager finished restoring its state.
		bool didRestorePersistenceManager;

		/// The AVAssetDownloadURLSession to use for managing AVAssetDownloadTasks.
		//AVAssetDownloadUrlSession assetDownloadURLSession;

		/// Internal map of AVAssetDownloadTask to its corresponding Asset.
		//Dictionary<AVAssetDownloadTask, MusicAsset> activeDownloadsMap = new Dictionary<AVAssetDownloadTask, MusicAsset> ();

		///// Internal map of AVAssetDownloadTask to its resoled AVMediaSelection
		//Dictionary<AVAssetDownloadTask, AVMediaSelection> mediaSelectionMap = new Dictionary<AVAssetDownloadTask, AVMediaSelection> ();

		///// The URL to the Library directory of the application's data container.
		//NSUrl baseDownloadURL;

		//MediaSessionCompat mediaSession;


		public void Setup ()
		{

			//var audioSession = AVAudioSession.SharedInstance ();

			//var error = audioSession.SetCategory (AVAudioSessionCategory.Playback);

			//if (error != null)
			//{
			//	Log.Debug ($"{error.LocalizedDescription}");
			//}
			//else
			//{
			//	var homeDirectory = NSHomeDirectoryNative ();

			//	baseDownloadURL = new NSUrl (homeDirectory);

			//	var backgroundConfiguration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration ("HLS-Identifier");

			//	assetDownloadURLSession = AVAssetDownloadUrlSession.CreateSession (backgroundConfiguration, this, NSOperationQueue.MainQueue);
			//}
		}


		/// Restores the Application state by getting all the AVAssetDownloadTasks and restoring their Asset structs.
		public async Task RestorePersistenceManagerAsync (List<AvContent> music)
		{
			//if (baseDownloadURL == null)
			//{
			//	throw new InvalidOperationException ("must call Setup() before anything else");
			//}

			if (didRestorePersistenceManager)
			{
				return;
			}

			didRestorePersistenceManager = true;

			// Grab all the tasks associated with the assetDownloadURLSession
			//var tasks = await assetDownloadURLSession.GetAllTasksAsync ();

			//// For each task, restore the state in the app by recreating Asset structs and reusing existing AVURLAsset objects.
			//foreach (var task in tasks)
			//{
			//	var assetDownloadTask = task as AVAssetDownloadTask;

			//	if (assetDownloadTask != null)
			//	{
			//		var song = music.FirstOrDefault (s => s.Id == assetDownloadTask.TaskDescription);

			//		if (song != null)
			//		{
			//			var asset = new MusicAsset (song, assetDownloadTask.UrlAsset);

			//			activeDownloadsMap [assetDownloadTask] = asset;
			//		}
			//	}
			//}

			DidRestore?.Invoke (this, EventArgs.Empty);
		}


		/// <summary>
		/// Returns the Active, Local, or a new Asset
		/// </summary>
		/// <returns>The music asset.</returns>
		/// <param name="music">Music.</param>
		public MusicAsset GetMusicAsset (AvContent music) => ActiveMusicAsset (music) ?? LocalMusicAsset (music) ?? new MusicAsset (music, music.RemoteAssetUri);


		/// Returns an Asset given a specific name if that Asset is asasociated with an active download.
		public MusicAsset ActiveMusicAsset (AvContent music)
		{
			//foreach (var item in activeDownloadsMap)
			//{
			//	if (item.Value.Id == music.Id)
			//	{
			//		return item.Value;
			//	}
			//}

			return null;
		}


		/// Returns an Asset pointing to a file on disk if it exists.
		public MusicAsset LocalMusicAsset (AvContent music)
		{
			//var localPath = localFilePath (music.Id);

			//if (localPath != null)
			//{
			//	return new MusicAsset (music, new AVUrlAsset (localPath));
			//}

			return null;
		}
	}
}