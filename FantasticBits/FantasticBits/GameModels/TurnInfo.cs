using System.Collections.Generic;

namespace FantasticBits.GameModels
{
	public class TurnInfo
	{
		public List<Souaffle> Souaffles { get; } = new List<Souaffle>();

		public List<Cognard> Cognards { get; } = new List<Cognard>();

		public List<Wizard> MyWizards { get; } = new List<Wizard>();

		public List<Wizard> OpponentWizards { get; } = new List<Wizard>();
	}
}