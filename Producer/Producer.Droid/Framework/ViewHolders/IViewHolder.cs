using Android.Views;
using System;

namespace Producer.Droid
{
	public interface IViewHolder<TData>
	{
		void FindViews (View rootView);

		void SetData (TData data);

		void SetClickHandler (Action<View, int> handler);
	}
}