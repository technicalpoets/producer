using Android.Widget;
using System.Collections.Generic;
using System.Linq;
using Java.Lang;

namespace Producer.Droid
{
	public abstract class BaseFilter<TData> : Filter
	{
		protected IFilterableDataProvider<TData> FilterableDataProvider;


		protected BaseFilter (IFilterableDataProvider<TData> filterableDataProvider)
		{
			FilterableDataProvider = filterableDataProvider;
		}

		#region implemented abstract members of Filter


		protected abstract IList<TData> GetFilterResults (string constraint);


		protected override FilterResults PerformFiltering (ICharSequence constraint)
		{
			var filterResults = new FilterResults ();

			var results = GetFilterResults (constraint.ToString ());

			// Nasty piece of .NET to Java wrapping, be careful with this!
			filterResults.Values = FromArray (results.Select (r => r.ToJavaObject ()).ToArray ());
			filterResults.Count = results.Count;

			constraint.Dispose ();

			return filterResults;
		}


		protected override void PublishResults (ICharSequence constraint, FilterResults results)
		{
			if (results.Count == FilterableDataProvider.AllItems.Count)
			{
				FilterableDataProvider.ResetResults ();
			}
			else
			{
				using (var values = results.Values)
					FilterableDataProvider.SetFilterResults (values.ToArray<Object> ().Select (r => r.ToNetObject<TData> ()).ToList ());
			}

			// Don't do this and see GREF counts rising
			constraint.Dispose ();
			results.Dispose ();
		}


		#endregion
	}
}