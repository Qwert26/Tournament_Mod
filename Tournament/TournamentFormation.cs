using UnityEngine;
namespace Tournament
{
	public struct TournamentFormation {
		public delegate Vector3 LocalPosition(bool isKing, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float offset, Tournament.SPAWN.LOC spawnLocation);
		public string Name { get; set; }
		public string Description { get; set; }
		public LocalPosition DetermineLocalPosition { get; set; }

		public static readonly TournamentFormation Line = new TournamentFormation()
		{
			Name = "Line",
			Description = "The classic Line Formation by wo0tness and Anickle. The Flagship should be in the middle position.",
			DetermineLocalPosition = FormationCalculation.LineFormation
		};
		public static readonly TournamentFormation Wedge = new TournamentFormation()
		{
			Name = "Wedge",
			Description = "Also known as a V-Formation. A positive Forward-Backward gap will result in an Arrow-Style, a negative Forward-Backward gap in a Funnel-style Formation. " +
							"The Flagship should be in first position as it will form the pivot point.",
			DetermineLocalPosition = FormationCalculation.WedgeFormation
		};
		public static readonly TournamentFormation DividedWedge = new TournamentFormation()
		{
			Name = "Divided Wedge",
			Description = "This variant of the V-Formation divides the fleet in thee groups. The Flagship should be in first position as it will form the pivot point. " +
			"Commandships are the second and third positions. The distance between the groups grows, the larger the fleet is.",
			DetermineLocalPosition = FormationCalculation.DividedWedgeFormation
		};
		public static readonly TournamentFormation[] tournamentFormations = { Line, Wedge };
	}
}