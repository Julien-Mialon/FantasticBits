namespace FantasticBits.GameModels
{
	public class SpeedVector
	{
		public int VX { get; set; }

		public int VY { get; set; }

		public SpeedVector() { }

		public SpeedVector(int vx, int vy)
		{
			VX = vx;
			VY = vy;
		}
	}
}