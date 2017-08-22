//using Android.Content;
//using Android.Content.Res;
//using Android.Runtime;
//using Android.Support.Design.Widget;
//using Android.Util;
//using Android.Views;
//using Android.Widget;

//namespace Producer.Droid
//{
//	[Register ("com.xamarin.partners.AvatarImageBehavior")]
//	public class AvatarImageBehavior : CoordinatorLayout.Behavior
//	{
//		const float MIN_AVATAR_PERCENTAGE_SIZE = 0.3f;
//		const int EXTRA_FINAL_AVATAR_PADDING = 80;

//		readonly Context context;

//		//float customFinalYPosition;
//		//float customStartXPosition;
//		//float customStartToolbarPosition;
//		//float customStartHeight;
//		float customFinalHeight;

//		int avatarMaxSize;
//		//float finalLeftAvatarPadding;
//		//float startPosition;
//		int startXPosition;
//		float startToolbarPosition;
//		int startYPosition;
//		int finalYPosition;
//		int startHeight;
//		int finalXPosition;
//		float changeBehaviorPoint;
//		bool leftToRight;

//		public AvatarImageBehavior (Context context, IAttributeSet attrs)
//		{
//			this.context = context;

//			if (attrs != null)
//			{
//				TypedArray a = context.ObtainStyledAttributes (attrs, Resource.Styleable.AvatarImageBehavior);
//				//customFinalYPosition = a.GetDimension (Resource.Styleable.AvatarImageBehavior_finalYPosition, 0);
//				//customStartXPosition = a.GetDimension (Resource.Styleable.AvatarImageBehavior_startXPosition, 0);
//				//customStartToolbarPosition = a.GetDimension (Resource.Styleable.AvatarImageBehavior_startToolbarPosition, 0);
//				//customStartHeight = a.GetDimension (Resource.Styleable.AvatarImageBehavior_startHeight, 0);
//				customFinalHeight = a.GetDimension (Resource.Styleable.AvatarImageBehavior_finalHeight, 0);
//				leftToRight = a.GetBoolean (Resource.Styleable.AvatarImageBehavior_animateLeftToRight, false);

//				a.Recycle ();
//			}

//			//finalLeftAvatarPadding = context.Resources.GetDimension (
//			//	Resource.Dimension.spacing_normal);
//		}


//		public override bool LayoutDependsOn (CoordinatorLayout parent, Java.Lang.Object child, Android.Views.View dependency)
//		{
//			return dependency is Android.Support.V7.Widget.Toolbar;
//		}


//		public override bool OnDependentViewChanged (CoordinatorLayout parent, Java.Lang.Object child, Android.Views.View dependency)
//		{
//			var imageView = (ImageView) child;

//			maybeInitProperties (imageView, dependency);

//			//var maxScrollDistance = (int)(startToolbarPosition);
//			var maxScrollDistance = (int) startToolbarPosition;
//			float expandedPercentageFactor = dependency.GetY () / maxScrollDistance;

//			float distanceYToSubtract = ((startYPosition - finalYPosition) * (1f - expandedPercentageFactor)) + (imageView.Height / 2);

//			float xDelta = ((startXPosition - finalXPosition) * (1f - expandedPercentageFactor));

//			float heightToSubtract = ((startHeight - customFinalHeight) * (1f - expandedPercentageFactor));

//			imageView.SetY (startYPosition - distanceYToSubtract);

//			if (!leftToRight)
//			{
//				imageView.SetX (startXPosition - xDelta + (imageView.Width / 2));
//			}
//			else
//			{
//				imageView.SetX (startXPosition + xDelta - (imageView.Width / 2));
//			}

//			//var proportionalAvatarSize = (int)(avatarMaxSize * (expandedPercentageFactor));

//			var lp = (CoordinatorLayout.LayoutParams) imageView.LayoutParameters;
//			lp.Width = (int) (startHeight - heightToSubtract);
//			lp.Height = (int) (startHeight - heightToSubtract);
//			imageView.LayoutParameters = lp;


//			//if (expandedPercentageFactor < changeBehaviorPoint) {
//			//	float heightFactor = (changeBehaviorPoint - expandedPercentageFactor) / changeBehaviorPoint;

//			//	float xDelta = ((startXPosition - finalXPosition)
//			//			* heightFactor) + (imageView.Height / 2);
//			//	float distanceYToSubtract = ((startYPosition - finalYPosition)
//			//			* (1f - expandedPercentageFactor)) + (imageView.Height / 2);

//			//	imageView.SetX (startXPosition + xDelta);
//			//	imageView.SetY (startYPosition - distanceYToSubtract);

//			//	float heightToSubtract = ((startHeight - customFinalHeight) * heightFactor);

//			//	var lp = (CoordinatorLayout.LayoutParams)imageView.LayoutParameters;
//			//	lp.Width = (int)(startHeight - heightToSubtract);
//			//	lp.Height = (int)(startHeight - heightToSubtract);
//			//	imageView.LayoutParameters = lp;
//			//} else {
//			//	float distanceYToSubtract = ((startYPosition - finalYPosition)
//			//			* (1f - expandedPercentageFactor)) + (startHeight / 2);

//			//	imageView.SetX (startXPosition - imageView.Width / 2);
//			//	imageView.SetY (startYPosition - distanceYToSubtract);

//			//	var lp = (CoordinatorLayout.LayoutParams)imageView.LayoutParameters;
//			//	lp.Width = startHeight;
//			//	lp.Height = startHeight;
//			//	imageView.LayoutParameters = lp;
//			//}

//			return true;
//		}


//		void maybeInitProperties (ImageView child, View dependency)
//		{
//			if (avatarMaxSize == 0)
//			{
//				avatarMaxSize = dependency.Height;
//			}

//			if (startYPosition == 0)
//				startYPosition = (int) (dependency.GetY ());

//			if (finalYPosition == 0)
//				finalYPosition = (dependency.Height / 2);

//			if (startHeight == 0)
//				startHeight = child.Height;

//			if (startXPosition == 0)
//				startXPosition = (int) (child.GetX () + (child.Width / 2));

//			if (finalXPosition == 0)
//				finalXPosition = context.Resources.GetDimensionPixelOffset (Resource.Dimension.abc_action_bar_content_inset_material) + ((int) customFinalHeight / 2);

//#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
//			if (startToolbarPosition == 0)
//				//startToolbarPosition = dependency.GetY () + (dependency.Height / 2);
//				startToolbarPosition = dependency.GetY ();

//			if (changeBehaviorPoint == 0)
//			{
//				changeBehaviorPoint = (child.Height - customFinalHeight) / (2f * (startYPosition - finalYPosition));
//			}
//#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
//		}


//		public int getStatusBarHeight ()
//		{
//			int result = 0;
//			int resourceId = context.Resources.GetIdentifier ("status_bar_height", "dimen", "android");

//			if (resourceId > 0)
//			{
//				result = context.Resources.GetDimensionPixelSize (resourceId);
//			}

//			return result;
//		}
//	}
//}