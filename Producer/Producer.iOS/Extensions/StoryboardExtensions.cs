using Foundation;
using UIKit;

namespace NomadCode.UIExtensions
{
	public static class StoryboardExtensions
	{
		/// <summary>
		/// Instantiate a new instance of a UIViewController subclass from the specified storyboard.
		/// This override assumes storyboardIdentifier of the UIViewController subclass is the same as
		/// the Name of the Type.
		/// </summary>
		/// <param name="storyboard">The UIStoryboard containing the UIViewController subclass.</param>
		/// <typeparam name="T">The Type of the UIViewController subclass to instantiate and return.</typeparam>
		public static T Instantiate<T> (this UIStoryboard storyboard)
			where T : UIViewController
		=> storyboard.InstantiateViewController (typeof (T).Name) as T;


		/// <summary>
		/// Instantiate a new instance of a UIViewController subclass from the specified storyboard.
		/// </summary>
		/// <param name="storyboard">The UIStoryboard containing the UIViewController subclass.</param>
		/// <param name="storyboardIdentifier">The UIViewController subclass's Storyboard identifier.</param>
		/// <typeparam name="T">The Type of the UIViewController subclass to instantiate and return.</typeparam>
		public static T Instantiate<T> (this UIStoryboard storyboard, string storyboardIdentifier)
			where T : UIViewController
		=> storyboard.InstantiateViewController (storyboardIdentifier) as T;


		/// <summary>
		/// Instantiate a new instance of a UIViewController subclass from the specified storyboard.
		/// </summary>
		/// <param name="storyboard">The UIStoryboard containing the UIViewController subclass.</param>
		/// <param name="storyboardIdentifier">The UIViewController subclass's Storyboard identifier.</param>
		public static UIViewController Instantiate (this UIStoryboard storyboard, string storyboardIdentifier)
		=> storyboard.InstantiateViewController (storyboardIdentifier) as UIViewController;


		/// <summary>
		/// Dequeues a reusable UITableViewCell subclass using specified UITableView and NSIndexPath.
		/// Usually called in the UITableViewDataSource (or UITableViewController) GetCell override.
		/// This override assumes that the reuseIdentifier of the UITableViewCell subclass is the same as
		/// the Name of the Type.
		/// </summary>
		/// <param name="tableView">The UITableView from which to dequeue the cell</param>
		/// <param name="indexPath">The cell's NSIndexPath</param>
		/// <typeparam name="T">The Type of the UITableViewCell subclass to dequeue and return.</typeparam>
		public static T Dequeue<T> (this UITableView tableView, NSIndexPath indexPath)
			where T : UITableViewCell
		=> tableView.DequeueReusableCell (typeof (T).Name, indexPath) as T;


		/// <summary>
		/// Dequeues a reusable UITableViewCell subclass using specified UITableView and NSIndexPath.
		/// Usually called in the UITableViewDataSource (or UITableViewController) GetCell override.
		/// </summary>
		/// <param name="tableView">The UITableView from which to dequeue the cell.</param>
		/// <param name="reuseIdentifier">The reuseIdentifier of the UITableViewCell subclass to return.</param>
		/// <param name="indexPath">The cell's NSIndexPath.</param>
		/// <typeparam name="T">The Type of the UITableViewCell subclass to dequeue and return.</typeparam>
		public static T Dequeue<T> (this UITableView tableView, string reuseIdentifier, NSIndexPath indexPath)
			where T : UITableViewCell
		=> tableView.DequeueReusableCell (reuseIdentifier, indexPath) as T;


		/// <summary>
		/// Dequeues a reusable UICollectionViewCell subclass using specified UICollectionView and NSIndexPath.
		/// Usually called in the UICollectionViewDataSource (or UICollectionViewController) GetCell override.
		/// This override assumes that the reuseIdentifier of the UICollectionViewCell subclass is the same as
		/// the Name of the Type.
		/// </summary>
		/// <param name="collectionView">The UICollectionView from which to dequeue the cell.</param>
		/// <param name="indexPath">The cell's NSIndexPath.</param>
		/// <typeparam name="T">The Type of the UICollectionViewCell subclass to dequeue and return.</typeparam>
		public static T Dequeue<T> (this UICollectionView collectionView, NSIndexPath indexPath)
			where T : UICollectionViewCell
		=> collectionView.DequeueReusableCell (typeof (T).Name, indexPath) as T;


		/// <summary>
		/// Dequeues a reusable UICollectionViewCell subclass using specified UICollectionView and NSIndexPath.
		/// Usually called in the UICollectionViewDataSource (or UICollectionViewController) GetCell override.
		/// </summary>
		/// <param name="collectionView">The UICollectionView from which to dequeue the cell.</param>
		/// <param name="reuseIdentifier">The reuseIdentifier of the UICollectionViewCell subclass to return.</param>
		/// <param name="indexPath">The cell's NSIndexPath.</param>
		/// <typeparam name="T">The Type of the UICollectionViewCell subclass to dequeue and return.</typeparam>
		public static T Dequeue<T> (this UICollectionView collectionView, string reuseIdentifier, NSIndexPath indexPath)
			where T : UICollectionViewCell
		=> collectionView.DequeueReusableCell (reuseIdentifier, indexPath) as T;


		/// <summary>
		/// Dequeues a reusable UICollectionReusableView subclass using specified UICollectionView, elementKind and NSIndexPath.
		/// Usually called in the UICollectionViewDataSource (or UICollectionViewController) GetViewForSupplementaryElement override.
		/// This override assumes that the reuseIdentifier of the UICollectionReusableView subclass is the same as
		/// the Name of the Type.
		/// </summary>
		/// <param name="collectionView">The UICollectionView from which to dequeue the reusable view</param>
		/// <param name="elementKind">The kind of supplementary view to provide. 
		/// The value of this string is defined by the layout object that supports the supplementary view.</param>
		/// <param name="indexPath">The index path that specifies the location of the new supplementary view.</param>
		/// <typeparam name="T">The Type of the UICollectionReusableView subclass to dequeue and return.</typeparam>
		public static T Dequeue<T> (this UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
			where T : UICollectionReusableView
		=> collectionView.DequeueReusableSupplementaryView (elementKind, typeof (T).Name, indexPath) as T;


		/// <summary>
		/// Dequeues a reusable UICollectionReusableView subclass using specified UICollectionView, elementKind and NSIndexPath.
		/// Usually called in the UICollectionViewDataSource (or UICollectionViewController) GetViewForSupplementaryElement override.
		/// This override assumes that the reuseIdentifier of the UICollectionReusableView subclass is the same as
		/// the Name of the Type.
		/// </summary>
		/// <param name="collectionView">The UICollectionView from which to dequeue the reusable view</param>
		/// <param name="elementKind">The kind of supplementary view to provide. 
		/// The value of this string is defined by the layout object that supports the supplementary view.</param>
		/// <param name="reuseIdentifier">The reuseIdentifier of the UICollectionReusableView subclass to return.</param>
		/// <param name="indexPath">The index path that specifies the location of the new supplementary view.</param>
		/// <typeparam name="T">The Type of the UICollectionReusableView subclass to dequeue and return.</typeparam>
		public static T Dequeue<T> (this UICollectionView collectionView, NSString elementKind, string reuseIdentifier, NSIndexPath indexPath)
			where T : UICollectionReusableView
		=> collectionView.DequeueReusableSupplementaryView (elementKind, reuseIdentifier, indexPath) as T;


		/// <summary>
		/// Dequeues a reusable UICollectionReusableView subclass using specified UICollectionView, elementKind and NSIndexPath.
		/// Usually called in the UICollectionViewDataSource (or UICollectionViewController) GetViewForSupplementaryElement override.
		/// This override assumes that the reuseIdentifier of the UICollectionReusableView subclass is the same as
		/// the Name of the Type.
		/// </summary>
		/// <param name="collectionView">The UICollectionView from which to dequeue the reusable view</param>
		/// <param name="elementKind">The kind of supplementary view to provide. 
		/// The value of this string is defined by the layout object that supports the supplementary view.</param>
		/// <param name="indexPath">The index path that specifies the location of the new supplementary view.</param>
		/// <typeparam name="T">The Type of the UICollectionReusableView subclass to dequeue and return.</typeparam>
		public static T Dequeue<T> (this UICollectionView collectionView, UICollectionElementKindSection elementKind, NSIndexPath indexPath)
			where T : UICollectionReusableView
		=> collectionView.DequeueReusableSupplementaryView (elementKind, typeof (T).Name, indexPath) as T;


		/// <summary>
		/// Dequeues a reusable UICollectionReusableView subclass using specified UICollectionView, elementKind and NSIndexPath.
		/// Usually called in the UICollectionViewDataSource (or UICollectionViewController) GetViewForSupplementaryElement override.
		/// This override assumes that the reuseIdentifier of the UICollectionReusableView subclass is the same as
		/// the Name of the Type.
		/// </summary>
		/// <param name="collectionView">The UICollectionView from which to dequeue the reusable view</param>
		/// <param name="elementKind">The kind of supplementary view to provide. 
		/// The value of this string is defined by the layout object that supports the supplementary view.</param>
		/// <param name="reuseIdentifier">The reuseIdentifier of the UICollectionReusableView subclass to return.</param>
		/// <param name="indexPath">The index path that specifies the location of the new supplementary view.</param>
		/// <typeparam name="T">The Type of the UICollectionReusableView subclass to dequeue and return.</typeparam>
		public static T Dequeue<T> (this UICollectionView collectionView, UICollectionElementKindSection elementKind, string reuseIdentifier, NSIndexPath indexPath)
			where T : UICollectionReusableView
		=> collectionView.DequeueReusableSupplementaryView (elementKind, reuseIdentifier, indexPath) as T;
	}
}