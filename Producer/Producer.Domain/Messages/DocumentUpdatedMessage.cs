using System;

namespace Producer.Domain
{
	public class DocumentUpdatedMessage
	{
		public const string DocumentIdKey = "documentId";

		public const string CollectionIdKey = "collectionId";

		public const string NotificationTagsKey = "notificationTags";

		public string Title { get; set; }

		public string Message { get; set; }

		public string DocumentId { get; private set; }

		public string CollectionId { get; private set; }

		public string NotificationTags { get; private set; }

		public DocumentUpdatedMessage (string documentId, string collectionId, UserRoles publishedTo)
			: this (documentId, collectionId, publishedTo.GetExpressionString ()) { }

		public DocumentUpdatedMessage (string documentId, string collectionId, string publishedToTagExpression)
		{
			DocumentId = documentId ?? throw new ArgumentNullException (nameof (documentId));

			CollectionId = collectionId ?? throw new ArgumentNullException (nameof (collectionId));

			NotificationTags = publishedToTagExpression;
		}
	}
}