using System;
using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using BrilliantSkies.Core.Widgets;
using UnityEngine;
namespace TournamentMod.Serialisation
{
	[Serializable]
	public class TeamCompositionConfiguration : PrototypeSystem
	{
		public TeamCompositionConfiguration(uint uniqueID) : base(uniqueID) { }
		/// <summary>
		/// The distance between two entries on the team along the left-right axis.
		/// </summary>
		[Variable(0)]
		public Var<int> SpawngapLR { get; set; } = new VarIntClamp(0, -StaticConstants.MAX_SPAWN_GAP_VALUE, StaticConstants.MAX_SPAWN_GAP_VALUE);
		/// <summary>
		/// The distance between two entries on the team along the forward-backward axis.
		/// </summary>
		[Variable(1)]
		public Var<int> SpawngapFB { get; set; } = new VarIntClamp(0, -StaticConstants.MAX_SPAWN_GAP_VALUE, StaticConstants.MAX_SPAWN_GAP_VALUE);
		/// <summary>
		/// Hard limits are -1000 to 100000. The Planet decides over the real limits.
		/// </summary>
		[Variable(2)]
		public Var<Vector2> AltitudeLimits { get; set; } = new VarVector2PairClamp(new Vector2(-50, 500), 0, -1000, 100000);
		/// <summary>
		/// Maximum Distance towards enemies for this team.
		/// </summary>
		[Variable(3)]
		public Var<int> DistanceLimit { get; set; } = new VarIntClamp(1500, 0, 10000);
		/// <summary>
		/// When active the team uses the horizontal distance only.
		/// </summary>
		[Variable(4)]
		public Var<bool> UsesProjectedDistance { get; set; } = new VarBool(false);
		/// <summary>
		/// After accumulating this time, a participant will despawn.
		/// </summary>
		[Variable(5)]
		public Var<int> MaximumPenaltyTime { get; set; } = new VarIntClamp(90, 0, StaticConstants.MAX_TIME);
		/// <summary>
		/// Going over this speed will add penalty time
		/// </summary>
		[Variable(6)]
		public Var<int> MaximumSpeed { get; set; } = new VarIntClamp(10000, 0, 10000);
		/// <summary>
		/// Soft limits allow particpants to avoid penalty time under certain circumstances.
		/// </summary>
		[Variable(7)]
		public Var<bool> UsesSoftLimits { get; set; } = new VarBool(true);
		/// <summary>
		/// Before penalty time get added, a time buffer must be consumed first. It will reset once a participant is considered to be back in bounds.
		/// </summary>
		[Variable(8)]
		public Var<int> MaximumBufferTime { get; set; } = new VarIntClamp(0, 0, StaticConstants.MAX_TIME);
		/// <summary>
		/// When an entry goes out-of-bounds with soft limits enabled, if they are not moving away from enemies faster than this value, penalty time will not be given.
		/// </summary>
		[Variable(9)]
		public Var<int> DistanceReversal { get; set; } = new VarIntClamp(3, -StaticConstants.MAX_REVERSAL_SPEED_VALUE, StaticConstants.MAX_REVERSAL_SPEED_VALUE);
		/// <summary>
		/// When an entry goes out-of-bounds with soft limits enabled, if they are not moving away from the altitude bracket faster than this value, penalty time will not be given.
		/// </summary>
		[Variable(10)]
		public Var<int> AltitudeReversal { get; set; } = new VarIntClamp(-3, -StaticConstants.MAX_REVERSAL_SPEED_VALUE, StaticConstants.MAX_REVERSAL_SPEED_VALUE);
		/// <summary>
		/// The amount of materials this team should have. Without distribution, each entry will be given this amount.
		/// </summary>
		[Variable(11)]
		public Var<int> Resources { get; set; } = new VarIntClamp(10000, 0, 1000000);
		/// <summary>
		/// When an entry is considered fleeing from the given percentage of enemies, penalty time will be given.
		/// </summary>
		[Variable(12)]
		public Var<int> EnemyAttackPercentage { get; set; } = new VarIntClamp(50, 0, 100);
		/// <summary>
		/// Contains a representation of the formation composition of this team. X is Position of the Formation, Y is Formation-Index & Z is Entry-count.
		/// </summary>
		[Variable(13)]
		public Vector3IntList Formation { get; set; } = new Vector3IntList();
		/// <summary>
		/// When active, entries will have individual Materials.
		/// </summary>
		[Variable(14)]
		public Var<bool> IndividualEntryMaterials { get; set; } = new VarBool(false);
		/// <summary>
		/// Stores all 4 Faction Colors
		/// </summary>
		[Variable(15)]
		public VarList<Color> TeamColors { get; set; } = new ColorList();
		#region Entries
		/// <summary>
		/// x is Direction, y is Height, z is currentmaterials.
		/// </summary>
		[Variable(16)]
		public VarList<Vector3> EntryInformation { get; set; } = new Vector3List();
		/// <summary>
		/// The filepath towards the blueprints of the entries.
		/// </summary>
		[Variable(17)]
		public VarList<string> EntryFiles { get; set; } = new StringList();
		#endregion
	}
}