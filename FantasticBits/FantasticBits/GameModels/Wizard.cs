namespace FantasticBits.GameModels
{
	public class Wizard : BaseEntity
	{
		public bool HasSouaffle { get; }

		public Wizard(int id, Coordinate position, SpeedVector speed, bool hasSouaffle) : base(id, position, speed)
		{
			HasSouaffle = hasSouaffle;
		}
	}
}