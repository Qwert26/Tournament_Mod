﻿namespace TournamentMod
{
	public static class StaticConstants
	{
		/// <summary>
		/// Maximum amount of Teams supported.
		/// </summary>
		public const int MAX_TEAMS = 6;
		/// <summary>
		/// Maximum axis-aligned distance between two entries of the same team.
		/// </summary>
		public const int MAX_SPAWN_GAP_VALUE = 1000;
		/// <summary>
		/// Maximum absolute value for getting back into bounds.
		/// </summary>
		public const int MAX_REVERSAL_SPEED_VALUE = 500;
		/// <summary>
		/// Maximum time-value for the buffer-, penalty- and battle-timer in seconds. The value equals to one hour.
		/// </summary>
		public const int MAX_TIME = 3600;
		public const float MIN_WEIGHT = 0.1f;
		public const float NORMAL_WEIGHT = 1f;
		public const float MAX_WEIGHT = 10f;
	}
}