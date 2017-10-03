namespace Producer
{
	public static class Routes
	{
		public const string Get = "get";

		public const string Post = "post";

		public const string Json = "application/json";


		public const string LoginGoogle = ".auth/login/google?access_type=offline";

		public const string AuthenticateUser = "api/user/config";

		public const string EncodeBlob = "uploads-avcontent/{fileName}.{fileExtension}";

		public const string GenerateContentToken = "api/tokens/content/{collectionId}";

		public static string ContentToken (string collectionId) => $"api/tokens/content/{collectionId}";

		public const string GenerateStorageToken = "api/tokens/storage/{collectionId}/{documentId}";

		public static string StorageToken (string collectionId, string documentId) => $"api/tokens/storage/{collectionId}/{documentId}";

		public const string NotifyClients = "";

		public const string PublishContent = "api/publish";

		public const string UpdateAvContent = "";

		public const string GetAppSettings = "api/settings";
	}
}
