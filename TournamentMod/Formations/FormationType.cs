using System;
namespace TournamentMod.Formations
{
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
	public static class FormationTypeExtensions
	{
		public static Formation getFormation(this FormationType type) {
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
