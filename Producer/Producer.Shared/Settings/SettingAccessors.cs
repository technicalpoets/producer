using System;

using Producer.Domain;

namespace Producer
{
	public static partial class Settings
	{

		#region Visible Settings


		public static string VersionDescription
		{
			get => StringForKey (SettingsKeys.VersionDescription);
#if __ANDROID__
			set => SetSetting (SettingsKeys.VersionDescription, value);
#endif
		}


		public static string VersionNumber
		{
			get => StringForKey (SettingsKeys.VersionNumber);
#if __ANDROID__
			set => SetSetting (SettingsKeys.VersionNumber, value);
#endif
		}


		public static string BuildNumber
		{
			get => StringForKey (SettingsKeys.BuildNumber);
#if __ANDROID__
			set => SetSetting (SettingsKeys.BuildNumber, value);
#endif
		}


		public static string GitHash => StringForKey (SettingsKeys.GitCommitHash);


		public static string RemoteFunctionsUrl
		{
			get => StringForKey (SettingsKeys.RemoteFunctionsUrl);
			set => SetSetting (SettingsKeys.RemoteFunctionsUrl, value ?? string.Empty);
		}


		static Uri _functionsUrl;

		public static Uri FunctionsUrl
		{
			get
			{
				if (_functionsUrl == null && !string.IsNullOrEmpty (RemoteFunctionsUrl))
				{
					if (!RemoteFunctionsUrl.StartsWith ("http", StringComparison.Ordinal) && RemoteFunctionsUrl.EndsWith (".azurewebsites.net", StringComparison.Ordinal))
					{
						var uriBuilder = new UriBuilder ("https", RemoteFunctionsUrl);

						_functionsUrl = uriBuilder.Uri;
					}
				}
				return _functionsUrl;
			}
		}


		public static string RemoteDocumentDbUrl
		{
			get => StringForKey (SettingsKeys.RemoteDocumentDbUrl);
			set => SetSetting (SettingsKeys.RemoteDocumentDbUrl, value ?? string.Empty);
		}


		static Uri _documentDbUrl;

		public static Uri DocumentDbUrl
		{
			get
			{
				if (_documentDbUrl == null && !string.IsNullOrEmpty (RemoteDocumentDbUrl))
				{
					if (!RemoteDocumentDbUrl.StartsWith ("http", StringComparison.Ordinal) && RemoteDocumentDbUrl.EndsWith (".documents.azure.com", StringComparison.Ordinal))
					{
						var uriBuilder = new UriBuilder ("https", RemoteDocumentDbUrl, 443);

						_documentDbUrl = uriBuilder.Uri;
					}
				}
				return _documentDbUrl;
			}
		}


		public static string NotificationsName
		{
			get => StringForKey (SettingsKeys.NotificationsName);
			set => SetSetting (SettingsKeys.NotificationsName, value ?? string.Empty);
		}


		public static string NotificationsConnectionString
		{
			get => StringForKey (SettingsKeys.NotificationsConnectionString);
			set => SetSetting (SettingsKeys.NotificationsConnectionString, value ?? string.Empty);
		}


		public static string EmbeddedSocialKey
		{
			get => StringForKey (SettingsKeys.EmbeddedSocialKey);
			set => SetSetting (SettingsKeys.EmbeddedSocialKey, value ?? string.Empty);
		}


		public static string MobileCenterKey
		{
			get => StringForKey (SettingsKeys.MobileCenterKey);
			set => SetSetting (SettingsKeys.MobileCenterKey, value ?? string.Empty);
		}


		public static string UserReferenceKey
		{
			get => StringForKey (SettingsKeys.UserReferenceKey);
			set => SetSetting (SettingsKeys.UserReferenceKey, value ?? "anonymous");
		}


		public static void ConfigureSettings (ProducerSettings producerSettings)
		{
			RemoteFunctionsUrl = producerSettings.RemoteFunctionsUrl;
			RemoteDocumentDbUrl = producerSettings.RemoteDocumentDbUrl;
			EmbeddedSocialKey = producerSettings.EmbeddedSocialKey;
			NotificationsName = producerSettings.NotificationsName;
			NotificationsConnectionString = producerSettings.NotificationsConnectionString;
#if __IOS__
			MobileCenterKey = producerSettings.MobileCenterKeyiOS;
#else
			MobileCenterKey = producerSettings.MobileCenterKeyAndroid;
#endif
		}


		#endregion


		#region Hidden Settings


		public static string AvContentListSelectedFilter
		{
			get => StringForKey (SettingsKeys.AvContentListSelectedFilter);
			set => SetSetting (SettingsKeys.AvContentListSelectedFilter, value);
		}


		public static int ProducerListSelectedRole
		{
			get => Int32ForKey (SettingsKeys.ProducerListSelectedRole);
			set => SetSetting (SettingsKeys.ProducerListSelectedRole, value);
		}


		public static string LastAvContentDescription
		{
			get => StringForKey (SettingsKeys.LastAvContentDescription);
			set => SetSetting (SettingsKeys.LastAvContentDescription, value);
		}


		public static string GetContentToken<T> () => StringForKey ($"{SettingsKeys.ContentTokenBase}{typeof (T).Name}");

		public static void SetContentToken<T> (string token) => SetSetting ($"{SettingsKeys.ContentTokenBase}{typeof (T).Name}", token);

		#endregion
	}
}
