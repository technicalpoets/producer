using Android.OS;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using System;
using Android.Content;

namespace Producer.Droid
{
	public abstract class RecyclerViewListFragment<TData, TViewHolder> : Fragment
		where TViewHolder : ViewHolder<TData>
	{
		public bool ShowDividers { get; set; } = true;

		public RecyclerView RecyclerView;
		public RecyclerView.Adapter Adapter;
		public RecyclerView.LayoutManager LayoutManager;

		protected abstract RecyclerViewAdapter<TData, TViewHolder> GetAdapter ();

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var rootView = inflater.Inflate (Resource.Layout.RecyclerViewFragment, container, false);
			//rootView.SetTag (rootView.Id, TAG);
			RecyclerView = rootView.FindViewById<RecyclerView> (Resource.Id.recyclerView);

			// A LinearLayoutManager is used here, this will layout the elements in a similar fashion
			// to the way ListView would layout elements. The RecyclerView.LayoutManager defines how the
			// elements are laid out.
			LayoutManager = new LinearLayoutManager (Activity);
			RecyclerView.SetLayoutManager (LayoutManager);

			//adds item divider lines if ShowDividers == true
			if (ShowDividers)
			{
				RecyclerView.AddItemDecoration (new DividerItemDecoration (Activity, DividerItemDecoration.VerticalList));
			}

			Adapter = GetAdapter ();
			RecyclerView.ScrollToPosition (0);
			RecyclerView.SetAdapter (Adapter);

			return rootView;
		}


		public override void OnStart ()
		{
			base.OnStart ();

			((RecyclerViewAdapter<TData, TViewHolder>) Adapter).ItemClick += Adapter_ItemClick;
			((RecyclerViewAdapter<TData, TViewHolder>) Adapter).ItemsFiltered += Adapter_ItemsFiltered;
		}


		void detachEvents ()
		{
			if (Adapter != null)
			{
				((RecyclerViewAdapter<TData, TViewHolder>) Adapter).ItemClick -= Adapter_ItemClick;
				((RecyclerViewAdapter<TData, TViewHolder>) Adapter).ItemsFiltered -= Adapter_ItemsFiltered;
			}
		}


		public override void OnStop ()
		{
			detachEvents ();

			base.OnStop ();
		}


		public override void OnDestroy ()
		{
			detachEvents ();

			base.OnDestroy ();
		}


		void Adapter_ItemsFiltered (object sender, EventArgs e)
		{
			LayoutManager.ScrollToPosition (0);
		}


		void Adapter_ItemClick (object sender, TData item)
		{
			var view = (View) sender;

			OnItemClick (view, item);
		}


		protected virtual void OnItemClick (View view, TData item)
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
			if (transitionView != null && Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
			{
				//ActivityOptionsCompat DOES NOT WORK :(:(:(  Using a build version check here b/c we need to use the non-compat API
				//	https://bugzilla.xamarin.com/show_bug.cgi?id=40527
				options = Android.App.ActivityOptions.MakeSceneTransitionAnimation (Activity, transitionView, transitionView.TransitionName).ToBundle ();
			}

			StartActivity (intent, options);
		}
	}
}