namespace TournamentMod
{
	/// <summary>
	/// Entries can violate multiple rules at once, and if these rules have different weights, they must be combined in deterministic, implementation-independent way.
	/// </summary>
	public enum WeightCombinerType : int
	{
		/// <summary>
		/// Uses the smallest weight to modify gained penalty time.
		/// </summary>
		MINIMUM=-2,
		/// <summary>
		/// Uses the harmonic mean to calculate a modifier.
		/// </summary>
		HARMONIC=-1,
		/// <summary>
		/// Uses the geometric mean to calculate a modifier.
		/// </summary>
		GEOMETRIC=0,
		/// <summary>
		/// Uses the arithmetic mean to calculate a modifier.
		/// </summary>
		ARITHMETIC=1,
		/// <summary>
		/// Uses the squared mean to calculate a modifier.
		/// </summary>
		SQUARED=2,
		/// <summary>
		/// Uses the cubic mean to calculate a modifier.
		/// </summary>
		CUBIC=3,
		/// <summary>
		/// Uses the biggest weight to modify gained penalty time.
		/// </summary>
		MAXIMUM=4
	}
	public static class WeightCombinerTypeExtensions
	{
		public static string GetDescription(this WeightCombinerType wct)
		{
			switch (wct)
			{
				case WeightCombinerType.MINIMUM:
					return "Uses the lowest Weight of all violations. Entries will pickup penalty time the slowest.";
				case WeightCombinerType.HARMONIC:
					return "Uses the harmonic mean of the weights. Entries will pickup penalty time faster than Minimum but slower than Geometric mean.";
				case WeightCombinerType.GEOMETRIC:
					return "Uses the geometric mean of the weights. Entries will pickup penalty time faster than Harmonic mean but slower than Arithmetic mean.";
				case WeightCombinerType.ARITHMETIC:
					return "Uses the arithmetic mean of the weights. Entries will pickup penalty time faster than Geometric mean but slower than Squared mean.";
				case WeightCombinerType.SQUARED:
					return "Uses the squared mean of the weights. Entries will pickup penalty time faster than Arithmetic mean but slower than Cubic mean.";
				case WeightCombinerType.CUBIC:
					return "Uses the cubic mean of the weights. Entries will pickup penalty time faster than Squared mean but slower than Maximum.";
				case WeightCombinerType.MAXIMUM:
					return "Uses the highest Weight of all violations. Entries will pickup penalty time the fastest.";
				default:
					return "How did you manage to get out of bounds here?";
			}
		}
	}
}