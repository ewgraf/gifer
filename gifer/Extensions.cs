using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace gifer
{
    public static class SizeExtensions
    {
        public static Size Divide(this Size size, int by)
        {
            return new Size(size.Width / by, size.Height / by);
        }

        public static bool AbsMore(this Size size1, Size size2)
        {
            return Math.Abs(size1.Width) > Math.Abs(size2.Width) && Math.Abs(size1.Height) > Math.Abs(size2.Height);
        }
    }

    public static class ListExtensions
	{
		public static T Next<T>(this List<T> list, T current)
		{
			int index = list.IndexOf(current);
			if (index < list.Count - 1) {
				return list.ElementAt(index + 1);
			} else {
				return list.ElementAt(0);
			}
		}

		public static T Previous<T>(this List<T> list, T current)
		{
			int index = list.IndexOf(current);
			if (index >= 1) {
				return list.ElementAt(index - 1);
			} else {
				return list.ElementAt(list.Count - 1);
			}
		}
	}
}
