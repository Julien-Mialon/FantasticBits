namespace FantasticBits.Models
{
	public class Move : IAction
	{
		public int X { get; set; }
	
		public int Y { get; set; }

		public int Speed { get; set; }

		public Move()
		{
			
		}

		public Move(int x, int y, int speed)
		{
			X = x;
			Y = y;
			Speed = speed;
		}
	}
}