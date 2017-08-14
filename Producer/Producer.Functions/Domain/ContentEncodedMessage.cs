using System;

namespace Producer.Functions
{
	public class ContentEncodedMessage
	{
		public string ContentId { get; private set; }

		public string RemoteAssetUri { get; private set; }

		public ContentEncodedMessage (string contentId, Uri remoteAssetUri)
		{
			ContentId = contentId;

			var uriBuilder = new UriBuilder (remoteAssetUri)
			{
				Scheme = Uri.UriSchemeHttps,
				Port = -1 // default port for scheme
			};

			RemoteAssetUri = uriBuilder.Uri.AbsoluteUri;
		}
	}
}
