using Assets.Scripts.Gui;
using Assets.Scripts.Persistence;
using BrilliantSkies.FromTheDepths.Game;
using BrilliantSkies.Ui.Layouts;
using BrilliantSkies.Core.UiSounds;
using BrilliantSkies.Ui.Tips;
using UnityEngine;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.Core.Timing;

namespace Tournament
{
	public class TournamentGUI : BrilliantSkies.FromTheDepths.Game.UserInterfaces.InteractiveOverlay.InteractiveOverlayGui
    {
		public Vector2 listpos;

		public Vector2 optpos, optpos2;

        private BrilliantSkies.Ui.TreeSelection.TreeSelectorGuiElement<BlueprintFile, BlueprintFolder> _treeSelector;

        public BrilliantSkies.ScriptableObjects.SO_LoadVehicleGUI _Style;

        private int sectionsNorthSouth, sectionsEastWest;

        private Tournament t;

		public TournamentGUI(Tournament tourny)
		{
			_Style = LazyLoader.LoadVehicle.Get();
            t = tourny;
		}

		public unsafe override void SetGuiSettings()
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
            if(t.eastWestBoard > sectionsEastWest)
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

		public unsafe override void OnGui()
		{
            GUILayout.BeginArea(new Rect(0f, 0f, 340f, 580f), "Select Contestants", GUI.skin.window);
            _treeSelector.OnGui(new Rect(30f, 35f, 280f, 520f));
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(340f, 0f, 600f, 400f), "Tournament Settings", GUI.skin.window);
			optpos = GUILayout.BeginScrollView(optpos, new GUILayoutOption[0]);
            
			GUISliders.TotalWidthOfWindow = 580;
			GUISliders.TextWidth= 240;
			GUISliders.DecimalPlaces = 0;
			GUISliders.UpperMargin = 0;

            t.spawndis = GUISliders.LayoutDisplaySlider("Spawn Distance", t.spawndis, 100f, 5000f, 0, new ToolTip("Spawn distance between teams"));
			t.spawngap = GUISliders.LayoutDisplaySlider("Spawn Gap Horizontal", t.spawngap, 0f, 500f, 0, new ToolTip("Spawn distance between team members left to right"));
            t.spawngap2 = GUISliders.LayoutDisplaySlider("Spawn Gap Forward-Back", t.spawngap2, 0f, 1000f, 0, new ToolTip("Spawn distance between team members front to back"));
            t.minalt = GUISliders.LayoutDisplaySlider("Min Alt", t.minalt, -500f, t.maxalt, 0, new ToolTip("Add to penalty time when below this"));
			t.maxalt = GUISliders.LayoutDisplaySlider("Max Alt", t.maxalt, t.minalt, 2000f, 0, new ToolTip("Add to penalty time when above this"));
			t.maxdis = GUISliders.LayoutDisplaySlider("Max Dis", t.maxdis, 0f, 10000f, 0, new ToolTip("Max distance from nearest enemy before penalty time added"));
			t.maxoob = GUISliders.LayoutDisplaySlider("Penalty Time", t.maxoob, 0f, 10000f, 0, new ToolTip("Max penalty time (seconds)"));
			t.maxtime = GUISliders.LayoutDisplaySlider("Match Time", t.maxtime, 0f, 10000f, 0, new ToolTip("Max match time (seconds)"));
			t.maxmat = GUISliders.LayoutDisplaySlider("Starting Material", t.maxmat, 0f, 100000f, 0, new ToolTip("Amount of material per team (centralised)"));
            GUILayout.EndScrollView();
			GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(340f, 400f, 600f, 180f), "Battle Location", GUI.skin.window);
            optpos2 = GUILayout.BeginScrollView(optpos2, new GUILayoutOption[0]);

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
			GUISliders.TotalWidthOfWindow = 400;
			GUISliders.TextWidth = 100;
			GUISliders.DecimalPlaces = 0;
			GUISliders.UpperMargin = 40;
			t.Dir = (Tournament.SPAWN.DIR)GUISliders.LayoutDisplaySlider(t.Dir.ToString(), (float)t.Dir, 0f, 3f, 0, new ToolTip("Direction"));
			t.Loc = (Tournament.SPAWN.LOC)GUISliders.LayoutDisplaySlider(t.Loc.ToString(), (float)t.Loc, 0f, 3f, 0, new ToolTip("Location"));
            t.offset = GUISliders.LayoutDisplaySlider("Height Offset", t.offset, -100f, 400f, 0, new ToolTip("Height Offset from location"));
            if (_treeSelector.CurrentData != null)
            {
                if (GUI.Button(new Rect(400f, 40f, 150f, 50f), "Add to Team 1"))
                {
                    GUISoundManager.GetSingleton().PlayBeep();
                    TournamentEntry tournamentEntry = new TournamentEntry
                    {
                        IsKing = true,
                        spawn_direction = t.Dir,
                        spawn_location = t.Loc,
                        offset = t.offset,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entry_t1.Add(tournamentEntry);
                }
                if (GUI.Button(new Rect(400f, 100f, 150f, 50f), "Add to Team 2"))
                {
                    GUISoundManager.GetSingleton().PlayBeep();
                    TournamentEntry tournamentEntry2 = new TournamentEntry
                    {
                        IsKing = false,
                        spawn_direction = t.Dir,
                        spawn_location = t.Loc,
                        offset = t.offset,
                        Bpf = _treeSelector.CurrentData
                    };
                    t.entry_t2.Add(tournamentEntry2);
                }
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(600f, 580f, 340f, 200f), "Mod Settings", GUI.skin.window);
            GUILayout.BeginVertical();
            //new Rect(80f, 50f, 200f, 50f), 
            if (GUILayout.Button("Save Settings"))
            {
                GUISoundManager.GetSingleton().PlayBeep();
                t.saveSettings();
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
			if (t.entry_t1.Count != 0)
			{
				foreach (TournamentEntry item in t.entry_t1)
				{
					string text = "";
					string[] labelCost = item.labelCost;
					foreach (string str in labelCost)
					{
						text = text + "\n" + str;
					}
					GUILayout.Box(string.Format("<color=#ffa500ff>{3} {2}\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#ffa500ff>~--------------------~</color>", item.Bpf.Name, (item.bp.CalculateResourceCost(false, true)).Material, item.spawn_location, item.spawn_direction, text), (GUILayoutOption[])new GUILayoutOption[0]);
					if (GUILayout.Button("^ Remove ^", new GUILayoutOption[0]))
					{
						t.entry_t1.Remove(item);
					}
				}
			}
			GUILayout.Box("<color=#ff0000ff>~---------T2---------~</color>", new GUILayoutOption[0]);
			if (t.entry_t2.Count != 0)
			{
				foreach (TournamentEntry item2 in t.entry_t2)
				{
					string text2 = "";
					string[] labelCost2 = item2.labelCost;
					foreach (string str2 in labelCost2)
					{
						text2 = text2 + "\n" + str2;
					}
					GUILayout.Box(string.Format("<color=#ff0000ff>{3} {2}\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#ffa500ff>~--------------------~</color>", item2.Bpf.Name, (item2.bp.CalculateResourceCost(false, true)).Material, item2.spawn_location, item2.spawn_direction, text2), (GUILayoutOption[])new GUILayoutOption[0]);
					if (GUILayout.Button("^ Remove ^", new GUILayoutOption[0]))
					{
						t.entry_t2.Remove(item2);
					}
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			if (GUI.Button(new Rect(970f, 660f, 280f, 50f), "Start") && t.entry_t1.Count > 0 && t.entry_t2.Count > 0)
			{
				DeactivateGui(0);
                t.LoadCraft();
				t.StartMatch();
			}
		}
	}
}
