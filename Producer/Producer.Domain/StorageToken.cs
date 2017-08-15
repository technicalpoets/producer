using System;

namespace Producer.Domain
{
	public class StorageToken
	{
		public string DocumentId { get; private set; }

		public string SasUri { get; private set; }

		public StorageToken (string documentId, string sasUri)
		{
			DocumentId = documentId ?? throw new ArgumentNullException (nameof (documentId));
			SasUri = sasUri ?? throw new ArgumentNullException (nameof (sasUri));
		}
	}
}
