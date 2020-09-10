using System.Collections.Generic;
using BrilliantSkies.Core.Id;
using BrilliantSkies.Ftd.Planets;
using BrilliantSkies.Ftd.Planets.Factions;
using BrilliantSkies.Ftd.Planets.Instances;

namespace TournamentMod
{
	/// <summary>
	/// Manages active Factions/Teams.
	/// </summary>
	internal class FactionManagement
	{
		public FactionManagement() {
			factions=new List<FactionSpecificationFaction>(StaticConstants.MAX_TEAMS);
		}
		public readonly List<FactionSpecificationFaction> factions;
		private bool added = false;
		/// <summary>
		/// Removes the Factions when the Editor is selected and re-adds them otherwise.
		/// </summary>
		public void OnInstanceChange() {
			if (added && GAME_STATE.GetGameType() == enumGameType.worldeditor)
			{
				foreach (FactionSpecificationFaction fsf in factions)
				{
					FactionSpecifications.i.RemoveFaction(fsf);
				}
				added = false;
			}
			else if (!added) {
				foreach (FactionSpecificationFaction fsf in factions)
				{
					Planet.i.AddNewFactionSpecification(fsf);
				}
				added = true;
				InstanceSpecification.i.PostLoadInitiate(Planet.i, PostLoadInitiateType.New);
				InstanceSpecification.i.UpdateWithPotentialChangesToInstanceDefinition();
				FactionSpecifications.i.PostLoadInitiate();
			}
		}
		/// <summary>
		/// If the planet changes, whe need to readd the Factions.
		/// </summary>
		public void OnUniverseChange() {
			added = false;
		}
		/// <summary>
		/// Creates a new Faction and if needed also adds it to the Planet.
		/// </summary>
		public void CreateFaction() {
			factions.Add(new FactionSpecificationFaction()
			{
				Name = $"Team {factions.Count + 1}",
				AbreviatedName = $"T{factions.Count + 1}"
			});
			if (added) {
				Planet.i.AddNewFactionSpecification(factions[factions.Count-1]);
				InstanceSpecification.i.PostLoadInitiate(Planet.i, PostLoadInitiateType.New);
				InstanceSpecification.i.UpdateWithPotentialChangesToInstanceDefinition();
				FactionSpecifications.i.PostLoadInitiate();
			}
		}
		/// <summary>
		/// Makes sure that there are enough Factions/Teams for the planed battle.
		/// </summary>
		/// <param name="activeFactions"></param>
		public void EnsureFactionCount(int activeFactions) {
			while (factions.Count < activeFactions) {
				CreateFaction();
			}
		}
		/// <summary>
		/// Get the index of a Faction/Team by its ID.
		/// </summary>
		/// <param name="factionID"></param>
		/// <returns></returns>
		public int TeamIndexFromObjectID(ObjectId factionID) {
			return factions.FindIndex((fsf) => fsf.Id == factionID);
		}
	}
}