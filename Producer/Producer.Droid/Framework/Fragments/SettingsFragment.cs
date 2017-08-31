using Android.Views;
using Android.OS;
using Android.Content;
using Producer.Domain;
using Android.Preferences;
using Android.Support.V4.Content;
using SettingsStudio;
using System.Collections.Generic;

namespace Producer.Droid
{
	public class SettingsFragment : PreferenceFragment
	{
		static readonly string [] Preferences = { 
			SettingsKeys.TestProducer,
			SettingsKeys.MobileCenterKey,
			SettingsKeys.RemoteFunctionsUrl,
			SettingsKeys.RemoteDocumentDbUrl,
			SettingsKeys.NotificationsName,
			SettingsKeys.NotificationsConnectionString,
			SettingsKeys.EmbeddedSocialKey,
			SettingsKeys.UserReferenceKey
		};

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetHasOptionsMenu (true);
			AddPreferencesFromResource (Resource.Xml.preferences);
			FindPreference ("VersionNumberString").Summary = $"{Settings.VersionNumber} ({Settings.BuildNumber})";
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

			foreach (var item in Preferences)
			{
				var preference = FindPreference (item);
				if (preference.GetType () == typeof (EditTextPreference))
				{
					preference.PreferenceChange += handlePreferenceChange;
					preference.Summary = ((EditTextPreference) preference).Text ?? " ";
				}
			}
		}

		public override void OnPause ()
		{
			base.OnPause ();

			foreach (var item in Preferences)
			{
				var preference = FindPreference (item);
				if (preference.GetType () == typeof (EditTextPreference))
				{
					preference.PreferenceChange -= handlePreferenceChange;
				}
			}
		}

		void handlePreferenceChange (object sender, Preference.PreferenceChangeEventArgs e)
		{
			var preference = e.Preference;

			if (preference.GetType () == typeof (EditTextPreference))
			{
				e.Preference.Summary = e.NewValue.ToString ();
			}
		}
	}
}