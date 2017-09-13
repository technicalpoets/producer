namespace Producer.Functions
{
	public static class Routes
	{
		public const string Get = "get";
		public const string Post = "post";

		public const string AuthenticateUser = "user/config";
		public const string EncodeBlob = "uploads-avcontent/{fileName}.{fileExtension}";
		public const string GenerateContentToken = "tokens/content/{collectionId}";
		public const string GenerateStorageToken = "tokens/storage/{collectionId}/{documentId}";
		public const string NotifyClients = "";
		public const string PublishContent = "publish";
		public const string UpdateAvContent = "publish";
	}
}
