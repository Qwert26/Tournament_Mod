namespace TournamentMod
{
	public enum PenaltyType : int
	{
		/// <summary>
		/// The entry has no longer any functional ais and if the corresponding cleanup-function is turned off, it will now accumulate penalty points.
		/// </summary>
		NoAi = 0,
		/// <summary>
		/// The entry is outside the set altitude limits and has been found to not move towards them.
		/// </summary>
		OutsideAltitude = 1,
		/// <summary>
		/// The entry has sustained too much damage and if the corresponding cleanup-function is turned off, it will now accumulate penalty points.
		/// </summary>
		TooMuchDamage = 2,
		/// <summary>
		/// The entry is too fast.
		/// </summary>
		TooFast = 3,
		/// <summary>
		/// The entry is too far from the set percentage from enemies and has been found to move away from them.
		/// </summary>
		FleeingFromBattle = 4
	}
}