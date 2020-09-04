namespace TournamentMod
{
	/// <summary>
	/// A collection of all possible types of penalty conditions.
	/// </summary>
	public enum PenaltyType : int
	{
		/// <summary>
		/// The entry has no longer any functional ais and if the corresponding cleanup-function is turned off, it will now accumulate penalty points.
		/// </summary>
		NoAi,
		/// <summary>
		/// The entry is above the upper altitude limit and has been found to not move towards it.
		/// </summary>
		AboveAltitude,
		/// <summary>
		/// The entry is below the lower altitude limit and has been found to not move towards it.
		/// </summary>
		UnderAltitude,
		/// <summary>
		/// The entry has sustained too much damage and if the corresponding cleanup-function is turned off, it will now accumulate penalty points.
		/// </summary>
		TooMuchDamage,
		/// <summary>
		/// The entry is too fast.
		/// </summary>
		TooFast,
		/// <summary>
		/// The entry is too far from the set percentage from enemies and has been found to move away from them.
		/// </summary>
		FleeingFromBattle
	}
	public static class PenaltyTypeExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		public static string GetDescription(this PenaltyType pt)
		{
			switch (pt)
			{
				case PenaltyType.AboveAltitude:
					return "Any entry, which is above the upper altitude limit, will have this time modifier applied to their penalty timer.";
				case PenaltyType.FleeingFromBattle:
					return "Any entry, which is considered to be fleeing from battle, will have this time modifier applied to their penalty timer.";
				case PenaltyType.NoAi:
					return "Any entry, which no longer has any AIs left in it, will have this time modifier applied to their penalty timer. " +
					"This will only have a effect, if the clean up function \"NoAI\" has been effectivly turned off.";
				case PenaltyType.TooFast:
					return "Any entry, which is above the speed limit will have this modifier applied to their penalty timer.";
				case PenaltyType.TooMuchDamage:
					return "Any entry, which is below a set percentage of hitpoint, will have this modifier applied to their penalty timer. " +
					"This will only have an effect, if the clean up function \"TooDamaged\" has been effectivly turned off.";
				case PenaltyType.UnderAltitude:
					return "";
				default:
					return "How did you manage to get out of bounds here?";
			}
		}
	}
}