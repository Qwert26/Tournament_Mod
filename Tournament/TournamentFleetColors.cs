using UnityEngine;
namespace Tournament
{
    public struct TournamentFleetColor
    {
        public Color Main { get; set; }
        public Color Secondary { get; set; }
        public Color Detail { get; set; }
        public Color Trim { get; set; }
        public Color[] Colors { get => new Color[4] { Main, Secondary, Trim, Detail }; }
        public string Name { get; set; }
        public string Description { get; set; }

        public readonly static TournamentFleetColor classicYellow = new TournamentFleetColor()
        {
            Name = "Classic Yellow",
            Description = "The classic yellow colorscheme by Wo0tness, usually used for the Team 1-Faction.",
            Main = new Color(1f, 0.84f, 0f, 1f),
            Secondary = new Color(0.85f, 0.65f, 0.13f, 1f),
            Trim = new Color(1f, 0.65f, 0f, 1f),
            Detail = new Color(0.85f, 0.55f, 0f, 1f)
        };
        public readonly static TournamentFleetColor classicRed = new TournamentFleetColor()
        {
            Name = "Classic Red",
            Description = "The classic red colorscheme by Wo0tness, usually used for the Team 2-Faction.",
            Main = new Color(1f, 0f, 0f, 1f),
            Secondary = new Color(0.55f, 0f, 0f, 1f),
            Trim = new Color(0.7f, 0.15f, 0.15f, 1f),
            Detail = new Color(1f, 0.35f, 0.35f, 1f)
        };
        public readonly static TournamentFleetColor neoBlue = new TournamentFleetColor()
        {
            Name = "Neo Blue",
            Description = "A new blue colorscheme by Qwert26, usually used for the Team 3-Faction.",
            Main = new Color(0f, 0f, 1f, 1f),
            Secondary = new Color(0f, 0f, 0.55f, 1f),
            Trim = new Color(0.15f, 0.15f, 0.7f, 1f),
            Detail = new Color(0.35f, 0.35f, 1f, 1f)
        };
        public readonly static TournamentFleetColor medievalOldRoyal = new TournamentFleetColor()
        {
            Name = "Medieval Old Royal",
            Description = "Based on the old colors for kings, these vehicles have spent a long time in the coloration chamber.",
            Main = new Color(0.5f, 0f, 0.5f, 1f), //Purple
            Secondary = new Color(0.93f, 0.51f, 0.93f, 1f), //Violett
            Trim = new Color(0f, 0f, 1f, 1f), //Blue
            Detail = new Color(0.5f, 0f, 0f, 1f) //Darkred
        };
        public readonly static TournamentFleetColor medievalMiddleRoyal = new TournamentFleetColor()
        {
            Name = "Medieval Middle Royal",
            Description = "Based on the old colors for kings, these vehicles have spent a medium time in the coloration chamber.",
            Main = new Color(0.93f, 0.51f, 0.93f, 1f), //Violet
            Secondary = new Color(0f, 0f, 1f, 1f), //Blue
            Trim = new Color(0.5f, 0f, 0f, 1f), //Darkred
            Detail = new Color(1f, 0.5f, 0.5f, 1f) //Hotpink
        };
        public readonly static TournamentFleetColor medievalYoungRoyal = new TournamentFleetColor()
        {
            Name = "Medieval Young Royal",
            Description = "Based on the old colors for kings, these vehicles have spent a short time in the coloration chamber.",
            Main = new Color(0f, 0f, 1f, 1f), //Blue
            Secondary = new Color(0.5f, 0f, 0f, 1f), //Darkred
            Trim = new Color(1f, 0.5f, 0.5f, 1f), //Hotpink
            Detail = new Color(0f, 0.5f, 0f, 1f) //Green
        };
        public readonly static TournamentFleetColor terranRepublic = new TournamentFleetColor()
        {
            Name = "Terran Republic, Planetside",
            Description = "TBD",
            Main = new Color(1f, 0f, 0f, 1f), //Red
            Secondary = new Color(0f, 0f, 0f, 1f), //Black
            Trim = new Color(0.5f, 0.5f, 0.5f, 1f), //Grey
            Detail = new Color(0f, 0f, 0f, 0f) //Transparent
        };
        public readonly static TournamentFleetColor newConglomerate = new TournamentFleetColor()
        {
            Name = "New Conglomerate, Planetside",
            Description = "TBD",
            Main = new Color(0f, 0f, 1f, 1f), //Blue
            Secondary = new Color(1f, 1f, 0f, 1f), //Yellow
            Trim = new Color(0.96f, 0.96f, 0.86f, 1f), //Beige
            Detail = new Color(0f, 0f, 0f, 0f) //Transparent
        };
        public readonly static TournamentFleetColor vanuSovereignty = new TournamentFleetColor()
        {
            Name = "Vanu Sovereignty, Planetside",
            Description = "TBD",
            Main = new Color(0.5f, 0f, 0.5f, 1f), //Purple
            Secondary = new Color(0f, 1f, 1f, 1f), //Aqua
            Trim = new Color(0.5f, 0.5f, 0.5f, 1f), //Grey
            Detail = new Color(0f, 0f, 0f, 0f) //Transparent
        };
        public readonly static TournamentFleetColor[] colorSchemes = {
            classicYellow,
            classicRed,
            neoBlue,
            medievalOldRoyal,
            medievalMiddleRoyal,
            medievalYoungRoyal,
            terranRepublic,
            newConglomerate,
            vanuSovereignty
        };
    }
}