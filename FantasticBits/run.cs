using System.Linq;
using FantasticBits.GameModels;
using FantasticBits.Helpers;
using FantasticBits.IO;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using FantasticBits.AI;


// File Constants.cs
namespace FantasticBits
{
	public static class Constants
	{
		public const int WIDTH = 16001;
		public const int HEIGHT = 7501;

		public const int GOAL_SIZE = 4000;
		public const int GOAL_RADIUS = 300;

		public const int TEAM0_GOAL_CENTER_X = 0;
		public const int TEAM0_GOAL_CENTER_Y = 3750;

		public const int TEAM1_GOAL_CENTER_X = 16000;
		public const int TEAM1_GOAL_CENTER_Y = 3750;

		public const int SOUAFFLE_RADIUS = 150;

		public const int WIZARD_RADIUS = 400;

		public const int MIN_WIZARD_MOVE = 0;
		public const int MAX_WIZARD_MOVE = 150;

		public const int MIN_THROW = 0;
		public const int MAX_THROW = 500;

		public const string TYPE_SOUAFFLE = "SNAFFLE";
		public const string TYPE_MY_WIZARD = "WIZARD";
		public const string TYPE_OPPONENT_WIZARD = "OPPONENT_WIZARD";
	}
}


// File Game.cs

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


// File Coordinate.cs
namespace FantasticBits.GameModels
{
	public class Coordinate
	{
		public int X { get; set;}
	
		public int Y { get; set; }
		
		public Coordinate() { }

		public Coordinate(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}


// File GameInfo.cs
namespace FantasticBits.GameModels
{
	public struct GameInfo
	{
		public int MyTeamId { get; }

		public Coordinate MyGoalCenter { get; }

		public Coordinate OpponentGoalCenter { get; }

		public GameInfo(int myTeam)
		{
			MyTeamId = myTeam;

			if (MyTeamId == 0)
			{
				MyGoalCenter = new Coordinate(Constants.TEAM0_GOAL_CENTER_X, Constants.TEAM0_GOAL_CENTER_Y);
				OpponentGoalCenter = new Coordinate(Constants.TEAM1_GOAL_CENTER_X, Constants.TEAM1_GOAL_CENTER_Y);
			}
			else
			{
				OpponentGoalCenter = new Coordinate(Constants.TEAM0_GOAL_CENTER_X, Constants.TEAM0_GOAL_CENTER_Y);
				MyGoalCenter = new Coordinate(Constants.TEAM1_GOAL_CENTER_X, Constants.TEAM1_GOAL_CENTER_Y);
			}
		}
	}
}


// File IEntity.cs
namespace FantasticBits.GameModels
{
	public interface IEntity
	{
		int Id { get; }

		Coordinate Position { get; }

		SpeedVector Speed { get; }
	}
}


// File Souaffle.cs
namespace FantasticBits.GameModels
{
	public class Souaffle : IEntity
	{
		public int Id { get; }

		public Coordinate Position { get; }

		public SpeedVector Speed { get; }

		public Souaffle(int id, Coordinate position, SpeedVector speed)
		{
			Id = id;
			Position = position;
			Speed = speed;
		}
	}
}


// File SpeedVector.cs
namespace FantasticBits.GameModels
{
	public class SpeedVector
	{
		public float VX { get; set; }

		public float VY { get; set; }

		public SpeedVector() { }

		public SpeedVector(float vx, float vy)
		{
			VX = vx;
			VY = vy;
		}
	}
}


// File TurnInfo.cs

namespace FantasticBits.GameModels
{
	public class TurnInfo
	{
		public List<Souaffle> Souaffles { get; } = new List<Souaffle>();

		public List<Wizard> MyWizards { get; } = new List<Wizard>();

		public List<Wizard> OpponentWizards { get; } = new List<Wizard>();
	}
}


// File Wizard.cs
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


// File CollectionExtensions.cs

namespace FantasticBits.Helpers
{
	public static class CollectionExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TItem MinItem<TItem>(this IEnumerable<TItem> items, Func<TItem, double> value)
		{
			double min = double.MaxValue;
			TItem minItem = default (TItem);

			foreach (TItem item in items)
			{
				double v = value(item);
				if (v < min)
				{
					min = v;
					minItem = item;
				}
			}
			return minItem;
		}
	}
}


// File MathHelper.cs

namespace FantasticBits.Helpers
{
	public static class MathHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Distance(Coordinate a, Coordinate b)
		{
			return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Distance(this IEntity a, IEntity b)
		{
			return Distance(a.Position, b.Position);
		}
	}
}


// File Output.cs

namespace FantasticBits.IO
{
	public static class Output
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Move(int x, int y, int speed)
		{
			Console.WriteLine($"MOVE {x} {y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Move(Coordinate c, int speed)
		{
			Console.WriteLine($"MOVE {c.X} {c.Y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Throw(int x, int y, int speed)
		{
			Console.WriteLine($"THROW {x} {y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Throw(Coordinate c, int speed)
		{
			Console.WriteLine($"THROW {c.X} {c.Y} {speed}");
		}
	}
}


// File Program.cs

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException

namespace FantasticBits.IO
{
	public class Program
	{
		static void Main(string[] args)
		{
			int teamId = int.Parse(Console.ReadLine());

			Game game = new Game(new GameInfo(teamId));

			while (true)
			{
				TurnInfo turn = new TurnInfo();
				int entitiesCount = int.Parse(Console.ReadLine());

				for (int i = 0; i < entitiesCount; ++i)
				{
					string[] entityInfo = Console.ReadLine().Split(' ');

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

