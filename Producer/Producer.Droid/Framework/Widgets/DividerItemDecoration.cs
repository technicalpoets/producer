/*
 * Copyright (C) 2014 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Producer.Droid
{
	public class DividerItemDecoration : RecyclerView.ItemDecoration
	{
		int [] Attrs = { Android.Resource.Attribute.ListDivider };

		public const int HorizontalList = LinearLayoutManager.Horizontal;
		public const int VerticalList = LinearLayoutManager.Vertical;

		Drawable divider;
		int orientation;


		public DividerItemDecoration (Context context, int orientation)
		{
			var a = context.ObtainStyledAttributes (Attrs);
			divider = a.GetDrawable (0);
			a.Recycle ();
			Orientation = orientation;
		}


		/// <summary>
		/// Gets or sets orientation
		/// </summary>
		public int Orientation
		{
			get { return orientation; }
			set
			{
				if (value != HorizontalList && value != VerticalList)
					throw new ArgumentException ("Invalid orientation", nameof (value));
				orientation = value;
			}
		}


		public void DrawVertical (Canvas c, RecyclerView parent)
		{
			var left = parent.PaddingLeft;
			var right = parent.Right - parent.PaddingRight;

			var childCount = parent.ChildCount;

			for (int i = 0; i < childCount; i++)
			{
				var child = parent.GetChildAt (i);
				var layoutParams = child.LayoutParameters.JavaCast<RecyclerView.MarginLayoutParams> ();
				var top = child.Bottom + layoutParams.BottomMargin;
				var bottom = top + divider.IntrinsicHeight;
				divider.SetBounds (left, top, right, bottom);
				divider.Draw (c);
			}
		}


		public void DrawHorizontal (Canvas c, RecyclerView parent)
		{
			var top = parent.PaddingTop;
			var bottom = parent.PaddingBottom;

			var childCount = parent.ChildCount;

			for (int i = 0; i < childCount; i++)
			{
				var child = parent.GetChildAt (i);
				var layoutParams = child.LayoutParameters.JavaCast<RecyclerView.MarginLayoutParams> ();
				var left = child.Right + layoutParams.RightMargin;
				var right = left + divider.IntrinsicHeight;
				divider.SetBounds (left, top, right, bottom);
				divider.Draw (c);
			}
		}


		public override void GetItemOffsets (Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
		{
			if (orientation == VerticalList)
			{
				outRect.Set (0, 0, 0, divider.IntrinsicHeight);
			}
			else
			{
				outRect.Set (0, 0, divider.IntrinsicWidth, 0);
			}
		}


		public override void OnDraw (Canvas c, RecyclerView parent, RecyclerView.State state)
		{
			if (orientation == VerticalList)
			{
				DrawVertical (c, parent);
			}
			else
			{
				DrawHorizontal (c, parent);
			}
		}
	}
}