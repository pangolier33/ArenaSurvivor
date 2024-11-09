using System;

namespace UI.Utils
{
	public static class EnumExtensions
	{
		public static bool TryParse<TSource, TTarget>(this TSource from, out TTarget result)
			where TSource : struct, Enum
			where TTarget : struct, Enum
		{
			return Enum.TryParse(from.ToString(), out result);
		}
	}
}