namespace FantasticBits.GameModels
{
	public class Souaffle : IEntity
	{
		public int Id { get; }

		public Coordinate Position { get; }

		public SpeedVector Speed { get; }

		public Souaffle(int id, Coordinate position, SpeedVector speed)
		{
			Id = id;
			Position = position;
			Speed = speed;
		}
	}
}