namespace FantasticBits.GameModels
{
	public class SpeedVector
	{
		public float VX { get; set; }

		public float VY { get; set; }

		public SpeedVector() { }

		public SpeedVector(float vx, float vy)
		{
			VX = vx;
			VY = vy;
		}
	}
}