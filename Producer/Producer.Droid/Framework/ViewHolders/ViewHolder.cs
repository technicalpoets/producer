using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace Producer.Droid
{
	public abstract class ViewHolder<TData> : RecyclerView.ViewHolder, IViewHolder<TData>
	{
		protected ViewHolder (View itemView) : base (itemView)
		{
#pragma warning disable RECS0021 // Warns about calls to virtual member functions occuring in the constructor
			FindViews (itemView);
#pragma warning restore RECS0021 // Warns about calls to virtual member functions occuring in the constructor
		}


		public void SetClickHandler (Action<View, int> handler)
		{
			ItemView.Click += (sender, e) => handler ((View) sender, AdapterPosition);
		}


		public abstract void FindViews (View rootView);


		public abstract void SetData (TData data);
	}


	public static class ViewHolder
	{
		//		public static THolder Create<THolder, TData>(View rootView)
		//			where THolder : ViewHolder<TData>, IViewHolder<TData>, new()
		//		{
		//			var holder = new THolder ();
		//			holder.ItemView = rootView;
		//			holder.FindViews (rootView);
		//
		//			return holder;
		//		}

		public static void SetData<TData> (this RecyclerView.ViewHolder holder, TData data)
		{
			var viewHolder = holder as ViewHolder<TData>;

			if (viewHolder != null)
			{
				viewHolder.SetData (data);
			}
		}
	}
}