#if __MOBILE__

using System.Collections.Generic;

using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;

#endif

using System.Threading.Tasks;

using Plugin.VersionTracking;

using SettingsStudio;

using Producer.Domain;


namespace Producer
{
	public static class Bootstrap
	{
		public static void Run ()
		{
			CrossVersionTracking.Current.Track ();

			Log.Debug ($"\n{CrossVersionTracking.Current}");

			Settings.RegisterDefaultSettings ();

#if __MOBILE__
#if __IOS__
			configureProducerSettings ();
#endif
			// Send installed version history with crash reports
			Crashes.GetErrorAttachments = (report) => new List<ErrorAttachmentLog>
			{
				ErrorAttachmentLog.AttachmentWithText (CrossVersionTracking.Current.ToString (), "versionhistory.txt")
			};

			if (!string.IsNullOrEmpty (Settings.MobileCenterKey))
			{
				Log.Debug ("Starting Mobile Center...");

				MobileCenter.Start (Settings.MobileCenterKey, typeof (Analytics), typeof (Crashes), typeof (Distribute));
			}
			else
			{
				Log.Debug ("To use Mobile Center, add your App Secret to Keys.MobileCenter.AppSecret");
			}

			Task.Run (async () =>
			{
				var installId = await MobileCenter.GetInstallIdAsync ();

				Settings.UserReferenceKey = installId?.ToString ("N") ?? "anonymous";
			});

#if __IOS__

			Distribute.DontCheckForUpdatesInDebug ();

#elif __ANDROID__

			Settings.VersionNumber = CrossVersionTracking.Current.CurrentVersion;

			Settings.BuildNumber = CrossVersionTracking.Current.CurrentBuild;

#endif

#endif
		}

		static void configureProducerSettings ()
		{
			var name = typeof (ProducerSettings).Name;
#if __IOS__
			var path = Foundation.NSBundle.MainBundle.PathForResource (name, "json");

			if (!string.IsNullOrEmpty (path))
			{
				using (var data = Foundation.NSData.FromFile (path))
				{
					var json = Foundation.NSString.FromData (data, Foundation.NSStringEncoding.ASCIIStringEncoding).ToString ();
					//#elif __ANDROID__
					//var path = $"{name}.json";

					//if (assetList.Contains(path))
					//{
					//using (var sr = new System.IO.StreamReader (context.Assets.Open (path)))
					//{
					//var json = sr.ReadToEnd ();
					//#endif
					var producerSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<ProducerSettings> (json);

					Log.Debug (producerSettings.ToString ());

					Settings.ConfigureSettings (producerSettings);
				}
			}
#endif
		}
	}
}
