using System;

using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Producer.iOS
{
	[Register ("CircularProgressLayer")]
	public class CircularProgressLayer : CALayer
	{

		public UIColor TrackTintColor { get; set; } = UIColor.FromWhiteAlpha (1.0f, 0.3f);

		public UIColor ProgressTintColor { get; set; } = UIColor.LightGray;

		public UIColor InnerTintColor { get; set; }


		public bool RoundedCorners { get; set; } = true;

		public bool ClockwiseProgress { get; set; } = true;

		public nfloat ThicknessRadio { get; set; } = 0.2f;


		public nfloat IndeterminateProgress { get; set; } = 0.3f;


		[Export ("progress")]
		public nfloat Progress { get; set; }


		[Export ("needsDisplayForKey:")]
		public static bool NeedsDisplayForKeyOverride (string key) => key == CircularProgressAnimationKeys.progress || NeedsDisplayForKey (key);


		public CircularProgressLayer (IntPtr handle) : base (handle) { }


		[Export ("initWithLayer:")]
		public CircularProgressLayer (CircularProgressLayer other) : base (other) { }


		public override void Clone (CALayer other)
		{
			var original = other as CircularProgressLayer;

			TrackTintColor = original.TrackTintColor;
			ProgressTintColor = original.ProgressTintColor;
			InnerTintColor = original.InnerTintColor;

			RoundedCorners = original.RoundedCorners;
			ClockwiseProgress = original.ClockwiseProgress;
			ThicknessRadio = original.ThicknessRadio;

			IndeterminateProgress = original.IndeterminateProgress;
			Progress = original.Progress;
		}


		public override void DrawInContext (CGContext ctx)
		{
			var rect = Bounds;

			var centerPoint = new CGPoint (rect.Width / 2, rect.Height / 2);

			var radius = NMath.Min (rect.Height, rect.Width) / 2;


			var progress = NMath.Min (Progress, 1 - nfloat.Epsilon);

			nfloat radians = 0;

			if (ClockwiseProgress)
			{
				radians = (progress * 2 * NMath.PI) - (NMath.PI / 2);
			}
			else
			{
				radians = (3 * (NMath.PI / 2)) - (progress * 2 * NMath.PI);
			}


			// fill track tint

			if (TrackTintColor != null)
			{
				ctx.SetFillColor (TrackTintColor.CGColor);

				using (var trackPath = new CGPath ())
				{
					trackPath.MoveToPoint (centerPoint);
					trackPath.AddArc (centerPoint.X, centerPoint.Y, radius, 2 * NMath.PI, 0, true);
					trackPath.CloseSubpath ();
					ctx.AddPath (trackPath);
					ctx.FillPath ();
				}
			}


			// fill progress if necessary

			if (progress > 0)
			{
				// fill progress

				ctx.SetFillColor (ProgressTintColor.CGColor);

				using (var progressPath = new CGPath ())
				{
					progressPath.MoveToPoint (centerPoint);
					progressPath.AddArc (centerPoint.X, centerPoint.Y, radius, (3 * (NMath.PI / 2)), radians, !ClockwiseProgress);
					progressPath.CloseSubpath ();
					ctx.AddPath (progressPath);
					ctx.FillPath ();
				}


				// round corners (if necessary)

				if (RoundedCorners)
				{
					var pathWidth = radius * ThicknessRadio;

					var xOffset = radius * (1 + ((1 - (ThicknessRadio / 2)) * NMath.Cos (radians)));
					var yOffset = radius * (1 + ((1 - (ThicknessRadio / 2)) * NMath.Sin (radians)));

					var endPoint = new CGPoint (xOffset, yOffset);

					var startEllipseRect = new CGRect (centerPoint.X - pathWidth / 2, 0, pathWidth, pathWidth);

					ctx.AddEllipseInRect (startEllipseRect);
					ctx.FillPath ();

					var endEllipseRect = new CGRect (endPoint.X - pathWidth / 2, endPoint.Y - pathWidth / 2, pathWidth, pathWidth);

					ctx.AddEllipseInRect (endEllipseRect);
					ctx.FillPath ();
				}
			}


			// notch center circle

			ctx.SetBlendMode (CGBlendMode.Clear);

			var innerRadius = radius * (1 - ThicknessRadio);

			var clearRect = new CGRect (centerPoint.X - innerRadius, centerPoint.Y - innerRadius, innerRadius * 2, innerRadius * 2);

			ctx.AddEllipseInRect (clearRect);
			ctx.FillPath ();


			// fill inner tint (if necessary)

			if (InnerTintColor != null)
			{
				ctx.SetBlendMode (CGBlendMode.Normal);
				ctx.SetFillColor (InnerTintColor.CGColor);
				ctx.AddEllipseInRect (clearRect);
				ctx.FillPath ();
			}
		}
	}
}
