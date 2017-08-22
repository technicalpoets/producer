using Android.Widget;
using Android.Views;
using System.Threading.Tasks;
//using Com.Bumptech.Glide;
using Android.Text.Style;
using Producer.Domain;

namespace Producer.Droid
{
	public class ContentViewHolder : ViewHolder<AvContent>
	{
		ImageView logo;
		TextView name;
		TextView location;

		static UpdatableForegroundColorSpan locationColorSpan;
		static RelativeSizeSpan locationSizeSpan;


		public ContentViewHolder (View v) : base (v)
		{
			if (locationColorSpan == null)
			{
				locationColorSpan = new UpdatableForegroundColorSpan (ItemView.Context.GetColorFromResource (Resource.Color.elite_orange));
			}

			if (locationSizeSpan == null)
			{
				locationSizeSpan = new RelativeSizeSpan (.8f);
			}
		}


		public override void FindViews (View rootView)
		{
			//logo = (ImageView) rootView.FindViewById (Resource.Id.partner_logo);
			//name = (TextView) rootView.FindViewById (Resource.Id.partner_name);
			//location = (TextView) rootView.FindViewById (Resource.Id.partner_location);
		}


		public override void SetData (AvContent data)
		{
			//logo.SetImageDrawable (null);
			name.SetText (data.Name, TextView.BufferType.Normal);


			//var primaryLocation = data.GetPrimaryLocationName ();
			//var locationDetails = data.GetLocationDetails ();

			////location.Text = $"{primaryLocation} {locationDetails}";

			//location.TextFormatted =
			//			new SimpleSpanBuilder ()
			//			.Append (primaryLocation)
			//			.Append (" ")
			//			.Append (locationDetails, locationColorSpan, locationSizeSpan)
			//			.Build ();

			//if (!string.IsNullOrEmpty (data.LogoUrl))
			//{
			//	Glide.With (ItemView.Context)
			//		 .Load (data.LogoUrl)
			//		 //.Placeholder (Resource.Drawable.abc_spinner_mtrl_am_alpha)
			//		 //.DiskCacheStrategy(DiskCacheStrategy.
			//		 //.CrossFade ()
			//		 .Into (logo);
			//}
			//else if (data.LogoUrl == null)
			//{
			//	Log.Debug ($"Partner LogoUrl not set for {data.Name}, retrieving from server");

			//	Glide.Clear (logo); //needed to stop any current Glide op that may populate this recyclerview cell (due to reuse)
			//						//logo.SetImageResource (Resource.Drawable.abc_spinner_mtrl_am_alpha);

			//	DataClient.Shared.GetPartnerLogoUrl (data)
			//	.ContinueWith (t =>
			//	{
			//		if (!string.IsNullOrEmpty (t.Result))
			//		{
			//			Glide.With (ItemView.Context)
			//				 .Load (t.Result)
			//				 //.Placeholder (Resource.Drawable.abc_spinner_mtrl_am_alpha)
			//				 //.DiskCacheStrategy(DiskCacheStrategy.
			//				 //.CrossFade ()
			//				 .Into (logo);
			//		}
			//	}, TaskScheduler.FromCurrentSynchronizationContext ());
			//}
			//else
			//{
			//	logo.SetImageDrawable (null);
			//}
		}
	}
}