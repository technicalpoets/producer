using System.Text;

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
			var sb = new StringBuilder ("ProducerSettings:\n");
			sb.Append ("\tRemoteFunctionsUrl".PadRight (30));
			sb.Append ($": {RemoteFunctionsUrl}\n");
			sb.Append ("\tRemoteDocumentDbUrl".PadRight (30));
			sb.Append ($": {RemoteDocumentDbUrl}\n");
			sb.Append ("\tRemoteDocumentDbKey".PadRight (30));
			sb.Append ($": {RemoteDocumentDbKey}\n");
			sb.Append ("\tEmbeddedSocialKey".PadRight (30));
			sb.Append ($": {EmbeddedSocialKey}\n");
			sb.Append ("\tNotificationsName".PadRight (30));
			sb.Append ($": {NotificationsName}\n");
			sb.Append ("\tNotificationsUrl".PadRight (30));
			sb.Append ($": {NotificationsUrl}\n");
			sb.Append ("\tNotificationsKey".PadRight (30));
			sb.Append ($": {NotificationsKey}\n");
			sb.Append ("\tNotificationsConnectionString".PadRight (30));
			sb.Append ($": {NotificationsConnectionString}\n");
			sb.Append ("\tMobileCenterKeyAndroid".PadRight (30));
			sb.Append ($": {MobileCenterKeyAndroid}\n");
			sb.Append ("\tMobileCenterKeyiOS".PadRight (30));
			sb.Append ($": {MobileCenterKeyiOS}\n");
			return sb.ToString ();
		}
	}
}
