﻿using System.Collections.Generic;
using BrilliantSkies.Ftd.Planets.Factions;
namespace Tournament
{
    internal class FactionManagement
    {
        public FactionManagement() {
            factions=new List<FactionSpecificationFaction>(3){
                new FactionSpecificationFaction
                {
                    Name = "Team 1",
                    AbreviatedName = "T1",
                    FleetColors = TournamentFleetColor.classicYellow.Colors,
                },
                new FactionSpecificationFaction
                {
                    Name = "Team 2",
                    AbreviatedName = "T2",
                    FleetColors = TournamentFleetColor.classicRed.Colors,
                },
                new FactionSpecificationFaction
                {
                    Name = "Team 3",
                    AbreviatedName = "T3",
                    FleetColors = TournamentFleetColor.neoBlue.Colors,
                },
            };
        }
        public readonly List<FactionSpecificationFaction> factions;
        private bool added = false;
        public void OnInstanceChange() {
            if (added && GAMESTATE.GetGameType() == enumGameType.worldeditor)
            {
                foreach (FactionSpecificationFaction fsf in factions)
                {
                    FactionSpecifications.i.Factions.Remove(fsf);
                }
                added = false;
            }
            else if (!added && GAMESTATE.GetGameType() == enumGameType.instance) {
                FactionSpecifications.i.Factions.AddRange(factions);
                added = true;
            }
        }
        public void CreateFaction() {
            factions.Add(new FactionSpecificationFaction() {
                Name=$"Team {factions.Count+1}",
                AbreviatedName=$"T{factions.Count+1}"
            });
        }
        public void EnsureFactionCount(int activeFactions) {
            while (factions.Count < activeFactions) {
                CreateFaction();
            }
        }
    }
}
