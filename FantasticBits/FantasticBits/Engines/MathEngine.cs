using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticBits.Engines
{
	public static class MathEngine
	{
		public static Vector Normalize(int x1, int y1, int x2, int y2)
		{
			int dx = x2 - x1;
			int dy = y2 - y1;

			float multiplier = 1 / (float)Math.Sqrt(dx * dx + dy * dy);
			return new Vector
			{
				X = dx * multiplier,
				Y = dy * multiplier
			};
		}

		public static float Distance(int x1, int y1, int x2, int y2)
		{
			int dx = x2 - x1;
			int dy = y2 - y1;

			return (float)Math.Sqrt(dx * dx + dy * dy);
		}
	}

	public class Vector
	{
		public float X { get; set; }

		public float Y { get; set; }
	}
}
