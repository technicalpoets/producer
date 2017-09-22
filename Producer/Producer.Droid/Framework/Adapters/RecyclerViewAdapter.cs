using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Views;
using System;
using Android.Widget;

namespace Producer.Droid
{
	public abstract class RecyclerViewAdapter<TData, TViewHolder> : RecyclerView.Adapter, IFilterable, IFilterableDataProvider<TData>
		where TViewHolder : ViewHolder<TData>, IViewHolder<TData>
	{
		public event EventHandler<TData> ItemClick;
		public event EventHandler ItemsFiltered;

		List<TData> originalDataSet;

		readonly List<TData> dataSet;


		#region IFilterableDataProvider Members


		public Filter Filter { get; set; }


		public IList<TData> AllItems { get { return originalDataSet ?? dataSet; } }


		public IList<TData> CurrentItems { get { return dataSet; } }


		#endregion


		// Initialize the dataset of the Adapter
		protected RecyclerViewAdapter (List<TData> dataSet)
		{
			this.dataSet = dataSet;
		}


		protected RecyclerViewAdapter (IEnumerable<TData> dataSet)
		{
			this.dataSet = new List<TData>(dataSet);
		}


		// Create new views (invoked by the layout manager)
		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			var inflater = LayoutInflater.From (parent.Context);

			var holder = CreateViewHolder (inflater, parent);

			holder.SetClickHandler (OnClick);

			return holder;
		}


		protected abstract TViewHolder CreateViewHolder (LayoutInflater inflater, ViewGroup parent);


		// Replace the contents of a view (invoked by the layout manager)
		public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			// Get element from your dataset at this position and replace the contents of the view with that element
			holder.SetData (dataSet [position]);
		}


		// Return the size of your dataset (invoked by the layout manager)
		public override int ItemCount
		{
			get { return dataSet?.Count ?? 0; }
		}


		void OnClick (View view, int position)
		{
			ItemClick?.Invoke (view, dataSet [position]);
		}


		/// <summary>
		/// Sets the items - use for situations where list is loaded async and isn't populated when the constructor is called.
		/// </summary>
		/// <param name="items">Items.</param>
		public void SetItems (IEnumerable<TData> items)
		{
			dataSet.Clear ();
			dataSet.AddRange (items);

			NotifyDataSetChanged ();
		}


		public TData RemoveItem (int position)
		{
			var item = dataSet [position];
			dataSet.RemoveAt (position);
			NotifyItemRemoved (position);
			return item;
		}


		public void AddItem (int position, TData item)
		{
			dataSet.Insert (position, item);
			NotifyItemInserted (position);
		}


		public void AddItems (IEnumerable<TData> items)
		{
			var initialCount = dataSet.Count;
			dataSet.AddRange (items);
			NotifyItemRangeInserted (initialCount - 1, dataSet.Count - initialCount);
		}


		public void MoveItem (int fromPosition, int toPosition)
		{
			var item = dataSet [fromPosition];
			dataSet.RemoveAt (fromPosition);
			dataSet.Insert (toPosition, item);
			NotifyItemMoved (fromPosition, toPosition);
		}


		public void SetFilterResults (IList<TData> items)
		{
			//on first filter, init our original data
			if (originalDataSet == null)
			{
				originalDataSet = new List<TData> (dataSet);
			}

			applyAndAnimateRemovals (items);
			applyAndAnimateAdditions (items);
			applyAndAnimateMovedItems (items);

			ItemsFiltered?.Invoke (this, EventArgs.Empty);
		}


		public void ResetResults ()
		{
			if (originalDataSet != null)
			{
				SetFilterResults (originalDataSet);
			}
		}


		void applyAndAnimateRemovals (IList<TData> newItems)
		{
			for (int i = dataSet.Count - 1; i >= 0; i--)
			{
				var item = dataSet [i];

				if (!newItems.Contains (item))
				{
					RemoveItem (i);
				}
			}
		}


		void applyAndAnimateAdditions (IList<TData> newItems)
		{
			for (int i = 0; i < newItems.Count; i++)
			{
				var item = newItems [i];

				if (!dataSet.Contains (item))
				{
					AddItem (i, item);
				}
			}
		}


		void applyAndAnimateMovedItems (IList<TData> newItems)
		{
			for (int toPosition = newItems.Count - 1; toPosition >= 0; toPosition--)
			{
				var item = newItems [toPosition];
				var fromPosition = dataSet.IndexOf (item);

				if (fromPosition >= 0 && fromPosition != toPosition)
				{
					MoveItem (fromPosition, toPosition);
				}
			}
		}
	}
}