using System;

namespace Producer.Domain
{
	public class ContentEncodedMessage : DocumentUpdatedMessage
	{
		public string RemoteAssetUri { get; private set; }

		public ContentEncodedMessage (string documentId, string collectionId, Uri remoteAssetUri)
			: base (documentId, collectionId, UserRoles.Producer)
		{
			if (remoteAssetUri == null) throw new ArgumentNullException (nameof (remoteAssetUri));

			var uriBuilder = new UriBuilder (remoteAssetUri)
			{
				Scheme = Uri.UriSchemeHttps,
				Port = -1 // default port for scheme
			};

			RemoteAssetUri = uriBuilder.Uri.AbsoluteUri;
		}
	}
}
