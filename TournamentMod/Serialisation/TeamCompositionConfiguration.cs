using System.Collections.Generic;
using UnityEngine;
using System;
namespace TournamentMod.Serialisation
{
	[Serializable]
	public class TeamCompositionConfiguration
	{
		/// <summary>
		/// The distance between two entries on the team along the left-right axis.
		/// </summary>
		public int SpawngapLR { get; set; } = 0;
		/// <summary>
		/// The distance between two entries on the team along the forward-backward axis.
		/// </summary>
		public int SpawngapFB { get; set; } = 0;
		/// <summary>
		/// Hard limits are -1000 to 100000. The Planet decides over the real limits.
		/// </summary>
		public Vector2 AltitudeLimits { get; set; } = new Vector2(-50, 500);
		/// <summary>
		/// Maximum Distance towards enemies for this team.
		/// </summary>
		public int DistanceLimit { get; set; } = 1500;
		/// <summary>
		/// When active the team uses the horizontal distance only.
		/// </summary>
		public bool UsesProjectedDistance { get; set; } = false;
		/// <summary>
		/// After accumulating this time, a participant will despawn.
		/// </summary>
		public int MaximumPenaltyTime { get; set; } = 90;
		/// <summary>
		/// Going over this speed will add penalty time
		/// </summary>
		public int MaximumSpeed { get; set; } = 10000;
		/// <summary>
		/// Soft limits allow particpants to avoid penalty time under certain circumstances.
		/// </summary>
		public bool UsesSoftLimits { get; set; } = true;
		/// <summary>
		/// Before penalty time get added, a time buffer must be consumed first. It will reset once a participant is considered to be back in bounds.
		/// </summary>
		public int MaximumBufferTime { get; set; } = 0;
		/// <summary>
		/// When an entry goes out-of-bounds with soft limits enabled, if they are not moving away from enemies faster than this value, penalty time will not be given.
		/// </summary>
		public int DistanceReversal { get; set; } = 3;
		/// <summary>
		/// When an entry goes out-of-bounds with soft limits enabled, if they are not moving away from the altitude bracket faster than this value, penalty time will not be given.
		/// </summary>
		public int AltitudeReversal { get; set; } = -3;
		/// <summary>
		/// The amount of materials this team should have. Without distribution, each entry will be given this amount.
		/// </summary>
		public int Resources { get; set; } = 10000;
		/// <summary>
		/// When an entry is considered fleeing from the given percentage of enemies, penalty time will be given.
		/// </summary>
		public int EnemyAttackPercentage { get; set; } = 50;
		/// <summary>
		/// Contains a representation of the formation composition of this team. X is Position of the Formation, Y is Formation-Index & Z is Entry-count.
		/// </summary>
		public List<Vector3Int> Formation { get; set; } = new List<Vector3Int>();
		/// <summary>
		/// When active, entries will have individual Materials.
		/// </summary>
		public bool IndividualEntryMaterials { get; set; } = false;
		/// <summary>
		/// Stores all 4 Faction Colors
		/// </summary>
		public List<Color> TeamColors { get; set; } = new List<Color>();
		#region Entries
		/// <summary>
		/// x is Direction, y is Height, z is currentmaterials.
		/// </summary>
		public List<Vector3> EntryInformation { get; set; } = new List<Vector3>();
		/// <summary>
		/// The filepath towards the blueprints of the entries.
		/// </summary>
		public List<string> EntryFiles { get; set; } = new List<string>();
		#endregion
	}
}