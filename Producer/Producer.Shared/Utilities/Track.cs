using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

using Producer.Domain;

namespace Producer
{
	public static class Track
	{

		public static void Play (AvContent content) => avContentEvent ("Plays", content);


		public static void Download (AvContent content) => avContentEvent ("Downloads", content, false);


		public static void Favorite (AvContent content) => avContentEvent ("Favorites", content);


		static void avContentEvent (string eventName, AvContent content, bool includeDownloaded = true)
		{
			if (content != null)
			{
				var props = new Dictionary<string, string>
				{
					{ "Name", content.Name },
					{ "Display Name", content.DisplayName },
					{ "Media Type", content.ContentType.ToString() },
					{ "Published To", content.PublishedTo.ToString() }
				};

				if (includeDownloaded)
				{
					props.Add ("Downloaded", content.HasLocalAssetUri ? "True" : "False");
				}

				Event (eventName, props);
			}
		}


		#region Utilities

		public static void Event (string name, IDictionary<string, string> properties = null)
		{
			if (!string.IsNullOrEmpty (Settings.MobileCenterKey))
			{
				Task.Run (async () =>
				{
					if (await Analytics.IsEnabledAsync ())
					{
						Analytics.TrackEvent (name, properties);
					}
				});
			}
			else
			{
				var props = (properties?.Count > 0) ? string.Join (" | ", properties.Select (p => $"{p.Key} = {p.Value}")) : "empty";

				log ($"TrackEvent :: name: {name.PadRight (30)} properties: {props}");
			}
		}


#if DEBUG
		public static void GenerateTestCrash () => Crashes.GenerateTestCrash ();
#endif

		static bool verboseLogging = false;

		static void log (string message, bool onlyVerbose = false)
		{
			if (!onlyVerbose || verboseLogging)
			{
				System.Diagnostics.Debug.WriteLine ($"[Track] {message}");
			}
		}

		#endregion
	}
}
