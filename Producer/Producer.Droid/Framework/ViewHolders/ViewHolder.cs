using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace Producer.Droid
{
	public abstract class ViewHolder<TData> : RecyclerView.ViewHolder, IViewHolder<TData>, View.IOnClickListener, View.IOnLongClickListener
	{
		Action<View, int> clickHandler;
		Action<View, int> longClickHandler;


		protected ViewHolder (View itemView) : base (itemView)
		{
#pragma warning disable RECS0021 // Warns about calls to virtual member functions occuring in the constructor
			FindViews (itemView);
#pragma warning restore RECS0021 // Warns about calls to virtual member functions occuring in the constructor
		}


		public void SetClickHandler (Action<View, int> handler)
		{
			ItemView.SetOnClickListener (this);

			clickHandler = handler;
		}


		public void SetLongClickHandler (Action<View, int> handler)
		{
			ItemView.SetOnLongClickListener (this);

			longClickHandler = handler;
		}


		public virtual void OnClick (View view)
		{
			clickHandler (view, AdapterPosition);
		}


		public bool OnLongClick (View view)
		{
			longClickHandler (view, AdapterPosition);

			view.PerformHapticFeedback (FeedbackConstants.LongPress);

			return true;
		}


		public abstract void FindViews (View rootView);


		//base behavior is to change the row state to activated if it's selected
		public virtual void SetData (TData data, bool selected, bool animateSelection) => ItemView.Activated = selected;
	}


	public static class ViewHolder
	{
		public static void SetData<TData> (this RecyclerView.ViewHolder holder, TData data, bool selected, bool animateSelection = false)
		{
			if (holder is ViewHolder<TData> viewHolder)
			{
				viewHolder.SetData (data, selected, animateSelection);
			}
		}
	}
}