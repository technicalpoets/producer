using System;

using CoreGraphics;
using UIKit;

using Producer.Domain;

namespace Producer.iOS
{
	public partial class ContentMusicTvCell : UITableViewCell
	{

		public ContentMusicTvCell (IntPtr handle) : base (handle) { }


		public void SetData (AvContent music, MusicAssetDownloadState downloadState)
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

			switch (downloadState)
			{
				case MusicAssetDownloadState.Downloaded:

					//DetailTextLabel.Text = null; //music.DurationString;

					accessoryProgressView.Hidden = true;

					accessoryButton.Hidden = true;

					AccessoryView = null;

					break;

				case MusicAssetDownloadState.Downloading:

					//DetailTextLabel.Text = null;

					accessoryButton.Hidden = true;

					AccessoryView = accessoryProgressView;

					accessoryProgressView.Hidden = false;

					accessoryProgressView.StartIndeterminate ();

					break;

				case MusicAssetDownloadState.NotDownloaded:

					//DetailTextLabel.Text = null;

					accessoryProgressView.Hidden = true;

					var audio = music.ContentType == AvContentTypes.Audio;

					AccessoryView = audio ? accessoryButton : null;

					accessoryButton.Hidden = !audio;

					// Set the tag to use during click event
					accessoryButton.Tag = Tag;

					break;
			}

			isPlaying = false;

			if (playingTag == Tag)
			{
				SetPlaying (true);
			}
		}


		public void UpdateDownloadProgress (nfloat progress)
		{
			if (progress > 0 && progress < 1)
			{
				accessoryProgressView.UpdateProgress (progress);
			}
		}


		public void UpdateProgress (double progress)
		{
			if (progress <= 0 || progress >= 1)
			{
				progressBar.Hidden = true;
			}
			else
			{
				progressBar.Hidden = false;

				progressBar.SetProgress ((float)progress, true);
			}
		}


		bool isPlaying;

		static nint playingTag = -1;

		public void SetPlaying (bool playing)
		{
			if (isPlaying != playing)
			{
				isPlaying = playing;

				if (isPlaying)
				{
					Log.Debug ($"StartPlaying : {Tag}");

					playingTag = Tag;

					setPlaying ();
				}
				else if (playingTag == Tag)
				{
					playingTag = -1;
				}
			}
		}

		void setPlaying ()
		{
			AnimateNotify (0.4, 0.0, UIViewAnimationOptions.CurveLinear, () => ImageView.Transform = CGAffineTransform.Rotate (ImageView.Transform, NMath.PI / 2), (finished) =>
			{
				if (finished && isPlaying)
				{
					setPlaying ();
				}
			});

		}
	}
}
