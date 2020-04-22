using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using BrilliantSkies.Core.Widgets;
using System;
using UnityEngine;
namespace TournamentMod.Serialisation
{
	/// <summary>
	/// Stores each and every Parameter of the Fight.
	/// </summary>
	[Serializable]
	public class Parameters : PrototypeSystem
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uniqueId"></param>
		public Parameters(uint uniqueId) : base(uniqueId) { }
		#region Standard-Parameter
		/// <summary>
		/// The distance at the start of the battle of each team to the currently selected center.
		/// </summary>
		[Variable(0, "Starting Distance(m)", "The Initial starting distance from the center to the teams.")]
		public Var<int> StartingDistance { get; set; } = new VarIntClamp(1000, 0, 20000);
		/// <summary>
		/// The distance of teammembers toward each other on the X-axis. It gets measured based on the geometric center or the origin block.
		/// </summary>
		[Variable(1, "Spawn gap Left-Right(m)", "Spawn distance between team members left to right.")]
		public VarList<int> SpawngapLR { get; set; } = new IntList();
		/// <summary>
		/// The distance of teammembers toward each other on the Z-axis. It gets measured based on the geometric center or the origin block.
		/// </summary>
		[Variable(2, "Spawn gap Forward-Backward(m)", "Spawn distance between team members front to back.")]
		public VarList<int> SpawngapFB { get; set; } = new IntList();
		/// <summary>
		/// In the Planet-Editor are -1000 and 100000 absolute borders. The real borders depend from the terrain and the space-border.
		/// </summary>
		[Variable(3, "Altitude Limits(m)", "x is minimum altitude and y is maximum altitude.")]
		public VarList<Vector2> AltitudeLimits { get; set; } = new Vector2List();
		/// <summary>
		/// The distance that a particular team is allowed from enemies. Going over it may result in penalty time.
		/// </summary>
		[Variable(4, "Distance Limit(m)", "Maximum permitted distance towards the nearest enemy.")]
		public VarList<int> DistanceLimit { get; set; } = new IntList();
		/// <summary>
		/// Teams which can have a lot of vertical freedom, might want to use a ground-projected distance.
		/// </summary>
		[Variable(5, "Projected distance", "Use a top-down projected 2D-distance instead of the true 3D-distance. Good for Tournaments with a lot of vertical freedom.")]
		public VarList<bool> ProjectedDistance { get; set; } = new BoolList();
		/// <summary>
		/// Each team can have its own maximum penalty time. Reaching or exceeding this limit will cause the offending entry to instantly detonate.
		/// </summary>
		[Variable(6, "Maximum penalty time(s)", "Any entries, which exceed this time will be removed from the field.")]
		public VarList<int> MaximumPenaltyTime { get; set; } = new IntList();
		/// <summary>
		/// Each team can either have soft limits or hard limits.
		/// </summary>
		[Variable(7, "Soft limits", "With soft limits, entries will not pick up penalty time if they are moving towards the limits. " +
			"With hard limits, entries will pick up penalty time for as long as they are outside those limits")]
		public VarList<bool> SoftLimits { get; set; } = new BoolList();
		/// <summary>
		/// If an entry is currently violating any limits, it will first consume its time buffer before accumulating penalty time.
		/// </summary>
		[Variable(8, "Maximum buffer time(s)", "For vehicles outside the limits, this buffer must deplete first, before penalty time is added.")]
		public VarList<int> MaximumBufferTime { get; set; } = new IntList();
		/// <summary>
		/// Staying outside the distance limit will cause a speed check: An entry must be moving a certain speed towards the current enemy to not get considered as violating the rules.
		/// </summary>
		[Variable(9, "Distance reversal(m/s)", "A positive value means that this speed is the maximum fleeing speed, " +
			"while a negative value means that this speed is the minimum for reengagments.")]
		public VarList<int> DistanceReverse { get; set; } = new IntList();
		/// <summary>
		/// Staying outside the altitude limit will cause a speed check: An entry must be moving a certain speed towards the limits to not get considered as violating the rules.
		/// </summary>
		[Variable(10, "Altitude reversal(m/s)", "A positive value means that this speed is the maximum speed for going away from the altutude limits, " +
			"while a negative value means that this is the minimum speed for going back into the altitude limits.")]
		public VarList<int> AltitudeReverse { get; set; } = new IntList();
		/// <summary>
		/// Maximum battle time, the game will be paused once reaching it.
		/// </summary>
		[Variable(11, "Maximum time(s)")]
		public Var<int> MaximumTime { get; set; } = new VarIntClamp(900, 0, 3600);
		/// <summary>
		/// Unpausing after reaching the maximum time will activate "Overtime". after each section the game will be paused again.
		/// </summary>
		[Variable(12, "Overtime(s)")]
		public Var<int> Overtime { get; set; } = new VarIntClamp(0, 0, 3600);
		/// <summary>
		/// Every team has the same material settings if this is active.
		/// </summary>
		[Variable(14, "Same materials", "When active, all teams will have the exact same starting materials.")]
		public VarBool SameMaterials { get; set; } = new VarBool(true);
		/// <summary>
		/// The total amount of materials a team has.
		/// </summary>
		[Variable(17, "Resources for a given Team at a given index")]
		public VarList<int> ResourcesPerTeam { get; set; } = new IntList();
		/// <summary>
		/// The rotation of all the teams around the center.
		/// </summary>
		[Variable(19, "Rotation")]
		public Var<int> Rotation { get; set; } = new VarIntClamp(0, -90, 90);
		/// <summary>
		/// When active, the mod uses a fixed keymapping. Otherwise it uses the one based on the user.
		/// </summary>
		[Variable(20, "Default Keymapping", "When true the Tournament-Mod uses a static keymap, otherwise it uses your currently configured Keymap.")]
		public VarBool DefaultKeys { get; set; } = new VarBool(false);
		/// <summary>
		/// Last used spawnheight. The real borders depend on the used map.
		/// </summary>
		[Variable(21, "Location")]
		public Var<int> SpawnHeight { get; set; } = new VarIntClamp(0, -1000, 100000);
		/// <summary>
		/// Last used spawn orientation.
		/// </summary>
		[Variable(22, "Direction")]
		public VarFloatAngle Direction { get; set; } = new VarFloatAngle(0, VarFloatAngle.LimitType.Z180To180);
		/// <summary>
		/// There are at max 31 bordsections, counted from 0 to 30. The real borders depend on the used map.
		/// </summary>
		[Variable(23, "East-West-Section")]
		public Var<int> EastWestBoard { get; set; } = new VarIntClamp(0, 0, 30);
		/// <summary>
		/// There are at max 31 bordsections, counted from 0 to 30. The real borders depend on the used map.
		/// </summary>
		[Variable(24, "North-South-Section")]
		public Var<int> NorthSouthBoard { get; set; } = new VarIntClamp(0, 0, 30);
		/// <summary>
		/// When active for a team, the local materials get distributed across all entries and any excess goes into storage.
		/// When not active, all entries are getting this many resources into their storage.
		/// </summary>
		[Variable(25, "Distribute local Materials", "When active, the materials get distributed along the entries of a team, any excess goes into faction storage.")]
		public Var<bool> DistributeLocalResources { get; set; } = new VarBool(false);
		/// <summary>
		/// When active all teams have uniform penalty rules.
		/// </summary>
		[Variable(26, "Uniform Rules", "When active, all teams will have the same Ruleset.")]
		public Var<bool> UniformRules { get; set; } = new VarBool(true);
		/// <summary>
		/// The maximum speed a team is allowed to move at. Going over this limit might result in penalty time.
		/// </summary>
		[Variable(27, "Maximum Speed(m/s)", "The maximum speed a construct is allowed to have.")]
		public VarList<int> MaximumSpeed { get; set; } = new IntList();
		/// <summary>
		/// During the check for distance violation, a teammember must be moving towards a certain percentage of enemies to not get penalty time.
		/// </summary>
		[Variable(28, "Attack Percentage(%)", "When out of bounds, a construct must move towards a certain percentage of enemies in order to not pick up penalty time.")]
		public VarList<int> EnemyAttackPercentage { get; set; } = new IntList();
		/// <summary>
		/// Once a winner has been determined, the game can be paused if this is active.
		/// </summary>
		[Variable(29, "Pause on Victory", "When active, the game will be paused once a team has won.")]
		public Var<bool> PauseOnVictory { get; set; } = new VarBool(false);
		/// <summary>
		/// There are at max 11 terrains per sector. The real borders depend on the used map.
		/// </summary>
		[Variable(30, "East-West-Terrain of Sector", "A Sector is made up from multiple Terrain-segments.")]
		public Var<int> EastWestTerrain { get; set; } = new VarIntClamp(0, -5, 5);
		/// <summary>
		/// There are at max 11 terrains per sector. The real borders depend on the used map.
		/// </summary>
		[Variable(31, "North-South-Terrain of Sector", "A Sector is made up from multiple Terrain-segments.")]
		public Var<int> NorthSouthTerrain { get; set; } = new VarIntClamp(0, -5, 5);
		/// <summary>
		/// A terrain ist at max 4096m long. The real borders depend on the used map.
		/// </summary>
		[Variable(32, "East-West-Offset from Terrain")]
		public Var<float> EastWestOffset { get; set; } = new VarFloatClamp(0, -2048, 2048);
		/// <summary>
		/// A terrain ist at max 4096m long. The real borders depend on the used map.
		/// </summary>
		[Variable(33, "North-South-Offset from Terrain")]
		public Var<float> NorthSouthOffset { get; set; } = new VarFloatClamp(0, -2048, 2048);
		#endregion
		#region Fortgeschrittene Optionen
		/// <summary>
		/// When active the advanced options will be shown. Only for exceptional tournaments are these needed.
		/// </summary>
		[Variable(100, "Show advanced options", "Usually closed, use this for further customization.")]
		public VarBool ShowAdvancedOptions { get; set; } = new VarBool(false);
		/// <summary>
		/// Stores the Lifesteal-value. -1 means that there is no material regain in selfdamage.
		/// </summary>
		[Variable(103, "Lifesteal(%)", "-1 is a special value: In this case, materials by friendly fire are not refunded.")]
		public Var<int> MaterialConversion { get; set; } = new VarIntClamp(0, -1, 100);
		/// <summary>
		/// Stores the type of Cleanup as an integer.
		/// </summary>
		[Variable(104, "Constructable cleanup")]
		public Var<int> CleanUpMode { get; set; } = new VarIntClamp(2, 0, 3);
		/// <summary>
		/// Stores the type of health as an integer.
		/// </summary>
		[Variable(105, "Health calculation")]
		public Var<int> HealthCalculation { get; set; } = new VarIntClamp(0, 0, 3);
		/// <summary>
		/// Any construct under this value will pickup penalty time. Only makes sense when automatic clean up is turned off.
		/// </summary>
		[Variable(106, "Minimum health(%)", "Any construct below this fraction will pickup penalty time.")]
		public Var<int> MinimumHealth { get; set; } = new VarIntClamp(0, 0, 100);
		/// <summary>
		/// How many teams are currently active.
		/// </summary>
		[Variable(107, "Active Factions")]
		public Var<int> ActiveFactions { get; set; } = new VarIntClamp(2, 2, 6);
		#region Cleanup Einstellungen
		/// <summary>
		/// When turned on, cleanup functions will despawn all constructs which are considered sinking
		/// </summary>
		[Variable(108, "Cleanup sinking constructs", "Removes all Constructs, which are currently sinking.")]
		public Var<bool> CleanUpSinkingConstructs { get; set; } = new VarBool(true);
		/// <summary>
		/// A construct must fall under this blockwise health fraction in order to be potentially sinking.
		/// </summary>
		[Variable(109, "Health fraction for sinking", "Any construct below this fraction might be considered as sinking.")]
		public Var<int> SinkingHealthFraction { get; set; } = new VarIntClamp(80, 0, 100);
		/// <summary>
		/// A construct must be below this altitude in order to be potentially sinking.
		/// </summary>
		[Variable(110, "Altitude for sinking", "Any construct below this fraction might be considered as sinking.")]
		public Var<int> SinkingAltitude { get; set; } = new VarIntClamp(-10, -1000, 0);
		/// <summary>
		/// When turned on, cleanup functions will despawn all constructs which have suffered too much damage.
		/// </summary>
		[Variable(111, "Cleanup damaged constructs", "Removes all constructs, which sustained too much damage.")]
		public Var<bool> CleanUpTooDamagedConstructs { get; set; } = new VarBool(true);
		/// <summary>
		/// A construct must fall under this blockwise health fraction in order to be too damaged.
		/// </summary>
		[Variable(112, "Health fraction for Damage", "Any construct below this fraction will be considered as too damaged.")]
		public Var<int> TooDamagedHealthFraction { get; set; } = new VarIntClamp(55, 0, 100);
		/// <summary>
		/// When turned on, cleanup functions will remove all non-drone constructs which have not enough blocks alive.
		/// </summary>
		[Variable(113, "Cleanup small Constructs", "Removes all Constructs, which have not enough alive blocks.")]
		public Var<bool> CleanUpTooSmallConstructs { get; set; } = new VarBool(true);
		/// <summary>
		/// A non-drone construct with a blockcount less than or equal to this value will be removed.
		/// </summary>
		[Variable(114, "Minimum Block count", "Removes any construct, which is not a drone and has less than this many blocks alive.")]
		public Var<int> TooSmallBlockCount { get; set; } = new VarIntClamp(10, 1, 100);
		/// <summary>
		/// When turned on, cleanup functions will remove all constructs which do not have an AI-Mainframe on them.
		/// </summary>
		[Variable(115, "Cleanup brainless Constructs", "Removes any constructs, which don't have any AI-Mainframes.")]
		public Var<bool> CleanUpNoAI { get; set; } = new VarBool(true);
		/// <summary>
		/// When turned on, cleanup functions will be delayed if a construct has recieved external repairs.
		/// </summary>
		[Variable(116, "Delay Cleanup by Repairs", "Delays the removal of any construct, which is repaired by other constructs.")]
		public Var<bool> CleanUpDelayedByRepairs { get; set; } = new VarBool(true);
		/// <summary>
		/// When a construct recieves external repairs, it must be made functional in this time frame or it will still be removed.
		/// </summary>
		[Variable(117, "Maximum Delay Time", "If the repairs could not make a construct operational in this timeframe, it will still be removed.")]
		public Var<int> RepairDelayTime { get; set; } = new VarIntClamp(100, 10, 600);
		/// <summary>
		/// Contains a representation of the formation composition of each team. X is Team-Index, Y is Position of the Formation, Z is Formation-Index & W is Entry-count.
		/// </summary>
		[Variable(118, "Team index-Formation position-Formation index-Starting index")]
		public Vector4IntList TeamFormations { get; set; } = new Vector4IntList();
		/// <summary>
		/// When set true for a team, its entries will have individualised materials.
		/// </summary>
		[Variable(119, "The Entries with a given team-index will have individual materials")]
		public BoolList TeamEntryMaterials { get; set; } = new BoolList();
		#endregion
		#endregion
		#region Teams
		/// <summary>
		/// Stores the main color of each team.
		/// </summary>
		[Variable(201, "Main Color of a given Team-index")]
		public VarList<Color> MainColorsPerTeam { get; set; } = new ColorList();
		/// <summary>
		/// Stores the secondary color of each team.
		/// </summary>
		[Variable(202, "Main Color of a given Team-index")]
		public VarList<Color> SecondaryColorsPerTeam { get; set; } = new ColorList();
		/// <summary>
		/// Stores the trim color of each team.
		/// </summary>
		[Variable(203, "Main Color of a given Team-index")]
		public VarList<Color> TrimColorsPerTeam { get; set; } = new ColorList();
		/// <summary>
		/// Stores the detail color of each team.
		/// </summary>
		[Variable(204, "Main Color of a given Team-index")]
		public VarList<Color> DetailColorsPerTeam { get; set; } = new ColorList();
		#endregion
		/// <summary>
		/// Computes the rotation around the center for a given team, factoring in the current amount of active teams.
		/// </summary>
		/// <param name="factionindex"></param>
		/// <returns></returns>
		public float ComputeFactionRotation(int factionindex)
		{
			return 360f * factionindex / ActiveFactions;
		}
		/// <summary>
		/// Enforces in each list, that there are at least 6 entries: One for each team.
		/// </summary>
		public void EnsureEnoughData()
		{
			while (ResourcesPerTeam.Count < 6)
			{
				ResourcesPerTeam.Add(10000);
			}
			while (MainColorsPerTeam.Count < 6)
			{
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
			while (AltitudeLimits.Count < 6)
			{
				AltitudeLimits.Add(new Vector2(-50, 500));
			}
			while (DistanceLimit.Count < 6)
			{
				DistanceLimit.Add(1500);
			}
			while (SpawngapFB.Count < 6)
			{
				SpawngapFB.Add(0);
			}
			while (SpawngapLR.Count < 6)
			{
				SpawngapLR.Add(100);
			}
			while (ProjectedDistance.Count < 6)
			{
				ProjectedDistance.Add(false);
			}
			while (MaximumPenaltyTime.Count < 6)
			{
				MaximumPenaltyTime.Add(90);
			}
			while (MaximumBufferTime.Count < 6)
			{
				MaximumBufferTime.Add(0);
			}
			while (SoftLimits.Count < 6)
			{
				SoftLimits.Add(true);
			}
			while (DistanceReverse.Count < 6)
			{
				DistanceReverse.Add(3);
			}
			while (AltitudeReverse.Count < 6)
			{
				AltitudeReverse.Add(-3);
			}
			while (MaximumSpeed.Count < 6)
			{
				MaximumSpeed.Add(10000);
			}
			while (EnemyAttackPercentage.Count < 6)
			{
				EnemyAttackPercentage.Add(50);
			}
			while (TeamEntryMaterials.Count < 6)
			{
				TeamEntryMaterials.Add(false);
			}
		}
		/// <summary>
		/// Makes each team have identical settings. Uses Team 1 as a base line.
		/// </summary>
		public void MakeUniform()
		{
			for (int i = 1; i < 6; i++)
			{
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
				TeamEntryMaterials.Us[i] = TeamEntryMaterials[0];
			}
		}
	}
}
