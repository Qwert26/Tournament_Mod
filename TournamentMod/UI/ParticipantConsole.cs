using Assets.Scripts.Gui;
using Assets.Scripts.Persistence;
using BrilliantSkies.Core.FilesAndFolders;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Special.PopUps;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.TreeSelection;
using TournamentMod.Serialisation;
namespace TournamentMod.UI
{
	/// <summary>
	/// GUI-Class for adding new participants to Teams.
	/// </summary>
	public class ParticipantConsole : ConsoleUi<Tournament>
	{
		/// <summary>
		/// The ParticipantManagementTab.
		/// </summary>
		private readonly ConsoleUiScreen _parentTab;
		/// <summary>
		/// The TreeSelector
		/// </summary>
		private readonly TreeSelectorGuiElement<BlueprintFile, BlueprintFolder> treeSelector;
		/// <summary>
		/// Used as minimum spawn height.
		/// </summary>
		private int heightmapRange;
		/// <summary>
		/// Used as maximum spawn height.
		/// </summary>
		private float fullGravityHeight;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="focus"></param>
		/// <param name="parentTab"></param>
		public ParticipantConsole(Tournament focus, ConsoleUiScreen parentTab) : base(focus) {
			_parentTab = parentTab;
			treeSelector = FtdGuiUtils.GetFileBrowserFor(GameFolders.GetCombinedBlueprintFolder(false));
		}
		/// <summary>
		/// Builds the Content.
		/// </summary>
		/// <param name="suggestedName"></param>
		/// <returns></returns>
		protected override ConsoleWindow BuildInterface(string suggestedName = "")
		{
			ConsoleWindow window = NewWindow("Add Participants", WindowSizing.GetRhs());
			window.DisplayTextPrompt = false;
			ConsoleUiScreen screen = window.Screen;
			screen.CreateHeader("Add Entries", new ToolTip("Select and Add Entries into active Teams."));
			ScreenSegmentTreeViewPopUp treeView = screen.AddSegment(new ScreenSegmentTreeViewPopUp(screen, treeSelector));
			treeView.SpaceAbove = 5;
			treeView.SpaceBelow = 5;
			ScreenSegmentStandardHorizontal horizontal = screen.CreateStandardHorizontalSegment();
			horizontal.SpaceBelow = horizontal.SpaceAbove = 5;
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -180, 180, 1, 0,
				M.m((Parameters tp) => tp.Direction), "Starting Direction: {0}°", delegate (Parameters tp, float f)
				{
					tp.Direction.Us = f;
				}, new ToolTip("0° is the old forward, 90° is the old right and -90° is the old left.")));
			heightmapRange = WorldSpecification.i.BoardLayout.WorldHeightAndDepth;
			fullGravityHeight = WorldSpecification.i.Physics.SpaceIsFullAgain;
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -heightmapRange, fullGravityHeight, 1, 0,
				M.m((Parameters tp) => tp.SpawnHeight), "Starting Height: {0}m", delegate (Parameters tp, float f)
				{
					tp.SpawnHeight.Us = (int)f;
				}, new ToolTip("The starting height of the entry")));
			horizontal = screen.CreateStandardHorizontalSegment();
			horizontal.SpaceBelow = horizontal.SpaceAbove = 5;
			horizontal.AddInterpretter(SubjectiveButton<TreeSelectorGuiElement<BlueprintFile,BlueprintFolder>>.Quick(treeSelector, "Add to all Teams", new ToolTip("Adds the currently selected Blueprint to all active Teams."), delegate (TreeSelectorGuiElement<BlueprintFile,BlueprintFolder> tree)
			{
				if (tree.HasSelection)
				{
					for (int i = 0; i < _focus.Parameters.ActiveFactions; i++)
					{
						_focus.entries[i].Add(new Entry()
						{
							FactionIndex = i,
							Spawn_direction = _focus.Parameters.Direction,
							Spawn_height = _focus.Parameters.SpawnHeight,
							Bpf = tree.CurrentData,
							FilePath = tree.GetLocationFromRootFolder((IData data) => string.IsNullOrEmpty(data.Name) ? "" : (data.Name + "/")) + tree.SelectionName
						});
					}
					_parentTab.TriggerRebuild();
				}
			}));
			for (int i = 0; i < StaticConstants.MAX_TEAMS; i++)
			{
				int factionIndex = i;
				horizontal.AddInterpretter(SubjectiveButton<TreeSelectorGuiElement<BlueprintFile,BlueprintFolder>>.Quick(treeSelector, $"Add to Team {i + 1}", new ToolTip($"Add the currently selected Blueprint only to Team {i + 1}."), delegate (TreeSelectorGuiElement<BlueprintFile,BlueprintFolder> tree)
				{
					if (tree.HasSelection)
					{
						_focus.entries[factionIndex].Add(new Entry()
						{
							FactionIndex = factionIndex,
							Spawn_direction = _focus.Parameters.Direction,
							Spawn_height = _focus.Parameters.SpawnHeight,
							Bpf = tree.CurrentData,
							FilePath = tree.GetLocationFromRootFolder((IData data) => string.IsNullOrEmpty(data.Name) ? "" : (data.Name + "/")) + tree.SelectionName
						});
						_parentTab.TriggerRebuild();
					}
				})).SetConditionalDisplayFunction(() => factionIndex < _focus.Parameters.ActiveFactions);
			}
			return window;
		}
	}
}