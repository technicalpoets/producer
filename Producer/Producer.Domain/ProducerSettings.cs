namespace Producer.Domain
{
	public class ProducerSettings
	{
		public string RemoteFunctionsUrl { get; set; }

		public string RemoteDocumentDbUrl { get; set; }

		public string RemoteDocumentDbKey { get; set; }

		public string EmbeddedSocialKey { get; set; }

		public string NotificationsName { get; set; }

		public string NotificationsUrl { get; set; }

		public string NotificationsKey { get; set; }

		public string NotificationsConnectionString { get; set; }

		public string MobileCenterKeyAndroid { get; set; }

		public string MobileCenterKeyiOS { get; set; }

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\nProducerSettings\n");
			sb.Append ("  RemoteFunctionsUrl".PadRight (34));
			sb.Append ($"{RemoteFunctionsUrl}\n");
			sb.Append ("  RemoteDocumentDbUrl".PadRight (34));
			sb.Append ($"{RemoteDocumentDbUrl}\n");
			sb.Append ("  RemoteDocumentDbKey".PadRight (34));
			sb.Append ($"{RemoteDocumentDbKey}\n");
			sb.Append ("  EmbeddedSocialKey".PadRight (34));
			sb.Append ($"{EmbeddedSocialKey}\n");
			sb.Append ("  NotificationsName".PadRight (34));
			sb.Append ($"{NotificationsName}\n");
			sb.Append ("  NotificationsUrl".PadRight (34));
			sb.Append ($"{NotificationsUrl}\n");
			sb.Append ("  NotificationsKey".PadRight (34));
			sb.Append ($"{NotificationsKey}\n");
			sb.Append ("  NotificationsConnectionString".PadRight (34));
			sb.Append ($"{NotificationsConnectionString}\n");
			sb.Append ("  MobileCenterKeyAndroid".PadRight (34));
			sb.Append ($"{MobileCenterKeyAndroid}\n");
			sb.Append ("  MobileCenterKeyiOS".PadRight (34));
			sb.Append ($"{MobileCenterKeyiOS}\n");
			return sb.ToString ();
		}
	}
}
