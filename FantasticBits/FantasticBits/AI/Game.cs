using System.Linq;
using FantasticBits.GameModels;
using FantasticBits.Helpers;
using FantasticBits.IO;

namespace FantasticBits.AI
{
	public class Game
	{
		private readonly GameInfo _gameInfo;

		public Game(GameInfo gameInfo)
		{
			_gameInfo = gameInfo;
		}

		public void Turn(TurnInfo turn)
		{
			foreach (Wizard wizard in turn.MyWizards)
			{
				if (wizard.HasSouaffle)
				{

					Output.Throw(_gameInfo.OpponentGoalCenter, Constants.MAX_THROW);
				}
				else
				{
					//find the nearest one
					Souaffle nearest = turn.Souaffles.MinItem(x => wizard.Distance(x));
					Output.Move(nearest.Position, Constants.MAX_WIZARD_MOVE);
				}
			}
		}
	}
}
