using System;
using Android.Content;
using Android.Views;
using Java.Lang;

namespace lols
{
	public static class BindedHelper
	{
        public static TextView CreateTextView(Context context, Android.Graphics.Color color, float rotation, int x, int y)
        {
            TextView textView = new TextView(context);
            textView.Text="lol?";
            textView.SetTextColor(color);
            textView.Rotation=rotation;

            RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            layoutParams.LeftMargin = x;
            layoutParams.TopMargin = y;
            textView.LayoutParameters=layoutParams;

            return textView;
        }

        public static void Add(RelativeLayout layout, TextView textView)
        {
            int childCount = layout.ChildCount;
            if (childCount > 500)
                layout.RemoveViewAt(childCount - 2);
            layout.AddView(textView, 0);
        }
    }
}

