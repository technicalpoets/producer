//using System;
//using Android.Content;
//using Android.Graphics;
//using Com.Bumptech.Glide.Load.Engine.Bitmap_recycle;
//using Com.Bumptech.Glide.Load.Resource.Bitmap;

//namespace Producer.Droid
//{
//	public class CircleTransform : BitmapTransformation
//	{
//		public override string Id
//		{
//			get
//			{
//				return nameof (CircleTransform);
//			}
//		}


//		public CircleTransform (Context context) : base (context)
//		{
//		}


//		protected override Bitmap Transform (IBitmapPool p0, Bitmap p1, int p2, int p3)
//		{
//			return circleCrop (p0, p1);
//		}


//		static Bitmap circleCrop (IBitmapPool pool, Bitmap source)
//		{
//			if (source == null) return null;

//			int size = Math.Min (source.Width, source.Height);
//			int x = (source.Width - size) / 2;
//			int y = (source.Height - size) / 2;

//			// TODO this could be acquired from the pool too
//			//Bitmap squared = Bitmap.CreateBitmap (source, x, y, size, size);

//			Bitmap bitmap = pool.Get (size, size, Bitmap.Config.Argb8888);

//			if (bitmap == null)
//			{
//				bitmap = Bitmap.CreateBitmap (size, size, Bitmap.Config.Argb8888);
//			}

//			using (var canvas = new Canvas (bitmap))
//			using (var paint = new Paint ())
//			{
//				var shader = new BitmapShader (source, Shader.TileMode.Clamp, Shader.TileMode.Clamp);

//				if (x != 0 || y != 0)
//				{
//					// source isn't square, move viewport to center
//					var matrix = new Matrix ();
//					matrix.SetTranslate (-x, -y);
//					shader.SetLocalMatrix (matrix);
//				}

//				paint.SetShader (shader);
//				paint.AntiAlias = true;

//				float radius = size / 2f;
//				canvas.DrawCircle (radius, radius, radius, paint);
//			}

//			//return BitmapResource.Obtain (bitmap, pool);

//			return bitmap;
//		}
//	}
//}