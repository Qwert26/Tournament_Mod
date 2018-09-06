using Assets.Scripts.Gui;
using Assets.Scripts.Persistence;
using BrilliantSkies.FromTheDepths.Game;
using BrilliantSkies.Ui.Layouts;
using BrilliantSkies.Core.UiSounds;
using BrilliantSkies.Ui.Tips;
using System;
using UnityEngine;

namespace Tournament
{
	public class TournamentGUI : BrilliantSkies.FromTheDepths.Game.UserInterfaces.InteractiveOverlay.InteractiveOverlayGui
    {
		public Vector2 listpos;

		public Vector2 optpos;

        private BrilliantSkies.Ui.TreeSelection.TreeSelectorGuiElement<BlueprintFile, BlueprintFolder> _treeSelector;

        public BrilliantSkies.ScriptableObjects.SO_LoadVehicleGUI _Style;

        private Tournament t;

		public TournamentGUI(Tournament tourny)
		{
			_Style = LazyLoader.LoadVehicle.Get();
            t = tourny;
		}

		public unsafe override void SetGuiSettings()
		{
			
			this.GuiSettings.PausesPlay = false;
			this.GuiSettings.PausesMultiplayerPlay = false;
            
		}

		public override void OnActivateGui()
		{
			_Style = LazyLoader.LoadVehicle.Get();
			BlueprintFolder val = GameFolders.GetCombinedBlueprintFolder();
			_treeSelector = FtdGuiUtils.GetFileBrowserFor(val);
			_treeSelector.Refresh();
            t.ResetCam();
		}

		public unsafe override void OnGui()
		{
            GUILayout.BeginArea(new Rect(0f, 0f, 340f, 580f), "Select Contestants", GUI.skin.window);
            _treeSelector.OnGui(new Rect(30f, 35f, 280f, 520f),(Action<BlueprintFile>)UpdateFileData);
            GUILayout.EndArea();
			GUILayout.BeginArea(new Rect(340f, 0f, 600f, 580f), "Tournament Settings", GUI.skin.window);
			optpos = GUILayout.BeginScrollView(optpos, (GUILayoutOption[])new GUILayoutOption[0]);
            
			GUISliders.TotalWidthOfWindow = 580;
			GUISliders.TextWidth= 240;
			GUISliders.DecimalPlaces = 0;
			GUISliders.UpperMargin = 0;

            t.spawndis = GUISliders.DisplaySlider(0, "Spawn Distance", t.spawndis, 100f, 5000f, 0, new ToolTip("Spawn distance between teams"));
			t.spawngap = GUISliders.DisplaySlider(1, "Spawn Gap", t.spawngap, 10f, 500f, 0, new ToolTip("Spawn distance between team members"));
			t.minalt = GUISliders.DisplaySlider(2, "Min Alt", t.minalt, -500f, t.maxalt, 0, new ToolTip("Add to penalty time when below this"));
			t.maxalt = GUISliders.DisplaySlider(3, "Max Alt", t.maxalt, t.minalt, 2000f, 0, new ToolTip("Add to penalty time when above this"));
			t.maxdis = GUISliders.DisplaySlider(4, "Max Dis", t.maxdis, 0f, 10000f, 0, new ToolTip("Max distance from nearest enemy before penalty time added"));
			t.maxoob = GUISliders.DisplaySlider(5, "Penalty Time", t.maxoob, 0f, 10000f, 0, new ToolTip("Max penalty time (seconds)"));
			t.maxtime = GUISliders.DisplaySlider(6, "Match Time", t.maxtime, 0f, 10000f, 0, new ToolTip("Max match time (seconds)"));
			//t.maxcost = GUISliders.DisplaySlider(7, "Max Design Cost", t.maxcost, 0f, 1E+07f, 0, new ToolTip("Max design cost, Currently doesn't effect anything"));
			t.maxmat = GUISliders.DisplaySlider(7, "Starting Material", t.maxmat, 0f, 100000f, 0, new ToolTip("Amount of material per team (centralised)"));
            //t.matconv = GUISliders.DisplaySlider(9, "Dmg to Mat %", t.matconv, -1f, 100f, 0, new ToolTip("Damage to material conversion, -1 disables self/team damage material return"));
            /*t.srules = Convert.ToBoolean(GUISliders.DisplaySlider(10, ((Tournament.OPTIONS.STANDARDRULES)Convert.ToInt32(t.srules)).ToString(), (float)Convert.ToInt32(t.srules), 0f, 1f, 0, new ToolTip("Standard despawn rules, or customise")));
			if (!t.srules)
			{
				t.penaltynoai = Convert.ToBoolean(GUISliders.DisplaySlider(11, ((Tournament.OPTIONS.AIPENALTY)Convert.ToInt32(t.penaltynoai)).ToString(), (float)Convert.ToInt32(t.penaltynoai), 0f, 1f, 0, new ToolTip("Does having no AI left add to penalty time?")));
				t.standardhp = Convert.ToBoolean(GUISliders.DisplaySlider(12, ((Tournament.OPTIONS.HPMODE)Convert.ToInt32(t.standardhp)).ToString(), (float)Convert.ToInt32(t.standardhp), 0f, 1f, 0, new ToolTip("Calculate HP by % of alive blocks or % of alive block costs")));
				t.penaltyhp = GUISliders.DisplaySlider(13, "HP Penalty %", t.penaltyhp, 0f, 100f, 0, new ToolTip("Adds to penalty time when below hp %, 0 disables"));
			}*/
            if (GUI.Button(new Rect(50f, 480f, 200f, 50f), "Save Settings"))
            {
                GUISoundManager.GetSingleton().PlayBeep();
                t.saveSettings();
            }

            if (GUI.Button(new Rect(300f, 480f, 200f, 50f), "Restore Defaults"))
            {
                GUISoundManager.GetSingleton().PlayBeep();
                t.spawndis =  t.spawndisD;
                t.spawngap = t.spawngapD;
                t.minalt = t.minaltD;
                t.maxalt = t.maxaltD;
                t.maxdis = t.maxdisD;
                t.maxoob = t.maxoobD;
                t.maxtime = t.maxtimeD;
                t.maxmat = t.maxmatD;
                t.Dir = t.DirD;
                t.Loc = t.LocD;
                t.offset = t.offsetD;
            }
            GUILayout.EndScrollView();
			GUILayout.EndArea();
			GUILayout.BeginArea(new Rect(0f, 580f, 940f, 200f), "Spawn Settings", GUI.skin.window);
			GUISliders.TotalWidthOfWindow = 600;
			GUISliders.TextWidth = 240;
			GUISliders.DecimalPlaces = 0;
			GUISliders.UpperMargin = 40;
			t.Dir = (Tournament.SPAWN.DIR)checked((int)unchecked(GUISliders.DisplaySlider(0, t.Dir.ToString(), (float)t.Dir, 0f, 3f, 0, new ToolTip("Direction"))));
			t.Loc = (Tournament.SPAWN.LOC)checked((int)unchecked(GUISliders.DisplaySlider(1, t.Loc.ToString(), (float)t.Loc, 0f, 3f, 0, new ToolTip("Location"))));
            t.offset = GUISliders.DisplaySlider(2, "Height Offset", t.offset, -100f, 100f, 0, new ToolTip("Height Offset from location"));
            if (_treeSelector.CurrentData != null)
			{
				if (GUI.Button(new Rect(600f, 25f, 280f, 50f), "Add to Team 1"))
				{
					GUISoundManager.GetSingleton().PlayBeep();
					TournamentEntry tournamentEntry = new TournamentEntry();
					tournamentEntry.IsKing = true;
					tournamentEntry.spawn_direction = t.Dir;
					tournamentEntry.spawn_location = t.Loc;
                    tournamentEntry.offset = t.offset;
                    tournamentEntry.bpf = _treeSelector.CurrentData;
					t.entry_t1.Add(tournamentEntry);
				}
				if (GUI.Button(new Rect(600f, 100f, 280f, 50f), "Add to Team 2"))
				{
					GUISoundManager.GetSingleton().PlayBeep();
					TournamentEntry tournamentEntry2 = new TournamentEntry();
					tournamentEntry2.IsKing = false;
					tournamentEntry2.spawn_direction = t.Dir;
					tournamentEntry2.spawn_location = t.Loc;
                    tournamentEntry2.offset = t.offset;
                    tournamentEntry2.bpf = _treeSelector.CurrentData;
					t.entry_t2.Add(tournamentEntry2);
				}
			}
			GUILayout.EndArea();
			GUILayout.BeginArea(new Rect(940f, 0f, 340f, 580f), "Selected", GUI.skin.window);
			listpos = GUILayout.BeginScrollView(listpos, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Box("<color=#ffa500ff>~---------T1---------~</color>", (GUILayoutOption[])new GUILayoutOption[0]);
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
					GUILayout.Box(string.Format("<color=#ffa500ff>{3} {2}\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#ffa500ff>~--------------------~</color>", item.bpf.Name, (item.bp.CalculateResourceCost(false, true)).Material, item.spawn_location, item.spawn_direction, text), (GUILayoutOption[])new GUILayoutOption[0]);
					if (GUILayout.Button("^ Remove ^", (GUILayoutOption[])new GUILayoutOption[0]))
					{
						t.entry_t1.Remove(item);
					}
				}
			}
			GUILayout.Box("<color=#ff0000ff>~---------T2---------~</color>", (GUILayoutOption[])new GUILayoutOption[0]);
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
					GUILayout.Box(string.Format("<color=#ff0000ff>{3} {2}\n{0} {1}\n~-------SPAWNS-------~</color>{4}\n<color=#ffa500ff>~--------------------~</color>", item2.bpf.Name, (item2.bp.CalculateResourceCost(false, true)).Material, item2.spawn_location, item2.spawn_direction, text2), (GUILayoutOption[])new GUILayoutOption[0]);
					if (GUILayout.Button("^ Remove ^", (GUILayoutOption[])new GUILayoutOption[0]))
					{
						t.entry_t2.Remove(item2);
					}
				}
			}
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			if (GUI.Button(new Rect(970f, 660f, 280f, 50f), "Start") && t.entry_t1.Count > 0 && t.entry_t2.Count > 0)
			{
				this.DeactivateGui(0);
                t.loadCraft();
				t.StartMatch();
			}
		}

		private void UpdateFileData(BlueprintFile file)
		{
		}
	}
}
