using System;
using System.Collections.Generic;
using FantasticBits.Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
	[TestClass]
	public class SampleTest
	{
		[TestMethod]
		public void TestInput1()
		{
			Reader reader = new Reader("Samples/sample.txt");
			TestReader(reader);
		}

		[TestMethod]
		public void TestInput2()
		{
			Reader reader = new Reader("Samples/sample2.txt");
			TestReader(reader);
		}

		public void TestReader(Reader reader)
		{
			GameEngine engine = new GameEngine(int.Parse(reader.ReadGameInfo()));
			TurnReader turn = reader.NextTurn();
			
			engine.Init(turn.TurnInfo());
			int turnCount = 0;
			do
			{
				turn.Reset();
				TestTurn(turn, engine, turnCount++);

				if (reader.HasMore)
				{
					turn = reader.NextTurn();
				}
			} while (reader.HasMore);
		}

		public void TestTurn(TurnReader reader, GameEngine engine, int turn)
		{
			//assert
			List<string> output = engine.Output();

			Assert.AreEqual(int.Parse(reader.ReadInputLine()), output.Count, $"Turn {turn}");
			foreach (string line in output)
			{
				Assert.AreEqual(reader.ReadInputLine(), line, $"Turn {turn}");
			}

			engine.Turn(reader.Actions());
		}
	}
}
