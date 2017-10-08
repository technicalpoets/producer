using System;

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


		public static string AzureSiteName
		{
			get => StringForKey (SettingsKeys.AzureSiteName);
			set => SetSetting (SettingsKeys.AzureSiteName, value ?? string.Empty);
		}


		public static bool HasAzureSiteName => !string.IsNullOrEmpty (AzureSiteName);


		public static string RemoteFunctionsUrl
		{
			get
			{
				var url = StringForKey (SettingsKeys.RemoteFunctionsUrl);

				if (string.IsNullOrEmpty (url) && HasAzureSiteName)
				{
					url = $"{AzureSiteName}.azurewebsites.net";

					SetSetting (SettingsKeys.RemoteFunctionsUrl, url);
				}

				return url;
			}
			set => SetSetting (SettingsKeys.RemoteFunctionsUrl, value ?? string.Empty);
		}


		static Uri _functionsUrl;

		public static Uri FunctionsUrl
		{
			get
			{
				if (_functionsUrl == null && !string.IsNullOrEmpty (RemoteFunctionsUrl))
				{
					var url = RemoteFunctionsUrl.Replace ("https://", string.Empty).Replace ("http://", string.Empty).TrimEnd ('/');

					_functionsUrl = string.IsNullOrEmpty (url) ? null : new UriBuilder ("https", url, 443).Uri;
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
					var url = RemoteDocumentDbUrl.Replace ("https://", string.Empty).Replace ("http://", string.Empty).TrimEnd ('/');

					_documentDbUrl = string.IsNullOrEmpty (url) ? null : new UriBuilder ("https", url, 443).Uri;
				}

				return _documentDbUrl;
			}
			set => RemoteDocumentDbUrl = value?.Host;
		}


		public static bool EndpointConfigured => HasAzureSiteName && DocumentDbUrl != null && FunctionsUrl != null;


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


		public static bool SetSettingsConfig (AppSettings settings)
		{
			var valid = !string.IsNullOrEmpty (settings?.DocumentDbUrl?.Host) &&
						!string.IsNullOrEmpty (settings?.NotificationsName) &&
						!string.IsNullOrEmpty (settings?.NotificationsConnectionString);

			DocumentDbUrl = settings?.DocumentDbUrl;
			NotificationsName = settings?.NotificationsName;
			NotificationsConnectionString = settings?.NotificationsConnectionString;

			return valid;
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


		public static string ContentDataCache
		{
			get => StringForKey (SettingsKeys.ContentDataCache);
			set => SetSetting (SettingsKeys.ContentDataCache, value);
		}

		public static string GetContentToken<T> () => StringForKey ($"{SettingsKeys.ContentTokenBase}{typeof (T).Name}");

		public static void SetContentToken<T> (string token) => SetSetting ($"{SettingsKeys.ContentTokenBase}{typeof (T).Name}", token);

		#endregion
	}
}
