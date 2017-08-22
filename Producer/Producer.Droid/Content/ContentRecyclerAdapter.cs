using System.Collections.Generic;
using Android.Views;
using Producer.Domain;

namespace Producer.Droid
{
	public class ContentRecyclerAdapter : RecyclerViewAdapter<AvContent, ContentViewHolder>//, FastScrollRecyclerView.ISectionedAdapter
	{
		public ContentRecyclerAdapter (IList<AvContent> dataSet) : base (dataSet)
		{
		}


		protected override ContentViewHolder CreateViewHolder (LayoutInflater inflater, ViewGroup parent)
		{
			//var rootView = inflater.Inflate (Resource.Layout.PartnerCardView, parent, false);

			return new ContentViewHolder (new View (parent.Context));
		}


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