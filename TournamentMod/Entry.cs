using Assets.Scripts;
using Assets.Scripts.Persistence;
using BrilliantSkies.Core.Id;
using System.Collections.Generic;
using UnityEngine;
using BrilliantSkies.Core.UniverseRepresentation;
using BrilliantSkies.Ftd.Planets.Factions;
using BrilliantSkies.Ftd.Persistence.Inits;
namespace TournamentMod
{
	using Formations;
	/// <summary>
	/// An Entry to be spawned into the fight.
	/// </summary>
	public class Entry
	{
		private BlueprintFile _bpf;
		public Blueprint bp;
		public int FactionIndex {
			get;
			set;
		}
		public float Spawn_direction {
			get;
			set;
		}
		public int Spawn_height {
			get;
			set;
		}
		public ObjectId Team_id {
			get;
			set;
		}
		public BlueprintFile Bpf {
			get {
				return _bpf;
			}
			set {
				_bpf = value;
				bp = Bpf.Load(true);
			}
		}
		public string[] LabelCost {
			get {
				if (bp != null)
				{
					List<Blueprint> list = bp.SCs.FindAll((Blueprint x) => !x.IsSubConstructable());
					int count = list.Count;
					if (count > 0)
					{
						string[] array = new string[checked(count + 1)];
						float num = 0f;
						for (int i = 0; i < count; i = checked(i + 1))
						{
							float material = list[i].CalculateResourceCost(false, true, false).Material;
							array[checked(i + 1)] = $"{list[i].blueprintName} <color=cyan>{material}</color>";
							num += material;
						}
						array[0] = $"{bp.blueprintName} <color=cyan>{bp.CalculateResourceCost(false, true, false).Material - num}</color>";
						return array;
					}
					return new string[1]
					{
						$"{bp.blueprintName} <color=cyan>{bp.CalculateResourceCost(false, true, false).Material}</color>"
					};
				}
				return null;
			}
		}
		public MainConstruct Spawn(float dis, float gapLR, float gapFB, int count, int pos)
		{
			MainConstruct val = BlueprintConverter.Convert(bp, SpawnInstructions.IgnoreDamage);
			FactionSpecificationFaction faction = TournamentPlugin.factionManagement.factions[FactionIndex];
			Team_id = faction.Id;
			BlueprintInitialisation initialisation = new BlueprintInitialisation(val)
			{
				Positioning = new BlueprintPositioning(PlanetList.MainFrame.FramePositionToUniversalPosition(VLoc(gapLR, gapFB, count, pos, dis)), VDir())
				{
					PositioningType = SpawnPositioning.OriginOrCentre
				}
			};
			initialisation.Run(Team_id);
			(val.Owner as ConstructableOwner).SetFleetColors(Team_id);
			return val;
		}
		public Vector3 VLoc(float gapLR, float gapFB, int count, int pos, float dis)
		{
			Vector3 ret = Tournament._me.GetFormation(FactionIndex).DetermineLocalPosition(Tournament._me.Parameters.ComputeFactionRotation(FactionIndex), gapLR, gapFB, count, pos, dis, Spawn_height);
			return Tournament._me.LocalOffsetFromTerrainCenter() + Tournament._me.Rotation * ret;
		}
		public Quaternion VDir()
		{
			return FormationCalculation.FactionRotation(Tournament._me.Parameters.ComputeFactionRotation(FactionIndex) + Spawn_direction + Tournament._me.Parameters.Rotation + 180);
		}
	}
}