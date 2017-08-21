using System.Collections.Generic;

namespace Producer.Droid
{
	public interface IFilterableDataProvider<TData>
	{
		IList<TData> AllItems { get; }

		void SetFilterResults (IList<TData> items);

		void ResetResults ();
	}
}