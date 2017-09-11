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

		public static void Download (AvContent content)
		{
			System.Diagnostics.Debug.WriteLine ($"Track Download: {content.Name}");
		}


		public static void Play (AvContent content)
		{
			System.Diagnostics.Debug.WriteLine ($"Track Play: {content.Name}");
		}


		#region Utilities

		public static void Event (string name, IDictionary<string, string> properties = null)
		{
			if (!string.IsNullOrEmpty (Settings.MobileCenterKey))
			{
				Task.Run (async () =>
				{
					var enabled = await Analytics.IsEnabledAsync ();

					Analytics.TrackEvent (name, properties);
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
				System.Diagnostics.Debug.WriteLine ($"[Analytics] {message}");
			}

		}

		#endregion
	}
}
