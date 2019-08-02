using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Displayer;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Consoles.Interpretters;
namespace Tournament.UI
{
    public class ParticipantManagementTab : SuperScreen<Tournament>
    {
        public ParticipantManagementTab(ConsoleWindow window, Tournament focus) : base(window, focus) {
            Name = new Content("Participants", "View and edit Participants.");
        }
        public override void Build()
        {
            base.Build();
            CreateHeader("Mangement",new ToolTip("Use the first button to manage your Entries, the second one is left from testing."));
            ScreenSegmentStandard prehead = CreateStandardSegment();
            prehead.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Switch GUI to add Participants", new ToolTip("As long as the Vehicle Loading GUI is in the old GUI, we have to make a switch towards it."), delegate (Tournament t)
            {
                new ParticipantGUI().ActivateGui(_focus, GuiActivateType.Stack);
            }));
            prehead.AddInterpretter(SubjectiveButton<ParticipantManagementTab>.Quick(this, "Update List", new ToolTip("Updates the List down below"), (ParticipantManagementTab pmt) => pmt.TriggerScreenRebuild()));
            bool ready = true;
            for (int i = 0; i < 6; i++) {
                int factionIndex = i;
                int teamSize=0;
                if (_focus.entries.ContainsKey(i))
                {
                    teamSize = _focus.entries[factionIndex].Count;
                }
                ready &= teamSize > 0 || i >= _focus.Parameters.ActiveFactions;
                CreateHeader("Team " + (i + 1), new ToolTip($"Current Entries for Team{i + 1}. The list goes from left to right and then top to bottom.")).SetConditionalDisplay(() => factionIndex < _focus.Parameters.ActiveFactions);
                ScreenSegmentTable list = CreateTableSegment(5,10);
                list.eTableOrder = ScreenSegmentTable.TableOrder.Rows;
                list.SetConditionalDisplay(() => factionIndex < _focus.Parameters.ActiveFactions);
                for (int j = 0; j < teamSize; j++) {
                    int indexInFaction = j;
                    TournamentEntry entry = _focus.entries[factionIndex][indexInFaction];
                    string text = "";
                    string[] labelCost = entry.LabelCost;
                    foreach (string str in labelCost)
                    {
                        text = text + "\n" + str;
                    }
                    list.AddInterpretter(StringDisplay.Quick(string.Format("{3}°@{2}m\n{0} {1}\n~-------SPAWNS-------~{4}\n~--------------------~", entry.Bpf.Name, entry.bp.CalculateResourceCost(false, true, false).Material, entry.Spawn_height, entry.Spawn_direction, text)));
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
    }
}
