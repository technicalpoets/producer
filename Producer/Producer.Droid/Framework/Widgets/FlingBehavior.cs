using Android.Content;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Util;

namespace Producer.Droid
{
	/// <summary>
	/// Fling behavior - fixes RecyclerView bug.  See http://stackoverflow.com/questions/30923889/flinging-with-recyclerview-appbarlayout.
	/// </summary>
	[Register ("partners.droid.FlingBehavior")]
	public class FlingBehavior : AppBarLayout.Behavior
	{
		const int TOP_CHILD_FLING_THRESHOLD = 3;
		bool isPositive;


		public FlingBehavior ()
		{
		}


		public FlingBehavior (Context context, IAttributeSet attrs) : base (context, attrs)
		{
		}


		public override bool OnNestedFling (CoordinatorLayout coordinatorLayout, AppBarLayout child, Android.Views.View target, float velocityX, float velocityY, bool consumed)
		{
			if (velocityY > 0 && !isPositive || velocityY < 0 && isPositive)
			{
				velocityY = velocityY * -1;
			}

			if (target is RecyclerView && velocityY < 0)
			{
				var recyclerView = (RecyclerView) target;
				var firstChild = recyclerView.GetChildAt (0);
				int childAdapterPosition = recyclerView.GetChildAdapterPosition (firstChild);
				consumed = childAdapterPosition > TOP_CHILD_FLING_THRESHOLD;
			}

			return base.OnNestedFling (coordinatorLayout, child, target, velocityX, velocityY, consumed);
		}


		public override void OnNestedPreScroll (CoordinatorLayout coordinatorLayout, AppBarLayout child, Android.Views.View target, int dx, int dy, int [] consumed)
		{
			base.OnNestedPreScroll (coordinatorLayout, child, target, dx, dy, consumed);
			isPositive = dy > 0;
		}
	}
}