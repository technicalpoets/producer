using System;
using Android.Animation;
using Android.App;
using Android.Views;

namespace Producer.Droid.Framework.Widgets
{
	public static class FlipAnimator
	{
		/// <summary>
		/// Performs flip animation on two views.
		/// </summary>
		/// <param name="back">Back.</param>
		/// <param name="front">Front.</param>
		/// <param name="showFront">If set to <c>true</c> show front.</param>
		public static void FlipView (View back, View front, bool showFront)
		{
			AnimatorSet leftIn, rightOut, leftOut, rightIn;

			leftIn = (AnimatorSet) AnimatorInflater.LoadAnimator (Application.Context, Resource.Animator.flip_left_in);
			rightOut = (AnimatorSet) AnimatorInflater.LoadAnimator (Application.Context, Resource.Animator.flip_right_out);
			leftOut = (AnimatorSet) AnimatorInflater.LoadAnimator (Application.Context, Resource.Animator.flip_left_out);
			rightIn = (AnimatorSet) AnimatorInflater.LoadAnimator (Application.Context, Resource.Animator.flip_right_in);

			var animator = new AnimatorSet ();
			animator.AnimationEnd += (sender, e) =>
			{
				animator.Dispose ();
				leftIn.Dispose ();
				rightOut.Dispose ();
				leftOut.Dispose ();
				rightIn.Dispose ();
			};

			if (showFront)
			{
				leftIn.SetTarget (back);
				rightOut.SetTarget (front);
				animator.PlayTogether (leftIn, rightOut);
			}
			else
			{
				leftOut.SetTarget (back);
				rightIn.SetTarget (front);
				animator.PlayTogether (rightIn, leftOut);
			}

			animator.Start ();
		}


		/// <summary>
		/// Performs flip animation on two views.
		/// </summary>
		/// <param name="back">Back.</param>
		/// <param name="front">Front.</param>
		/// <param name="showFront">If set to <c>true</c> show front.</param>
		/// <param name="afterAnimation"></param>
		public static void FlipView (View back, View front, bool showFront, Action afterAnimation)
		{
			AnimatorSet leftIn, rightOut, leftOut, rightIn;

			leftIn = (AnimatorSet) AnimatorInflater.LoadAnimator (Application.Context, Resource.Animator.flip_left_in);
			rightOut = (AnimatorSet) AnimatorInflater.LoadAnimator (Application.Context, Resource.Animator.flip_right_out);
			leftOut = (AnimatorSet) AnimatorInflater.LoadAnimator (Application.Context, Resource.Animator.flip_left_out);
			rightIn = (AnimatorSet) AnimatorInflater.LoadAnimator (Application.Context, Resource.Animator.flip_right_in);

			var animator = new AnimatorSet ();
			animator.AnimationEnd += (sender, e) =>
			{
				animator.Dispose ();
				leftIn.Dispose ();
				rightOut.Dispose ();
				leftOut.Dispose ();
				rightIn.Dispose ();

				afterAnimation ();
			};

			if (showFront)
			{
				leftIn.SetTarget (back);
				rightOut.SetTarget (front);
				animator.PlayTogether (leftIn, rightOut);
			}
			else
			{
				leftOut.SetTarget (back);
				rightIn.SetTarget (front);
				animator.PlayTogether (rightIn, leftOut);
			}

			animator.Start ();
		}
	}
}