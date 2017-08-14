using System;

using UIKit;

using Producer.Domain;

namespace Producer.iOS
{
	public partial class ProduceTvCell : UITableViewCell
	{
		public ProduceTvCell (IntPtr handle) : base (handle) { }


		public void SetData (AvContent music)
		{
			TextLabel.Text = music.DisplayName;
			DetailTextLabel.Text = music.Description;

			switch (music.ContentType)
			{
				case AvContentTypes.Audio:
					ImageView.Image = UIImage.FromBundle ("i_content_audio");
					break;
				case AvContentTypes.Video:
					ImageView.Image = UIImage.FromBundle ("i_content_video");
					break;
				case AvContentTypes.Unknown:
					ImageView.Image = null;
					break;
			}
		}
	}
}
