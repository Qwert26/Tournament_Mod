using UnityEngine;
namespace TournamentMod.Formations
{
	public struct Formation {
		public delegate Vector3 LocalPosition(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height);
		public delegate string PositionDescription(float gapLeftRight, float gapForwardBackward, int count, int index);
		public delegate Vector2 Size(float gapLeftRight, float gapForwardBackward, int count);
		public string Name { get; set; }
		public string Description { get; set; }
		public LocalPosition DetermineLocalPosition { get; set; }
		public PositionDescription DeterminePositionDescription { get; set; }
		public Size DetermineSize{ get; set; }
		public static readonly Formation Line = new Formation()
		{
			Name = "Line",
			Description = "The classic Line Formation by wo0tness and Anickle. The Flagship should be in the middle position.",
			DetermineLocalPosition = FormationCalculation.LineFormation,
			DeterminePositionDescription = FormationPositionDescription.LineFormation,
			DetermineSize = FormationSizeCalculation.LineFormation
		};
		public static readonly Formation Wedge = new Formation()
		{
			Name = "Wedge",
			Description = "Also known as a V-Formation. A positive Forward-Backward gap will result in an Arrow-Style, a negative Forward-Backward gap in a Funnel-style Formation. " +
							"The Flagship should be in first position as it will form the pivot point.",
			DetermineLocalPosition = FormationCalculation.WedgeFormation,
			DeterminePositionDescription = FormationPositionDescription.WedgeFormation,
			DetermineSize = FormationSizeCalculation.WedgeFormation
		};
		public static readonly Formation DividedWedge = new Formation()
		{
			Name = "Divided Wedge",
			Description = "This variant of the V-Formation divides the fleet in thee groups. The Flagship should be in first position as it will form the pivot point. " +
			"Commandships are the second and third positions. The distance between the groups grows, the larger the fleet is.",
			DetermineLocalPosition = FormationCalculation.DividedWedgeFormation,
			DeterminePositionDescription = FormationPositionDescription.DividedWedgeFormation,
			DetermineSize = FormationSizeCalculation.DividedWedgeFormation
		};
		public static readonly Formation ParallelColumns = new Formation()
		{
			Name = "Columns",
			Description = "The fleet is in a rectangular formation, where the start of one column is at an 22° angle to the end of a neighboring column. " +
			"The Columns are getting evenly filled front to back and the maximum amount of ships in a column gets determined by the ratio of the two gap values.",
			DetermineLocalPosition = FormationCalculation.ParallelColumnsFormation,
			DeterminePositionDescription = FormationPositionDescription.ParallelColumnsFormation,
			DetermineSize = FormationSizeCalculation.ParallelColumnsFormation
		};
		public static readonly Formation CommandedParallelColumns = new Formation()
		{
			Name = "Commanded Columns",
			Description = "Similar to the Columns-Formation, but each pair of Columns also has an Commandship diagonally in front of it.",
			DetermineLocalPosition = FormationCalculation.CommandedParallelColumnsFormation,
			DeterminePositionDescription = FormationPositionDescription.CommandedParallelColumnsFormation,
			DetermineSize = FormationSizeCalculation.CommandedParallelColumnsFormation
		};
		public static readonly Formation RomanManipelBase = new Formation()
		{
			Name = "Roman Legion, Manipel Base",
			Description = "The Baseformation of the Manipel after Marcus Furius Camillus. Each Group consists of only six Entries, put together in vertical pairs.",
			DetermineLocalPosition = FormationCalculation.ManipelBaseFormation,
			DeterminePositionDescription = FormationPositionDescription.ManipelBaseFormation,
			DetermineSize = FormationSizeCalculation.ManipelFormation
		};
		public static readonly Formation RomanManipelAttack = new Formation()
		{
			Name = "Roman Legion, Manipel Attack",
			Description = "Similar to the Baseformation, but the second line of the Baseformation moved into the gaps of the first line, forming a solid line.",
			DetermineLocalPosition = FormationCalculation.ManipelAttackFormation,
			DeterminePositionDescription = FormationPositionDescription.ManipelAttackFormation,
			DetermineSize = FormationSizeCalculation.ManipelFormation
		};
		public static readonly Formation GuardLine = new Formation()
		{
			Name = "Guard Line",
			Description = "Similar to the Line-Formation, but its intent is to be part of a greater \"Super-Formation\".",
			DetermineLocalPosition = FormationCalculation.GuardLineFormation,
			DeterminePositionDescription = FormationPositionDescription.GuardLineFormation,
			DetermineSize = FormationSizeCalculation.GuardLineFormation,
		};
		public static readonly Formation[] tournamentFormations = {
			Line,
			Wedge,
			DividedWedge,
			ParallelColumns,
			CommandedParallelColumns,
			RomanManipelBase,
			RomanManipelAttack,
			GuardLine
		};
	}
}