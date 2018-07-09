using Android.Widget;
using Android.Views;
//using Com.Bumptech.Glide;
using Android.Text.Style;
using Producer.Domain;
using Producer.Droid.Framework.Utilities.Extensions;
using Producer.Droid.Framework.Widgets;
using System;

namespace Producer.Droid
{
	public class ContentViewHolder : ViewHolder<MusicAsset>
	{
		TextView title;
		TextView artist;
		RelativeLayout iconContainer, iconBack, iconFront;
		ImageView iconContent;

		//static UpdatableForegroundColorSpan locationColorSpan;
		//static RelativeSizeSpan locationSizeSpan;

		Action<View, int> iconClickHandler;
		Action<View, MusicAsset> itemClickHandler;



		public ContentViewHolder (View v) : base (v)
		{
			//if (locationColorSpan == null)
			//{
			//	locationColorSpan = new UpdatableForegroundColorSpan (ItemView.Context.GetColorFromResource (Resource.Color.elite_orange));
			//}

			//if (locationSizeSpan == null)
			//{
			//	locationSizeSpan = new RelativeSizeSpan (.8f);
			//}
		}


		public override void FindViews (View rootView)
		{
			title = rootView.FindViewById<TextView> (Resource.Id.contentTitle);
			artist = rootView.FindViewById<TextView> (Resource.Id.contentArtist);
			iconContent = rootView.FindViewById<ImageView> (Resource.Id.icon_content);
			iconContainer = rootView.FindViewById<RelativeLayout> (Resource.Id.icon_container);
			iconBack = rootView.FindViewById<RelativeLayout> (Resource.Id.icon_back);
			iconFront = rootView.FindViewById<RelativeLayout> (Resource.Id.icon_front);

			iconContainer.SetOnClickListener (this);
		}


		public override void SetData (MusicAsset data, bool selected, bool animateSelection)
		{
			base.SetData (data, selected, animateSelection);

			title.SetText (data.Music.DisplayName, TextView.BufferType.Normal);
			artist.SetText (data.Music.Description, TextView.BufferType.Normal);

			switch (data.Music.ContentType)
			{
				case AvContentTypes.Audio:
					iconContent.SetImageResource (Resource.Drawable.ic_content_audio);
					break;
				case AvContentTypes.Video:
					iconContent.SetImageResource (Resource.Drawable.ic_content_video);
					break;
			}

			ApplyIconAnimation (selected, animateSelection);

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


		public void SetIconClickHandler (Action<View, int> handler)
		{
			iconClickHandler = handler;
		}

		public void SetItemClickHandler (Action<View, MusicAsset> handler)
		{
			itemClickHandler = handler;
		}

		void ApplyIconAnimation (bool selected, bool animateSelection)
		{
			if (animateSelection)
			{
				//we need to restore the initial state of the row, since this VH will have re-inflated the layout
				setIconState (!selected);

				FlipAnimator.FlipView (iconBack, iconFront, selected);
			}
			else
			{
				setIconState (selected);
			}
		}


		void setIconState (bool selected)
		{
			var firstView = selected ? iconFront : iconBack;
			var secondView = selected ? iconBack : iconFront;

			firstView.Alpha = 0;
			secondView.ResetYRotation ();
			secondView.Alpha = 1;
		}


		public override void OnClick (View view)
		{
			//if the click was on our icon, then handle selection via click handler
			if (view == iconContainer)
			{
				iconClickHandler (iconContainer, AdapterPosition);
				return;
			}
			itemClickHandler (iconContainer, GetData ());

			//call thru to base to handle row click
			base.OnClick (view);
		}
	}
}