using Android.Graphics;
using Android.Text.Style;
using Android.Text;
using Android.OS;
using Android.Runtime;

namespace Producer.Droid
{
	public class UpdatableForegroundColorSpan : CharacterStyle, IParcelableSpan
	{
		Color color;
		public Color ForegroundColor => color;

		public UpdatableForegroundColorSpan (Color color)
		{
			this.color = color;
		}


		public override void UpdateDrawState (TextPaint tp)
		{
			tp.Color = ForegroundColor;
		}


		public void UpdateColor (Color newColor)
		{
			color = newColor;
		}


		public void UpdateAlpha (byte alpha)
		{
			color.A = alpha;
		}


		public int SpanTypeId
		{
			get
			{
				return 0;
			}
		}


		public int DescribeContents ()
		{
			return 0;
		}


		public void WriteToParcel (Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
		{
			dest.WriteInt (ForegroundColor);
		}
	}
}