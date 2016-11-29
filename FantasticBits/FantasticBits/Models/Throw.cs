namespace FantasticBits.Models
{
	public class Throw : IAction
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int Speed { get; set; }

		public Throw()
		{

		}

		public Throw(int x, int y, int speed)
		{
			X = x;
			Y = y;
			Speed = speed;
		}
	}
}