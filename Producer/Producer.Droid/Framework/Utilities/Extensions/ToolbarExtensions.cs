using Android.Widget;

namespace Producer.Droid
{
	public static class ToolbarExtensions
	{
		public static ImageButton GetToolbarNavigationButton (this Android.Support.V7.Widget.Toolbar toolbar)
		{
			for (int i = 0; i < toolbar.ChildCount; i++)
			{
				var child = toolbar.GetChildAt (i);

				if (child is ImageButton btn)
				{
					if (btn.Drawable == toolbar.NavigationIcon)
					{
						return btn;
					}
				}
			}

			return null;
		}
	}
}