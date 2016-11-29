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
					
				}
				else if (action is Throw)
				{
					
				}
				else if (action is Spell)
				{
					//TODO
				}
			}
		}

		public List<string> Output()
		{
			return _wizards
				.Concat((IEnumerable<BaseEntityEngine>) _cognards)
				.Concat(_souaffles)
				.OrderBy(x => x.Id)
				.Select(x => $"{x.Id} {x.Type} {x.X} {x.Y} {x.VX} {x.VY} {(x.State ? 1 : 0)}")
				.ToList();
		}

		private class BaseEntityEngine
		{
			public int Id { get; set; }

			public int X { get; set; }

			public int Y { get; set; }

			public int VX { get; set; }

			public int VY { get; set; }

			public virtual bool State { get; } = false;

			public virtual string Type { get; } = "";
		}

		private class WizardEngine : BaseEntityEngine
		{
			public override bool State => Owned != null;

			public override string Type { get; }

			public int TurnBeforeGettingSouaffle { get; set; }

			public SouaffleEngine Owned { get; set; }

			public WizardEngine(string type)
			{
				Type = type;
			}
		}

		private class CognardEngine : BaseEntityEngine
		{
			public override string Type => "BLUDGER";

			public WizardEngine LastCollide { get; set; }
		}

		private class SouaffleEngine : BaseEntityEngine
		{
			public override string Type => "SNAFFLE";

			public WizardEngine CaughtBy { get; set; }
		}
	}
}
