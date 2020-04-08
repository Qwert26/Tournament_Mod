using System;
namespace TournamentMod.Formations
{
	/// <summary>
	/// The implemented and available formations.
	/// </summary>
	public enum FormationType
	{
		Line,
		Wedge,
		DividedWedge,
		ParallelColumns,
		CommandedParallelColumns,
		RomanManipelBase,
		RomanManipelAttack,
		GuardLine
	}
	/// <summary>
	/// Extension-Methods for the FormationTypes.
	/// </summary>
	public static class FormationTypeExtensions
	{
		/// <summary>
		/// Gets a Formation-struct based on the value of the FormationType.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Formation GetFormation(this FormationType type) {
			switch (type)
			{
				case FormationType.Line:
					return Formation.Line;
				case FormationType.Wedge:
					return Formation.Wedge;
				case FormationType.DividedWedge:
					return Formation.DividedWedge;
				case FormationType.ParallelColumns:
					return Formation.ParallelColumns;
				case FormationType.CommandedParallelColumns:
					return Formation.CommandedParallelColumns;
				case FormationType.RomanManipelBase:
					return Formation.RomanManipelBase;
				case FormationType.RomanManipelAttack:
					return Formation.RomanManipelAttack;
				case FormationType.GuardLine:
					return Formation.GuardLine;
				default:
					throw new ArgumentOutOfRangeException("type","No known Formation for this type!");
			}
		}
	}
}
