using Assets.Scripts.Gui;
using Assets.Scripts.Persistence;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Special.PopUps;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.TreeSelection;
using Tournament.Serialisation;
namespace Tournament.UI
{
    public class ParticipantConsole:ConsoleUi<Tournament>
    {
        private ConsoleUiScreen _parentTab;
        private TreeSelectorGuiElement<BlueprintFile, BlueprintFolder> treeSelector;
        private int heightmapRange;
        private float fullGravityHeight;
        public ParticipantConsole(Tournament focus, ConsoleUiScreen parentTab) : base(focus) {
            _parentTab = parentTab;
            treeSelector = FtdGuiUtils.GetFileBrowserFor(GameFolders.GetCombinedBlueprintFolder(false));
        }
        protected override ConsoleWindow BuildInterface(string suggestedName = "")
        {
            ConsoleWindow window = NewWindow("Add Participants", WindowSizing.GetRhs());
            window.DisplayTextPrompt = false;
            ConsoleUiScreen screen = window.Screen;
            screen.CreateHeader("Add Entries", new ToolTip("Select and Add Entries into active Teams."));
            ScreenSegmentTreeViewPopUp treeView = screen.AddSegment(new ScreenSegmentTreeViewPopUp(screen, treeSelector));
            treeView.SpaceAbove = 20;
            treeView.SpaceBelow = 20;
            ScreenSegmentStandardHorizontal horizontal = screen.CreateStandardHorizontalSegment();
            horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -180, 180, 1, 0,
                M.m((TournamentParameters tp) => tp.Direction), "Starting Direction: {0}°", delegate (TournamentParameters tp, float f)
                {
                    tp.Direction.Us = f;
                }, new ToolTip("0° is the old forward, 90° is the old right and -90° is the old left.")));
            heightmapRange = WorldSpecification.i.BoardLayout.WorldHeightAndDepth;
            fullGravityHeight = WorldSpecification.i.Physics.SpaceIsFullAgain;
            horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -heightmapRange, fullGravityHeight, 1, 0,
                M.m((TournamentParameters tp) => tp.SpawnHeight), "Starting Height: {0}m", delegate (TournamentParameters tp, float f)
                {
                    tp.SpawnHeight.Us = (int)f;
                }, new ToolTip("The starting height of the entry")));
            horizontal = screen.CreateStandardHorizontalSegment();
            horizontal.AddInterpretter(SubjectiveButton<TreeSelectorGuiElement<BlueprintFile,BlueprintFolder>>.Quick(treeSelector, "Add to all Teams", new ToolTip("Adds the currently selected Blueprint to all active Teams."), delegate (TreeSelectorGuiElement<BlueprintFile,BlueprintFolder> tree)
            {
                if (tree.HasSelection)
                {
                    for (int i = 0; i < _focus.Parameters.ActiveFactions; i++)
                    {
                        _focus.entries[i].Add(new TournamentEntry()
                        {
                            FactionIndex = i,
                            Spawn_direction = _focus.Parameters.Direction,
                            Spawn_height = _focus.Parameters.SpawnHeight,
                            Bpf = tree.CurrentData
                        });
                    }
                    _parentTab.TriggerRebuild();
                }
            }));
            for (int i = 0; i < 6; i++)
            {
                int factionIndex = i;
                horizontal.AddInterpretter(SubjectiveButton<TreeSelectorGuiElement<BlueprintFile,BlueprintFolder>>.Quick(treeSelector, $"Add to Team {i + 1}", new ToolTip($"Add the currently selected Blueprint only to Team {i + 1}."), delegate (TreeSelectorGuiElement<BlueprintFile,BlueprintFolder> tree)
                {
                    if (tree.HasSelection)
                    {
                        _focus.entries[factionIndex].Add(new TournamentEntry()
                        {
                            FactionIndex = factionIndex,
                            Spawn_direction = _focus.Parameters.Direction,
                            Spawn_height = _focus.Parameters.SpawnHeight,
                            Bpf = tree.CurrentData
                        });
                        _parentTab.TriggerRebuild();
                    }
                })).SetConditionalDisplayFunction(() => factionIndex < _focus.Parameters.ActiveFactions);
            }
            return window;
        }
    }
}