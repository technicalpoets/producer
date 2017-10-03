using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;

namespace Producer.Droid
{
	public abstract class RecyclerViewListFragment<TData, TViewHolder> : Fragment, SwipeRefreshLayout.IOnRefreshListener
		where TViewHolder : ViewHolder<TData>
	{
		public bool ShowDividers { get; set; } = true;
		public bool EnableLongClick { get; set; }
		public bool EnablePullToRefresh { get; set; } = true;

		public RecyclerView RecyclerView;
		public RecyclerView.Adapter Adapter;
		public RecyclerView.LayoutManager LayoutManager;
		public RecyclerView.ItemAnimator ItemAnimator;
		public SwipeRefreshLayout SwipeRefreshLayout;

		public RecyclerViewAdapter<TData, TViewHolder> TypedAdapter { get; private set; }

		protected Task LoadDataTask;


		protected abstract RecyclerViewAdapter<TData, TViewHolder> CreateAdapter ();


		#region Lifecycle Methods


		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var rootView = inflater.Inflate (Resource.Layout.RecyclerViewFragment, container, false);

			RecyclerView = rootView.FindViewById<RecyclerView> (Resource.Id.recyclerView);

			LayoutManager = GetLayoutManager ();
			RecyclerView.SetLayoutManager (LayoutManager);
			ItemAnimator = GetItemAnimator ();
			RecyclerView.SetItemAnimator (ItemAnimator);

			SwipeRefreshLayout = rootView.FindViewById<SwipeRefreshLayout> (Resource.Id.swipe_refresh_layout);

			if (EnablePullToRefresh)
			{
				SwipeRefreshLayout.SetOnRefreshListener (this);
			}

			//adds item divider lines if ShowDividers == true
			if (ShowDividers)
			{
				RecyclerView.AddItemDecoration (new DividerItemDecoration (Activity, DividerItemDecoration.VerticalList));
			}

			Adapter = TypedAdapter = CreateAdapter ();
			RecyclerView.ScrollToPosition (0);
			RecyclerView.SetAdapter (Adapter);

			//start to load the data that will populate the RecyclerView
			loadData ();

			return rootView;
		}


		public override void OnStart ()
		{
			base.OnStart ();

			AttachEvents ();
		}


		public override void OnStop ()
		{
			DetachEvents ();

			base.OnStop ();
		}


		public override void OnDestroy ()
		{
			DetachEvents ();

			base.OnDestroy ();
		}


		#endregion


		protected virtual void AttachEvents ()
		{
			TypedAdapter.ItemsFiltered += Adapter_ItemsFiltered;

			TypedAdapter.SetItemClickHandler (OnItemClick);

			if (EnableLongClick)
			{
				TypedAdapter.SetItemLongClickHandler (OnItemLongClick);
			}
		}


		void DetachEvents ()
		{
			if (Adapter != null)
			{
				TypedAdapter.ItemsFiltered -= Adapter_ItemsFiltered;

				TypedAdapter.SetItemClickHandler (null);
				TypedAdapter.SetItemLongClickHandler (null);
			}
		}


		void Adapter_ItemsFiltered (object sender, EventArgs e)
		{
			LayoutManager.ScrollToPosition (0);
		}


		/// <summary>
		/// Gets the layout manager that will be used for this RecyclerView.  Defaults to <see cref="LinearLayoutManager"/>.
		/// </summary>
		/// <returns>The layout manager.</returns>
		protected virtual RecyclerView.LayoutManager GetLayoutManager () => new LinearLayoutManager (Context);


		/// <summary>
		/// Gets the item animator that will be used for this RecyclerView.  Defaults to <see cref="DefaultItemAnimator"/>.
		/// </summary>
		/// <returns>The item animator.</returns>
		protected virtual RecyclerView.ItemAnimator GetItemAnimator () => new DefaultItemAnimator ();


		protected virtual Task LoadData () => Task.Delay (0);


		void loadData ()
		{
			SwipeRefreshLayout.Refreshing = true;

			//only start a content refresh if there isn't on running already
			if (LoadDataTask == null || LoadDataTask.IsFaulted || LoadDataTask.IsCanceled)
			{
				Log.Debug ("Starting refreshTask");
				LoadDataTask = LoadData ();
			}

			if (LoadDataTask.IsCompleted)
			{
				Log.Debug ("refreshTask complete; updating adapter(s)");
				Activity.RunOnUiThread (() => OnDataLoaded ());
			}
			else //add a Task continuation that will run after the data Task is finished
			{
				Log.Debug ("refreshTask in process, adding completion to update adapter(s)");
				LoadDataTask.ContinueWith (t => OnDataLoaded (), TaskScheduler.FromCurrentSynchronizationContext ());
			}
		}


		protected virtual void OnDataLoaded ()
		{
			SwipeRefreshLayout.Refreshing = false;
			LoadDataTask = null;

			//if we don't want pull to refresh functionality, disable the refresh layout after we've loaded here the first time
			if (!EnablePullToRefresh)
			{
				SwipeRefreshLayout.Enabled = false;
			}
		}


		protected virtual void OnItemClick (View view, TData item, int position)
		{
		}


		protected virtual void OnItemLongClick (View view, TData item, int position)
		{
		}


		/// <summary>
		/// Transitions to activity, optionally with a shared element transition.
		/// </summary>
		/// <param name="intent">The Intent for the activity to launch.</param>
		/// <param name="transitionView">The view that contains the shared element transition, if any.</param>
		protected virtual void TransitionToActivity (Intent intent, View transitionView = null)
		{
			Bundle options = null;

			// shared element transitions are only supported on Android 5.0+
			if (transitionView != null)
			{
				options = ActivityOptionsCompat.MakeSceneTransitionAnimation (Activity, transitionView, transitionView.TransitionName).ToBundle ();
			}

			StartActivity (intent, options);
		}


		/// <summary>
		/// Called when a pull to refresh operation is triggered.  <see cref="EnablePullToRefresh"/> must be set to <c>True</c>.
		/// </summary>
		public virtual void OnRefresh ()
		{
			loadData ();
		}
	}
}