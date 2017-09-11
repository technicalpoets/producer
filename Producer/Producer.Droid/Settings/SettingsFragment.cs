using Android.Views;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.Content;

namespace Producer.Droid
{
	public class SettingsFragment : PreferenceFragment
	{
		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
			AddPreferencesFromResource (Resource.Xml.preferences);
			Preference preference = FindPreference (nameof(Settings.VersionDescription));
			preference.Summary = $"{Settings.VersionNumber} ({Settings.BuildNumber})";
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = base.OnCreateView (inflater, container, savedInstanceState);
			var color = new Android.Graphics.Color (ContextCompat.GetColor (this.Context, Resource.Color.primary));
			view.SetBackgroundColor (color);
			return view;
		}

		public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
		{
			base.OnCreateOptionsMenu (menu, inflater);
		}

		public override void OnResume ()
		{
			base.OnResume ();

			foreach (var item in SettingsKeys.VisibleSettings)
			{
				if (FindPreference (item) is EditTextPreference preference)
				{
					preference.PreferenceChange += handlePreferenceChange;
					preference.Summary = ((EditTextPreference) preference).Text ?? " ";
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
			e.Preference.Summary = e.NewValue.ToString ();
		}
	}
}