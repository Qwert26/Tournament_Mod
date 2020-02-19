using UnityEngine;
namespace TournamentMod
{
	public struct FleetColor
	{
		public Color Main { get; set; }
		public Color Secondary { get; set; }
		public Color Detail { get; set; }
		public Color Trim { get; set; }
		public Color[] Colors { get => new Color[4] { Main, Secondary, Trim, Detail }; }
		public string Name { get; set; }
		public string Description { get; set; }
		public readonly static FleetColor classicYellow = new FleetColor()
		{
			Name = "Classic Yellow",
			Description = "The classic yellow colorscheme by Wo0tness, usually used for the Team 1-Faction.",
			Main = new Color(1f, 0.84f, 0f, 1f),
			Secondary = new Color(0.85f, 0.65f, 0.13f, 1f),
			Trim = new Color(1f, 0.65f, 0f, 1f),
			Detail = new Color(0.85f, 0.55f, 0f, 1f)
		};
		public readonly static FleetColor classicRed = new FleetColor()
		{
			Name = "Classic Red",
			Description = "The classic red colorscheme by Wo0tness, usually used for the Team 2-Faction.",
			Main = new Color(1f, 0f, 0f, 1f),
			Secondary = new Color(0.55f, 0f, 0f, 1f),
			Trim = new Color(0.7f, 0.15f, 0.15f, 1f),
			Detail = new Color(1f, 0.35f, 0.35f, 1f)
		};
		public readonly static FleetColor neoBlue = new FleetColor()
		{
			Name = "Neo Blue",
			Description = "A new blue colorscheme by Qwert26, usually used for the Team 3-Faction.",
			Main = new Color(0f, 0f, 1f, 1f),
			Secondary = new Color(0f, 0f, 0.55f, 1f),
			Trim = new Color(0.15f, 0.15f, 0.7f, 1f),
			Detail = new Color(0.35f, 0.35f, 1f, 1f)
		};
		public readonly static FleetColor neoGreen = new FleetColor()
		{
			Name = "Neo Green",
			Description = "A new green colorscheme by Qwert26, if there would have been a 4.Team, it would have used these.",
			Main = new Color(0, 1, 0, 1),
			Secondary = new Color(0, 0.55f, 0, 1),
			Trim = new Color(0.15f, 0.7f, 0.15f, 1),
			Detail = new Color(0.35f, 1, 0.35f, 1)
		};
		public readonly static FleetColor medievalOldRoyal = new FleetColor()
		{
			Name = "Medieval Old Royal",
			Description = "Based on the old colors for kings, these vehicles have spent a long time in the coloration chamber.",
			Main = new Color(0.5f, 0f, 0.5f, 1f), //Purple
			Secondary = new Color(0.93f, 0.51f, 0.93f, 1f), //Violett
			Trim = new Color(0f, 0f, 1f, 1f), //Blue
			Detail = new Color(0.5f, 0f, 0f, 1f) //Darkred
		};
		public readonly static FleetColor medievalMiddleRoyal = new FleetColor()
		{
			Name = "Medieval Middle Royal",
			Description = "Based on the old colors for kings, these vehicles have spent a medium time in the coloration chamber.",
			Main = new Color(0.93f, 0.51f, 0.93f, 1f), //Violet
			Secondary = new Color(0f, 0f, 1f, 1f), //Blue
			Trim = new Color(0.5f, 0f, 0f, 1f), //Darkred
			Detail = new Color(1f, 0.5f, 0.5f, 1f) //Hotpink
		};
		public readonly static FleetColor medievalYoungRoyal = new FleetColor()
		{
			Name = "Medieval Young Royal",
			Description = "Based on the old colors for kings, these vehicles have spent a short time in the coloration chamber.",
			Main = new Color(0f, 0f, 1f, 1f), //Blue
			Secondary = new Color(0.5f, 0f, 0f, 1f), //Darkred
			Trim = new Color(1f, 0.5f, 0.5f, 1f), //Hotpink
			Detail = new Color(0f, 0.5f, 0f, 1f) //Green
		};
		public readonly static FleetColor terranRepublic = new FleetColor()
		{
			Name = "Terran Republic, Planetside",
			Description = "Taken from the offical color palette of the TR.",
			Main = new Color(0.37f, 0.35f, 0.34f, 1f), //Dark Grey
			Secondary = new Color(0.59f, 0.07f, 0f, 1f), //Dark Red
			Trim = new Color(0.83f, 0.83f, 0.83f, 1f), //Light Grey
			Detail = new Color(0f, 0f, 0f, 1f) //Black
		};
		public readonly static FleetColor newConglomerate = new FleetColor()
		{
			Name = "New Conglomerate, Planetside",
			Description = "Taken from the offical color palette of the NC.",
			Main = new Color(0.11f, 0.27f, 0.6f, 1f), //Dark Blue
			Secondary = new Color(0.98f, 0.82f, 0f, 1f), //Yellow
			Trim = new Color(0.83f, 0.83f, 0.83f, 1f), //Light Grey
			Detail = new Color(0.61f, 0.56f, 0.47f, 1f) //Tan
		};
		public readonly static FleetColor vanuSovereignty = new FleetColor()
		{
			Name = "Vanu Sovereignty, Planetside",
			Description = "Taken from the offical color palette of the VS.",
			Main = new Color(0.38f, 0.15f, 0.59f, 1f), //Blue violet
			Secondary = new Color(0f, 0.5f, 0.51f, 1f), //Dark Cyan
			Trim = new Color(0.61f, 0.61f, 0.64f, 1f), //Grey
			Detail = new Color(0f, 0f, 0f, 1f) //Black
		};
		public readonly static FleetColor naniteSystemsOps = new FleetColor()
		{
			Name = "Nanite Systems Operative, Planetside",
			Description = "Taken from the offical color palette of the NSO.",
			Main = new Color(0.95f, 0.93f, 0.91f, 1f), //Silver
			Secondary = new Color(0.49f, 0.47f, 0.36f, 1f), //Dark grey
			Trim = new Color(0.83f, 0.83f, 0.83f, 1f), //Light Grey
			Detail = new Color(0.18f, 0.18f, 0.18f, 1f) //Light Black
		};
		public readonly static FleetColor[] colorSchemes = {
			classicYellow,
			classicRed,
			neoBlue,
			neoGreen,
			medievalOldRoyal,
			medievalMiddleRoyal,
			medievalYoungRoyal,
			terranRepublic,
			newConglomerate,
			vanuSovereignty,
			naniteSystemsOps
		};
	}
}