using System;

namespace Producer.Domain
{
	public class ContentEncodedMessage : DocumentUpdatedMessage
	{
		public string RemoteAssetUri { get; private set; }

		public ContentEncodedMessage (string documentId, string collectionId)
			: base (documentId, collectionId, UserRoles.Producer)
		{ }

		public void SetRemoteAssetUri (Uri remoteAssetUri)
		{
			if (remoteAssetUri == null) throw new ArgumentNullException (nameof (remoteAssetUri));

			var uriBuilder = new UriBuilder (remoteAssetUri)
			{
				Scheme = Uri.UriSchemeHttps,
				Port = -1 // default port for scheme
			};

			RemoteAssetUri = uriBuilder.Uri.AbsoluteUri;
		}


		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nContentEncodedMessage\n");
			sb.Append ("  Title".PadRight (20));
			sb.Append ($"{Title}\n");
			sb.Append ("  Message".PadRight (20));
			sb.Append ($"{Message}\n");
			sb.Append ("  DocumentId".PadRight (20));
			sb.Append ($"{DocumentId}\n");
			sb.Append ("  CollectionId".PadRight (20));
			sb.Append ($"{CollectionId}\n");
			sb.Append ("  NotificationTags".PadRight (20));
			sb.Append ($"{NotificationTags}\n");
			sb.Append ("  RemoteAssetUri".PadRight (20));
			sb.Append ($"{RemoteAssetUri}\n");
			return sb.ToString ();
		}
	}
}
