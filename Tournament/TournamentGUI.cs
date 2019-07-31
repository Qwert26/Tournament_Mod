using Assets.Scripts.Gui;
using Assets.Scripts.Persistence;
using BrilliantSkies.Ui.Layouts;
using BrilliantSkies.Core.UiSounds;
using BrilliantSkies.Ui.Tips;
using UnityEngine;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.Core.Timing;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using System.Collections.Generic;
using System;
namespace Tournament
{
    [Obsolete("No longer needed, replaced by the Console",true)]
    public class TournamentGUI : BrilliantSkies.Ui.Displayer.Types.ThrowAwayObjectGui<Tournament>
    {
        public Vector2 listpos;

        public Vector2 optpos, optpos2, optpos3;

        private bool showEyecandy = false;

        private BrilliantSkies.Ui.TreeSelection.TreeSelectorGuiElement<BlueprintFile, BlueprintFolder> _treeSelector;

        private int sectionsNorthSouth, sectionsEastWest;

        private Tournament t;

        private int team1CurrentIndex = 0, team2CurrentIndex = 1, team3CurrentIndex = -1;
        private int team1NewIndex = 0, team2NewIndex = 1, team3NewIndex = -1;

        public TournamentGUI(Tournament tourny)
        {
            t = tourny;
        }

        public override void SetGuiSettings()
        {
            GuiSettings.PausesPlay = false;
            GuiSettings.PausesMultiplayerPlay = false;
            GuiSettings.QGui = false;
        }

        public override void OnActivateGui()
        {
            BlueprintFolder val = GameFolders.GetCombinedBlueprintFolder();
            sectionsNorthSouth = WorldSpecification.i.BoardLayout.NorthSouthBoardSectionCount - 1;
            sectionsEastWest = WorldSpecification.i.BoardLayout.EastWestBoardSectionCount - 1;
            if (t.Parameters.EastWestBoard > sectionsEastWest)
            {
                t.Parameters.EastWestBoard.Reset();
            }
            if (t.Parameters.NorthSouthBoard > sectionsNorthSouth)
            {
                t.Parameters.NorthSouthBoard.Reset();
            }
            _treeSelector = FtdGuiUtils.GetFileBrowserFor(val);
            _treeSelector.Refresh();
            t.ResetCam();
            GameEvents.UpdateEvent += t.UpdateBoardSectionPreview;
        }

        public override void OnGui()
        {
            GUILayout.BeginArea(new Rect(0f, 0f, 340f, 580f), "Select Contestants", GUI.skin.window);
            _treeSelector.OnGui(new Rect(30f, 35f, 280f, 520f));
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(340f, 0f, 600f, 430f), "Tournament Settings", GUI.skin.window);
            optpos = GUILayout.BeginScrollView(optpos);

            GUISliders.TotalWidthOfWindow = 600;
            GUISliders.TextWidth = 240;
            GUISliders.DecimalPlaces = 0;
            GUISliders.UpperMargin = 0;

            t.Parameters.StartingDistance.Us = (int)GUISliders.LayoutDisplaySlider("Spawn Distance", t.Parameters.StartingDistance, 0, 20000, 0, new ToolTip("Spawn distance from the center of the map to the teams."));
            t.Parameters.SpawngapLR.Us = (int)GUISliders.LayoutDisplaySlider("Spawn Gap Left-Right", t.Parameters.SpawngapLR, -1000, 1000, 0, new ToolTip("Spawn distance between team members left to right"));
            t.Parameters.SpawngapFB.Us = (int)GUISliders.LayoutDisplaySlider("Spawn Gap Forward-Back", t.Parameters.SpawngapFB, -1000, 1000, 0, new ToolTip("Spawn distance between team members front to back"));
            t.Parameters.AltitudeLimits.Lower = GUISliders.LayoutDisplaySlider("Minimum Altitude", t.Parameters.AltitudeLimits.Lower, -500, t.Parameters.AltitudeLimits.Upper, 0, new ToolTip("Add to penalty time when below this"));
            t.Parameters.AltitudeLimits.Upper = GUISliders.LayoutDisplaySlider("Maximum Altitude", t.Parameters.AltitudeLimits.Upper, t.Parameters.AltitudeLimits.Lower, 3000, 0, new ToolTip("Add to penalty time when above this"));
            t.Parameters.DistanceLimit.Us = (int)GUISliders.LayoutDisplaySlider("Maximum Distance", t.Parameters.DistanceLimit.Us, 0f, 10000, 0, new ToolTip("Max distance from nearest enemy before penalty time is added"));
            t.Parameters.ProjectedDistance.Us = GUILayout.Toggle(t.Parameters.ProjectedDistance, "Use on Ground projected Distance.");
            GUILayout.Label("When turned on, the distance for fleeing will be calculated using a top-down view.");
            t.Parameters.MaximumPenaltyTime.Us = (int)GUISliders.LayoutDisplaySlider("Penalty Time", t.Parameters.MaximumPenaltyTime, 0, 3600, 0, new ToolTip("Max penalty time (seconds)"));
            if (t.Parameters.SoftLimits.Us = GUILayout.Toggle(t.Parameters.SoftLimits, "Soft Limits"))
            {
                GUILayout.Label("Soft Limits are active: Entries will <b>not</b> pick up DQ-Time if they do move towards the limits with a certain speed or don't surpass the permitted fleeing speed.");
                t.Parameters.MaximumBufferTime.Us = (int)GUISliders.LayoutDisplaySlider("Out Of Bounds Buffer", t.Parameters.MaximumBufferTime, 0, 360, enumMinMax.none, new ToolTip("The Buffer time for being out of bounds. This buffer will reset, once an entry is back inside."));
                t.Parameters.DistanceReverse.Us = (int)GUISliders.LayoutDisplaySlider("Distance Reverse", t.Parameters.DistanceReverse, -300, 300, enumMinMax.none, new ToolTip("A positive Value allows this many m/s to flee, while a negative value requires you to move this many m/s towards the nearest target."));
                t.Parameters.AltitudeReverse.Us = (int)GUISliders.LayoutDisplaySlider("Altitude Reverse", t.Parameters.AltitudeReverse, -300, 300, enumMinMax.none, new ToolTip("A positive Value allows this many m/s to drift away from the set altitude limits, while negative value rquires you to move this many m/s towards the altitude limits."));
            }
            else {
                GUILayout.Label("Hard Limits are active: Entries will pick up DQ-Time, for as long as they are out of bounds.");
            }
            t.Parameters.MaximumTime.Us = (int)GUISliders.LayoutDisplaySlider("Match Time", t.Parameters.MaximumTime, 0, 3600, 0, new ToolTip("Max match time (seconds)"));
            t.Parameters.Overtime.Us = (int)GUISliders.LayoutDisplaySlider("Overtime", t.Parameters.Overtime, 0, t.Parameters.MaximumTime, enumMinMax.min, new ToolTip("Length of one Overtime-section (seconds)"));

            t.Parameters.LocalResources.Us = GUILayout.Toggle(t.Parameters.LocalResources, "Use local Resources");
            if (t.Parameters.SameMaterials.Us = GUILayout.Toggle(t.Parameters.SameMaterials, "Same Materials for both teams"))
            {
                if (!(t.Parameters.InfinteResourcesTeam1.Us = t.Parameters.InfinteResourcesTeam2.Us = t.Parameters.InfinteResourcesTeam3.Us = GUILayout.Toggle(t.Parameters.InfinteResourcesTeam1, "Infinte Resources")))
                {
                    t.Parameters.ResourcesTeam1.Us = (int)GUISliders.LayoutDisplaySlider("Starting Materials", t.Parameters.ResourcesTeam1, 0, 1000000, 0, new ToolTip(t.Parameters.LocalResources ? "Amount of material on each constructable (localised)" : "Amount of material per team (centralised)"));
                    t.Parameters.ResourcesTeam3.Us=t.Parameters.ResourcesTeam2.Us = t.Parameters.ResourcesTeam1.Us;
                }
            }
            else {
                if (!(t.Parameters.InfinteResourcesTeam1.Us = GUILayout.Toggle(t.Parameters.InfinteResourcesTeam1, "Infinite Resources for Team 1")))
                {
                    t.Parameters.ResourcesTeam1.Us = (int)GUISliders.LayoutDisplaySlider("Starting Materials Team 1", t.Parameters.ResourcesTeam1, 0, 1000000, enumMinMax.none, new ToolTip(t.Parameters.LocalResources ? "Amount of material on each constructable of team 1 (localised)" : "Amount of material of team 1 (centralised)"));
                }
                if (!(t.Parameters.InfinteResourcesTeam2.Us = GUILayout.Toggle(t.Parameters.InfinteResourcesTeam2, "Infinite Resources for Team 2")))
                {
                    t.Parameters.ResourcesTeam2.Us = (int)GUISliders.LayoutDisplaySlider("Starting Materials Team 2", t.Parameters.ResourcesTeam2, 0, 1000000, enumMinMax.none, new ToolTip(t.Parameters.LocalResources ? "Amount of material on each constructable of team 2 (localised)" : "Amount of material of team 2 (centralised)"));
                }
                if (t.Parameters.ActiveFactions >= 3&& !(t.Parameters.InfinteResourcesTeam3.Us = GUILayout.Toggle(t.Parameters.InfinteResourcesTeam2, "Infinite Resources for Team 3"))) {
                    t.Parameters.ResourcesTeam3.Us = (int)GUISliders.LayoutDisplaySlider("Starting Materials Team 3", t.Parameters.ResourcesTeam3, 0, 1000000, enumMinMax.none, new ToolTip(t.Parameters.LocalResources ? "Amount of material on each constructable of team 3 (localised)" : "Amount of material of team 3 (centralised)"));
                }
            }
			//Fortgeschrittene Optionen
            if (t.Parameters.ShowAdvancedOptions.Us = GUILayout.Toggle(t.Parameters.ShowAdvancedOptions,"Show Advanced Battle Options"))
            {
                GUILayout.Label("Usually you don't need to modify these, but if you need to customise the battles further it can be done here.");
                t.Parameters.ActiveFactions.Us = (int)GUISliders.LayoutDisplaySlider("Active Teams", t.Parameters.ActiveFactions, 2, 3, enumMinMax.none, new ToolTip("Controls how many Factions are active for the fight. Their settings are grouped with the first two teams."));
				t.Parameters.Team1FormationIndex.Us = (int)GUISliders.LayoutDisplaySlider("Team 1 Formation: " + TournamentFormation.tournamentFormations[t.Parameters.Team1FormationIndex].Name, t.Parameters.Team1FormationIndex, 0, TournamentFormation.tournamentFormations.Length-1, enumMinMax.none, new ToolTip(TournamentFormation.tournamentFormations[t.Parameters.Team1FormationIndex].Description));
				t.Parameters.Team2FormationIndex.Us = (int)GUISliders.LayoutDisplaySlider("Team 2 Formation: " + TournamentFormation.tournamentFormations[t.Parameters.Team2FormationIndex].Name, t.Parameters.Team2FormationIndex, 0, TournamentFormation.tournamentFormations.Length-1, enumMinMax.none, new ToolTip(TournamentFormation.tournamentFormations[t.Parameters.Team2FormationIndex].Description));
                if (t.Parameters.ActiveFactions >= 3) {
                    t.Parameters.Team3FormationIndex.Us= (int)GUISliders.LayoutDisplaySlider("Team 3 Formation: " + TournamentFormation.tournamentFormations[t.Parameters.Team3FormationIndex].Name, t.Parameters.Team3FormationIndex, 0, TournamentFormation.tournamentFormations.Length - 1, enumMinMax.none, new ToolTip(TournamentFormation.tournamentFormations[t.Parameters.Team3FormationIndex].Description));
                }
				t.Parameters.MaterialConversion.Us = (int)GUISliders.LayoutDisplaySlider("Materialconversion", t.Parameters.MaterialConversion, -1, 100, enumMinMax.none, new ToolTip("Conversionfactor Damage to Materials, also known as Lifesteal."));
                string describeCleanupMode()
                {
                    switch (t.Parameters.CleanUpMode)
                    {
                        case 0:
                            return "Entries will not be auto-removed: They will only be removed by disqualification.";
                        case 1:
                            return "Non-Player Entries will be auto-removed based on recieved damaged and current situation.";
                        case 2:
                            return "All Entries will be auto-removed based on recieved damaged and current situation.";
                        case 3:
                            return "All Entries will be auto-removed based on recieved damaged and current situation. A HeartStone must also be present.";
                        default:
                            return "How did you manage to go out of bounds here?";
                    }
                }
                t.Parameters.CleanUpMode.Us = (int)GUISliders.LayoutDisplaySlider("Constructs-Cleanup: "+(ConstructableCleanUp)t.Parameters.CleanUpMode.Us, t.Parameters.CleanUpMode, 0, 3, enumMinMax.none, new ToolTip(describeCleanupMode()));
                string healthCalculationTip()
                {
                    switch (t.Parameters.HealthCalculation)
                    {
                        case 0:
                            return "Health will be based on the current number of alive blocks. This is the default and should be best for block-count-based Tournaments.";
                        case 1:
                            return "Health will be based on the current resource costs of alive blocks. This should be best for resource-based Tournaments.";
                        case 2:
                            return "Health will be based on the current volume of alive blocks. This should be best for volume-based Tournaments.";
                        case 3:
                            return "Health will be based on the current size of alive blocks. This should be best for Tournaments based on array-elements.";
                        default:
                            return "How did you manage to go out of bounds here?";
                    }
                }
                string describeHealthCalculation() {
                    switch (t.Parameters.HealthCalculation) {
                        case 0:
                            return "Blockcount";
                        case 1:
                            return "Materialcost";
                        case 2:
                            return "Volume";
                        case 3:
                            return "Array-Elements";
                        default:
                            return "How did you manage to go out of bounds here?";
                    }
                }
                t.Parameters.HealthCalculation.Us = (int)GUISliders.LayoutDisplaySlider("Healthcalculation: "+describeHealthCalculation(), t.Parameters.HealthCalculation, 0, 3, enumMinMax.none, new ToolTip(healthCalculationTip()));
                t.Parameters.MinimumHealth.Us = (int)GUISliders.LayoutDisplaySlider("Minimum Health", t.Parameters.MinimumHealth, 0, 100, enumMinMax.none, new ToolTip("Add to penalty time when below this."));
            }
            else {
                t.Parameters.MaterialConversion.Reset();
                t.Parameters.CleanUpMode.Reset();
                t.Parameters.HealthCalculation.Reset();
                t.Parameters.MinimumHealth.Reset();
                t.Parameters.Team1FormationIndex.Reset();
                t.Parameters.Team2FormationIndex.Reset();
                t.Parameters.ActiveFactions.Reset();
            }
            if (showEyecandy = GUILayout.Toggle(showEyecandy, "Show Eyecandy"))
            {
                GUILayout.Label("If you are bored of the same fleet colors for every match, you can change them here. <b>But there are no color selectors only predefined packages!</b>");
                team1NewIndex = (int)GUISliders.LayoutDisplaySlider("Team 1: "+TournamentFleetColor.colorSchemes[team1CurrentIndex].Name, team1CurrentIndex, 0, TournamentFleetColor.colorSchemes.Length-1, enumMinMax.none, new ToolTip(TournamentFleetColor.colorSchemes[team1CurrentIndex].Description));
                team1CurrentIndex = MakeCollisionFree(team1CurrentIndex,team1NewIndex,team2CurrentIndex,team3CurrentIndex);
                team2NewIndex = (int)GUISliders.LayoutDisplaySlider("Team 2: "+TournamentFleetColor.colorSchemes[team2CurrentIndex].Name, team2CurrentIndex, 0, TournamentFleetColor.colorSchemes.Length-1, enumMinMax.none, new ToolTip(TournamentFleetColor.colorSchemes[team2CurrentIndex].Description));
                team2CurrentIndex = MakeCollisionFree(team2CurrentIndex,team2NewIndex,team1CurrentIndex,team3CurrentIndex);
                if (t.Parameters.ActiveFactions >= 3)
                {
                    if (team3CurrentIndex == -1)
                    {
                        team3CurrentIndex = MakeCollisionFree(team3CurrentIndex, 2, team1CurrentIndex, team2CurrentIndex);
                    }
                    team3NewIndex = (int)GUISliders.LayoutDisplaySlider("Team 3: " + TournamentFleetColor.colorSchemes[team3CurrentIndex].Name, team3CurrentIndex, 0, TournamentFleetColor.colorSchemes.Length - 1, enumMinMax.none, new ToolTip(TournamentFleetColor.colorSchemes[team3CurrentIndex].Description));
                    team3CurrentIndex = MakeCollisionFree(team3CurrentIndex, team3NewIndex, team1CurrentIndex, team2CurrentIndex);
                }
                else {
                    team3CurrentIndex = team3NewIndex = -1;
                }
            }
            else
            {
                team1CurrentIndex = team1NewIndex = 0;
                team2CurrentIndex = team2NewIndex = 1;
                team3CurrentIndex = team3NewIndex = (t.Parameters.ActiveFactions>=3)?2:-1;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(340f, 430f, 600f, 150f), "Battle Location", GUI.skin.window);
            optpos2 = GUILayout.BeginScrollView(optpos2);

            GUISliders.TotalWidthOfWindow = 600;
            GUISliders.TextWidth = 240;
            GUISliders.DecimalPlaces = 0;
            GUISliders.UpperMargin = 25;

            t.Parameters.EastWestBoard.Us = (int)GUISliders.LayoutDisplaySlider("Map Tile East-West", t.Parameters.EastWestBoard, 0, sectionsEastWest, enumMinMax.none, new ToolTip("The east-west boardindex, it is the first number on the map. 0 is the left side"));
            t.Parameters.NorthSouthBoard.Us = (int)GUISliders.LayoutDisplaySlider("Map Tile North-South", t.Parameters.NorthSouthBoard, 0, sectionsNorthSouth, enumMinMax.none, new ToolTip("The north-south boardindex, it is the second number on the map. 0 is the bottom side."));
            t.MoveCam();
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, 580f, 600f, 200f), "Spawn Settings", GUI.skin.window);
            GUISliders.TotalWidthOfWindow = 600;
            GUISliders.TextWidth = 100;
            GUISliders.DecimalPlaces = 0;
            GUISliders.UpperMargin = 40;
            optpos3 = GUILayout.BeginScrollView(optpos3);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            t.Parameters.Direction.Us = GUISliders.LayoutDisplaySlider("Angle", t.Parameters.Direction, -180, 180, enumMinMax.none, new ToolTip("Direction to face when spawning. 90° is the old right direction, -90° is the old left direction."));
            t.Parameters.SpawnHeight.Us = (int)GUISliders.LayoutDisplaySlider("Height", t.Parameters.SpawnHeight, -500, 3000, enumMinMax.none, new ToolTip("Spawnheight."));
            t.Parameters.Rotation.Us = (int)GUISliders.LayoutDisplaySlider("Rotation", t.Parameters.Rotation, -180/t.Parameters.ActiveFactions, 180/t.Parameters.ActiveFactions, enumMinMax.none, new ToolTip("Rotation angle of the entire battlefield around the origin point."));
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (_treeSelector.CurrentData != null)
            {
                if (GUILayout.Button(new GUIContent("Add to Team 1","Add the currently selected Blueprint to the Team 1-Faction.")))
                {
                    GUISoundManager.GetSingleton().PlayBeep();
                    TournamentEntry tournamentEntry = new TournamentEntry
                    {
                        FactionIndex = 0,
                        Spawn_direction = t.Parameters.Direction,
                        Spawn_height = t.Parameters.SpawnHeight,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entries_t1.Add(tournamentEntry);
                }
                if (GUILayout.Button(new GUIContent("Add to Team 2", "Add the currently selected Blueprint to the Team 2-Faction.")))
                {
                    GUISoundManager.GetSingleton().PlayBeep();
                    TournamentEntry tournamentEntry2 = new TournamentEntry
                    {
                        FactionIndex = 1,
                        Spawn_direction = t.Parameters.Direction,
                        Spawn_height = t.Parameters.SpawnHeight,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entries_t2.Add(tournamentEntry2);
                }
                if (t.Parameters.ActiveFactions >= 3 && GUILayout.Button(new GUIContent("Add to Team 3", "Add the currently selected Blueprint to the Team 3-Faction."))) {
                    GUISoundManager.GetSingleton().PlayBeep();
                    TournamentEntry tournamentEntry3 = new TournamentEntry
                    {
                        FactionIndex = 2,
                        Spawn_direction = t.Parameters.Direction,
                        Spawn_height = t.Parameters.SpawnHeight,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entries_t3.Add(tournamentEntry3);
                }
                if (GUILayout.Button(new GUIContent("Add to all Teams", "Add the currently selected Blueprint to all active Factions."))) {
                    GUISoundManager.GetSingleton().PlayBeep();
                    TournamentEntry tournamentEntry = new TournamentEntry
                    {
                        FactionIndex = 0,
                        Spawn_direction = t.Parameters.Direction,
                        Spawn_height = t.Parameters.SpawnHeight,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entries_t1.Add(tournamentEntry);
                    tournamentEntry = new TournamentEntry
                    {
                        FactionIndex = 1,
                        Spawn_direction = t.Parameters.Direction,
                        Spawn_height = t.Parameters.SpawnHeight,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entries_t2.Add(tournamentEntry);
                    if (t.Parameters.ActiveFactions >= 3) {
                        tournamentEntry = new TournamentEntry
                        {
                            FactionIndex = 2,
                            Spawn_direction = t.Parameters.Direction,
                            Spawn_height = t.Parameters.SpawnHeight,
                            Bpf = _treeSelector.CurrentData
                        };
                        t.entries_t3.Add(tournamentEntry);
                    }
                }
            }
            if (GUILayout.Button(new GUIContent("Rotate Teams", "Each Entry Rotates through the Teams, for a quick rematch.")))
            {
                List<TournamentEntry> temp = new List<TournamentEntry>();
                temp.AddRange(t.entries_t1);
                switch (t.Parameters.ActiveFactions) {
                    case 2:
                        GUISoundManager.GetSingleton().PlayBeep();
                        t.entries_t1.Clear();
                        t.entries_t1.AddRange(t.entries_t2);
                        t.entries_t2.Clear();
                        t.entries_t2.AddRange(temp);
                        break;
                    case 3:
                        GUISoundManager.GetSingleton().PlayBeep();
                        t.entries_t1.Clear();
                        t.entries_t1.AddRange(t.entries_t2);
                        t.entries_t2.Clear();
                        t.entries_t2.AddRange(t.entries_t3);
                        t.entries_t3.Clear();
                        t.entries_t3.AddRange(temp);
                        break;
                    default:
                        GUISoundManager.GetSingleton().PlayFailure();
                        break;
                }
                temp.Clear();
                temp = null;
                t.entries_t1.ForEach((te) => { te.FactionIndex = 0; });
                t.entries_t2.ForEach((te) => { te.FactionIndex = 1; });
                t.entries_t3.ForEach((te) => { te.FactionIndex = 2; });
            }
            if (GUILayout.Button(new GUIContent("Swap Orientations", "Each Entry swaps its orientation."))) {
                GUISoundManager.GetSingleton().PlayBeep();
                t.entries_t1.ForEach((te) => { te.Spawn_direction = (te.Spawn_direction + 180) % 360; });
                t.entries_t2.ForEach((te) => { te.Spawn_direction = (te.Spawn_direction + 180) % 360; });
                t.entries_t3.ForEach((te) => { te.Spawn_direction = (te.Spawn_direction + 180) % 360; });
            }
            if (GUILayout.Button(new GUIContent("Swap Teams\nand orientations", "Each Entry rotates through the Teams and inverts its orientation, for a quick rematch in an asymmetric enviroment.")))
            {
                List<TournamentEntry> temp = new List<TournamentEntry>();
                temp.AddRange(t.entries_t1);
                switch (t.Parameters.ActiveFactions)
                {
                    case 2:
                        GUISoundManager.GetSingleton().PlayBeep();
                        t.entries_t1.Clear();
                        t.entries_t1.AddRange(t.entries_t2);
                        t.entries_t2.Clear();
                        t.entries_t2.AddRange(temp);
                        break;
                    case 3:
                        GUISoundManager.GetSingleton().PlayBeep();
                        t.entries_t1.Clear();
                        t.entries_t1.AddRange(t.entries_t2);
                        t.entries_t2.Clear();
                        t.entries_t2.AddRange(t.entries_t3);
                        t.entries_t3.Clear();
                        t.entries_t3.AddRange(temp);
                        break;
                    default:
                        GUISoundManager.GetSingleton().PlayFailure();
                        break;
                }
                temp.Clear();
                temp = null;
                t.entries_t1.ForEach((te) => {
                    te.FactionIndex = 0;
                    te.Spawn_direction = te.Spawn_direction = (te.Spawn_direction + 180) % 360;
                });
                t.entries_t2.ForEach((te) => {
                    te.FactionIndex = 1;
                    te.Spawn_direction = te.Spawn_direction = (te.Spawn_direction + 180) % 360;
                });
                t.entries_t3.ForEach((te) => {
                    te.FactionIndex = 2;
                    te.Spawn_direction = te.Spawn_direction = (te.Spawn_direction + 180) % 360;
                });
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(600f, 580f, 340f, 200f), "Mod Settings", GUI.skin.window);
            GUILayout.BeginVertical();
            //new Rect(80f, 50f, 200f, 50f), 
            if (GUILayout.Button("Save Settings"))
            {
                GUISoundManager.GetSingleton().PlayBeep();
                t.SaveSettingsNew();
            }
            //new Rect(140f, 300f, 200f, 50f),
            if (GUILayout.Button("Restore Defaults"))
            {
                GUISoundManager.GetSingleton().PlayBeep();
                t.LoadDefaults();
            }
            t.Parameters.DefaultKeys.Us = GUILayout.Toggle(t.Parameters.DefaultKeys, "Use Default Keybinds");
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(940f, 0f, 340f, 580f), "Selected", GUI.skin.window);
            listpos = GUILayout.BeginScrollView(listpos);
            GUILayout.Box("<color=#ffa500ff>~---------T1---------~</color>");
            for (int i=0;i<t.entries_t1.Count;i++)
            {
                TournamentEntry item = t.entries_t1[i];
                string text = "";
                string[] labelCost = item.LabelCost;
                foreach (string str in labelCost)
                {
                    text = text + "\n" + str;
                }
                GUILayout.Box(string.Format("<color=#ffa500ff>{3}°@{2}m\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#ffa500ff>~--------------------~</color>", item.Bpf.Name, item.bp.CalculateResourceCost(false, true, false).Material, item.Spawn_height, item.Spawn_direction, text));
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                if (GUILayout.Button("^ Remove ^"))
                {
                    t.entries_t1.Remove(item);
                }
                if (GUILayout.Button("^ Update ^"))
                {
                    item.Spawn_height = t.Parameters.SpawnHeight;
                    item.Spawn_direction = t.Parameters.Direction;
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (i!=0&&GUILayout.Button("^ Move up ^")) {
                    t.entries_t1.RemoveAt(i);
                    t.entries_t1.Insert(i - 1, item);
                }
                if (i + 1 != t.entries_t1.Count && GUILayout.Button("^ Move down ^")) {
                    t.entries_t1.RemoveAt(i);
                    t.entries_t1.Insert(i + 1, item);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                }
            GUILayout.Box("<color=#ff0000ff>~---------T2---------~</color>");
            for (int i=0;i<t.entries_t2.Count;i++)
            {
                TournamentEntry item = t.entries_t2[i];
                string text2 = "";
                string[] labelCost2 = item.LabelCost;
                foreach (string str2 in labelCost2)
                {
                    text2 = text2 + "\n" + str2;
                }
                GUILayout.Box(string.Format("<color=#ff0000ff>{3}°@{2}m\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#ff0000ff>~--------------------~</color>", item.Bpf.Name, item.bp.CalculateResourceCost(false, true, false).Material, item.Spawn_height, item.Spawn_direction, text2));
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                if (GUILayout.Button("^ Remove ^"))
                {
                    t.entries_t2.Remove(item);
                }
                if (GUILayout.Button("^ Update ^"))
                {
                    item.Spawn_height = t.Parameters.SpawnHeight;
                    item.Spawn_direction = t.Parameters.Direction;
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                if (i != 0 && GUILayout.Button("^ Move up ^"))
                {
                    t.entries_t2.RemoveAt(i);
                    t.entries_t2.Insert(i - 1, item);
                }
                if (i + 1 != t.entries_t2.Count && GUILayout.Button("^ Move down ^"))
                {
                    t.entries_t2.RemoveAt(i);
                    t.entries_t2.Insert(i + 1, item);
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            GUILayout.Box("<color=#0000ffff>~---------T3---------~</color>");
            if (t.Parameters.ActiveFactions >= 3)
            {
                for (int i = 0; i < t.entries_t3.Count; i++)
                {
                    TournamentEntry item = t.entries_t3[i];
                    string text2 = "";
                    string[] labelCost2 = item.LabelCost;
                    foreach (string str2 in labelCost2)
                    {
                        text2 = text2 + "\n" + str2;
                    }
                    GUILayout.Box(string.Format("<color=#0000ffff>{3}°@{2}m\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#0000ffff>~--------------------~</color>", item.Bpf.Name, item.bp.CalculateResourceCost(false, true, false).Material, item.Spawn_height, item.Spawn_direction, text2));
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    if (GUILayout.Button("^ Remove ^"))
                    {
                        t.entries_t3.Remove(item);
                    }
                    if (GUILayout.Button("^ Update ^"))
                    {
                        item.Spawn_height = t.Parameters.SpawnHeight;
                        item.Spawn_direction = t.Parameters.Direction;
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    if (i != 0 && GUILayout.Button("^ Move up ^"))
                    {
                        t.entries_t3.RemoveAt(i);
                        t.entries_t3.Insert(i - 1, item);
                    }
                    if (i + 1 != t.entries_t3.Count && GUILayout.Button("^ Move down ^"))
                    {
                        t.entries_t3.RemoveAt(i);
                        t.entries_t3.Insert(i + 1, item);
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            else {
                GUILayout.Label($"There are currently {t.entries_t3.Count} entries for Team 3, which are currently hidden away.");
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            if (GUI.Button(new Rect(970f, 660f, 280f, 50f), "Start") && t.entries_t1.Count > 0 && t.entries_t2.Count > 0)
            {
                DeactivateGui(0);
                TournamentPlugin.factionTeam1.OverrideFleetColors(TournamentFleetColor.colorSchemes[team1CurrentIndex].Colors);
                TournamentPlugin.factionTeam2.OverrideFleetColors(TournamentFleetColor.colorSchemes[team2CurrentIndex].Colors);
				t.formationTeam1 = TournamentFormation.tournamentFormations[t.Parameters.Team1FormationIndex];
				t.formationTeam2 = TournamentFormation.tournamentFormations[t.Parameters.Team2FormationIndex];
                if (t.Parameters.ActiveFactions >= 3) {
                    TournamentPlugin.factionTeam3.OverrideFleetColors(TournamentFleetColor.colorSchemes[team3CurrentIndex].Colors);
                    t.formationTeam3 = TournamentFormation.tournamentFormations[t.Parameters.Team3FormationIndex];
                }
                t.LoadCraft();
                t.StartMatch();
            }
        }
        private int MakeCollisionFree(int currentIndex, int newIndex, params int[] otherCurrentIndices) {
            bool ascending = newIndex > currentIndex;
            while (Array.Exists(otherCurrentIndices,(current)=>current==newIndex)) {
                if (ascending)
                {
                    newIndex = (newIndex + 1) % TournamentFleetColor.colorSchemes.Length;
                }
                else {
                    newIndex = (newIndex - 1 + TournamentFleetColor.colorSchemes.Length) % TournamentFleetColor.colorSchemes.Length;
                }
            }
            return newIndex;
        }
    }
}
