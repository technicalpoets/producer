using System;
using System.Collections.Generic;
using Android.Views;
using Producer.Domain;

namespace Producer.Droid
{
	public class ContentRecyclerAdapter : RecyclerViewAdapter<MusicAsset, ContentViewHolder>//, FastScrollRecyclerView.ISectionedAdapter
	{
		Action<View, MusicAsset, int> ItemIconClick;
		Action<View, MusicAsset> ItemClick;


		public ContentRecyclerAdapter (IList<MusicAsset> dataSet) : base (dataSet)
		{
		}


		protected override ContentViewHolder CreateViewHolder (LayoutInflater inflater, ViewGroup parent)
		{
			var rootView = inflater.Inflate (Resource.Layout.ContentCell, parent, false);

			var viewHolder = new ContentViewHolder (rootView);

			viewHolder.SetIconClickHandler (OnIconClick);
			viewHolder.SetItemClickHandler (OnItemClick);

			return viewHolder;
		}


		public void SetIconClickHandler (Action<View, MusicAsset, int> handler) => ItemIconClick = handler;
		public void SetItemClickHandler (Action<View, MusicAsset> handler) => ItemClick = handler;


		void OnIconClick (View view, int position) => ItemIconClick?.Invoke (view, GetItem (position), position);
		void OnItemClick (View view, MusicAsset data) => ItemClick?.Invoke (view, data);

		//#region FastScrollRecyclerView.ISectionedAdapter Members


		//public string GetSectionName (int position)
		//{
		//	//return the first letter of the partner's name
		//	var partner = CurrentItems [position];

		//	return partner.Name.Substring (0, 1).ToUpper ();
		//}


		//#endregion
	}
}