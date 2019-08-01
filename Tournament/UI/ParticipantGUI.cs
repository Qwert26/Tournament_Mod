using Assets.Scripts.Gui;
using Assets.Scripts.Persistence;
using BrilliantSkies.Ui.Displayer.Types;
using BrilliantSkies.Ui.TreeSelection;
using BrilliantSkies.Ui.Layouts;
using BrilliantSkies.Ui.Tips;
using UnityEngine;
using System.Collections.Generic;
namespace Tournament.UI
{
    public class ParticipantGUI : ThrowAwayObjectGui<Tournament>
    {
        private TreeSelectorGuiElement<BlueprintFile, BlueprintFolder> treeSelector;
        private Vector2 centralScroll=Vector2.zero,rightScroll=Vector2.zero;
        public ParticipantGUI() : base() { }
        public override void OnGui()
        {
            GUILayout.BeginArea(new Rect(0, 0, 400, 800), "Select Participants", GUI.skin.window);
            treeSelector.OnGui(new Rect(30, 30, 340, 760));
            if (GuiCommon.DisplayCloseButton(400)) {
                DeactivateGui();
            }
            GUILayout.EndArea();

            GUISliders.TotalWidthOfWindow = 400;
            GUISliders.TextWidth = 150;
            GUISliders.DecimalPlaces = 0;
            GUISliders.UpperMargin = 0;

            GUILayout.BeginArea(new Rect(400, 0, 400, 800), "Spawn Settings", GUI.skin.window);
            centralScroll=GUILayout.BeginScrollView(centralScroll);
            _focus.Parameters.Direction.Us = GUISliders.LayoutDisplaySlider("Direction",_focus.Parameters.Direction,-180,180,toolTip:new ToolTip("Spawndirection for Participant"));
            _focus.Parameters.SpawnHeight.Us = (int)GUISliders.LayoutDisplaySlider("Height",_focus.Parameters.SpawnHeight, -500, 3000, toolTip: new ToolTip("Spawnheight for Participant"));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save")) {
                _focus.SaveSettings();
            }
            if (GUILayout.Button("Load Defaults")) {
                _focus.LoadDefaults();
            }
            GUILayout.EndHorizontal();
            if (treeSelector.HasSelection&&GUILayout.Button("Add to all Teams")) {
                for (int i = 0; i < _focus.Parameters.ActiveFactions; i++) {
                    _focus.entries[i].Add(new TournamentEntry() {
                        FactionIndex = i,
                        Spawn_direction = _focus.Parameters.Direction,
                        Spawn_height = _focus.Parameters.SpawnHeight,
                        Bpf = treeSelector.CurrentData
                    });
                }
            }
            for (int i = 0; treeSelector.HasSelection && i < _focus.Parameters.ActiveFactions; i++) {
                if (GUILayout.Button("Add to Team "+i)) {
                    _focus.entries[i].Add(new TournamentEntry() {
                        FactionIndex = i,
                        Spawn_direction = _focus.Parameters.Direction,
                        Spawn_height = _focus.Parameters.SpawnHeight,
                        Bpf = treeSelector.CurrentData
                    });
                }
            }
            if (GUILayout.Button("Cycle Teams")) {
                List<TournamentEntry> temp = _focus.entries[0];
                int i;
                for (i = 1; i < _focus.Parameters.ActiveFactions; i++) {
                    _focus.entries[i - 1] = _focus.entries[i];
                }
                _focus.entries[i - 1] = temp;
                foreach (KeyValuePair<int, List<TournamentEntry>> teamList in _focus.entries) {
                    teamList.Value.ForEach((TournamentEntry te) => te.FactionIndex = teamList.Key);
                }
            }
            for (int i = 0; i < _focus.Parameters.ActiveFactions; i++) {
                if (GUILayout.Button("Invert Direction for Team " + i)) {
                    _focus.entries[i].ForEach((TournamentEntry te) => te.Spawn_direction = (te.Spawn_direction + 180) % 360);
                }
            }
            GUILayout.EndScrollView();
            if (GuiCommon.DisplayCloseButton(400))
            {
                DeactivateGui();
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(800, 0, 400, 800), "Participant List", GUI.skin.window);
            rightScroll = GUILayout.BeginScrollView(rightScroll);
            foreach (KeyValuePair<int,List<TournamentEntry>> teamlist in _focus.entries)
            {
                if (teamlist.Key >= _focus.Parameters.ActiveFactions) {
                    break;
                }
                GUILayout.Box($"~---------Team {teamlist.Key+1}---------~");
                for (int i=0;i<teamlist.Value.Count;i++) {
                    TournamentEntry entry = teamlist.Value[i];
                    string text = "";
                    string[] labelCost = entry.LabelCost;
                    foreach (string str in labelCost)
                    {
                        text = text + "\n" + str;
                    }
                    GUILayout.Box(string.Format("{3}°@{2}m\n{0} {1}\n~-------SPAWNS-------~{4}\n~--------------------~", entry.Bpf.Name, entry.bp.CalculateResourceCost(false, true, false).Material, entry.Spawn_height, entry.Spawn_direction, text));
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    if (GUILayout.Button("^ Remove ^"))
                    {
                        teamlist.Value.Remove(entry);
                    }
                    if (GUILayout.Button("^ Update ^"))
                    {
                        entry.Spawn_height = _focus.Parameters.SpawnHeight;
                        entry.Spawn_direction = _focus.Parameters.Direction;
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    if (i != 0 && GUILayout.Button("^ Move up ^"))
                    {
                        teamlist.Value.RemoveAt(i);
                        teamlist.Value.Insert(i - 1, entry);
                    }
                    if (i + 1 != teamlist.Value.Count && GUILayout.Button("^ Move down ^"))
                    {
                        teamlist.Value.RemoveAt(i);
                        teamlist.Value.Insert(i + 1, entry);
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
            if (GuiCommon.DisplayCloseButton(400))
            {
                DeactivateGui();
            }
            GUILayout.EndArea();
        }
        public override void SetGuiSettings()
        {
            GuiSettings.PausesMultiplayerPlay = false;
            GuiSettings.PausesPlay = false;
            GuiSettings.Use1280by800 = true;
            GuiSettings.QGui = false;
        }
        public override void OnActivateGui()
        {
            base.OnActivateGui();
            treeSelector = FtdGuiUtils.GetFileBrowserFor(GameFolders.GetCombinedBlueprintFolder());
        }
    }
}
