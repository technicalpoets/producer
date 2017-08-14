using System;
using System.Collections.Generic;
using System.Linq;

namespace Producer
{
	public static class StringExtensions
	{
		public static readonly string [] EmptyStringArray = new string [0];

		public static bool IsEmpty (this string value) => string.IsNullOrEmpty (value);


		public static bool IsNullOrEmpty (this string value) => string.IsNullOrEmpty (value);


		public static bool IsNullOrBullshit (this string value) => value.IsNullOrEmpty () || value.EqualsIgnoreCase ("-") || value.EqualsIgnoreCase ("n/a");


		public static bool EqualsIgnoreCase (this string value, string other)
			=> string.Equals (value, other, StringComparison.CurrentCultureIgnoreCase);


		public static bool ContainsIgnoreCase (this string source, string toCheck)
			=> source.IndexOf (toCheck, StringComparison.OrdinalIgnoreCase) >= 0;


		public static int LowestIndexOf (this IEnumerable<string> sources, string ofString)
		{
			var found = sources.Select (s => s.IndexOf (ofString, StringComparison.OrdinalIgnoreCase)).Where (i => i >= 0);

			return found.Any () ? found.Min () : -1;
		}


		public static string [] SplitOnFirst (this string strVal, char needle)
		{
			if (strVal == null) return EmptyStringArray;
			var pos = strVal.IndexOf (needle);
			return pos == -1
				? new [] { strVal }
				: new [] { strVal.Substring (0, pos), strVal.Substring (pos + 1) };
		}


		public static string [] SplitOnFirst (this string strVal, string needle)
		{
			if (strVal == null) return EmptyStringArray;
			var pos = strVal.IndexOf (needle, StringComparison.OrdinalIgnoreCase);
			return pos == -1
				? new [] { strVal }
				: new [] { strVal.Substring (0, pos), strVal.Substring (pos + needle.Length) };
		}


		public static string [] SplitOnLast (this string strVal, char needle)
		{
			if (strVal == null) return EmptyStringArray;
			var pos = strVal.LastIndexOf (needle);
			return pos == -1
				? new [] { strVal }
				: new [] { strVal.Substring (0, pos), strVal.Substring (pos + 1) };
		}


		public static string [] SplitOnLast (this string strVal, string needle)
		{
			if (strVal == null) return EmptyStringArray;
			var pos = strVal.LastIndexOf (needle, StringComparison.OrdinalIgnoreCase);
			return pos == -1
				? new [] { strVal }
				: new [] { strVal.Substring (0, pos), strVal.Substring (pos + needle.Length) };
		}


		#region Fmt

		public static string Fmt (this string text, params object [] args) => string.Format (text, args);

		public static string Fmt (this string text, object arg1) => string.Format (text, arg1);

		public static string Fmt (this string text, object arg1, object arg2) => string.Format (text, arg1, arg2);

		public static string Fmt (this string text, object arg1, object arg2, object arg3) => string.Format (text, arg1, arg2, arg3);

		#endregion
	}
}