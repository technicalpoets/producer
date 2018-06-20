
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Ads;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Source.Smoothstreaming;
using Com.Google.Android.Exoplayer2.Source.Chunk;
using Com.Google.Android.Exoplayer2.Util;
using static Com.Google.Android.Exoplayer2.Source.Ads.AdsMediaSource;
using Com.Google.Android.Exoplayer2.Upstream;
using static Com.Google.Android.Exoplayer2.Source.ExtractorMediaSource;
using Java.Lang;
using Com.Google.Android.Exoplayer2.Extractor;

namespace Producer.Droid.Framework.Utilities
{
	//[Activity (Label = "PlayerManager")]
	public class PlayerManager //: AdsMediaSource.IMediaSourceFactory
	{


		public IntPtr Handle => throw new NotImplementedException ();

		public IMediaSource CreateMediaSource (Android.Net.Uri p0, Handler p1, IMediaSourceEventListener p2)
		{
			throw new NotImplementedException ();
		}

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}

		public int [] GetSupportedTypes ()
		{
			throw new NotImplementedException ();
		}







	}


}
