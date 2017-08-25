using System;

namespace Producer.Domain
{
	public class DocumentUpdatedMessage
	{
		public const string DocumentIdKey = "documentId";

		public const string CollectionIdKey = "collectionId";

		public string Title { get; set; }

		public string Message { get; set; }

		public string DocumentId { get; private set; }

		public string CollectionId { get; private set; }

		public int PublishedTo { get; set; }

		public DocumentUpdatedMessage (string documentId, string collectionId)
		{
			DocumentId = documentId ?? throw new ArgumentNullException (nameof (documentId));

			CollectionId = collectionId ?? throw new ArgumentNullException (nameof (collectionId));
		}
	}
}
