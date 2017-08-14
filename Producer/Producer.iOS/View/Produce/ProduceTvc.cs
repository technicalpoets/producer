using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UIKit;
using Foundation;

using SettingsStudio;

using NomadCode.UIExtensions;

using Producer.Domain;
using Producer.Shared;

namespace Producer.iOS
{
	public partial class ProduceTvc : UITableViewController
	{

		UserRoles role;

		NSIndexPath indexPathCache;

		List<AvContent> content => ContentClient.Shared.AvContent [role];


		public ProduceTvc (IntPtr handle) : base (handle) { }


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationItem.RightBarButtonItem = EditButtonItem;
		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			segmentControl.SelectedSegment = Settings.ProducerListSelectedRole;

			role = (UserRoles)Math.Abs (segmentControl.SelectedSegment - 2);

			ContentClient.Shared.AvContentChanged += handleAvContentChanged;

			TableView.ReloadData ();
		}


		public override void ViewWillDisappear (bool animated)
		{
			ContentClient.Shared.AvContentChanged -= handleAvContentChanged;

			base.ViewWillDisappear (animated);
		}


		public override nint RowsInSection (UITableView tableView, nint section) => content.Count;


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.Dequeue<ProduceTvCell> (indexPath);

			var asset = content [indexPath.Row];

			cell.Tag = indexPath.Row;

			cell.SetData (asset);

			return cell;
		}


		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			indexPathCache = indexPath;

			tableView.CellAt (indexPath).SetSelected (false, true);

			handleTableAction ();
		}


		partial void segmentControlValueChanged (NSObject sender)
		{
			Settings.ProducerListSelectedRole = (int)segmentControl.SelectedSegment;

			role = (UserRoles)Math.Abs (segmentControl.SelectedSegment - 2);

			TableView.ReloadData ();

			if (TableView.NumberOfRowsInSection (0) > 0)
			{
				var topIndex = NSIndexPath.FromRowSection (0, 0);

				TableView.ScrollToRow (topIndex, UITableViewScrollPosition.Top, false);
			}
		}


		void handleAvContentChanged (object sender, UserRoles e) => BeginInvokeOnMainThread (() => { TableView.ReloadData (); });


		#region TableCell Action Handlers


		//void handleTableActionMore (UITableViewRowAction action, NSIndexPath indexPath)
		void handleTableAction ()
		{
			var asset = content [indexPathCache.Row];

			Log.Debug ($"More: {asset?.DisplayName}");

			var alertController = UIAlertController.Create (asset.DisplayName, null, UIAlertControllerStyle.ActionSheet);

			//var downloadState = AssetPersistenceManager.Shared.DownloadState (activeAsset);

			//alertController.AddAction (UIAlertAction.Create ("Share", UIAlertActionStyle.Default, handleAlertControllerActionShare));
			alertController.AddAction (UIAlertAction.Create ("Edit Item", UIAlertActionStyle.Default, handleAlertControllerActionEditItem));

			switch (role)
			{
				case UserRoles.Producer:
					alertController.AddAction (UIAlertAction.Create ("Publish to Insiders", UIAlertActionStyle.Default, handleAlertControllerActionPublishToInsiders));
					alertController.AddAction (UIAlertAction.Create ("Publish to Everyone", UIAlertActionStyle.Default, handleAlertControllerActionPublishToEveryone));
					break;
				case UserRoles.Insider:
					alertController.AddAction (UIAlertAction.Create ("Publish to Everyone", UIAlertActionStyle.Default, handleAlertControllerActionPublishToEveryone));
					alertController.AddAction (UIAlertAction.Create ("Move to Drafts", UIAlertActionStyle.Default, handleAlertControllerActionPublishToDrafts));
					break;
				case UserRoles.General:
					alertController.AddAction (UIAlertAction.Create ("Move to Insiders", UIAlertActionStyle.Default, handleAlertControllerActionPublishToInsiders));
					alertController.AddAction (UIAlertAction.Create ("Move to Drafts", UIAlertActionStyle.Default, handleAlertControllerActionPublishToDrafts));
					break;
			}

			alertController.AddAction (UIAlertAction.Create ("Delete Item", UIAlertActionStyle.Destructive, handleAlertControllerActionDeleteItem));

			alertController.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, handleAlertControllerActionDismiss));

			PresentViewController (alertController, true, null);
		}



		void handleAlertControllerActionEditItem (UIAlertAction obj)
		{
			var asset = content [indexPathCache.Row];

			if (asset != null)
			{
				var composeNc = Storyboard.Instantiate<ComposeNc> ();

				var composeVc = composeNc?.TopViewController as ComposeVc;

				if (composeVc != null)
				{
					composeVc.SetData (asset);

					ShowViewController (composeNc, this);
				}
			}
		}


		void handleAlertControllerActionPublishToEveryone (UIAlertAction obj) => updatePublishedTo (UserRoles.General);


		void handleAlertControllerActionPublishToInsiders (UIAlertAction obj) => updatePublishedTo (UserRoles.Insider);


		void handleAlertControllerActionPublishToDrafts (UIAlertAction obj) => updatePublishedTo (UserRoles.Producer);


		void updatePublishedTo (UserRoles userRole)
		{
			var asset = content [indexPathCache.Row];

			if (asset != null)
			{
				var oldRole = asset.PublishedTo;

				asset.PublishedTo = userRole;

				Task.Run (() => ContentClient.Shared.UpdateAvContent (asset, oldRole));
			}
		}


		void handleAlertControllerActionDeleteItem (UIAlertAction obj)
		{
			var asset = content [indexPathCache.Row];

			// TODO: Prompt for confirmation explaining that actual asset will be deleted
			//		 Then delete
		}


		void handleAlertControllerActionDismiss (UIAlertAction obj) => DismissViewController (true, null);

		#endregion
	}
}
