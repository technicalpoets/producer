namespace Producer.Domain
{
	public class StorageToken
	{
		public const string ContentIdParam = "contentId";

		public const string RequestApiName = "getStorageToken";

		public string ContentId { get; set; }

		public string SasUri { get; set; }
	}
}
