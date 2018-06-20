using System;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
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


			this.InitNotificationChannels ();

		}

		public IDataSourceFactory BuildDataSourceFactory (DefaultBandwidthMeter bandwidthMeter)
		{
			return new DefaultDataSourceFactory (this, bandwidthMeter,
				BuildHttpDataSourceFactory (bandwidthMeter));
		}

		public IHttpDataSourceFactory BuildHttpDataSourceFactory (DefaultBandwidthMeter bandwidthMeter)
		{
			return new DefaultHttpDataSourceFactory (Util.GetUserAgent (this, "ProducerExoPlayer"), bandwidthMeter);
		}

	}
}