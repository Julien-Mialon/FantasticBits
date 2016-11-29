using System;
using System.Collections.Generic;
using System.Linq;
using FantasticBits.GameModels;
using FantasticBits.Helpers;
using FantasticBits.IO;

namespace FantasticBits.AI
{
	public class Game
	{
		private readonly GameInfo _gameInfo;
		private int _magicCount;

		public Game(GameInfo gameInfo)
		{
			_gameInfo = gameInfo;
		}

		public void Turn(TurnInfo turn)
		{
			List<Souaffle> notOwnedSouaffles = new List<Souaffle>();
			List<Souaffle> ownedSouaffles = new List<Souaffle>();
			List<Wizard> wizardsWithSouaffles = turn.MyWizards.Concat(turn.OpponentWizards).Where(x => x.HasSouaffle).ToList();

			foreach (Souaffle souaffle in turn.Souaffles)
			{
				if (wizardsWithSouaffles.Any(krum => krum.Position.X == souaffle.Position.X && krum.Position.Y == souaffle.Position.Y))
				{
					ownedSouaffles.Add(souaffle);
				}
				else
				{
					notOwnedSouaffles.Add(souaffle);
				}
			}

			Wizard harry = turn.MyWizards[0];
			Wizard ginny = turn.MyWizards[1];

			List<Souaffle> harryTargets = notOwnedSouaffles.OrderBy(x => harry.Distance(x)).Concat(ownedSouaffles.OrderBy(x => harry.Distance(x))).ToList();
			List<Souaffle> ginnyTargets = notOwnedSouaffles.OrderBy(x => ginny.Distance(x)).Concat(ownedSouaffles.OrderBy(x => ginny.Distance(x))).ToList();

			foreach (Wizard wizard in turn.MyWizards)
			{
				if (wizard.HasSouaffle)
				{
					Output.Throw(_gameInfo.OpponentGoalCenter, Constants.MAX_THROW);
					continue;
				}

				Souaffle nearest = notOwnedSouaffles.MinItem(x => wizard.Distance(x)) ?? ownedSouaffles.MinItem(x => wizard.Distance(x));
				int dx = nearest.Position.X - wizard.Position.X;
				bool isOnTheRightPath = _gameInfo.MarkOnRight ? dx > 0 : dx < 0;
				if (_magicCount >= 20 && wizard.Distance(nearest) > 500 && !isOnTheRightPath)
				{
					Output.Accio(nearest);
					_magicCount -= 20;
				}
				else
				{
					//find the nearest one
					Output.Move(nearest.Position, Constants.MAX_WIZARD_MOVE);
				}
			}

			_magicCount++;
		}
	}
}
