namespace FantasticBits.GameModels
{
	public class BaseEntity : IEntity
	{
		public int Id { get; }

		public Coordinate Position { get; }

		public SpeedVector Speed { get; }

		public BaseEntity(int id, Coordinate position, SpeedVector speed)
		{
			Id = id;
			Position = position;
			Speed = speed;
		}
	}
}