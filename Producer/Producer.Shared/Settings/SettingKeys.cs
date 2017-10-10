using System.Collections.Generic;

namespace Producer
{
	public static partial class SettingsKeys
	{
		#region Visible Settings

		public const string VersionDescription = nameof (VersionDescription);

		public const string VersionNumber = nameof (VersionNumber);

		public const string BuildNumber = nameof (BuildNumber);

		public const string GitCommitHash = nameof (GitCommitHash);

		public const string AzureSiteName = nameof (AzureSiteName);

		public const string RemoteFunctionsUrl = nameof (RemoteFunctionsUrl);

		public const string RemoteDocumentDbUrl = nameof (RemoteDocumentDbUrl);

		public const string EmbeddedSocialKey = nameof (EmbeddedSocialKey);

		public const string NotificationsName = nameof (NotificationsName);

		public const string NotificationsConnectionString = nameof (NotificationsConnectionString);

		public const string MobileCenterKey = nameof (MobileCenterKey);

		public const string UserReferenceKey = nameof (UserReferenceKey);

		public static List<string> VisibleSettings = new List<string>
		{
			RemoteFunctionsUrl,
			RemoteDocumentDbUrl,
			EmbeddedSocialKey,
			NotificationsName,
			NotificationsConnectionString,
			MobileCenterKey,
			UserReferenceKey
		};


		#endregion


		#region Hidden Settings


		public const string AvContentListSelectedFilter = nameof (AvContentListSelectedFilter);

		public const string ProducerListSelectedRole = nameof (ProducerListSelectedRole);

		public const string LastAvContentDescription = nameof (LastAvContentDescription);

		public const string ContentTokenBase = nameof (ContentTokenBase);

		public const string ContentDataCache = nameof (ContentDataCache);


		#endregion
	}
}