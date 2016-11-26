using System;
using System.Runtime.CompilerServices;
using FantasticBits.GameModels;

namespace FantasticBits.Helpers
{
	public static class MathHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Distance(Coordinate a, Coordinate b)
		{
			return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Distance(this IEntity a, IEntity b)
		{
			return Distance(a.Position, b.Position);
		}
	}
}
