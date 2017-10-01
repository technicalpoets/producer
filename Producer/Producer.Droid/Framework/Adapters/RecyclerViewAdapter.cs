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
		public event EventHandler ItemsFiltered;

		Action<View, TData, int> ItemClick;
		Action<View, TData, int> ItemLongClick;

		bool LongClickEnabled => ItemLongClick != null;
		bool clearingSelections;

		List<TData> originalDataSet;

		readonly List<TData> dataSet;

		// index is used to animate only the last selected/deselected row
		int lastSelectionIndex = -1;
		HashSet<int> selectedItemIndices = new HashSet<int> ();


		public int SelectedItemCount => selectedItemIndices.Count;


		// Initialize the dataset of the Adapter
		protected RecyclerViewAdapter (List<TData> dataSet)
		{
			this.dataSet = dataSet;
		}


		protected RecyclerViewAdapter (IEnumerable<TData> dataSet)
		{
			this.dataSet = new List<TData> (dataSet);
		}


		// Create new views (invoked by the layout manager)
		public override RecyclerView.ViewHolder OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			var inflater = LayoutInflater.From (parent.Context);

			var holder = CreateViewHolder (inflater, parent);

			holder.SetClickHandler (OnClick);

			if (LongClickEnabled)
			{
				holder.SetLongClickHandler (OnLongClick);
			}

			return holder;
		}


		protected abstract TViewHolder CreateViewHolder (LayoutInflater inflater, ViewGroup parent);


		// Replace the contents of a view (invoked by the layout manager)
		public override void OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			//want to see if a) this item is selected, and b) if this selection is 'new' and needs to be (optionally) animated
			var selected = selectedItemIndices.Contains (position);
			var animateSelection = lastSelectionIndex == position || clearingSelections && selected; //was it just selected or are we clearing all selections?

			// Get element from your dataset at this position and replace the contents of the view with that element
			holder.SetData (dataSet [position], selected && !clearingSelections, animateSelection);

			//reset our selection tracking vars
			if (animateSelection)
			{
				lastSelectionIndex = -1;

				if (clearingSelections && selected)
				{
					selectedItemIndices.Remove (position);
				}

				//see if we're done clearing selections
				clearingSelections &= selectedItemIndices.Count != 0;
			}
		}


		// Return the size of your dataset (invoked by the layout manager)
		public override int ItemCount
		{
			get { return dataSet?.Count ?? 0; }
		}


		public void SetItemClickHandler (Action<View, TData, int> handler)
		{
			ItemClick = handler;
		}


		void OnClick (View view, int position)
		{
			ItemClick?.Invoke (view, dataSet [position], position);
		}


		public void SetItemLongClickHandler (Action<View, TData, int> handler)
		{
			ItemLongClick = handler;
		}


		void OnLongClick (View view, int position)
		{
			ItemLongClick?.Invoke (view, dataSet [position], position);
		}


		public void ToggleSelection (int position)
		{
			lastSelectionIndex = position;

			if (selectedItemIndices.Contains (position))
			{
				selectedItemIndices.Remove (position);
			}
			else
			{
				selectedItemIndices.Add (position);
			}

			NotifyItemChanged (position);
		}


		public void ClearSelectedItems ()
		{
			//not actually clearing anything here, just starting the clear operation here
			//	then, when ViewHolders are rebinding above, each will be evaluated and removed if selected
			clearingSelections = true;
			NotifyDataSetChanged ();
		}


		#region Item Operations


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


		#endregion


		#region IFilterableDataProvider Members & Helpers


		public Filter Filter { get; set; }


		public IList<TData> AllItems { get { return originalDataSet ?? dataSet; } }


		public IList<TData> CurrentItems { get { return dataSet; } }


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


		#endregion
	}
}