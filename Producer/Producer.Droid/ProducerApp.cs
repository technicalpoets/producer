using System;
using Android.App;
using Android.Runtime;
//using Plugin.MediaManager;
//using Plugin.MediaManager.ExoPlayer;
//using Plugin.MediaManager.MediaSession;
using Producer.Droid.Providers;

namespace Producer.Droid
{
	[Application]
	public class ProducerApp : Application
	{
		/// <summary>
		/// Base constructor which must be implemented if it is to successfully inherit from the Application class.
		/// </summary>
		/// <param name="handle"></param>
		/// <param name="transfer"></param>
		public ProducerApp (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}


		public override void OnCreate ()
		{
			base.OnCreate ();

			//JsonHttpClient.GlobalHttpMessageHandlerFactory = () => new NativeMessageHandler ();
			//JsConfig.PropertyConvention = PropertyConvention.Lenient;

			AssetPersistenceManager.Shared.Setup ();

			//configure the Exoplayer
			//((MediaManagerImplementation) CrossMediaManager.Current).MediaSessionManager = new MediaSessionManager (Context, typeof (ExoPlayerAudioService));
			//var exoPlayer = new ExoPlayerAudioImplementation (((MediaManagerImplementation) CrossMediaManager.Current).MediaSessionManager);
			//CrossMediaManager.Current.AudioPlayer = exoPlayer;
		}
	}
}