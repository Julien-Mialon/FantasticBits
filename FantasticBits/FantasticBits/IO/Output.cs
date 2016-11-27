using System;
using System.Runtime.CompilerServices;
using FantasticBits.GameModels;

namespace FantasticBits.IO
{
	public static class Output
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Out(string text)
		{
			Console.WriteLine(text);
			Console.Error.WriteLine(text);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Move(int x, int y, int speed)
		{
			Out($"MOVE {x} {y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Move(Coordinate c, int speed)
		{
			Out($"MOVE {c.X} {c.Y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Throw(int x, int y, int speed)
		{
			Out($"THROW {x} {y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Throw(Coordinate c, int speed)
		{
			Out($"THROW {c.X} {c.Y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Oubliette(Cognard cognard)
		{
			Out($"OBLIVIATE {cognard.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void PetrificusTotalus(Cognard cognard)
		{
			Out($"PETRIFICUS {cognard.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void PetrificusTotalus(Souaffle souaffle)
		{
			Out($"PETRIFICUS {souaffle.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void PetrificusTotalus(Wizard opponent)
		{
			Out($"PETRIFICUS {opponent.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Accio(Cognard cognard)
		{
			Out($"ACCIO {cognard.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Accio(Souaffle souaffle)
		{
			Out($"ACCIO {souaffle.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Flipendo(Cognard cognard)
		{
			Out($"FLIPENDO {cognard.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Flipendo(Souaffle souaffle)
		{
			Out($"FLIPENDO {souaffle.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Flipendo(Wizard opponent)
		{
			Out($"FLIPENDO {opponent.Id}");
		}
	}
}
