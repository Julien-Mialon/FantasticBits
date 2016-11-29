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
