using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FantasticBits;
using FantasticBits.GameModels;
using FantasticBits.Models;

namespace Engine.Tests
{
	public class Reader
	{
		private const string DEBUG_HEADER = "Sortie d'erreur :";
		private const string STANDARD_HEADER = "Sortie standard :";
		/*
		private const string INFO_HEADER = "Informations :";
		private const string SUMMARY_HEADER = "Résumé du jeu :";
		*/
		private readonly List<string> _lines;
		private int _index = 0;

		public Reader(string file)
		{
			_lines = File.ReadAllLines(file).ToList();
		}

		public string ReadGameInfo()
		{
			_index = 2;
			return _lines[1];
		}

		public bool HasMore => _index < _lines.Count;

		public TurnReader NextTurn()
		{
			int entityCount = int.Parse(_lines[_index]);
			List<string> inputs = new List<string>
			{
				_lines[_index]
			};
			_index++;
			for (int i = 0; i < entityCount; ++i, _index++)
			{
				inputs.Add(_lines[_index]);
			}

			List<string> outputs = new List<string>();

			for (int x = 0; x < 2; ++x)
			{
				for (; _lines[_index] != STANDARD_HEADER; _index++) ;
				_index++;
				for (int y = 0; y < 2; ++y, _index++)
				{
					outputs.Add(_lines[_index]);
				}
			}

			for (; _index < _lines.Count && _lines[_index] != DEBUG_HEADER; _index++) ;
			_index++;

			return new TurnReader(inputs, outputs);
		}
	}

	public class TurnReader
	{
		private readonly List<string> _inputLines;
		private readonly List<string> _outputLines;

		private int _indexInput = 0;
		private int _indexOutput = 0;

		public int InputCount => _inputLines.Count;

		public TurnReader(List<string> inputLines, List<string> outputLines)
		{
			_inputLines = inputLines;
			_outputLines = outputLines;
		}

		public void Reset()
		{
			_indexInput = 0;
			_indexOutput = 0;
		}

		public string ReadInputLine() => _inputLines[_indexInput++];

		public string ReadOutputLine() => _outputLines[_indexOutput++];

		public TurnInfo TurnInfo()
		{
			Reset();
			TurnInfo turn = new TurnInfo();
			int entitiesCount = int.Parse(ReadInputLine());
			Console.Error.WriteLine($"{entitiesCount}");
			for (int i = 0; i < entitiesCount; ++i)
			{
				string[] entityInfo = ReadInputLine().Split(' ');
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
			return turn;
		}

		public List<IAction> Actions()
		{
			return _outputLines.Select(line =>
			{
				string[] parts = line.Split(' ');
				switch (parts[0])
				{
					case "MOVE":
						return (IAction)new Move(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
					case "THROW":
						return new Throw(int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
					case "OBLIVIATE":
						return new Spell(SpellType.Oubliette, int.Parse(parts[1]));
					case "PETRIFICUS":
						return new Spell(SpellType.PetrificusTotalus, int.Parse(parts[1]));
					case "ACCIO":
						return new Spell(SpellType.Accio, int.Parse(parts[1]));
					case "FLIPENDO":
						return new Spell(SpellType.Flipendo, int.Parse(parts[1]));
					default:
						throw new ArgumentException($"Invalid prefix: {line}", nameof(line));
				}
			}).ToList();
		}
	}
}
