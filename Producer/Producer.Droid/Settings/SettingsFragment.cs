using Android.OS;
using Android.Preferences;

namespace Producer.Droid
{
	public class SettingsFragment : PreferenceFragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetHasOptionsMenu (true);

			AddPreferencesFromResource (Resource.Xml.preferences);

			FindPreference (SettingsKeys.VersionDescription).Summary = Settings.VersionDescription;
		}


		public override void OnResume ()
		{
			base.OnResume ();

			foreach (var item in SettingsKeys.VisibleSettings)
			{
				if (FindPreference (item) is EditTextPreference preference)
				{
					preference.PreferenceChange += handlePreferenceChange;
					preference.Summary = preference.Text ?? " ";
				}
			}
		}


		public override void OnPause ()
		{
			base.OnPause ();

			foreach (var item in SettingsKeys.VisibleSettings)
			{
				if (FindPreference (item) is EditTextPreference preference)
				{
					preference.PreferenceChange -= handlePreferenceChange;
				}
			}
		}


		void handlePreferenceChange (object sender, Preference.PreferenceChangeEventArgs e)
		{
			e.Preference.Summary = e.NewValue?.ToString ();
		}
	}
}