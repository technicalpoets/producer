using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Distribute;

using Plugin.VersionTracking;

namespace Producer
{

	public static class ProducerDefaults
	{
		public const string MobileCenterKey = "9d538360-1ae5-4dc2-b0ea-bec01585a6bb";
		public const string RemoteFunctionsUrl = "producer.azurewebsites.net";
		public const string RemoteDocumentDbUrl = "producer.documents.azure.com";
		public const string NotificationsName = "producer";
		public const string NotificationsConnectionString = "Endpoint=sb://producer.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=14RzlGYJqu0M7diG6NS+d2399yMUllnAgpn0Vse6y6M=";		
	}

	public static class Bootstrap
	{
		public static void Run ()
		{
			CrossVersionTracking.Current.Track ();

			Log.Debug (CrossVersionTracking.Current.ToString ());

			Settings.RegisterDefaultSettings ();

			Settings.MobileCenterKey = ProducerDefaults.MobileCenterKey;
			Settings.RemoteFunctionsUrl = ProducerDefaults.RemoteFunctionsUrl;
			Settings.RemoteDocumentDbUrl = ProducerDefaults.RemoteDocumentDbUrl;
			Settings.NotificationsName = ProducerDefaults.NotificationsName;
			Settings.NotificationsConnectionString = ProducerDefaults.NotificationsConnectionString;

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

				Settings.UserReferenceKey = installId?.ToString ("N");
			});

#if __IOS__

			Distribute.DontCheckForUpdatesInDebug ();

#elif __ANDROID__

			Settings.VersionNumber = CrossVersionTracking.Current.CurrentVersion;

			Settings.BuildNumber = CrossVersionTracking.Current.CurrentBuild;

			Settings.VersionDescription = $"{CrossVersionTracking.Current.CurrentVersion} ({CrossVersionTracking.Current.CurrentBuild})";
#endif
		}
	}
}
