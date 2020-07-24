using Assets.Scripts;
using Assets.Scripts.Persistence;
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
		/// <summary>
		/// The Bluprintfile
		/// </summary>
		private BlueprintFile _bpf;
		/// <summary>
		/// The Blueprint to spawn.
		/// </summary>
		public Blueprint bp;
		/// <summary>
		/// The Index of the Faction this entry belongs to.
		/// </summary>
		public int FactionIndex {
			get;
			set;
		}
		/// <summary>
		/// The angle of the initial spawn.
		/// </summary>
		public float Spawn_direction {
			get;
			set;
		}
		/// <summary>
		/// The height of the initial spawn.
		/// </summary>
		public float Spawn_height {
			get;
			set;
		}
		/// <summary>
		/// The BluprintFile to load the Blueprint from.
		/// </summary>
		public BlueprintFile Bpf {
			get {
				return _bpf;
			}
			set {
				_bpf = value;
				bp = Bpf.Load(true);
			}
		}
		/// <summary>
		/// The Path
		/// </summary>
		public string FilePath { get; set; }
		/// <summary>
		/// Get the material capacity from the ConstructableSpecialInfo, will be used for Entry-specific materials.
		/// </summary>
		public float MaxMaterials => bp.CSI.MaterialCapacity;
		/// <summary>
		/// Amount of Materials this entry should be spawning in.
		/// </summary>
		public float CurrentMaterials { get; set; } = 0;
		/// <summary>
		/// Creates an array of the mainconstruct and all available drones in the style of name and cost.
		/// </summary>
		public string[] LabelCost {
			get {
				if (bp != null)
				{
					List<Blueprint> list = bp.SCs.FindAll((Blueprint x) => !x.IsSubConstructable());
					int count = list.Count;
					if (count > 0)
					{
						string[] array = new string[(count + 1)];
						float num = 0f;
						for (int i = 0; i < count; i++)
						{
							float material = list[i].CalculateResourceCost(false, true, false).Material;
							array[i + 1] = $"{list[i].blueprintName} <color=cyan>{material}</color>";
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
		public bool LoadBlueprintFile()
		{
			if (FilePath != null)
			{
				Bpf = GameFolders.GetCombinedBlueprintFolder(false).GetFile(FilePath + ".blueprint", false);
				if (Bpf != null)
				{
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// Converts the Blueprint into a MainConstruct and places it at the position provided by the Formation-Setting of the Team.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The total entry-count inside the team.</param>
		/// <param name="index">The position of this entry.</param>
		/// <param name="distance">The initial distance from the center.</param>
		/// <returns>A MainConstruct for further manipulation.</returns>
		public MainConstruct Spawn(float distance, float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			FactionSpecificationFaction faction = TournamentPlugin.factionManagement.factions[FactionIndex];
			BlueprintInitialisation initialisation = new BlueprintInitialisation();
			MainConstruct val = initialisation.Run(new BlueprintPositioning(PlanetList.MainFrame.FramePositionToUniversalPosition(Location(gapLeftRight, gapForwardBackward, count, index, distance)), Direction())
			{
				PositioningType = SpawnPositioning.OriginOrCentre
			}, new Blueprint2Construct(bp, SpawnInstructions.IgnoreDamage | SpawnInstructions.Creative | SpawnInstructions.PrepareForAction)
			{
				AssignNameToForce = true
			}, faction.Id);
			return val;
		}
		/// <summary>
		/// Calculates the spawn location.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The total entry-count inside the team.</param>
		/// <param name="index">The position of this entry.</param>
		/// <param name="distance">The initial distance from the center.</param>
		/// <returns>Final position</returns>
		private Vector3 Location(float gapLeftRight, float gapForwardBackward, int count, int index, float distance)
		{
			Vector3 ret = Tournament._me.GetFormation(FactionIndex).DetermineLocalPosition(Tournament._me.Parameters.ComputeFactionRotation(FactionIndex), gapLeftRight, gapForwardBackward, count, index, distance, Spawn_height);
			return Tournament._me.LocalOffsetFromTerrainCenter() + Tournament._me.Rotation * ret;
		}
		/// <summary>
		/// Calculates the spawn rotation.
		/// </summary>
		/// <returns>Final rotation</returns>
		private Quaternion Direction()
		{
			return FormationCalculation.FactionRotation(Tournament._me.Parameters.ComputeFactionRotation(FactionIndex) + Spawn_direction + Tournament._me.Parameters.Rotation + 180);
		}
	}
}