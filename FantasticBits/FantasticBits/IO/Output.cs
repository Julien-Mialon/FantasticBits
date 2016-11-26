using System;
using System.Runtime.CompilerServices;
using FantasticBits.GameModels;

namespace FantasticBits.IO
{
	public static class Output
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Move(int x, int y, int speed)
		{
			Console.WriteLine($"MOVE {x} {y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Move(Coordinate c, int speed)
		{
			Console.WriteLine($"MOVE {c.X} {c.Y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Throw(int x, int y, int speed)
		{
			Console.WriteLine($"THROW {x} {y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Throw(Coordinate c, int speed)
		{
			Console.WriteLine($"THROW {c.X} {c.Y} {speed}");
		}
	}
}
