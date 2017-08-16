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

			var producerSettings = Foundation.NSDictionary.FromFile ("ProducerSettings.plist");

			if (producerSettings == null)
			{
				throw new System.IO.FileNotFoundException ("Must be present to use azure services", "ProducerSettings.plist");
			}

			foreach (var key in SettingsKeys.ProducerSettingsKeys)
			{
				var val = producerSettings [key].ToString ();

				Log.Debug ($"ProducerSettings: {key.PadRight (20)}: {val}");

				Settings.SetSetting (key, val ?? string.Empty);
			}

#endif

			// Send installed version history with crash reports
			Crashes.GetErrorAttachments = (report) => new List<ErrorAttachmentLog>
			{
				ErrorAttachmentLog.AttachmentWithText (CrossVersionTracking.Current.ToString (), "versionhistory.txt")
			};

			if (!string.IsNullOrEmpty (Keys.MobileCenter.AppSecret))
			{
				Log.Debug ("Starting Mobile Center...");

				MobileCenter.Start (Keys.MobileCenter.AppSecret, typeof (Analytics), typeof (Crashes), typeof (Distribute));
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

			//InitializeDataStore ();

			//Task.Run (async () =>
			//{
			//	var blobs = await AzureStorageClient.Shared.ListPublicBlobsAsync ();

			//	foreach (var blob in blobs)
			//	{
			//		System.Diagnostics.Debug.WriteLine ($"{blob}");
			//	}
			//});
#endif
		}

		public static async Task InitializeDataStoreAsync ()
		{
			//AzureClient.Shared.RegisterTable<AvContent> ();

			//await AzureClient.Shared.InitializeAzync (Settings.ServerUrl);
		}
	}
}
