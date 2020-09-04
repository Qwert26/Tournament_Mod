using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Displayer;
using BrilliantSkies.Ui.Special.PopUps;
namespace TournamentMod.UI
{
	using Assets.Scripts.Gui;
	using BrilliantSkies.Core.Constants;
	using BrilliantSkies.Core.FilesAndFolders;
	using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
	using Serialisation;
	using System;

	public class PenaltiesWeightingTab : AbstractTournamentTab
	{
		public PenaltiesWeightingTab(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(parent, window, focus)
		{
			Name = new Content("Penalty Weights", "Control how much a penalty matters and set a penalty combiner.");
		}
		public override void Build()
		{
			CreateHeader("Weight Combiner", new ToolTip("Entries can violate multiple boundaries at once, with each having a unique weight, you need to set a deterministic way to combine them."));
			ScreenSegmentStandard segment = CreateStandardSegment();
			segment.SpaceAbove = segment.SpaceBelow = 5;
			segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<Parameters>(M.m<Parameters>(-2), M.m<Parameters>(4), M.m((Parameters p) => p.WeightCombiner),
				M.m<Parameters>(1), M.m<Parameters>(1), _focus.Parameters, M.m((Parameters p) => "Current Combiner: " + (WeightCombinerType) p.WeightCombiner), delegate (Parameters p, float f)
						 {
							 p.WeightCombiner = (int) f;
						 }, null, M.m((Parameters p) => new ToolTip(((WeightCombinerType) p.WeightCombiner).GetDescription()))));
			CreateHeader("Penalty Weights, Uniform", new ToolTip("Penalty weights for all teams.")).SetConditionalDisplay(() => _focus.Parameters.UniformRules);
			segment = CreateStandardSegment();
			segment.SpaceAbove = segment.SpaceBelow = 5;
			segment.SetConditionalDisplay(() => _focus.Parameters.UniformRules);
			foreach (PenaltyType pt in Enum.GetValues(typeof(PenaltyType)))
			{
				PenaltyType penalty = pt;
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Tournament>.Quick(_focus, StaticConstants.MIN_WEIGHT, StaticConstants.MAX_WEIGHT, 0.1f, StaticConstants.NORMAL_WEIGHT, M.m((Tournament t) => t.teamPenaltyWeights[0][pt]),
					$"Weight for penalty type \"{penalty}\": {{0}}", delegate (Tournament t, float f)
					{
						for (int i = 0; i < StaticConstants.MAX_TEAMS; i++)
						{
							t.teamPenaltyWeights[i][penalty] = f;
						}
					}, new ToolTip(penalty.GetDescription())));
			}
			CreateHeader("Penalty Weights, team-based", new ToolTip("Penalty weights for individual teams.")).SetConditionalDisplay(() => !_focus.Parameters.UniformRules);
			segment = CreateStandardSegment();
			segment.SpaceAbove = segment.SpaceBelow = 5;
			segment.SetConditionalDisplay(() => !_focus.Parameters.UniformRules);
			for (int t = 0; t < StaticConstants.MAX_TEAMS; t++)
			{
				int team = t;
				segment.AddInterpretter(StringDisplay.Quick($"Penalty weights for Team {team + 1}")).SetConditionalDisplayFunction(() => team < _focus.Parameters.ActiveFactions);
				foreach (PenaltyType pt in Enum.GetValues(typeof(PenaltyType)))
				{
					PenaltyType penalty = pt;
					segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Tournament>.Quick(_focus, StaticConstants.MIN_WEIGHT, StaticConstants.MAX_WEIGHT, 0.1f, StaticConstants.NORMAL_WEIGHT, M.m((Tournament tournament) => tournament.teamPenaltyWeights[team][pt]),
						$"Weight for penalty type \"{penalty}\" for Team {team + 1}: {{0}}", delegate (Tournament tournament, float f)
						{
							tournament.teamPenaltyWeights[team][penalty] = f;
						}, new ToolTip(penalty.GetDescription()))).SetConditionalDisplayFunction(() => team < _focus.Parameters.ActiveFactions);
				}
			}
			ScreenSegmentStandardHorizontal horizontal = CreateStandardHorizontalSegment();
			horizontal.SpaceAbove = horizontal.SpaceBelow = 5;
			ParametersFolder folder = new ParametersFolder(new FilesystemFolderSource(Get.PermanentPaths.GetSpecificModDir("Tournament").ToString()), false);
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quicksave Settings", new ToolTip("Saves the current Parameters into the Mod-Folder."), (t) => t.SaveSettings()));
			horizontal.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, "Save Settings", new ToolTip("Saves the current Parameters into a file of your chosing."), delegate (Parameters tp)
			{
				GuiPopUp.Instance.Add(new PopupTreeViewSave<Parameters>("Save Parameters", FtdGuiUtils.GetFileBrowserFor<ParametersFile, ParametersFolder>(folder), delegate (string s, bool b)
				{
				}, _focus.Parameters, "Settings"));
			}));
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quickload Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) =>
			{
				t.LoadSettings();
				t.MoveCam();
				TriggerScreenRebuild();
			}));
			horizontal.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, "Load Parameters", new ToolTip("Loads new Parameters from a file of your choosing."), delegate (Parameters tp) {
				GuiPopUp.Instance.Add(new PopupTreeView("Load Parameters", FtdGuiUtils.GetFileBrowserFor<ParametersFile, ParametersFolder>(folder), delegate (string s, bool b)
				{
					if (b)
					{
						Parameters parameters = folder.GetFile(s, true).Load();
						_focus.Parameters = parameters;
						DeactivatePopup();
						GuiDisplayer.GetSingleton().CloseAllUis();
						new TournamentConsole(_focus).ActivateGui(GuiActivateType.Stack);
					}
				}));
			}));
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) =>
			{
				t.LoadDefaults();
				t.MoveCam();
				TriggerScreenRebuild();
			}));
			horizontal.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Use Default Keymap", new ToolTip("Uses the internal fixed keymap instead of your customized keymap."), delegate (Parameters tp, bool b)
			{
				tp.DefaultKeys = b;
			}, (tp) => tp.DefaultKeys));
		}
	}
}