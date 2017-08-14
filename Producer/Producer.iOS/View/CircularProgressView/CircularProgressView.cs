using System;

using CoreAnimation;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Producer.iOS
{
	[Register ("CircularProgressView")]
	public class CircularProgressView : UIView
	{
		bool isSetup;


		CircularProgressLayer progressLayer => Layer as CircularProgressLayer;


		/// The color of the empty progress track (gets drawn over)
		public UIColor TrackTintColor {
			get { return progressLayer.TrackTintColor; }
			set {
				progressLayer.TrackTintColor = value;
				progressLayer.SetNeedsDisplay ();
			}
		}


		/// The color of the progress bar
		public UIColor ProgressTintColor {
			get { return progressLayer.ProgressTintColor; }
			set {
				progressLayer.ProgressTintColor = value;
				progressLayer.SetNeedsDisplay ();
			}
		}


		/// The color the notched out circle within the progress area (if there is one)
		public UIColor InnerTintColor {
			get { return progressLayer.InnerTintColor; }
			set {
				progressLayer.InnerTintColor = value;
				progressLayer.SetNeedsDisplay ();
			}
		}


		/// Sets whether or not the corners of the progress bar should be rounded
		public bool RoundedCorners {
			get { return progressLayer.RoundedCorners; }
			set {
				progressLayer.RoundedCorners = value;
				progressLayer.SetNeedsDisplay ();
			}
		}


		/// Sets whether or not the animation should be clockwise
		public bool ClockwiseProgress {
			get { return progressLayer.ClockwiseProgress; }
			set {
				progressLayer.ClockwiseProgress = value;
				progressLayer.SetNeedsDisplay ();
			}
		}


		/// Sets how thick the progress bar should be (pinned between <c>0.01</c> and <c>1</c>)
		public nfloat ThicknessRadio {
			get { return progressLayer.ThicknessRadio; }
			set {
				progressLayer.ThicknessRadio = pin (value, 0.01f, 1.0f);
				progressLayer.SetNeedsDisplay ();
			}
		}


		/// A timing function defining the pacing of the animation. Defaults to ease in, ease out.
		public CAMediaTimingFunction TimingFunction { get; set; } = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);


		/// Getter for the current progress (not observed from any active animations)
		public nfloat Progress => progressLayer.Progress;


		/// Sets how much of the progress bar should be filled during an indeterminate animation, pinned between <c>0.05</c> and <c>0.9</c> - This can be overriden / animated from by using updateProgress(...)
		public nfloat IndeterminateProgress {
			get { return progressLayer.IndeterminateProgress; }
			set { progressLayer.IndeterminateProgress = pin (value, 0.05f, 0.9f); }
		}


		/// Controls the speed at which the indeterminate progress bar animates
		public double IndeterminateDuration { get; set; } = 1.0;


		[Export ("layerClass")]
		public static Class LayerClass () => new Class (typeof (CircularProgressLayer));


		public CircularProgressView ()
		{
			setup ();
		}


		public CircularProgressView (IntPtr handle) : base (handle)
		{
			setup ();
		}


		public override bool ClearsContextBeforeDrawing {
			get { return true; }
			set { base.ClearsContextBeforeDrawing = value; }
		}


		public override void MovedToWindow ()
		{
			base.MovedToWindow ();

			if (Window != null)
			{
				progressLayer.ContentsScale = Window.Screen.Scale;
				progressLayer.SetNeedsDisplay ();
			}
		}


		/// Enables the indeterminate (spinning) animation.
		public void StartIndeterminate () => addIndeterminateAnimation ();


		/// <summary>
		/// Updates the progress bar to the given value with the optional properties
		/// </summary>
		/// <param name="progress">The progress to update to, pinned between <c>0</c> and <c>1</c>.</param>
		/// <param name="animated">Whether or not the update should be animated (defaults to <c>true</c>).</param>
		/// <param name="initialDelay">Sets an initial delay before the animation begins.</param>
		/// <param name="duration">Sets the overal duration that the animation should complete within.</param>
		public void UpdateProgress (nfloat progress, bool animated = true, double initialDelay = 0, double? duration = null)
		{
			var pinnedProgress = pin (progress, 0.0f, 1.0f);

			var indeterminate = progressLayer.AnimationForKey (CircularProgressAnimationKeys.indeterminate);

			if (indeterminate != null && pinnedProgress > 0)
			{
				progressLayer.RemoveAnimation (CircularProgressAnimationKeys.indeterminate);
			}

			if (animated)
			{
				// Get duration
				double animationDuration = 0;

				if (duration.HasValue && Math.Abs (duration.Value) > double.Epsilon)
				{
					animationDuration = duration.Value;
				}
				else
				{
					// Same duration as UIProgressView animation
					animationDuration = NMath.Abs (Progress - pinnedProgress);
				}

				// Get current progress (to avoid jumpy behavior)
				// Basic animations have their value reset to the original once the animation is finished
				// since only the presentation layer is animating
				nfloat currentProgress = 0;

				var presentationLayer = progressLayer.PresentationLayer as CircularProgressLayer;

				if (presentationLayer != null)
				{
					currentProgress = presentationLayer.Progress;
				}

				progressLayer.Progress = currentProgress;

				progressLayer.RemoveAnimation (CircularProgressAnimationKeys.progress);

				animate (progress, currentProgress, initialDelay, animationDuration);
			}
			else
			{
				progressLayer.RemoveAnimation (CircularProgressAnimationKeys.progress);

				progressLayer.Progress = pinnedProgress;

				progressLayer.SetNeedsDisplay ();
			}
		}


		void setup ()
		{
			if (isSetup) return;

			isSetup = true;

			BackgroundColor = UIColor.Clear;
		}


		void animate (nfloat pinnedProgress, nfloat currentProgress, double initialDelay, double duration)
		{
			var animation = CABasicAnimation.FromKeyPath (CircularProgressAnimationKeys.progress);

			animation.Duration = duration;

			animation.TimingFunction = TimingFunction;

			animation.From = NSNumber.FromDouble (currentProgress);

			animation.FillMode = CAFillMode.Forwards;

			//animation.RemovedOnCompletion = false;
			animation.RemovedOnCompletion = true;

			animation.To = NSNumber.FromDouble (pinnedProgress);

			animation.BeginTime = CAAnimation.CurrentMediaTime () + initialDelay;

			animation.AnimationStopped += handleAnimationStopped;

			progressLayer.AddAnimation (animation, CircularProgressAnimationKeys.progress);
		}


		void addIndeterminateAnimation ()
		{
			var indeterminate = progressLayer.AnimationForKey (CircularProgressAnimationKeys.indeterminate);

			if (indeterminate == null)
			{
				var animation = CABasicAnimation.FromKeyPath (CircularProgressAnimationKeys.transformRotation);

				animation.By = NSNumber.FromNFloat (ClockwiseProgress ? 2 * NMath.PI : -2 * NMath.PI);

				animation.Duration = IndeterminateDuration;

				animation.RepeatCount = float.MaxValue;

				//animation.RemovedOnCompletion = false;
				animation.RemovedOnCompletion = true;

				progressLayer.Progress = IndeterminateProgress;

				progressLayer.AddAnimation (animation, CircularProgressAnimationKeys.indeterminate);
			}

			progressLayer.Progress = IndeterminateProgress;
		}


		void handleAnimationStopped (object sender, CAAnimationStateEventArgs e)
		{
			var animation = sender as CABasicAnimation;

			Log.Debug ($"AnimationStopped Finished={e.Finished} | animation={animation}");

			if (animation != null)
			{
				var completedValue = animation.GetToAs<NSNumber> ().DoubleValue;

				progressLayer.Progress = (nfloat)completedValue;
			}
		}


		// Pin certain values between 0.0 and 1.0
		nfloat pin (nfloat val, nfloat minVal, nfloat maxVal) => NMath.Min (NMath.Max (val, minVal), maxVal);
	}
}
