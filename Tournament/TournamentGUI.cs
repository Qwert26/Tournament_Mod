using Assets.Scripts.Gui;
using Assets.Scripts.Persistence;
using BrilliantSkies.FromTheDepths.Game;
using BrilliantSkies.Ui.Layouts;
using BrilliantSkies.Core.UiSounds;
using BrilliantSkies.Ui.Tips;
using UnityEngine;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.Core.Timing;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using System.Collections.Generic;
using BrilliantSkies.Core.Pooling;
namespace Tournament
{
    public class TournamentGUI : BrilliantSkies.Ui.Displayer.Types.ThrowAwayObjectGui<Tournament>
    {
        public Vector2 listpos;

        public Vector2 optpos, optpos2, optpos3;

        private bool showEyecandy = false;

        private BrilliantSkies.Ui.TreeSelection.TreeSelectorGuiElement<BlueprintFile, BlueprintFolder> _treeSelector;

        public BrilliantSkies.ScriptableObjects.SO_LoadVehicleGUI _Style;

        private int sectionsNorthSouth, sectionsEastWest;

        private Tournament t;

        private int kingIndexTFC = 0, challengerIndexTFC = 1;
        private int kingIndexNew = 0, challengerIndexNew = 1;

		private int kingIndexTF = 0, challengerIndexTF = 0;

        public TournamentGUI(Tournament tourny)
        {
            _Style = LazyLoader.LoadVehicle.Get();
            t = tourny;
        }

        public override void SetGuiSettings()
        {

            GuiSettings.PausesPlay = false;
            GuiSettings.PausesMultiplayerPlay = false;

        }

        public override void OnActivateGui()
        {
            _Style = LazyLoader.LoadVehicle.Get();
            BlueprintFolder val = GameFolders.GetCombinedBlueprintFolder();
            sectionsNorthSouth = WorldSpecification.i.BoardLayout.NorthSouthBoardSectionCount - 1;
            sectionsEastWest = WorldSpecification.i.BoardLayout.EastWestBoardSectionCount - 1;
            if (t.eastWestBoard > sectionsEastWest)
            {
                t.eastWestBoard = t.eastWestBoardD;
            }
            if (t.northSouthBoard > sectionsNorthSouth)
            {
                t.northSouthBoard = t.northSouthBoardD;
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

            GUISliders.TotalWidthOfWindow = 580;
            GUISliders.TextWidth = 240;
            GUISliders.DecimalPlaces = 0;
            GUISliders.UpperMargin = 0;

            t.spawndis = GUISliders.LayoutDisplaySlider("Spawn Distance", t.spawndis, 100, 5000, 0, new ToolTip("Spawn distance between teams"));
            t.spawngap = GUISliders.LayoutDisplaySlider("Spawn Gap Left-Right", t.spawngap, -1000, 1000, 0, new ToolTip("Spawn distance between team members left to right"));
            t.spawngap2 = GUISliders.LayoutDisplaySlider("Spawn Gap Forward-Back", t.spawngap2, -1000, 1000, 0, new ToolTip("Spawn distance between team members front to back"));
            t.minalt = GUISliders.LayoutDisplaySlider("Minimum Altitude", t.minalt, -500, t.maxalt, 0, new ToolTip("Add to penalty time when below this"));
            t.maxalt = GUISliders.LayoutDisplaySlider("Maximum Altitude", t.maxalt, t.minalt, 2000, 0, new ToolTip("Add to penalty time when above this"));
            t.maxdis = GUISliders.LayoutDisplaySlider("Maximum Distance", t.maxdis, 0f, 10000, 0, new ToolTip("Max distance from nearest enemy before penalty time is added"));
            t.project2D = GUILayout.Toggle(t.project2D, "Use on Ground projected Distance.");
            t.maxoob = GUISliders.LayoutDisplaySlider("Penalty Time", t.maxoob, 0, 10000, 0, new ToolTip("Max penalty time (seconds)"));
            t.oobMaxBuffer = GUISliders.LayoutDisplaySlider("Out Of Bounds Buffer", t.oobMaxBuffer, 0, 100, enumMinMax.none, new ToolTip("The Buffer time for being out of bounds"));
            t.oobReverse = GUISliders.LayoutDisplaySlider("Out OfBounds Reverse", t.oobReverse, -300, 300, enumMinMax.none, new ToolTip("A positive Value allows this many m/s to flee, while a negative value requires you to move this many m/s towards the nearest target."));
            t.maxtime = GUISliders.LayoutDisplaySlider("Match Time", t.maxtime, 0, 10000, 0, new ToolTip("Max match time (seconds)"));
            t.localResources = GUILayout.Toggle(t.localResources, "Use local Resorces");
            if (t.sameMaterials = GUILayout.Toggle(t.sameMaterials, "Same Materials for both teams"))
            {
                if (!(t.infinteResourcesT1 = t.infinteResourcesT2 = GUILayout.Toggle(t.infinteResourcesT1, "Infinte Resources")))
                {
                    t.maxmat = GUISliders.LayoutDisplaySlider("Starting Materials", t.maxmat, 0, 1000000, 0, new ToolTip(t.localResources ? "Amount of material on each constructable (localised)" : "Amount of material per team (centralised)"));
                    t.t1_res = t.t2_res = t.maxmat;
                }
            }
            else {
                if (!(t.infinteResourcesT1 = GUILayout.Toggle(t.infinteResourcesT1, "Infinite Resources for Team 1")))
                {
                    t.t1_res = GUISliders.LayoutDisplaySlider("Starting Materials Team 1", t.t1_res, 0, 1000000, enumMinMax.none, new ToolTip(t.localResources ? "Amount of material on each constructable of team 1 (localised)" : "Amount of material of team 1 (centralised)"));
                }
                if (!(t.infinteResourcesT2 = GUILayout.Toggle(t.infinteResourcesT2, "Infinite Resources for Team 2")))
                {
                    t.t2_res = GUISliders.LayoutDisplaySlider("Starting Materials Team 2", t.t2_res, 0, 1000000, enumMinMax.none, new ToolTip(t.localResources ? "Amount of material on each constructable of team 2 (localised)" : "Amount of material of team 2 (centralised)"));
                }
            }
			//Fortgeschrittene Optionen
            if (t.showAdvancedOptions = GUILayout.Toggle(t.showAdvancedOptions,"Show Advanced Battle Options"))
            {
                GUILayout.Label("Usually you don't need to modify these, but if you need to customise the battles further it can be done here.");
				kingIndexTF = (int)GUISliders.LayoutDisplaySlider("Team 1 Formation: " + TournamentFormation.tournamentFormations[kingIndexTF].Name, kingIndexTF, 0, TournamentFormation.tournamentFormations.Length, enumMinMax.none, new ToolTip(TournamentFormation.tournamentFormations[kingIndexTF].Description));
				challengerIndexTF = (int)GUISliders.LayoutDisplaySlider("Team 2 Formation: " + TournamentFormation.tournamentFormations[challengerIndexTF].Name, challengerIndexTF, 0, TournamentFormation.tournamentFormations.Length, enumMinMax.none, new ToolTip(TournamentFormation.tournamentFormations[challengerIndexTF].Description));
				t.matconv = GUISliders.LayoutDisplaySlider("Materialconversion", t.matconv, -1, 100, enumMinMax.none, new ToolTip("Conversionfactor Damage to Materials, also known as Lifesteal."));
                string describeCleanupMode()
                {
                    switch (t.cleanUp)
                    {
                        case ConstructableCleanUp.Off:
                            return "Entries will not be auto-removed: They will only be removed by disqualification.";
                        case ConstructableCleanUp.Ai:
                            return "Non-Player Entries will be auto-removed based on recieved damaged and current situation.";
                        case ConstructableCleanUp.All:
                            return "All Entries will be auto-removed based on recieved damaged and current situation.";
                        case ConstructableCleanUp.AllHeartStone:
                            return "All Entries will be auto-removed based on recieved damaged and current situation. A HeartStone must also be present.";
                        default:
                            return "How did you manage to go out of bounds here?";
                    }
                }
                t.cleanUp = (ConstructableCleanUp)GUISliders.LayoutDisplaySlider(t.cleanUp.ToString(), (float)t.cleanUp, 0, 3, enumMinMax.none, new ToolTip(describeCleanupMode()));
                string describeHealthCalculation()
                {
                    switch (t.healthCalculation)
                    {
                        case Tournament.HealthCalculation.NumberOfBlocks:
                            return "Health will be based on the current number of alive blocks. This is the default and should be best for block-count-based Tournaments.";
                        case Tournament.HealthCalculation.ResourceCost:
                            return "Health will be based on the current resource costs of alive blocks. This should be best for resource-based Tournaments.";
                        case Tournament.HealthCalculation.Volume:
                            return "Health will be based on the current volume of alive blocks. This should be best for volume-based Tournaments.";
                        case Tournament.HealthCalculation.ArrayElements:
                            return "Health will be based on the current size of alive blocks. This should be best for Tournaments based on array-elements.";
                        default:
                            return "How did you manage to go out of bounds here?";
                    }
                }
                t.healthCalculation = (Tournament.HealthCalculation)GUISliders.LayoutDisplaySlider(t.healthCalculation.ToString(), (float)t.healthCalculation, 0, 3, enumMinMax.none, new ToolTip(describeHealthCalculation()));
                t.minimumHealth = GUISliders.LayoutDisplaySlider("Minimum Health", t.minimumHealth, 0, 100, enumMinMax.none, new ToolTip("Add to penalty time when below this."));
            }
            else {
                t.matconv = t.matconvD;
                t.cleanUp = t.cleanUpD;
                t.healthCalculation = t.healthCalculationD;
                t.minimumHealth = t.minimumHealthD;
				kingIndexTF = challengerIndexTF = 0;
            }
            if (GUILayout.Button("Delete Pools")) {
                Pooler.DeleteAll();
            }
            GUILayout.Label("Usually changing the Poolsize requires a restart, but by deleting the current pools this change can happen while the game is running. " +
                "This might sound radical, but Pools are created dynamicly by the game. <b>You will experience lag, while the new Pool is filled with Objects.</b>");
            if (showEyecandy = GUILayout.Toggle(showEyecandy, "Show Eyecandy"))
            {
                GUILayout.Label("If you are bored of the same fleet colors for every match, you can change them here. <b>But there are no color selectors only predefined packages!</b>");
                kingIndexNew = (int)GUISliders.LayoutDisplaySlider("Team 1: "+TournamentFleetColor.colorSchemes[kingIndexTFC].Name, kingIndexTFC, 0, TournamentFleetColor.colorSchemes.Length-1, enumMinMax.none, new ToolTip(TournamentFleetColor.colorSchemes[kingIndexTFC].Description));
                if (kingIndexNew == challengerIndexTFC)
                {
                    if (kingIndexNew < kingIndexTFC)
                    {
                        kingIndexNew--;
                        if (kingIndexNew < 0)
                        {
                            kingIndexNew = 1;
                        }
                    }
                    else
                    {
                        kingIndexNew++;
                        if (kingIndexNew >= TournamentFleetColor.colorSchemes.Length)
                        {
                            kingIndexNew = TournamentFleetColor.colorSchemes.Length - 2;
                        }
                    }
                }
                kingIndexTFC = kingIndexNew;
                challengerIndexNew = (int)GUISliders.LayoutDisplaySlider("Team 2: "+TournamentFleetColor.colorSchemes[challengerIndexTFC].Name, challengerIndexTFC, 0, TournamentFleetColor.colorSchemes.Length-1, enumMinMax.none, new ToolTip(TournamentFleetColor.colorSchemes[challengerIndexTFC].Description));
                if (challengerIndexNew == kingIndexTFC)
                {
                    if (challengerIndexNew < challengerIndexTFC)
                    {
                        challengerIndexNew--;
                        if (challengerIndexNew < 0)
                        {
                            challengerIndexNew = 1;
                        }
                    }
                    else
                    {
                        challengerIndexNew++;
                        if (challengerIndexNew >= TournamentFleetColor.colorSchemes.Length)
                        {
                            challengerIndexNew = TournamentFleetColor.colorSchemes.Length - 2;
                        }
                    }
                }
                challengerIndexTFC = challengerIndexNew;
            }
            else
            {
                kingIndexTFC = kingIndexNew = 0;
                challengerIndexTFC = challengerIndexNew = 1;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(340f, 430f, 600f, 150f), "Battle Location", GUI.skin.window);
            optpos2 = GUILayout.BeginScrollView(optpos2);

            GUISliders.TotalWidthOfWindow = 580;
            GUISliders.TextWidth = 240;
            GUISliders.DecimalPlaces = 0;
            GUISliders.UpperMargin = 25;

            t.eastWestBoard = (int)GUISliders.LayoutDisplaySlider("Map Tile East-West", t.eastWestBoard, 0, sectionsEastWest, enumMinMax.none, new ToolTip("The east-west boardindex, it is the first number on the map. 0 is the left side"));
            t.northSouthBoard = (int)GUISliders.LayoutDisplaySlider("Map Tile North-South", t.northSouthBoard, 0, sectionsNorthSouth, enumMinMax.none, new ToolTip("The north-south boardindex, it is the second number on the map. 0 is the bottom side."));
            t.MoveCam();
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, 580f, 600f, 200f), "Spawn Settings", GUI.skin.window);
            GUISliders.TotalWidthOfWindow = 580;
            GUISliders.TextWidth = 100;
            GUISliders.DecimalPlaces = 0;
            GUISliders.UpperMargin = 40;
            optpos3 = GUILayout.BeginScrollView(optpos3);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            t.Dir = (Tournament.SPAWN.DIR)GUISliders.LayoutDisplaySlider(t.Dir.ToString(), (float)t.Dir, 0f, 3f, 0, new ToolTip("Direction"));
            t.Loc = (Tournament.SPAWN.LOC)GUISliders.LayoutDisplaySlider(t.Loc.ToString(), (float)t.Loc, 0f, 3f, 0, new ToolTip("Location"));
            t.offset = GUISliders.LayoutDisplaySlider("Height Offset", t.offset, -100f, 400f, 0, new ToolTip("Height Offset from location"));
            t.rotation = GUISliders.LayoutDisplaySlider("Rotation", t.rotation, -90, 90, enumMinMax.none, new ToolTip("Rotation angle of the entire battlefield around the origin point."));
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if (_treeSelector.CurrentData != null)
            {
                if (GUILayout.Button(new GUIContent("Add to Team 1","Add the currently selected Blueprint to the King-Faction.")))
                {
                    GUISoundManager.GetSingleton().PlayBeep();
                    TournamentEntry tournamentEntry = new TournamentEntry
                    {
                        IsKing = true,
                        Spawn_direction = t.Dir,
                        Spawn_location = t.Loc,
                        Offset = t.offset,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entries_t1.Add(tournamentEntry);
                }
                if (GUILayout.Button(new GUIContent("Add to Team 2", "Add the currently selected Blueprint to the Challenger-Faction.")))
                {
                    GUISoundManager.GetSingleton().PlayBeep();
                    TournamentEntry tournamentEntry2 = new TournamentEntry
                    {
                        IsKing = false,
                        Spawn_direction = t.Dir,
                        Spawn_location = t.Loc,
                        Offset = t.offset,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entries_t2.Add(tournamentEntry2);
                }
            }
            if (GUILayout.Button(new GUIContent("Swap Teams", "Each Entry swaps Teams, for a quick rematch.")))
            {
                List<TournamentEntry> temp = new List<TournamentEntry>();
                temp.AddRange(t.entries_t1);
                t.entries_t1.Clear();
                t.entries_t1.AddRange(t.entries_t2);
                t.entries_t2.Clear();
                t.entries_t2.AddRange(temp);
                temp.Clear();
                temp = null;
                t.entries_t1.ForEach((te) => { te.IsKing = true; });
                t.entries_t2.ForEach((te) => { te.IsKing = false; });
            }
            if (GUILayout.Button(new GUIContent("Swap Teams\nand orientation", "Each Entry swaps Teams and inverts its orientation, for a quick rematch in an asymmetric enviroment.")))
            {
                List<TournamentEntry> temp = new List<TournamentEntry>();
                temp.AddRange(t.entries_t1);
                t.entries_t1.Clear();
                t.entries_t1.AddRange(t.entries_t2);
                t.entries_t2.Clear();
                t.entries_t2.AddRange(temp);
                temp.Clear();
                temp = null;
                t.entries_t1.ForEach((te) => {
                    te.IsKing = true;
                    te.Spawn_direction = ((int)te.Spawn_direction % 2 == 0) ? te.Spawn_direction + 1 : te.Spawn_direction - 1;
                });
                t.entries_t2.ForEach((te) => {
                    te.IsKing = false;
                    te.Spawn_direction = ((int)te.Spawn_direction % 2 == 0) ? te.Spawn_direction + 1 : te.Spawn_direction - 1;
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
                t.SaveSettings();
            }
            //new Rect(140f, 300f, 200f, 50f),
            if (GUILayout.Button("Restore Defaults"))
            {
                GUISoundManager.GetSingleton().PlayBeep();
                t.LoadDefaults();
            }
            t.defaultKeysBool = GUILayout.Toggle(t.defaultKeysBool, "Use Default Keybinds");
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(940f, 0f, 340f, 580f), "Selected", GUI.skin.window);
            listpos = GUILayout.BeginScrollView(listpos, new GUILayoutOption[0]);
            GUILayout.Box("<color=#ffa500ff>~---------T1---------~</color>", new GUILayoutOption[0]);
            if (t.entries_t1.Count != 0)
            {
                foreach (TournamentEntry item in t.entries_t1)
                {
                    string text = "";
                    string[] labelCost = item.LabelCost;
                    foreach (string str in labelCost)
                    {
                        text = text + "\n" + str;
                    }
                    GUILayout.Box(string.Format("<color=#ffa500ff>{3} {2}@{5}m\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#ffa500ff>~--------------------~</color>", item.Bpf.Name, item.bp.CalculateResourceCost(false, true).Material, item.Spawn_location, item.Spawn_direction, text,item.Offset));
                    if (GUILayout.Button("^ Remove ^", new GUILayoutOption[0]))
                    {
                        t.entries_t1.Remove(item);
                    }
                }
            }
            GUILayout.Box("<color=#ff0000ff>~---------T2---------~</color>", new GUILayoutOption[0]);
            if (t.entries_t2.Count != 0)
            {
                foreach (TournamentEntry item2 in t.entries_t2)
                {
                    string text2 = "";
                    string[] labelCost2 = item2.LabelCost;
                    foreach (string str2 in labelCost2)
                    {
                        text2 = text2 + "\n" + str2;
                    }
                    GUILayout.Box(string.Format("<color=#ff0000ff>{3} {2}@{5}m\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#ff0000ff>~--------------------~</color>", item2.Bpf.Name, item2.bp.CalculateResourceCost(false, true).Material, item2.Spawn_location, item2.Spawn_direction, text2, item2.Offset));
                    if (GUILayout.Button("^ Remove ^", new GUILayoutOption[0]))
                    {
                        t.entries_t2.Remove(item2);
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            if (GUI.Button(new Rect(970f, 660f, 280f, 50f), "Start") && t.entries_t1.Count > 0 && t.entries_t2.Count > 0)
            {
                DeactivateGui(0);
                TournamentPlugin.kingFaction.OverrideFleetColors(TournamentFleetColor.colorSchemes[kingIndexTFC].Colors);
                TournamentPlugin.challengerFaction.OverrideFleetColors(TournamentFleetColor.colorSchemes[challengerIndexTFC].Colors);
				t.kingFormation = TournamentFormation.tournamentFormations[kingIndexTF];
				t.challengerFormation = TournamentFormation.tournamentFormations[challengerIndexTF];
                t.LoadCraft();
                t.StartMatch();
            }
        }
    }
}
