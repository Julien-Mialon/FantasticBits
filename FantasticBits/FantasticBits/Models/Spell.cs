namespace FantasticBits.Models
{
	public class Spell : IAction
	{
		public SpellType Type { get; set; }

		public int Target { get; set; }

		public Spell()
		{
			
		}

		public Spell(SpellType type, int target)
		{
			Type = type;
			Target = target;
		}
	}
}