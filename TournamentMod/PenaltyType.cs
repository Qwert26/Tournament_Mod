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
}