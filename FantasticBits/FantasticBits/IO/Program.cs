using System;
using FantasticBits.AI;
using FantasticBits.GameModels;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException

namespace FantasticBits.IO
{
	public class Program
	{
		static void Main(string[] args)
		{
			int teamId = int.Parse(Console.ReadLine());

			Console.Error.WriteLine($"{teamId}");

			Game game = new Game(new GameInfo(teamId));

			while (true)
			{
				TurnInfo turn = new TurnInfo();
				int entitiesCount = int.Parse(Console.ReadLine());
				Console.Error.WriteLine($"{entitiesCount}");
				for (int i = 0; i < entitiesCount; ++i)
				{
					string[] entityInfo = Console.ReadLine().Split(' ');
					Console.Error.WriteLine($"{string.Join(" ", entityInfo)}");

					if (entityInfo[1] == Constants.TYPE_SOUAFFLE)
					{
						turn.Souaffles.Add(new Souaffle(
							int.Parse(entityInfo[0]),
							new Coordinate(
								int.Parse(entityInfo[2]),
								int.Parse(entityInfo[3])
								),
							new SpeedVector(
								int.Parse(entityInfo[4]),
								int.Parse(entityInfo[5])
								)
							));
					}
					else if (entityInfo[1] == Constants.TYPE_COGNARD)
					{
						turn.Cognards.Add(new Cognard(
							int.Parse(entityInfo[0]),
							new Coordinate(
								int.Parse(entityInfo[2]),
								int.Parse(entityInfo[3])
								),
							new SpeedVector(
								int.Parse(entityInfo[4]),
								int.Parse(entityInfo[5])
								)
							));
					}
					else if (entityInfo[1] == Constants.TYPE_MY_WIZARD)
					{
						turn.MyWizards.Add(new Wizard(
							int.Parse(entityInfo[0]),
							new Coordinate(
								int.Parse(entityInfo[2]),
								int.Parse(entityInfo[3])
								),
							new SpeedVector(
								int.Parse(entityInfo[4]),
								int.Parse(entityInfo[5])
								),
							entityInfo[6] == "1"
							));
					}
					else
					{
						turn.OpponentWizards.Add(new Wizard(
							   int.Parse(entityInfo[0]),
							   new Coordinate(
								   int.Parse(entityInfo[2]),
								   int.Parse(entityInfo[3])
								   ),
							   new SpeedVector(
								   int.Parse(entityInfo[4]),
								   int.Parse(entityInfo[5])
								   ),
							   entityInfo[6] == "1"
							   ));

					}
				}

				game.Turn(turn);
			}
		}
	}
}
