using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using System.Collections.Generic;
using BrilliantSkies.Ui.Consoles.Interpretters;
using System;
using BrilliantSkies.Ui.Displayer;
namespace Tournament.UI
{
    public class ParticipantManagementTab : SuperScreen<Tournament>
    {
        private ParticipantConsole participantConsole;
        public ParticipantManagementTab(ConsoleWindow window, Tournament focus) : base(window, focus) {
            Name = new Content("Participants", "View and edit Participants.");
            participantConsole = new ParticipantConsole(focus, this);
        }
        public override void Build()
        {
            CreateHeader("Modify current Entries", new ToolTip("Modify current Entries from active Teams."));
            ScreenSegmentStandardHorizontal horizontal = CreateStandardHorizontalSegment();
            horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Cycle Teams", new ToolTip("Cycles the entries through the currently active Teams. Non-active Teams will be excluded."), delegate (Tournament t)
            {
                List<TournamentEntry> temp = t.entries[0];
                int i;
                for (i = 1; i < t.Parameters.ActiveFactions; i++)
                {
                    t.entries[i - 1] = t.entries[i];
                }
                t.entries[i - 1] = temp;
                foreach (KeyValuePair<int, List<TournamentEntry>> teamList in t.entries)
                {
                    teamList.Value.ForEach((TournamentEntry te) => te.FactionIndex = teamList.Key);
                }
                TriggerRebuild();
            }));
            for (int i = 0; i < 6; i++)
            {
                int factionIndex = i;
                horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Invert Direction for Team {i + 1}", new ToolTip($"Inverts the direction for Team {i + 1}, by turning each entry 180°."), delegate (Tournament t)
                {
                    t.entries[factionIndex].ForEach((TournamentEntry te) => te.Spawn_direction = (te.Spawn_direction + 180) % 360);
                    TriggerRebuild();
                })).SetConditionalDisplayFunction(() => factionIndex < _focus.Parameters.ActiveFactions);
            }
            bool ready = true;
            for (int i = 0; i < 6; i++) {
                int factionIndex = i;
                int teamSize = 0;
                if (_focus.entries.ContainsKey(i))
                {
                    teamSize = _focus.entries[factionIndex].Count;
                }
                ready &= teamSize > 0 || i >= _focus.Parameters.ActiveFactions;
                CreateHeader("Team " + (i + 1), new ToolTip($"Current Entries for Team{i + 1}. The list goes from top to bottom.")).SetConditionalDisplay(() => factionIndex < _focus.Parameters.ActiveFactions);
                for (int j = 0; j < teamSize; j++) {
                    int indexInFaction = j;
                    TournamentEntry entry = _focus.entries[factionIndex][indexInFaction];
                    string text = "";
                    string[] labelCost = entry.LabelCost;
                    foreach (string str in labelCost)
                    {
                        text = text + "\n" + str;
                    }
                    ScreenSegmentTable entryControl = CreateTableSegment(3, 2);
                    entryControl.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
                    entryControl.SqueezeTable = false;
                    entryControl.SetConditionalDisplay(() => factionIndex < _focus.Parameters.ActiveFactions);
                    entryControl.AddInterpretter(StringDisplay.Quick(string.Format("{3}°@{2}m\n{0} {1}\n~-------SPAWNS-------~{4}\n~--------------------~", entry.Bpf.Name, entry.bp.CalculateResourceCost(false, true, false).Material, entry.Spawn_height, entry.Spawn_direction, text)), 0, 0);
                    entryControl.AddInterpretter(new Empty(), 1, 0);
                    entryControl.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Remove", new ToolTip("Removes this entry."), delegate (Tournament t)
                    {
                        t.entries[factionIndex].Remove(entry);
                        TriggerRebuild();
                    }), 0, 1);
                    entryControl.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Update", new ToolTip("Updates this entry with new values for its direction and height."), delegate (Tournament t)
                    {
                        entry.Spawn_direction = t.Parameters.Direction;
                        entry.Spawn_height = t.Parameters.SpawnHeight;
                        TriggerRebuild();
                    }), 1, 1);
                    entryControl.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Move up", new ToolTip("Moves the entry one place up in its teamlist."), delegate (Tournament t)
                    {
                        t.entries[factionIndex].RemoveAt(indexInFaction);
                        t.entries[factionIndex].Insert(indexInFaction - 1, entry);
                        TriggerRebuild();
                    }), 0, 2).SetConditionalDisplayFunction(() => indexInFaction != 0);
                    entryControl.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Move up", new ToolTip("Moves the entry one place up in its teamlist."), delegate (Tournament t)
                    {
                        t.entries[factionIndex].RemoveAt(indexInFaction);
                        t.entries[factionIndex].Insert(indexInFaction + 1, entry);
                        TriggerRebuild();
                    }), 1, 2).SetConditionalDisplayFunction(() => indexInFaction + 1 != teamSize);
                }
            }
            ScreenSegmentStandard posthead = CreateStandardSegment();
            posthead.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "START", new ToolTip("Start the fighting!"), delegate (Tournament t)
            {
                ((ConsoleUi<Tournament>)Window._governingUi).DeactivateGui();
                t.ApplyFactionColors();
                t.LoadCraft();
                t.StartMatch();
            })).SetConditionalDisplayFunction(() => ready);
            posthead.AddInterpretter(StringDisplay.Quick("It seems at least one Team has no Entry. Reduce the number of Teams or give the Team(s) in question at least one Entry.")).SetConditionalDisplayFunction(() => !ready);
        }
        public override Action OnSelectTab => () => {
            participantConsole.ActivateGui(GuiActivateType.Add);
            GuiDisplayer.GetSingleton().EvenOutUisAcrossTheScreen();
        };
        public override Action<OnDeselectTabSource> OnDeselectTab => (OnDeselectTabSource source) => {
            participantConsole.DeactivateGui(GuiDeactivateType.Standard);
            Window.SetSizeAndPosition(WindowSizing.GetCentralHuge());
        };
    }
}
