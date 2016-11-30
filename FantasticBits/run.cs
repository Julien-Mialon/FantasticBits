using System;
using System.Collections.Generic;
using System.Linq;
using FantasticBits.GameModels;
using FantasticBits.Helpers;
using FantasticBits.IO;
using FantasticBits.Models;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using FantasticBits.AI;


// File Constants.cs
namespace FantasticBits
{
	public static class Constants
	{
		//general
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
		public const int COGNARD_RADIUS = 200;

		public const int MIN_WIZARD_MOVE = 0;
		public const int MAX_WIZARD_MOVE = 150;

		public const int MIN_THROW = 0;
		public const int MAX_THROW = 500;

		//IO
		public const string TYPE_SOUAFFLE = "SNAFFLE";
		public const string TYPE_COGNARD = "BLUDGER";
		public const string TYPE_MY_WIZARD = "WIZARD";
		public const string TYPE_OPPONENT_WIZARD = "OPPONENT_WIZARD";


		//spells
		public const int COST_OUBLIETTE = 5;
		public const int COST_PETRIFICUS = 10;
		public const int COST_ACCIO = 20;
		public const int COST_FLIPENDO = 20;

		public const int TIME_OUBLIETTE = 3;
		public const int TIME_PETRIFICUS = 1;
		public const int TIME_ACCIO = 6;
		public const int TIME_FLIPENDO = 3;
	}
}


// File Game.cs

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
					
					//Console.Error.WriteLine($"Diff target1={ginnyTarget1.Id} h2={harryTarget2.Id} g2={ginnyTarget2.Id} : {diffD1} ; {diffD2}");
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


// File GameEngine.cs

namespace FantasticBits.Engines
{
	public class GameEngine
	{
		private readonly int _myTeam;

		private List<WizardEngine> _wizards;
		private List<CognardEngine> _cognards;
		private List<SouaffleEngine> _souaffles;

		public GameEngine(int myTeam)
		{
			_myTeam = myTeam;
		}

		public void Init(TurnInfo turn)
		{
			_wizards = turn.MyWizards.Select(x => new WizardEngine("WIZARD")
			{
				Id = x.Id,
				X = x.Position.X,
				Y = x.Position.Y,
				VX = x.Speed.VX,
				VY = x.Speed.VY
			}).Concat(turn.OpponentWizards.Select(x => new WizardEngine("OPPONENT_WIZARD")
			{
				Id = x.Id,
				X = x.Position.X,
				Y = x.Position.Y,
				VX = x.Speed.VX,
				VY = x.Speed.VY
			})).OrderBy(x => x.Id).ToList();

			_cognards = turn.Cognards.Select(x => new CognardEngine
			{
				Id = x.Id,
				X = x.Position.X,
				Y = x.Position.Y,
				VX = x.Speed.VX,
				VY = x.Speed.VY
			}).ToList();

			_souaffles = turn.Souaffles.Select(x => new SouaffleEngine
			{
				Id = x.Id,
				X = x.Position.X,
				Y = x.Position.Y,
				VX = x.Speed.VX,
				VY = x.Speed.VY
			}).ToList();

			_wizards.ForEach(x => x.Init());
			_cognards.ForEach(x => x.Init());
			_souaffles.ForEach(x => x.Init());
		}

		public void Turn(List<IAction> actions)
		{
			if (actions.Count < 4)
			{
				//issue
				throw new ArgumentOutOfRangeException(nameof(actions), "not enough actions, expected 4");
			}

			for (int i = 0; i < 4; ++i)
			{
				IAction action = actions[i];
				if (action is Move)
				{
					Move move = action as Move;
					WizardEngine wizard = _wizards[i];

					Vector normalizedVector = MathEngine.Normalize(wizard.X, wizard.Y, move.X, move.Y);

					wizard.CurrentVX = wizard.VX + normalizedVector.X * move.Speed;
					wizard.CurrentVY = wizard.VY + normalizedVector.Y * move.Speed;
				}
				else if (action is Throw)
				{
					//TODO
				}
				else if (action is Spell)
				{
					//TODO
				}
			}

			//find target for cognards
			foreach (CognardEngine cognard in _cognards)
			{
				WizardEngine target = null;
				float closest = float.MaxValue;
				foreach (WizardEngine wizard in _wizards)
				{
					if (cognard.LastCollide == wizard)
					{
						continue;
					}

					float d = MathEngine.Distance(cognard.X, cognard.Y, wizard.X, wizard.Y);
					if (d < closest)
					{
						closest = d;
						target = wizard;
					}
				}

				// ReSharper disable once PossibleNullReferenceException
				Vector normalizedVector = MathEngine.Normalize(cognard.X, cognard.Y, target.X, target.Y);

				cognard.CurrentVX = cognard.VX + normalizedVector.X * 125;
				cognard.CurrentVY = cognard.VY + normalizedVector.Y * 125;
			}

			ExecuteMoves();
		}

		private void ExecuteMoves()
		{
			//TODO: move all entities according to speed
			foreach (WizardEngine wizard in _wizards)
			{
				wizard.CurrentX = wizard.X + wizard.CurrentVX;
				wizard.CurrentY = wizard.Y + wizard.CurrentVY;

				wizard.AfterTurn();
			}

			foreach (CognardEngine cognard in _cognards)
			{
				cognard.CurrentX = cognard.X + cognard.CurrentVX;
				cognard.CurrentY = cognard.Y + cognard.CurrentVY;

				cognard.AfterTurn();
			}

			foreach (SouaffleEngine souaffle in _souaffles)
			{
				souaffle.CurrentX = souaffle.X + souaffle.CurrentVX;
				souaffle.CurrentY = souaffle.Y + souaffle.CurrentVY;

				souaffle.AfterTurn();
			}
		}

		public List<string> Output()
		{
			return _wizards
				.Concat((IEnumerable<IBaseEntityEngine>)_cognards)
				.Concat(_souaffles)
				.OrderBy(x => x.Id)
				.Select(x => $"{x.Id} {x.Type} {x.X} {x.Y} {x.VX} {x.VY} {(x.State ? 1 : 0)}")
				.ToList();
		}

		private interface IBaseEntityEngine
		{
			int Id { get; }

			int X { get; }

			int Y { get; }

			int VX { get; }

			int VY { get; }

			bool State { get; }

			string Type { get; }
		}

		private class WizardEngine : IBaseEntityEngine
		{
			public int Id { get; set; }

			public int X { get; set; }

			public int Y { get; set; }

			public int VX { get; set; }

			public int VY { get; set; }

			public bool State => Owned != null;

			public string Type { get; }

			public float CurrentX { get; set; }

			public float CurrentY { get; set; }

			public float CurrentVX { get; set; }

			public float CurrentVY { get; set; }


			public int TurnBeforeGettingSouaffle { get; set; }

			public SouaffleEngine Owned { get; set; }

			public WizardEngine(string type)
			{
				Type = type;
			}

			public void Init()
			{
				CurrentX = X;
				CurrentY = Y;
				CurrentVX = VX;
				CurrentVY = VY;
			}

			public void AfterTurn()
			{
				CurrentX = X = (int)Math.Round(CurrentX, MidpointRounding.AwayFromZero);
				CurrentY = Y = (int)Math.Round(CurrentY, MidpointRounding.AwayFromZero);

				CurrentVX *= 0.75f;
				CurrentVY *= 0.75f;

				CurrentVX = VX = (int)Math.Round(CurrentVX, MidpointRounding.AwayFromZero);
				CurrentVY = VY = (int)Math.Round(CurrentVY, MidpointRounding.AwayFromZero);
			}
		}

		private class CognardEngine : IBaseEntityEngine
		{
			public int Id { get; set; }

			public int X { get; set; }

			public int Y { get; set; }

			public int VX { get; set; }

			public int VY { get; set; }

			public bool State { get; } = false;

			public string Type => "BLUDGER";

			public float CurrentX { get; set; }

			public float CurrentY { get; set; }

			public float CurrentVX { get; set; }

			public float CurrentVY { get; set; }

			public WizardEngine LastCollide { get; set; }

			public void Init()
			{
				CurrentX = X;
				CurrentY = Y;
				CurrentVX = VX;
				CurrentVY = VY;
			}

			public void AfterTurn()
			{
				CurrentX = X = (int)Math.Round(CurrentX, MidpointRounding.AwayFromZero);
				CurrentY = Y = (int)Math.Round(CurrentY, MidpointRounding.AwayFromZero);

				CurrentVX *= 0.9f;
				CurrentVY *= 0.9f;

				CurrentVX = VX = (int)Math.Round(CurrentVX, MidpointRounding.AwayFromZero);
				CurrentVY = VY = (int)Math.Round(CurrentVY, MidpointRounding.AwayFromZero);
			}
		}

		private class SouaffleEngine : IBaseEntityEngine
		{
			public int Id { get; set; }

			public int X { get; set; }

			public int Y { get; set; }

			public int VX { get; set; }

			public int VY { get; set; }

			public bool State { get; } = false;

			public string Type => "SNAFFLE";

			public WizardEngine CaughtBy { get; set; }

			public float CurrentX { get; set; }

			public float CurrentY { get; set; }

			public float CurrentVX { get; set; }

			public float CurrentVY { get; set; }

			public void Init()
			{
				CurrentX = X;
				CurrentY = Y;
				CurrentVX = VX;
				CurrentVY = VY;
			}

			public void AfterTurn()
			{
				CurrentX = X = (int)Math.Round(CurrentX, MidpointRounding.AwayFromZero);
				CurrentY = Y = (int)Math.Round(CurrentY, MidpointRounding.AwayFromZero);

				CurrentVX *= 0.75f;
				CurrentVY *= 0.75f;

				CurrentVX = VX = (int)Math.Round(CurrentVX, MidpointRounding.AwayFromZero);
				CurrentVY = VY = (int)Math.Round(CurrentVY, MidpointRounding.AwayFromZero);
			}
		}
	}
}


// File MathEngine.cs

namespace FantasticBits.Engines
{
	public static class MathEngine
	{
		public static Vector Normalize(int x1, int y1, int x2, int y2)
		{
			int dx = x2 - x1;
			int dy = y2 - y1;

			float multiplier = 1 / (float)Math.Sqrt(dx * dx + dy * dy);
			return new Vector
			{
				X = dx * multiplier,
				Y = dy * multiplier
			};
		}

		public static float Distance(int x1, int y1, int x2, int y2)
		{
			int dx = x2 - x1;
			int dy = y2 - y1;

			return (float)Math.Sqrt(dx * dx + dy * dy);
		}
	}

	public class Vector
	{
		public float X { get; set; }

		public float Y { get; set; }
	}
}


// File BaseEntity.cs
namespace FantasticBits.GameModels
{
	public class BaseEntity : IEntity
	{
		public int Id { get; }

		public Coordinate Position { get; }

		public SpeedVector Speed { get; }

		public BaseEntity(int id, Coordinate position, SpeedVector speed)
		{
			Id = id;
			Position = position;
			Speed = speed;
		}
	}
}


// File Cognard.cs
namespace FantasticBits.GameModels
{
	public class Cognard : BaseEntity
	{
		public Cognard(int id, Coordinate position, SpeedVector speed) : base(id, position, speed)
		{

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

		public bool MarkOnRight { get; }

		public Coordinate MyGoalCenter { get; }

		public Coordinate OpponentGoalCenter { get; }

		public GameInfo(int myTeam)
		{
			MyTeamId = myTeam;

			if (MyTeamId == 0)
			{
				MarkOnRight = true;
				MyGoalCenter = new Coordinate(Constants.TEAM0_GOAL_CENTER_X, Constants.TEAM0_GOAL_CENTER_Y);
				OpponentGoalCenter = new Coordinate(Constants.TEAM1_GOAL_CENTER_X, Constants.TEAM1_GOAL_CENTER_Y);
			}
			else
			{
				MarkOnRight = false;
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
	public class Souaffle : BaseEntity
	{
		public Souaffle(int id, Coordinate position, SpeedVector speed) : base(id, position, speed)
		{

		}
	}
}


// File SpeedVector.cs
namespace FantasticBits.GameModels
{
	public class SpeedVector
	{
		public int VX { get; set; }

		public int VY { get; set; }

		public SpeedVector() { }

		public SpeedVector(int vx, int vy)
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

		public List<Cognard> Cognards { get; } = new List<Cognard>();

		public List<Wizard> MyWizards { get; } = new List<Wizard>();

		public List<Wizard> OpponentWizards { get; } = new List<Wizard>();
	}
}


// File Wizard.cs
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
		private static void Out(string text)
		{
			Console.WriteLine(text);
			Console.Error.WriteLine(text);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Move(int x, int y, int speed)
		{
			Out($"MOVE {x} {y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Move(Coordinate c, int speed)
		{
			Out($"MOVE {c.X} {c.Y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Throw(int x, int y, int speed)
		{
			Out($"THROW {x} {y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Throw(Coordinate c, int speed)
		{
			Out($"THROW {c.X} {c.Y} {speed}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Oubliette(Cognard cognard)
		{
			Out($"OBLIVIATE {cognard.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void PetrificusTotalus(Cognard cognard)
		{
			Out($"PETRIFICUS {cognard.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void PetrificusTotalus(Souaffle souaffle)
		{
			Out($"PETRIFICUS {souaffle.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void PetrificusTotalus(Wizard opponent)
		{
			Out($"PETRIFICUS {opponent.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Accio(Cognard cognard)
		{
			Out($"ACCIO {cognard.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Accio(Souaffle souaffle)
		{
			Out($"ACCIO {souaffle.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Flipendo(Cognard cognard)
		{
			Out($"FLIPENDO {cognard.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Flipendo(Souaffle souaffle)
		{
			Out($"FLIPENDO {souaffle.Id}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Flipendo(Wizard opponent)
		{
			Out($"FLIPENDO {opponent.Id}");
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


// File IAction.cs
namespace FantasticBits.Models
{
	public interface IAction
	{
	}
}


// File Move.cs
namespace FantasticBits.Models
{
	public class Move : IAction
	{
		public int X { get; set; }
	
		public int Y { get; set; }

		public int Speed { get; set; }

		public Move()
		{
			
		}

		public Move(int x, int y, int speed)
		{
			X = x;
			Y = y;
			Speed = speed;
		}
	}
}


// File Spell.cs
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


// File SpellType.cs
namespace FantasticBits.Models
{
	public enum SpellType
	{
		Accio,
		Flipendo,
		Oubliette,
		PetrificusTotalus
	}
}


// File Throw.cs
namespace FantasticBits.Models
{
	public class Throw : IAction
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int Speed { get; set; }

		public Throw()
		{

		}

		public Throw(int x, int y, int speed)
		{
			X = x;
			Y = y;
			Speed = speed;
		}
	}
}

