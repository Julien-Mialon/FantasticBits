using System;
using System.Collections.Generic;
using System.Linq;
using FantasticBits.GameModels;
using FantasticBits.Models;

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
