namespace Producer
{
	public static partial class Keys
	{
		public static partial class MobileCenter
		{
#if __IOS__
			public const string AppSecret = @"";
#elif __ANDROID__
			public const string AppSecret = @"";
#endif
		}

		public static partial class Azure
		{
			//#if DEBUG
			//            public const string ServiceUrl = @"";
			//            //public const string ServiceUrl = @"";
			//#else
			//			public const string ServiceUrl = @"";
			//#endif



			public static partial class Storage
			{
				public const string AccountName = @"";

				const string accountKey = @"";

				public const string EndpointSuffix = "core.windows.net";
				////<add name = "MS_AzureStorageAccountConnectionString" connectionString=""/>

				public static string ConnectionString = $"DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={accountKey};EndpointSuffix=core.windows.net";

				public const string BaseUrl = @"";
			}
		}
	}
}
