using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using BrilliantSkies.Core.Widgets;
using UnityEngine;
namespace Tournament
{
    public class TournamentParameters : PrototypeSystem
    {
        public TournamentParameters(uint uniqueId) : base(uniqueId) {}
        #region Standard-Parameter
        [Variable(0,"Starting Distance(m)","The Initial starting distance of the two teams.")]
        public Var<int> StartingDistance { get; set; } = new VarIntClamp(1000, 0, 40000);
        [Variable(1, "Spawn gap Left-Right(m)", "Spawn distance between team members left to right.")]
        public Var<int> SpawngapLR { get; set; } = new VarIntClamp(100, -1000, 1000);
        [Variable(2, "Spawn gap Forward-Backward(m)", "Spawn distance between team members front to back.")]
        public Var<int> SpawngapFB { get; set; } = new VarIntClamp(0, -1000, 1000);
        [Variable(3, "Altitude Limits(m)","x is minimum altitude and y is maximum altitude.")]
        public Var<Vector2> AltitudeLimits { get; set; } = new VarVector2PairClamp(new Vector2(-50, 500), 0, -500, 3000);
        [Variable(4, "Distance Limit(m)", "Maximum permitted distance towards the nearest enemy.")]
        public Var<int> DistanceLimit { get; set; } = new VarIntClamp(1500, 0, 10000);
        [Variable(5, "Projected distance", "Use a top-down projected 2D-distance instead of the true 3D-distance. Good for Tournaments with a lot of vertical freedom.")]
        public Var<bool> ProjectedDistance { get; set; } = new VarBool(false);
        [Variable(6, "Maximum penalty time(s)", "Any entries, which exceed this time will be removed from the field.")]
        public Var<int> MaximumPenaltyTime { get; set; } = new VarIntClamp(120, 0, 3600);
        [Variable(7, "Soft limits", "With soft limits, entries will not pick up penalty time if they are moving towards the limits. " +
            "With hard limits, entries will pick up penalty time for as long as they are outside those limits")]
        public Var<bool> SoftLimits { get; set; } = new VarBool(true);
        [Variable(8, "Maximum buffer time(s)", "For vehicles outside the limits, this buffer must deplete first, before penalty time is added.")]
        public Var<int> MaximumBufferTime { get; set; } = new VarIntClamp(3, 0, 360);
        [Variable(9, "Distance reversal(m/s)", "A positive value means that this speed is the maximum fleeing speed, " +
            "while a negative value means that this speed is the minimum for reengagments.")]
        public Var<int> DistanceReverse { get; set; } = new VarIntClamp(3, -300, 300);
        [Variable(10, "Altitude reversal(m/s)", "A positive value means that this speed is the maximum speed for going away from the altutude limits, " +
            "while a negative value means that this is the minimum speed for going back into the altitude limits.")]
        public Var<int> AltitudeReverse { get; set; } = new VarIntClamp(3, -300, 300);
        [Variable(11, "Maximum time(s)")]
        public Var<int> MaximumTime { get; set; } = new VarIntClamp(900, 0, 3600);
        [Variable(12,"Overtime(s)")]
        public Var<int> Overtime { get; set; } = new VarIntClamp(0, 0, 3600);
        [Variable(13, "Local resources", "When active, fills up the material containers of each entry up to the specified amounts.")]
        public Var<bool> LocalResources { get; set; } = new VarBool(false);
        [Variable(14, "Same materials", "When active, both teams will have the exact same starting materials.")]
        public Var<bool> SameMaterials { get; set; } = new VarBool(true);
        [Variable(15, "Infinte Resources for Team 1", "When active, Team 1 will have infinte Resources. If \"Same Materials\" is also active, both teams will have infinte resources.")]
        public Var<bool> InfinteResourcesTeam1 { get; set; } = new VarBool(false);
        [Variable(16, "Infinte Resources for Team 2", "When active, Team 2 will have infinte Resources. ")]
        public Var<bool> InfinteResourcesTeam2 { get; set; } = new VarBool(false);
        [Variable(17, "Resources for Team 1")]
        public Var<int> ResourcesTeam1 { get; set; } = new VarIntClamp(10000, 0, 1000000);
        [Variable(18, "Resources for Team 1")]
        public Var<int> ResourcesTeam2 { get; set; } = new VarIntClamp(10000, 0, 1000000);
        [Variable(19,"Rotation")]
        public Var<int> Rotation { get; set; } = new VarIntClamp(0, -90, 90);
        [Variable(20,"Default Keymapping","When true uses the Tournament-Mod a static keymap, otherwise it uses your currently configured Keymap.")]
        public Var<bool> DefaultKeys { get; set; } = new VarBool(false);
        [Variable(21,"Height Offset","Height Offset for the next entry.")]
        public Var<int> Offset { get; set; } = new VarIntClamp(0, -100, 400);
        [Variable(22,"Location")]
        public Var<int> Location { get; set; } = new VarIntClamp(1, 0, 3);
        [Variable(23, "Direction")]
        public Var<int> Direction { get; set; } = new VarIntClamp(0, 0, 3);
        /// <summary>
        /// Es gibt maximal 31 Bordsektionen, nummeriert von 0 bis 30. Die eigentliche obere Schranke häng von der aktuellen Karte ab.
        /// </summary>
        [Variable(24,"East-West-Section")]
        public Var<int> EastWestBoard { get; set; } = new VarIntClamp(7, 0, 30);
        /// <summary>
        /// Es gibt maximal 31 Bordsektionen, nummeriert von 0 bis 30. Die eigentliche obere Schranke häng von der aktuellen Karte ab.
        /// </summary>
        [Variable(25, "North-South-Section")]
        public Var<int> NorthSouthBoard { get; set; } = new VarIntClamp(7, 0, 30);
        #endregion
        #region Fortgeschrittene Optionen
        [Variable(100,"Show advanced options", "Usually closed, use this for further customization.")]
        public Var<bool> ShowAdvancedOptions { get; set; } = new Var<bool>(false);
        [Variable(101,"Team 1 formation index", "The index for the formation of Team 1.")]
        public Var<int> Team1FormationIndex { get; set; } = new VarIntClamp(0, 0, TournamentFormation.tournamentFormations.Length - 1);
        [Variable(102, "Team 2 formation index", "The index for the formation of Team 2.")]
        public Var<int> Team2FormationIndex { get; set; } = new VarIntClamp(0, 0, TournamentFormation.tournamentFormations.Length - 1);
        [Variable(103,"Lifesteal(%)", "-1 is a special value: In this case, materials by friendly fire are not refunded.")]
        public Var<int> MaterialConversion { get; set; } = new VarIntClamp(0, -1, 100);
        [Variable(104, "Constructable cleanup")]
        public Var<int> CleanUpMode { get; set; } = new VarIntClamp(2, 0, 3);
        [Variable(105, "Health calculation")]
        public Var<int> HealthCalculation { get; set; } = new VarIntClamp(0, 0, 3);
        [Variable(106, "Minimum health(%)")]
        public Var<int> MinimumHealth { get; set; } = new VarIntClamp(0, 0, 100);
        #endregion
        #region Augenschmaus
        [Variable(200,"Show Eyecandy")]
        public Var<bool> ShowEyecandy { get; set; } = new VarBool(false);
        #region Team 1
        [Variable(201,"Team 1 Main Color")]
        public Var<Color> Team1Main { get; set; } = new VarColor(new Color(1f, 0.84f, 0f, 1f));
        [Variable(202, "Team 1 Secondary Color")]
        public Var<Color> Team1Secondary { get; set; } = new VarColor(new Color(0.85f, 0.65f, 0.13f, 1f));
        [Variable(203, "Team 1 Trim Color")]
        public Var<Color> Team1Trim { get; set; } = new VarColor(new Color(1f, 0.65f, 0f, 1f));
        [Variable(204, "Team 1 Detail Color")]
        public Var<Color> Team1Detail { get; set; } = new VarColor(new Color(0.85f, 0.55f, 0f, 1f));
        #endregion
        #region Team 2
        [Variable(205, "Team 2 Main Color")]
        public Var<Color> Team2Main { get; set; } = new VarColor(new Color(1f, 0f, 0f, 1f));
        [Variable(206, "Team 2 Secondary Color")]
        public Var<Color> Team2Secondary { get; set; } = new VarColor(new Color(0.55f, 0f, 0f, 1f));
        [Variable(207, "Team 2 Trim Color")]
        public Var<Color> Team2Trim { get; set; } = new VarColor(new Color(0.7f, 0.15f, 0.15f, 1f));
        [Variable(208, "Team 2 Detail Color")]
        public Var<Color> Team2Detail { get; set; } = new VarColor(new Color(1f, 0.4f, 0.3f, 1f));
        #endregion
        #endregion
    }
}
