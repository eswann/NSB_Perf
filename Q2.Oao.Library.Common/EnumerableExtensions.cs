using System.Collections.Generic;
using System.Linq;

namespace Q2.Oao.Library.Common
{
    public static class EnumerableExtensions
    {
	    public static bool SafeAny<T>(this IEnumerable<T> enumerable)
	    {
		    return enumerable != null && enumerable.Any();
	    }

		public static bool SafeAny<T>(this IList<T> list)
		{
			return list != null && list.Count > 0;
		}

	}
}
