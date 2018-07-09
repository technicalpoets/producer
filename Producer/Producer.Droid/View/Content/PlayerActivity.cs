
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Source.Smoothstreaming;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using Java.Lang;
using Uri = Android.Net.Uri;

namespace Producer.Droid
{
	[Activity (Label = "PlayerActivity")]
	public class PlayerActivity : BaseActivity
	{
		private static readonly DefaultBandwidthMeter BANDWIDTH_METER = new DefaultBandwidthMeter ();
		private IDataSourceFactory mediaDataSourceFactory;
		private SimpleExoPlayer player;
		private long contentPosition;
		private string url;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			mediaDataSourceFactory = BuildDataSourceFactory (true);

			SetContentView (Resource.Layout.Player);

			// Create your application here

			 url = Intent.GetStringExtra ("MyData") ?? "Data not available";


			Init (this,FindViewById<PlayerView> (Resource.Id.player_view));
		}

		public void Init (Context context, Com.Google.Android.Exoplayer2.UI.PlayerView playerView)
		{
			// Create a default track selector.
			var bandwidthMeter = new DefaultBandwidthMeter ();
			var videoTrackSelectionFactory =
				new AdaptiveTrackSelection.Factory (bandwidthMeter);
			TrackSelector trackSelector = new DefaultTrackSelector (videoTrackSelectionFactory);

			// Create a player instance.
			player = ExoPlayerFactory.NewSimpleInstance (context, trackSelector);

			// Bind the player to the view.
			playerView.Player = (player);

			// This is the MediaSource representing the content media (i.e. not the ad).
			var contentUrl = url;//context.GetString (Resource.String.test_content_url);
			HlsMediaSource.Factory hlsfac = new HlsMediaSource.Factory (mediaDataSourceFactory);
			//var hlsMediaSource = hlsfac.CreateMediaSource (Uri.Parse(contentUrl));
			var hlsMediaSource = new HlsMediaSource (Uri.Parse (contentUrl), mediaDataSourceFactory, null, null);
			//var contentMediaSource =
				//BuildMediaSource (Uri.Parse (contentUrl),"");

			// Compose the content media source into a new AdsMediaSource with both ads and content.
			var mediaSourceWithAds = hlsMediaSource;
			//        new AdsMediaSource(
			//                hlsMediaSource,
			//            /* adMediaSourceFactory= */ this,
			//             adsLoader,
			//            playerView.getOverlayFrameLayout(),
			//            /* eventHandler= */ null,
			//            /* eventListener= */ null);

			// Prepare the player with the source.
			player.SeekTo (contentPosition);
			player.Prepare (hlsMediaSource);
			player.PlayWhenReady = (true);
		}

		private IMediaSource BuildMediaSource (Android.Net.Uri uri, string overrideExtension)
		{
			
			int type = TextUtils.IsEmpty (overrideExtension) ? Util.InferContentType (uri)
				: Util.InferContentType ("." + overrideExtension);
			switch (type)
			{
				case C.TypeSs:
					return new SsMediaSource (uri, BuildDataSourceFactory (false),
											  new DefaultSsChunkSource.Factory (mediaDataSourceFactory), null, null);
				case C.TypeDash:
					return new DashMediaSource (uri, BuildDataSourceFactory (false),
												new DefaultDashChunkSource.Factory (mediaDataSourceFactory), null, null);
				case C.TypeHls:
					return new HlsMediaSource (uri, mediaDataSourceFactory, null, null);
				case C.TypeOther:
					return new ExtractorMediaSource (uri, mediaDataSourceFactory, new DefaultExtractorsFactory (),
													 null, null);
				default:
					{
						throw new IllegalStateException ("Unsupported type: " + type);
					}
			}
		}

		/**
		 * Returns a new DataSource factory.
		 *
		 * @param useBandwidthMeter Whether to set {@link #BANDWIDTH_METER} as a listener to the new
		 *     DataSource factory.
		 * @return A new DataSource factory.
		 */
		private IDataSourceFactory BuildDataSourceFactory (bool useBandwidthMeter)
		{
			return ((ProducerApp) Application)
				.BuildDataSourceFactory (useBandwidthMeter ? BANDWIDTH_METER : null);
		}

		/**
		 * Returns a new HttpDataSource factory.
		 *
		 * @param useBandwidthMeter Whether to set {@link #BANDWIDTH_METER} as a listener to the new
		 *     DataSource factory.
		 * @return A new HttpDataSource factory.
		 */
		private IHttpDataSourceFactory BuildHttpDataSourceFactory (bool useBandwidthMeter)
		{
			return ((ProducerApp) Application)
				.BuildHttpDataSourceFactory (useBandwidthMeter ? BANDWIDTH_METER : null);
		}
	}
}
