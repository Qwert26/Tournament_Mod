using BrilliantSkies.Core.Types;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace TournamentMod.Serialisation
{
	/// <summary>
	/// Stores each and every Parameter of the Fight.
	/// </summary>
	[Serializable]
	public class Parameters
	{
		/// <summary>
		/// Container for all revelevant Parameters of a Tournament-grade Battle.
		/// </summary>
		public Parameters() { }
		#region Standard-Parameter
		/// <summary>
		/// The distance at the start of the battle of each team to the currently selected center.
		/// </summary>
		public int StartingDistance { get; set; } = 1000;
		/// <summary>
		/// The distance of teammembers toward each other on the X-axis. It gets measured based on the geometric center or the origin block.
		/// </summary>
		public List<int> SpawngapLR { get; set; } = new List<int>();
		/// <summary>
		/// The distance of teammembers toward each other on the Z-axis. It gets measured based on the geometric center or the origin block.
		/// </summary>
		public List<int> SpawngapFB { get; set; } = new List<int>();
		/// <summary>
		/// In the Planet-Editor are -1000 and 100000 absolute borders. The real borders depend from the terrain and the space-border.
		/// </summary>
		public List<Vector2> AltitudeLimits { get; set; } = new List<Vector2>();
		/// <summary>
		/// The distance that a particular team is allowed from enemies. Going over it may result in penalty time.
		/// </summary>
		public List<int> DistanceLimit { get; set; } = new List<int>();
		/// <summary>
		/// Teams which can have a lot of vertical freedom, might want to use a ground-projected distance.
		/// </summary>
		public List<bool> ProjectedDistance { get; set; } = new List<bool>();
		/// <summary>
		/// Each team can have its own maximum penalty time. Reaching or exceeding this limit will cause the offending entry to instantly detonate.
		/// </summary>
		public List<int> MaximumPenaltyTime { get; set; } = new List<int>();
		/// <summary>
		/// Each team can either have soft limits or hard limits.
		/// </summary>
		public List<bool> SoftLimits { get; set; } = new List<bool>();
		/// <summary>
		/// If an entry is currently violating any limits, it will first consume its time buffer before accumulating penalty time.
		/// </summary>
		public List<int> MaximumBufferTime { get; set; } = new List<int>();
		/// <summary>
		/// Staying outside the distance limit will cause a speed check: An entry must be moving a certain speed towards the current enemy to not get considered as violating the rules.
		/// </summary>
		public List<int> DistanceReverse { get; set; } = new List<int>();
		/// <summary>
		/// Staying outside the altitude limit will cause a speed check: An entry must be moving a certain speed towards the limits to not get considered as violating the rules.
		/// </summary>
		public List<int> AltitudeReverse { get; set; } = new List<int>();
		/// <summary>
		/// Maximum battle time, the game will be paused once reaching it.
		/// </summary>
		public int MaximumTime { get; set; } = 900;
		/// <summary>
		/// Unpausing after reaching the maximum time will activate "Overtime". after each section the game will be paused again.
		/// </summary>
		public int Overtime { get; set; } = 0;
		/// <summary>
		/// Every team has the same material settings if this is active.
		/// </summary>
		public bool SameMaterials { get; set; } = true;
		/// <summary>
		/// The total amount of materials a team has.
		/// </summary>
		public List<int> ResourcesPerTeam { get; set; } = new List<int>();
		/// <summary>
		/// The rotation of all the teams around the center.
		/// </summary>
		public int Rotation { get; set; } = 0;
		/// <summary>
		/// When active, the mod uses a fixed keymapping. Otherwise it uses the one based on the user.
		/// </summary>
		public bool DefaultKeys { get; set; } = false;
		/// <summary>
		/// Last used spawnheight. The real borders depend on the used map but the editor has absolute borders at -1000m and 100000m.
		/// </summary>
		public int SpawnHeight { get; set; } = 0;
		/// <summary>
		/// Last used spawn orientation.
		/// </summary>
		public float Direction { get; set; } = 0;
		/// <summary>
		/// There are at max 31 bordsections, counted from 0 to 30. The real borders depend on the used map.
		/// </summary>
		public int EastWestBoard { get; set; } = 0;
		/// <summary>
		/// There are at max 31 bordsections, counted from 0 to 30. The real borders depend on the used map.
		/// </summary>
		public int NorthSouthBoard { get; set; } = 0;
		/// <summary>
		/// When active for a team, the local materials get distributed across all entries and any excess goes into storage.
		/// When not active, all entries are getting this many resources into their storage.
		/// </summary>
		public bool DistributeLocalResources { get; set; } = false;
		/// <summary>
		/// When active all teams have uniform spawning- and penalty-rules.
		/// </summary>
		public bool UniformRules { get; set; } = true;
		/// <summary>
		/// The maximum speed a team is allowed to move at. Going over this limit might result in penalty time.
		/// </summary>
		public List<int> MaximumSpeed { get; set; } = new List<int>();
		/// <summary>
		/// During the check for distance violation, a teammember must be moving towards a certain percentage of enemies to not get penalty time.
		/// </summary>
		public List<int> EnemyAttackPercentage { get; set; } = new List<int>();
		/// <summary>
		/// Once a winner has been determined, the game can be paused if this is active.
		/// </summary>
		public bool PauseOnVictory { get; set; } = false;
		/// <summary>
		/// There are at max 11 terrains per sector. The real borders depend on the used map.
		/// </summary>
		public int EastWestTerrain { get; set; } = 0;
		/// <summary>
		/// There are at max 11 terrains per sector. The real borders depend on the used map.
		/// </summary>
		public int NorthSouthTerrain { get; set; } = 0;
		/// <summary>
		/// A terrain ist at max 4096m long. The real borders depend on the used map.
		/// </summary>
		public float EastWestOffset { get; set; } = 0;
		/// <summary>
		/// A terrain ist at max 4096m long. The real borders depend on the used map.
		/// </summary>
		public float NorthSouthOffset { get; set; } = 0;
		/// <summary>
		/// The current penalty weights. X is Team-index, Y is the type of penalty, Z is the relative weight.
		/// </summary>
		public List<Vector3> PenaltyWeights { get; set; } = new List<Vector3>();
		/// <summary>
		/// The current method of combining weights.
		/// </summary>
		public int WeightCombiner { get; set; } = (int) WeightCombinerType.ARITHMETIC;
		#endregion
		#region Fortgeschrittene Optionen
		/// <summary>
		/// When active the advanced options will be shown. Only for exceptional tournaments are these needed.
		/// </summary>
		public bool ShowAdvancedOptions { get; set; } = false;
		/// <summary>
		/// Stores the Lifesteal-value. -1 means that there is no material regain in selfdamage.
		/// </summary>
		public int MaterialConversion { get; set; } = 0;
		/// <summary>
		/// Stores the type of Cleanup as an integer.
		/// </summary>
		public int CleanUpMode { get; set; } = 2;
		/// <summary>
		/// Stores the type of health as an integer.
		/// </summary>
		public int HealthCalculation { get; set; } = 0;
		/// <summary>
		/// Any construct under this value will pickup penalty time. Only makes sense when automatic clean up is turned off.
		/// </summary>
		public int MinimumHealth { get; set; } = 0;
		/// <summary>
		/// How many teams are currently active.
		/// </summary>
		public int ActiveFactions { get; set; } = 2;
		#region Cleanup Einstellungen
		/// <summary>
		/// When turned on, cleanup functions will despawn all constructs which are considered sinking
		/// </summary>
		public bool CleanUpSinkingConstructs { get; set; } = true;
		/// <summary>
		/// A construct must fall under this blockwise health fraction in order to be potentially sinking.
		/// </summary>
		public int SinkingHealthFraction { get; set; } = 80;
		/// <summary>
		/// A construct must be below this altitude in order to be potentially sinking.
		/// </summary>
		public int SinkingAltitude { get; set; } = -10;
		/// <summary>
		/// When turned on, cleanup functions will despawn all constructs which have suffered too much damage.
		/// </summary>
		public bool CleanUpTooDamagedConstructs { get; set; } = true;
		/// <summary>
		/// A construct must fall under this blockwise health fraction in order to be too damaged.
		/// </summary>
		public int TooDamagedHealthFraction { get; set; } = 55;
		/// <summary>
		/// When turned on, cleanup functions will remove all non-drone constructs which have not enough blocks alive.
		/// </summary>
		public bool CleanUpTooSmallConstructs { get; set; } = true;
		/// <summary>
		/// A non-drone construct with a blockcount less than or equal to this value will be removed.
		/// </summary>
		public int TooSmallBlockCount { get; set; } = 10;
		/// <summary>
		/// When turned on, cleanup functions will remove all constructs which do not have an AI-Mainframe on them.
		/// </summary>
		public bool CleanUpNoAI { get; set; } = true;
		/// <summary>
		/// When turned on, cleanup functions will be delayed if a construct has recieved external repairs.
		/// </summary>
		public bool CleanUpDelayedByRepairs { get; set; } = true;
		/// <summary>
		/// When a construct recieves external repairs, it must be made functional in this time frame or it will still be removed.
		/// </summary>
		public int RepairDelayTime { get; set; } = 100;
		/// <summary>
		/// Contains a representation of the formation composition of each team. X is Team-Index, Y is Position of the Formation, Z is Formation-Index & W is Entry-count.
		/// </summary>
		public List<Vector4i> TeamFormations { get; set; } = new List<Vector4i>();
		/// <summary>
		/// When set true for a team, its entries will have individualised materials.
		/// </summary>
		public List<bool> TeamEntryMaterials { get; set; } = new List<bool>();
		#endregion
		#endregion
		#region Teams
		/// <summary>
		/// Stores the main color of each team.
		/// </summary>
		public List<Color> MainColorsPerTeam { get; set; } = new List<Color>();
		/// <summary>
		/// Stores the secondary color of each team.
		/// </summary>
		public List<Color> SecondaryColorsPerTeam { get; set; } = new List<Color>();
		/// <summary>
		/// Stores the trim color of each team.
		/// </summary>
		public List<Color> TrimColorsPerTeam { get; set; } = new List<Color>();
		/// <summary>
		/// Stores the detail color of each team.
		/// </summary>
		public List<Color> DetailColorsPerTeam { get; set; } = new List<Color>();
		/// <summary>
		/// Stores the last used color-gradient for the accumulated penalty time.
		/// </summary>
		public int PenaltyTimeGradient { get; set; } = 0;
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
			while (ResourcesPerTeam.Count < StaticConstants.MAX_TEAMS)
			{
				ResourcesPerTeam.Add(10000);
			}
			while (MainColorsPerTeam.Count < StaticConstants.MAX_TEAMS)
			{
				MainColorsPerTeam.Add(new Color(0, 0, 0, 0));
			}
			while (SecondaryColorsPerTeam.Count < StaticConstants.MAX_TEAMS)
			{
				SecondaryColorsPerTeam.Add(new Color(0, 0, 0, 0));
			}
			while (TrimColorsPerTeam.Count < StaticConstants.MAX_TEAMS)
			{
				TrimColorsPerTeam.Add(new Color(0, 0, 0, 0));
			}
			while (DetailColorsPerTeam.Count < StaticConstants.MAX_TEAMS)
			{
				DetailColorsPerTeam.Add(new Color(0, 0, 0, 0));
			}
			while (AltitudeLimits.Count < StaticConstants.MAX_TEAMS)
			{
				AltitudeLimits.Add(new Vector2(-50, 500));
			}
			while (DistanceLimit.Count < StaticConstants.MAX_TEAMS)
			{
				DistanceLimit.Add(1500);
			}
			while (SpawngapFB.Count < StaticConstants.MAX_TEAMS)
			{
				SpawngapFB.Add(0);
			}
			while (SpawngapLR.Count < StaticConstants.MAX_TEAMS)
			{
				SpawngapLR.Add(100);
			}
			while (ProjectedDistance.Count < StaticConstants.MAX_TEAMS)
			{
				ProjectedDistance.Add(false);
			}
			while (MaximumPenaltyTime.Count < StaticConstants.MAX_TEAMS)
			{
				MaximumPenaltyTime.Add(90);
			}
			while (MaximumBufferTime.Count < StaticConstants.MAX_TEAMS)
			{
				MaximumBufferTime.Add(0);
			}
			while (SoftLimits.Count < StaticConstants.MAX_TEAMS)
			{
				SoftLimits.Add(true);
			}
			while (DistanceReverse.Count < StaticConstants.MAX_TEAMS)
			{
				DistanceReverse.Add(3);
			}
			while (AltitudeReverse.Count < StaticConstants.MAX_TEAMS)
			{
				AltitudeReverse.Add(-3);
			}
			while (MaximumSpeed.Count < StaticConstants.MAX_TEAMS)
			{
				MaximumSpeed.Add(10000);
			}
			while (EnemyAttackPercentage.Count < StaticConstants.MAX_TEAMS)
			{
				EnemyAttackPercentage.Add(50);
			}
			while (TeamEntryMaterials.Count < StaticConstants.MAX_TEAMS)
			{
				TeamEntryMaterials.Add(false);
			}
		}
		/// <summary>
		/// Makes each team have identical settings. Uses Team 1(index 0) as a base line.
		/// </summary>
		public void MakeUniform()
		{
			for (int i = 1; i < StaticConstants.MAX_TEAMS; i++)
			{
				ResourcesPerTeam[i] = ResourcesPerTeam[0];
				AltitudeLimits[i] = AltitudeLimits[0];
				DistanceLimit[i] = DistanceLimit[0];
				SpawngapFB[i] = SpawngapFB[0];
				SpawngapLR[i] = SpawngapLR[0];
				ProjectedDistance[i] = ProjectedDistance[0];
				MaximumBufferTime[i] = MaximumBufferTime[0];
				MaximumPenaltyTime[i] = MaximumPenaltyTime[0];
				SoftLimits[i] = SoftLimits[0];
				DistanceReverse[i] = DistanceReverse[0];
				AltitudeReverse[i] = AltitudeReverse[0];
				MaximumSpeed[i] = MaximumSpeed[0];
				EnemyAttackPercentage[i] = EnemyAttackPercentage[0];
			}
		}
	}
}
