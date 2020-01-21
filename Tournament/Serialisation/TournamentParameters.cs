using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using BrilliantSkies.Core.Widgets;
using System;
using UnityEngine;
namespace Tournament.Serialisation
{
	[Serializable]
	public class TournamentParameters : PrototypeSystem
	{
		public TournamentParameters(uint uniqueId) : base(uniqueId) {}
		#region Standard-Parameter
		[Variable(0,"Starting Distance(m)","The Initial starting distance from the center to the teams.")]
		public Var<int> StartingDistance { get; set; } = new VarIntClamp(1000, 0, 20000);
		[Variable(1, "Spawn gap Left-Right(m)", "Spawn distance between team members left to right.")]
		public VarList<int> SpawngapLR { get; set; } = new IntList();
		[Variable(2, "Spawn gap Forward-Backward(m)", "Spawn distance between team members front to back.")]
		public VarList<int> SpawngapFB { get; set; } = new IntList();
		/// <summary>
		/// Laut dem Planeten-Editor sind -1000 und 100000 absolute Grenzen. Die eigentlichen Grenzen hängen vom Terrain und von der Weltraum-Grenze ab.
		/// </summary>
		[Variable(3, "Altitude Limits(m)", "x is minimum altitude and y is maximum altitude.")]
		public VarList<Vector2> AltitudeLimits { get; set; } = new Vector2List();
		[Variable(4, "Distance Limit(m)", "Maximum permitted distance towards the nearest enemy.")]
		public VarList<int> DistanceLimit { get; set; } = new IntList();
		[Variable(5, "Projected distance", "Use a top-down projected 2D-distance instead of the true 3D-distance. Good for Tournaments with a lot of vertical freedom.")]
		public VarList<bool> ProjectedDistance { get; set; } = new BoolList();
		[Variable(6, "Maximum penalty time(s)", "Any entries, which exceed this time will be removed from the field.")]
		public VarList<int> MaximumPenaltyTime { get; set; } = new IntList();
		[Variable(7, "Soft limits", "With soft limits, entries will not pick up penalty time if they are moving towards the limits. " +
			"With hard limits, entries will pick up penalty time for as long as they are outside those limits")]
		public VarList<bool> SoftLimits { get; set; } = new BoolList();
		[Variable(8, "Maximum buffer time(s)", "For vehicles outside the limits, this buffer must deplete first, before penalty time is added.")]
		public VarList<int> MaximumBufferTime { get; set; } = new IntList();
		[Variable(9, "Distance reversal(m/s)", "A positive value means that this speed is the maximum fleeing speed, " +
			"while a negative value means that this speed is the minimum for reengagments.")]
		public VarList<int> DistanceReverse { get; set; } = new IntList();
		[Variable(10, "Altitude reversal(m/s)", "A positive value means that this speed is the maximum speed for going away from the altutude limits, " +
			"while a negative value means that this is the minimum speed for going back into the altitude limits.")]
		public VarList<int> AltitudeReverse { get; set; } = new IntList();
		[Variable(11, "Maximum time(s)")]
		public Var<int> MaximumTime { get; set; } = new VarIntClamp(900, 0, 3600);
		[Variable(12,"Overtime(s)")]
		public Var<int> Overtime { get; set; } = new VarIntClamp(0, 0, 3600);
		[Variable(13, "Local resources", "When active, fills up the material containers of each entry up to the specified amounts.")]
		public VarBool LocalResources { get; set; } = new VarBool(false);
		[Variable(14, "Same materials", "When active, all teams will have the exact same starting materials.")]
		public VarBool SameMaterials { get; set; } = new VarBool(true);
		[Variable(15, "Infinte Resources for given Team", "When active, the Team at a particular index will have infinte Resources.")]
		public VarList<bool> InfinteResourcesPerTeam { get; set; } = new BoolList();
		[Variable(17, "Resources for a given Team at a given index")]
		public VarList<int> ResourcesPerTeam { get; set; } = new IntList();
		[Variable(19,"Rotation")]
		public Var<int> Rotation { get; set; } = new VarIntClamp(0, -90, 90);
		[Variable(20,"Default Keymapping","When true the Tournament-Mod uses a static keymap, otherwise it uses your currently configured Keymap.")]
		public VarBool DefaultKeys { get; set; } = new VarBool(false);
		/// <summary>
		/// Zu letzt genutzte Spawnhöhe. Die eigentliche Schranke hängt von der aktuelle Karte ab.
		/// </summary>
		[Variable(21,"Location")]
		public Var<int> SpawnHeight { get; set; } = new VarIntClamp(0, -1000, 100000);
		/// <summary>
		/// Zu letzt genutzte Spawnrichtung.
		/// </summary>
		[Variable(22, "Direction")]
		public VarFloatAngle Direction { get; set; } = new VarFloatAngle(0, VarFloatAngle.LimitType.Z180To180);
		/// <summary>
		/// Es gibt maximal 31 Bordsektionen, nummeriert von 0 bis 30. Die eigentliche obere Schranke häng von der aktuellen Karte ab.
		/// </summary>
		[Variable(23,"East-West-Section")]
		public Var<int> EastWestBoard { get; set; } = new VarIntClamp(0, 0, 30);
		/// <summary>
		/// Es gibt maximal 31 Bordsektionen, nummeriert von 0 bis 30. Die eigentliche obere Schranke häng von der aktuellen Karte ab.
		/// </summary>
		[Variable(24, "North-South-Section")]
		public Var<int> NorthSouthBoard { get; set; } = new VarIntClamp(0, 0, 30);
		[Variable(25,"Distribute local Resources","When active, the materials get distributed along the entries of a team, any excess goes into faction storage.")]
		public Var<bool> DistributeLocalResources { get; set; } = new VarBool(false);
		[Variable(26, "Uniform Rules", "When active, all teams will have the same Ruleset.")]
		public Var<bool> UniformRules { get; set; } = new VarBool(true);
		[Variable(27,"Maximum Speed(m/s)","The maximum speed a construct is allowed to have.")]
		public VarList<int> MaximumSpeed { get; set; } = new IntList();
		[Variable(28,"Attack Percentage(%)","When out of bounds, a construct must move towards a certain percentage of enemies in order to not pick up penalty time.")]
		public VarList<int> EnemyAttackPercentage { get; set; } = new IntList();
		[Variable(29,"Pause on Victory","When active, the game will be paused once a team has won.")]
		public Var<bool> PauseOnVictory { get; set; } = new VarBool(false);
		#endregion
		#region Fortgeschrittene Optionen
		[Variable(100,"Show advanced options", "Usually closed, use this for further customization.")]
		public VarBool ShowAdvancedOptions { get; set; } = new VarBool(false);
		[Variable(101, "Formation index for a given Team at a given index", "The index for the formation of a Team.")]
		public VarList<int> FormationIndexPerTeam { get; set; } = new IntList();
		[Variable(103,"Lifesteal(%)", "-1 is a special value: In this case, materials by friendly fire are not refunded.")]
		public Var<int> MaterialConversion { get; set; } = new VarIntClamp(0, -1, 100);
		[Variable(104, "Constructable cleanup")]
		public Var<int> CleanUpMode { get; set; } = new VarIntClamp(2, 0, 3);
		[Variable(105, "Health calculation")]
		public Var<int> HealthCalculation { get; set; } = new VarIntClamp(0, 0, 3);
		[Variable(106, "Minimum health(%)", "Any construct below this fraction will pickup penalty time.")]
		public Var<int> MinimumHealth { get; set; } = new VarIntClamp(0, 0, 100);
		[Variable(107, "Active Factions")]
		public Var<int> ActiveFactions { get; set; } = new VarIntClamp(2, 2, 6);
		#region Cleanup Einstellungen
		[Variable(108, "Cleanup sinking constructs", "Removes all Constructs, which are currently sinking.")]
		public Var<bool> CleanUpSinkingConstructs { get; set; } = new VarBool(true);
		[Variable(109, "Health fraction for sinking", "Any construct below this fraction might be considered as sinking.")]
		public Var<int> SinkingHealthFraction { get; set; } = new VarIntClamp(80, 0, 100);
		[Variable(110, "Altitude for sinking", "Any construct below this fraction might be considered as sinking.")]
		public Var<int> SinkingAltitude { get; set; } = new VarIntClamp(-10, -1000, 0);
		[Variable(111, "Cleanup damaged constructs", "Removes all constructs, which sustained too much damage.")]
		public Var<bool> CleanUpTooDamagedConstructs { get; set; } = new VarBool(true);
		[Variable(112, "Health fraction for Damage", "Any construct below this fraction will be considered as too damaged.")]
		public Var<int> TooDamagedHealthFraction { get; set; } = new VarIntClamp(55, 0, 100);
		[Variable(113, "Cleanup small Constructs", "Removes all Constructs, which have not enough alive blocks.")]
		public Var<bool> CleanUpTooSmallConstructs { get; set; } = new VarBool(true);
		[Variable(114, "Minimum Block count", "Removes any construct, which is not a drone and has less than this many blocks alive.")]
		public Var<int> TooSmallBlockCount { get; set; } = new VarIntClamp(10, 1, 100);
		[Variable(115, "Cleanup brainless Constructs", "Removes any constructs, which don't have any AI-Mainframes.")]
		public Var<bool> CleanUpNoAI { get; set; } = new VarBool(true);
		[Variable(116, "Delay Cleanup by Repairs", "Delays the removal of any construct, which is repaired by other constructs.")]
		public Var<bool> CleanUpDelayedByRepairs { get; set; } = new VarBool(true);
		[Variable(117, "Maximum Delay Time", "If the repairs could not make a construct operational in this timeframe, it will still be removed.")]
		public Var<int> RepairDelayTime { get; set; } = new VarIntClamp(100, 10, 600);
		#endregion
		#endregion
		#region Teams
		[Variable(201,"Main Color of a given Team-index")]
		public VarList<Color> MainColorsPerTeam { get; set; } = new ColorList();
		[Variable(202, "Main Color of a given Team-index")]
		public VarList<Color> SecondaryColorsPerTeam { get; set; } = new ColorList();
		[Variable(203, "Main Color of a given Team-index")]
		public VarList<Color> TrimColorsPerTeam { get; set; } = new ColorList();
		[Variable(204, "Main Color of a given Team-index")]
		public VarList<Color> DetailColorsPerTeam { get; set; } = new ColorList();
		#endregion
		public float ComputeFactionRotation(int factionindex) {
			return 360f * factionindex / ActiveFactions;
		}
		public void EnsureEnoughData() {
			while (InfinteResourcesPerTeam.Count < 6) {
				InfinteResourcesPerTeam.Add(false);
			}
			while (ResourcesPerTeam.Count < 6) {
				ResourcesPerTeam.Add(10000);
			}
			while (FormationIndexPerTeam.Count < 6) {
				FormationIndexPerTeam.Add(0);
			}
			while (MainColorsPerTeam.Count < 6) {
				MainColorsPerTeam.Add(new Color(0, 0, 0, 0));
			}
			while (SecondaryColorsPerTeam.Count < 6)
			{
				SecondaryColorsPerTeam.Add(new Color(0, 0, 0, 0));
			}
			while (TrimColorsPerTeam.Count < 6)
			{
				TrimColorsPerTeam.Add(new Color(0, 0, 0, 0));
			}
			while (DetailColorsPerTeam.Count < 6)
			{
				DetailColorsPerTeam.Add(new Color(0, 0, 0, 0));
			}
			while (AltitudeLimits.Count < 6) {
				AltitudeLimits.Add(new Vector2(-50, 500));
			}
			while (DistanceLimit.Count < 6) {
				DistanceLimit.Add(1500);
			}
			while (SpawngapFB.Count < 6) {
				SpawngapFB.Add(0);
			}
			while (SpawngapLR.Count < 6) {
				SpawngapLR.Add(100);
			}
			while (ProjectedDistance.Count < 6) {
				ProjectedDistance.Add(false);
			}
			while (MaximumPenaltyTime.Count < 6) {
				MaximumPenaltyTime.Add(90);
			}
			while (MaximumBufferTime.Count < 6) {
				MaximumBufferTime.Add(0);
			}
			while (SoftLimits.Count < 6) {
				SoftLimits.Add(true);
			}
			while (DistanceReverse.Count < 6) {
				DistanceReverse.Add(3);
			}
			while (AltitudeReverse.Count < 6) {
				AltitudeReverse.Add(-3);
			}
			while (MaximumSpeed.Count < 6) {
				MaximumSpeed.Add(10000);
			}
			while (EnemyAttackPercentage.Count < 6)
			{
				EnemyAttackPercentage.Add(50);
			}
		}
		public void MakeUniform() {
			for (int i = 1; i < 6; i++) {
				InfinteResourcesPerTeam.Us[i] = InfinteResourcesPerTeam[0];
				ResourcesPerTeam.Us[i] = ResourcesPerTeam[0];
				AltitudeLimits.Us[i] = AltitudeLimits[0];
				DistanceLimit.Us[i] = DistanceLimit[0];
				SpawngapFB.Us[i] = SpawngapFB[0];
				SpawngapLR.Us[i] = SpawngapLR[0];
				ProjectedDistance.Us[i] = ProjectedDistance[0];
				MaximumBufferTime.Us[i] = MaximumBufferTime[0];
				MaximumPenaltyTime.Us[i] = MaximumPenaltyTime[0];
				SoftLimits.Us[i] = SoftLimits[0];
				DistanceReverse.Us[i] = DistanceReverse[0];
				AltitudeReverse.Us[i] = AltitudeReverse[0];
				MaximumSpeed.Us[i] = MaximumSpeed[0];
				EnemyAttackPercentage.Us[i] = EnemyAttackPercentage[0];
			}
		}
	}
}
