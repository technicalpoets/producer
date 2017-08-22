using System;
using System.Collections.Generic;
using System.Text;
using Android.Text;

namespace Producer.Droid
{
	public class SimpleSpanBuilder
	{
		class SpanSection
		{
			readonly String text;
			readonly int startIndex;
			readonly IParcelableSpan [] spans;

			public SpanSection (String text, int startIndex, params IParcelableSpan [] spans)
			{
				this.spans = spans;
				this.text = text;
				this.startIndex = startIndex;
			}


			public void Apply (SpannableStringBuilder spanStringBuilder)
			{
				if (spanStringBuilder == null) return;

				foreach (IParcelableSpan span in spans)
				{
					spanStringBuilder.SetSpan ((Java.Lang.Object) span, startIndex, startIndex + text.Length, SpanTypes.InclusiveExclusive);
				}
			}
		}


		readonly List<SpanSection> spanSections;
		readonly StringBuilder stringBuilder;

		public SimpleSpanBuilder ()
		{
			stringBuilder = new StringBuilder ();
			spanSections = new List<SpanSection> ();
		}


		public SimpleSpanBuilder Append (String text, params IParcelableSpan [] spans)
		{
			if (spans != null && spans.Length > 0)
			{
				spanSections.Add (new SpanSection (text, stringBuilder.Length, spans));
			}

			stringBuilder.Append (text);

			return this;
		}


		public SpannableStringBuilder Build ()
		{
			var ssb = new SpannableStringBuilder (stringBuilder.ToString ());

			foreach (SpanSection section in spanSections)
			{
				section.Apply (ssb);
			}

			return ssb;
		}


		public override string ToString ()
		{
			return stringBuilder.ToString ();
		}
	}
}