using System;
using System.Linq;
using System.Threading.Tasks;

using Foundation;
using MobileCoreServices;
using UIKit;

using Producer.Domain;
using Producer.Shared;
using SettingsStudio;

namespace Producer.iOS
{
	public partial class ComposeVc : UIViewController
	{

		NSUrl filePath;

		bool isValidMediaItem;

		bool initializedVeiw;

		// TODO: Make this an object
		(string UTType, string UTSubtype, string Filename) utiData;


		AvContent avContent;

		public ComposeVc (IntPtr handle) : base (handle) { }


		public void SetData (NSUrl url)
		{
			initializedVeiw = false;

			filePath = url;

			utiData = filePath.GetAvUtiConformance ();
		}


		public void SetData (AvContent content)
		{
			initializedVeiw = false;

			avContent = content;

			//filePath = url;

			//utiData = filePath.GetAvUtiConformance ();
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			createButton.Layer.CornerRadius = 4;

			fileTypeTextField.Enabled = false;
		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (!initializedVeiw)
			{
				if (!string.IsNullOrEmpty (utiData.UTType))
				{
					initializeViewForUtiData ();
				}
				else if (avContent != null)
				{
					initializeViewForEditingAvContent ();
				}
			}
		}


		public override void ViewDidDisappear (bool animated)
		{
			utiData = (null, null, null);
			filePath = null;
			avContent = null;
			initializedVeiw = false;
			isValidMediaItem = false;

			// TODO: make sure this isn't called whien displaying a dialog

			base.ViewDidDisappear (animated);
		}


		//partial void createButtonClicked (NSObject sender)
		//{
		//	createButton.Enabled = true;
		//	fileNameTextField.Enabled = true;
		//	fileDisplayNameTextField.Enabled = true;
		//	descriptionTextField.Enabled = true;


		//	avContent = new AvContent
		//	{
		//		Name = fileNameTextField.Text,
		//		DisplayName = fileDisplayNameTextField.Text,
		//		Description = descriptionTextField.Text,
		//		ProducerId = "admin", // TODO: fix this
		//		ContentType = utiData.Item1 == UTType.Audio ? AvContentTypes.Audio : utiData.Item1 == UTType.Movie ? AvContentTypes.Video : AvContentTypes.Unknown
		//	};

		//	UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;


		//	Task.Run (async () =>
		//	{
		//		try
		//		{
		//			await ContentClient.Shared.CreateAvContent (avContent);
		//		}
		//		catch (Exception ex)
		//		{
		//			Log.Debug (ex.Message);
		//		}
		//		finally
		//		{
		//			BeginInvokeOnMainThread (() =>
		//			{
		//				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;

		//				NavigationController.PopViewController (true);
		//			});
		//		}
		//	});
		//}


		partial void createButtonClicked (NSObject sender)
		{
			createButton.Enabled = false;
			fileNameTextField.Enabled = false;
			fileDisplayNameTextField.Enabled = false;
			descriptionTextField.Enabled = false;


			if (avContent != null) // editing existing item
			{
				if (updateAvContentItem ())
				{
					//Settings.LastAvContentDescription = avContent.Description ?? string.Empty;

					Task.Run (async () =>
					{
						await ContentClient.Shared.UpdateAvContent (avContent);

						BeginInvokeOnMainThread (() => NavigationController.PopViewController (true));
					});
				}
			}
			else // creating new draft
			{
				avContent = new AvContent
				{
					Name = fileNameTextField.Text,
					DisplayName = fileDisplayNameTextField.Text,
					Description = descriptionTextField.Text,
					ProducerId = ProducerClient.Shared.User.Id,
					ContentType = utiData.UTType == UTType.Audio ? AvContentTypes.Audio : utiData.UTType == UTType.Movie ? AvContentTypes.Video : AvContentTypes.Unknown
				};

				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;


				Task.Run (async () =>
				{
					avContent = await ContentClient.Shared.CreateAvContent (avContent);

					var storageToken = await ProducerClient.Shared.GetStorageToken (avContent);

					if (storageToken != null)
					{
						// store the inbox path incase upload fails, is interrupted, app
						// crashes, etc. and we need to reinitialize the upload later
						avContent.LocalInboxPath = filePath.Path;

						var success = await AzureStorageClient.Shared.AddNewFileAsync (avContent, storageToken);

						BeginInvokeOnMainThread (() =>
						{
							UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;

							if (success)
							{
								Settings.LastAvContentDescription = avContent.Description ?? string.Empty;

								NavigationController.PopViewController (true);

								// TODO: Remove this file once we get a successful streaming url
								//NSError error;

								//NSFileManager.DefaultManager.Remove (filePath.Path, out error);

								//if (error != null)
								//{
								//	System.Diagnostics.Debug.WriteLine ($"{error}");
								//	System.Diagnostics.Debug.WriteLine ($"Error trying to get resource attributes\n\t{error.Code}\n\t{error.Domain}\n\t{error.Description}");
								//}
							}
							else
							{
								// TODO: show failed/retry alert
							}

							UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
						});
					}
					else
					{
						BeginInvokeOnMainThread (() => { UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false; });
					}
				});
			}
		}


		bool updateAvContentItem ()
		{
			bool changed = false;

			if (!avContent.DisplayName.Equals (fileDisplayNameTextField.Text, StringComparison.OrdinalIgnoreCase))
			{
				avContent.DisplayName = fileDisplayNameTextField.Text;

				changed = true;
			}

			if (!avContent.Description.Equals (descriptionTextField.Text, StringComparison.OrdinalIgnoreCase))
			{
				avContent.Description = descriptionTextField.Text;

				changed = true;
			}

			return changed;
		}


		void initializeViewForEditingAvContent ()
		{
			initializedVeiw = true;

			Title = avContent.DisplayName;

			fileNameTextField.Text = avContent.Name;
			fileDisplayNameTextField.Text = avContent.DisplayName;
			fileTypeTextField.Text = avContent.ContentType == AvContentTypes.Audio ? "Streaming audio" : avContent.ContentType == AvContentTypes.Video ? "Streaming video" : "unknown";
			descriptionTextField.Text = avContent.Description;

			createButton.SetTitle ("Update Item", UIControlState.Normal);
		}


		void initializeViewForUtiData ()
		{
			initializedVeiw = true;

			if (utiData.Item1 == UTType.Audio)
			{
				if (utiData.Item2 == UTType.Audio)
				{
					showUnsupportedAlert ("Unsupported audio file", "The folowing formats are currently supported:\nmp3, wav");
				}
				else
				{
					setInitialFormData (utiData);
				}
			}
			else if (utiData.Item1 == UTType.Movie)
			{
				if (utiData.Item2 == UTType.Movie)
				{
					showUnsupportedAlert ("Unsupported movie file", "The folowing formats are currently supported:\nmp4, mov");
				}
				else
				{
					setInitialFormData (utiData);
				}
			}
			else if (utiData.Item1 == UTType.PlainText)
			{
				showUnsupportedAlert ("File import failed", $"{utiData.Item2}\n\nThe folowing formats are currently supported:\nmp3, wav, mp4, mov");
			}

			createButton.Enabled = isValidMediaItem;
		}


		void setInitialFormData ((string, string, string) data)
		{
			isValidMediaItem = true;

			var cleanName = data.Item3.SplitOnLast ('.').FirstOrDefault ();

			Title = cleanName;

			fileNameTextField.Text = data.Item3;
			fileDisplayNameTextField.Text = cleanName;
			fileTypeTextField.Text = data.Item2;
			descriptionTextField.Text = Settings.LastAvContentDescription;
		}


		void showUnsupportedAlert (string title, string message)
		{
			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			alertController.AddAction (UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, handleUnsupportedAlertAction));

			PresentViewController (alertController, true, null);
		}

		void handleUnsupportedAlertAction (UIAlertAction obj) => DismissViewController (true, handleUnsupportedAlertActionDismissedViewController);

		void handleUnsupportedAlertActionDismissedViewController () => NavigationController.PopViewController (true);


		void showUploadFailedAlert (string title, string message)
		{
			var alertController = UIAlertController.Create (title, message, UIAlertControllerStyle.Alert);

			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
			alertController.AddAction (UIAlertAction.Create ("Retry", UIAlertActionStyle.Default, null));

			PresentViewController (alertController, true, null);
		}
	}
}
