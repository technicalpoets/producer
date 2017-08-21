using Android.Content;
using System;

namespace Producer.Droid
{
	//poor man's 'IParcelable' :)
	public class IntentData
	{
		public const string Delimiter = "|";
		public readonly string Key;
		public object [] Values;

		protected IntentData ()
		{
			Key = GetType ().Name;
		}


		public static IntentData Create (params object [] values)
		{
			var intentData = new IntentData
			{
				Values = values
			};

			return intentData;
		}


		public static IntentData FromIntent (Intent intent)
		{
			var intentData = new IntentData ();
			var data = intent.GetStringExtra (intentData.Key);
			var values = data.Split (Delimiter [0]);
			intentData.Values = values;

			return intentData;
		}


		public string GetString (Enum key)
		{
			return Values [Convert.ToInt32 (key)].ToString ();
		}


		public int GetInt (Enum key)
		{
			return Convert.ToInt32 (Values [Convert.ToInt32 (key)]);
		}
	}
}