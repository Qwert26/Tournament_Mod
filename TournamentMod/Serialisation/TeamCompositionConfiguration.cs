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
		[Variable(0)]
		public Var<int> SpawngapLR { get; set; } = new VarIntClamp(0, -1000, 1000);
		[Variable(1)]
		public Var<int> SpawngapFB { get; set; } = new VarIntClamp(0, -1000, 1000);
		[Variable(2)]
		public Var<Vector2> AltitudeLimits { get; set; } = new VarVector2Pair(new Vector2(-50, 500), 0);
		[Variable(3)]
		public Var<int> DistanceLimit { get; set; } = new VarIntClamp(1500, 0, 10000);
		[Variable(4)]
		public Var<bool> UsesProjectedDistance { get; set; } = new VarBool(false);
		[Variable(5)]
		public Var<int> MaximumPenaltyTime { get; set; } = new VarIntClamp(90, 0, 3600);
		[Variable(6)]
		public Var<int> MaximumSpeed { get; set; } = new VarIntClamp(10000, 0, 10000);
		[Variable(7)]
		public Var<bool> UsesSoftLimits { get; set; } = new VarBool(true);
		[Variable(8)]
		public Var<int> MaximumBufferTime { get; set; } = new VarIntClamp(0, 0, 3600);
		[Variable(9)]
		public Var<int> DistanceReversal { get; set; } = new VarIntClamp(3, -500, 500);
		[Variable(10)]
		public Var<int> AltitudeReversal { get; set; } = new VarIntClamp(-3, -500, 500);
		[Variable(11)]
		public Var<int> Resources { get; set; } = new VarIntClamp(10000, 0, 1000000);
		[Variable(12)]
		public Var<int> EnemyAttackPercentage { get; set; } = new VarIntClamp(50, 0, 100);
		/// <summary>
		/// Contains a representation of the formation composition of this team. X is Position of the Formation, Y is Formation-Index & Z is Entry-count.
		/// </summary>
		[Variable(13)]
		public Vector3IntList Formation { get; set; } = new Vector3IntList();
		[Variable(14)]
		public Var<bool> IndividualEntryMaterials { get; set; } = new VarBool(false);
		/// <summary>
		/// Stores all 4 Faction Colors
		/// </summary>
		[Variable(15)]
		public VarList<Color> TeamColors { get; set; } = new ColorList();
	}
}