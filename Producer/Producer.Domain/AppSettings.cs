using System;

namespace Producer
{
	public class AppSettings
	{
		public Uri DocumentDbUrl { get; set; }

		public string NotificationsName { get; set; }

		public string NotificationsConnectionString { get; set; }
	}
}
