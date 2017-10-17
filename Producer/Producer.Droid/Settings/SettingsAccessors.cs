using System.Threading.Tasks;

namespace Producer
{
	public static partial class Settings
	{
		static TaskCompletionSource<bool> configurationTcs;

		public static Task<bool> IsConfigured () => configurationTcs?.Task ?? Task.FromResult (EndpointConfigured);


		public static void BeginConfig ()
		{
			configurationTcs = new TaskCompletionSource<bool> ();
		}


		public static void CompleteConfig ()
		{
			configurationTcs.TrySetResult (true);
		}


		#region Hidden Settings


		public static string RegistrationId
		{
			get => StringForKey (SettingsKeys.RegistrationId);
			set => SetSetting (SettingsKeys.RegistrationId, value);
		}


		public static string FcmToken
		{
			get => StringForKey (SettingsKeys.FcmToken);
			set => SetSetting (SettingsKeys.FcmToken, value);
		}


		public static string NotificationTags
		{
			get => StringForKey (SettingsKeys.NotificationTags);
			set => SetSetting (SettingsKeys.NotificationTags, value);
		}


		#endregion
	}
}