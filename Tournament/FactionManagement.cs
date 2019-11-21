using System.Collections.Generic;
using BrilliantSkies.Core.Id;
using BrilliantSkies.Ftd.Planets.Factions;
namespace Tournament
{
    internal class FactionManagement
    {
        public FactionManagement() {
            factions=new List<FactionSpecificationFaction>(6);
        }
        public readonly List<FactionSpecificationFaction> factions;
        private bool added = false;
        public void OnInstanceChange() {
            if (added && GAMESTATE.GetGameType() == enumGameType.worldeditor)
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
                    FactionSpecifications.i.AddNew(fsf);
                    fsf.PostLoadInitiate();
                }
                added = true;
            }
        }
        public void OnUniverseChange() {
            added = false;
        }
        public void CreateFaction() {
            factions.Add(new FactionSpecificationFaction() {
                Name=$"Team {factions.Count+1}",
                AbreviatedName=$"T{factions.Count+1}"
            });
            if (added) {
                FactionSpecifications.i.AddNew(factions[factions.Count-1]);
                factions[factions.Count - 1].PostLoadInitiate();
            }
        }
        public void EnsureFactionCount(int activeFactions) {
            while (factions.Count < activeFactions) {
                CreateFaction();
            }
        }
        public int TeamIndexFromObjectID(ObjectId factionID) {
            return factions.FindIndex((fsf) => fsf.Id == factionID);
        }
    }
}