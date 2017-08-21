using System;
using Android.Support.V4.App;

namespace Producer.Droid
{
	/// <summary>
	/// Base adapter for Fragments that tags Fragments to keep track of them and provide lookup/retrieval capabilities.
	/// </summary>
	public abstract class BaseFragmentPagerAdapter : FragmentPagerAdapter
	{
		readonly FragmentManager fragmentManager;

		string [] tags;
		string [] Tags => tags ?? (tags = new string [Count]);


		protected BaseFragmentPagerAdapter (FragmentManager manager) : base (manager)
		{
			fragmentManager = manager;
		}


		public override Java.Lang.Object InstantiateItem (Android.Views.ViewGroup container, int position)
		{
			try
			{
				var obj = base.InstantiateItem (container, position);

				if (obj is Fragment)
				{
					// record the fragment tag here
					var f = (Fragment) obj;
					var tag = f.Tag;
					Tags [position] = tag;
				}

				return obj;
			}
			catch (Exception ex)
			{
				Log.Debug (ex.Message);
			}

			return null;
		}


		public Fragment GetFragmentAtPosition (int position)
		{
			string tag = tags [position];

			if (tag == null)
				return null;

			return fragmentManager.FindFragmentByTag (tag);
		}
	}
}