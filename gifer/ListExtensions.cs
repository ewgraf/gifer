using System.Collections.Generic;
using System.Linq;

namespace gifer
{
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
