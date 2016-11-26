namespace FantasticBits.GameModels
{
	public class Wizard : IEntity
	{
		public int Id { get; }

		public Coordinate Position { get; }

		public SpeedVector Speed { get; }

		public bool HasSouaffle { get; }

		public Wizard(int id, Coordinate position, SpeedVector speed, bool hasSouaffle)
		{
			Id = id;
			Position = position;
			Speed = speed;
			HasSouaffle = hasSouaffle;
		}
	}
}