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

		[Newtonsoft.Json.JsonConstructor]
		public DocumentUpdatedMessage (string documentId, string collectionId, string notificationTags)
		{
			DocumentId = documentId ?? throw new ArgumentNullException (nameof (documentId));

			CollectionId = collectionId ?? throw new ArgumentNullException (nameof (collectionId));

			NotificationTags = notificationTags;
		}

		public override string ToString ()
		{
			var sb = new System.Text.StringBuilder ("\n\nDocumentUpdatedMessage\n");
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
			return sb.ToString ();
		}
	}
}