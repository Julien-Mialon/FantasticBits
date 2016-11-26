using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FantasticBits.Helpers
{
	public static class CollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TItem MinItem<TItem>(this IEnumerable<TItem> items, Func<TItem, double> value)
		{
			double min = double.MaxValue;
			TItem minItem = default (TItem);

			foreach (TItem item in items)
			{
				double v = value(item);
				if (v < min)
				{
					min = v;
					minItem = item;
				}
			}
			return minItem;
		}
	}
}
