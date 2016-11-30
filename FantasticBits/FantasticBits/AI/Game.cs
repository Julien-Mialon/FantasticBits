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

			if (harry.HasSouaffle || ginny.HasSouaffle)
			{
				if (harry.HasSouaffle)
				{
					Output.Throw(_gameInfo.OpponentGoalCenter, Constants.MAX_THROW);
				}
				else
				{
					ActionForWizard(harry, harryTargets.First());
				}

				if (ginny.HasSouaffle)
				{
					Output.Throw(_gameInfo.OpponentGoalCenter, Constants.MAX_THROW);
				}
				else
				{
					ActionForWizard(ginny, ginnyTargets.First());
				}
			}
			else if (turn.Souaffles.Count == 1)
			{
				Souaffle souaffle = turn.Souaffles.First();
				ActionForWizard(harry, souaffle);
				ActionForWizard(ginny, souaffle);
			}
			else
			{
				Souaffle harryTarget1 = harryTargets[0];
				Souaffle ginnyTarget1 = ginnyTargets[0];

				if (harryTarget1.Id != ginnyTarget1.Id)
				{
					ActionForWizard(harry, harryTarget1);
					ActionForWizard(ginny, ginnyTarget1);
				}
				else
				{
					Souaffle harryTarget2 = harryTargets[1];
					Souaffle ginnyTarget2 = ginnyTargets[1];

					double distanceHarry1 = harry.Distance(harryTarget1);
					double distanceHarry2 = harry.Distance(harryTarget2);
					
					double distanceGinny1 = ginny.Distance(ginnyTarget1);
					double distanceGinny2 = ginny.Distance(ginnyTarget2);
					
					if (distanceGinny1 + distanceHarry2 < distanceGinny2 + distanceHarry1)
					{
						ActionForWizard(harry, harryTarget2);
						ActionForWizard(ginny, ginnyTarget1);
					}
					else
					{
						ActionForWizard(harry, harryTarget1);
						ActionForWizard(ginny, ginnyTarget2);
					}
				}
			}
			
			_magicCount++;
		}

		private void ActionForWizard(Wizard wizard, Souaffle target)
		{
			int dx = target.Position.X - wizard.Position.X;
			bool isOnTheRightPath = _gameInfo.MarkOnRight ? dx > 0 : dx < 0;
			if (_magicCount >= 20 && wizard.Distance(target) > 2000 && !isOnTheRightPath)
			{
				Output.Accio(target);
				_magicCount -= 20;
			}
			else
			{
				//find the nearest one
				Output.Move(target.Position, Constants.MAX_WIZARD_MOVE);
			}
		}
	}
}
