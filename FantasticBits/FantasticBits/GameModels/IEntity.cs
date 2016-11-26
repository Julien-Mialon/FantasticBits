namespace FantasticBits.GameModels
{
	public interface IEntity
	{
		int Id { get; }

		Coordinate Position { get; }

		SpeedVector Speed { get; }
	}
}