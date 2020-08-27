﻿using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using System.Collections.Generic;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using Assets.Scripts.Gui;
using BrilliantSkies.Core.Constants;
using BrilliantSkies.Core.FilesAndFolders;
using BrilliantSkies.Ui.Displayer;
using BrilliantSkies.Ui.Special.PopUps;
namespace TournamentMod.UI
{
	using Serialisation;
	/// <summary>
	/// GUI-Class for advanced settings such as active teams, formations and cleanup-functions.
	/// </summary>
	public class AdvancedSettingsTab : AbstractTournamentTab
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="window"></param>
		/// <param name="focus"></param>
		public AdvancedSettingsTab(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(parent, window, focus) {
			Name = new Content("Advanced Settings", "Setup advanced Parameters for the fight.");
		}
		/// <summary>
		/// Builds the Tab.
		/// </summary>
		public override void Build()
		{
			base.Build();
			CreateHeader("Customize advanced fighting parameters", new ToolTip("Usually hidden, so you need to activate them first."));
			ScreenSegmentStandard segment1 = CreateStandardSegment();
			segment1.SpaceAbove = segment1.SpaceBelow = 5;
			segment1.AddInterpretter(SubjectiveFloatClampedWithBar<Parameters>.Quick(_focus.Parameters, 2, StaticConstants.MAX_TEAMS, 1,
				M.m((Parameters tp) => tp.ActiveFactions), "Active Teams: {0}", delegate (Parameters tp, float f)
				{
					tp.ActiveFactions = (int)f;
					TournamentPlugin.factionManagement.EnsureFactionCount((int)f);
					tp.EnsureEnoughData();
					for (int i = 0; i < f; i++) {
						if (!_focus.entries.ContainsKey(i)) {
							_focus.entries.Add(i, new List<Entry>());
						}
					}
				}, new ToolTip("The amount of active Teams.")));
			for (int i = 0; i < StaticConstants.MAX_TEAMS; i++)
			{
				int index = i;
				segment1.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Open Formation-Console for Team {index + 1}.", new ToolTip($"Manage the Formation of Team {index + 1}."), delegate (Tournament t)
					   {
						   PopThisUp(new FormationConsole(t.teamFormations[index], index, t.entries[index].Count));
					   })).SetConditionalDisplayFunction(() => index < _focus.Parameters.ActiveFactions);
			}
			segment1.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Activate Advanced Options", new ToolTip("Shows the advanced options. Unless you know exactly what you want in here, i suggest leaving it closed."), delegate (Parameters tp, bool b)
			   {
				   tp.ShowAdvancedOptions = b;
				   if (!b)
				   {
					   _focus.Parameters.MaterialConversion = 0;
					   _focus.Parameters.CleanUpMode = 0;
					   _focus.Parameters.HealthCalculation = 0;
					   _focus.Parameters.MinimumHealth = 0;
					   _focus.Parameters.CleanUpDelayedByRepairs = true;
					   _focus.Parameters.RepairDelayTime = 100;
					   _focus.Parameters.CleanUpNoAI = true;
					   _focus.Parameters.CleanUpSinkingConstructs = true;
					   _focus.Parameters.SinkingAltitude = -10;
					   _focus.Parameters.SinkingHealthFraction = 80;
					   _focus.Parameters.CleanUpTooDamagedConstructs = true;
					   _focus.Parameters.TooDamagedHealthFraction = 55;
					   _focus.Parameters.CleanUpTooSmallConstructs = true;
					   _focus.Parameters.TooSmallBlockCount = 10;
				   }
			   }, (tp) => tp.ShowAdvancedOptions));
			ScreenSegmentStandard segment2 = CreateStandardSegment();
			segment2.SpaceAbove = segment2.SpaceBelow = 5;
			segment2.SetConditionalDisplay(() => _focus.Parameters.ShowAdvancedOptions);
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -1, 100, 1, 0,
				M.m((Parameters tp)=>tp.MaterialConversion), "Material-Conversion: {0}%", delegate (Parameters tp, float f)
				{
					tp.MaterialConversion = (int)f;
				}, new ToolTip("Set the Material-Conversion-Factor, also known as Lifesteal. The maximum in campaigns is 10% but here you can go up to 100%! The value -1% is special: " +
				"In the case of friendly fire, a team will not get any materials back!")));
			string describeCleanupMode()
			{
				switch (_focus.Parameters.CleanUpMode)
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
			segment2.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<Parameters>(M.m<Parameters>(0), M.m<Parameters>(3),
				M.m((Parameters tp) => tp.CleanUpMode), M.m<Parameters>(1), M.m<Parameters>(2),
				_focus.Parameters, M.m((Parameters tp) => $"Constructable-Cleanup set to {(ConstructableCleanUp)tp.CleanUpMode}"), delegate (Parameters tp, float f)
				{
					tp.CleanUpMode = (int)f;
				}, null, M.m((Parameters tp) => new ToolTip(describeCleanupMode()))));
			segment2.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Enable \"Below and Sinking\"-Cleanup function", new ToolTip("Disable this function to allow submarines to last longer underwater."), delegate (Parameters tp, bool b)
			{
				tp.CleanUpSinkingConstructs = b;
			}, (tp) => tp.CleanUpSinkingConstructs)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 100, 1, 80,
				M.m((Parameters tp) => tp.SinkingHealthFraction), "\"Below and Sinking\"-Healthfraction: {0}%", delegate (Parameters tp, float f)
				{
					tp.SinkingHealthFraction = (int)f;
				}, new ToolTip("Any construct below a certain altitude and below this health-fraction will be scrapped after 10s."))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpSinkingConstructs);
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -500, 0, 1, -10,
				M.m((Parameters tp) => tp.SinkingAltitude), "\"Below and Sinking\"-Altitude: {0}m", delegate (Parameters tp, float f)
				{
					tp.SinkingAltitude = (int)f;
				}, new ToolTip("Any construct below this altitude and below a certain health-fraction will be scrapped after 10s."))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpSinkingConstructs);
			segment2.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Enable \"Too Damaged\"-Cleanup function", new ToolTip("Disable this function to allow entries to fight until they are considered \"dead\"."), delegate (Parameters tp, bool b) {
				tp.CleanUpTooDamagedConstructs = b;
			}, (tp) => tp.CleanUpTooDamagedConstructs)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 100, 1, 55,
				M.m((Parameters tp) => tp.TooDamagedHealthFraction), "\"Too Damaged\"-Healthfraction: {0}%", delegate (Parameters tp, float f)
				{
					tp.TooDamagedHealthFraction = (int)f;
				}, new ToolTip("Any construct below this Healthfraction will be scrapped after 10s."))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpTooDamagedConstructs);
			segment2.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Enable \"Too few Blocks\"-Cleanup function", new ToolTip("Disable this function to allow entries below a certain number of blocks to exist, if they meet all other criteria."), delegate (Parameters tp, bool b)
			{
				tp.CleanUpTooSmallConstructs = b;
			}, (tp) => tp.CleanUpTooSmallConstructs)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 1, 100, 1, 10,
				M.m((Parameters tp) => tp.TooSmallBlockCount), "\"Too few Blocks\"-Block count: {0}", delegate (Parameters tp, float f)
				{
					tp.TooSmallBlockCount = (int)f;
				}, new ToolTip("Any construct, which is not a drone and has less blocks alive than this amount, will be scrapped after 10s."))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpTooSmallConstructs);
			segment2.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Enable \"AI Dead\"-Cleanup function", new ToolTip("Any construct without an AI-Mainframe will be scrapped after 10s"), delegate (Parameters tp, bool b)
			{
				tp.CleanUpNoAI = b;
			}, (tp) => tp.CleanUpNoAI)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
			segment2.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Enable \"Sustained by Repairs\"-Delay function", new ToolTip("Repairs by other constructs delay the scrapping. This is important for freshly spawned in drones."), delegate (Parameters tp, bool b)
			{
				tp.CleanUpDelayedByRepairs = b;
			}, (tp) => tp.CleanUpDelayedByRepairs)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 10, 600, 1, 100,
				M.m((Parameters tp) => tp.RepairDelayTime), "\"Sustained by Repairs\"-Delay time: {0}s", delegate (Parameters tp, float f)
				{
					tp.RepairDelayTime = (int)f;
				}, new ToolTip("Any construct, that was being repaired by another construct in the last 8 seconds, will have their scrapping delayed by this amount of time."))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpDelayedByRepairs);
			string healthCalculationTip()
			{
				switch (_focus.Parameters.HealthCalculation)
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
			string describeHealthCalculation()
			{
				switch (_focus.Parameters.HealthCalculation)
				{
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
			segment2.AddInterpretter(new SubjectiveFloatClampedWithBar<Parameters>(M.m<Parameters>(0), M.m<Parameters>(3),
			   M.m((Parameters tp) => tp.HealthCalculation), M.m<Parameters>(1), _focus.Parameters,
			   M.m((Parameters tp) => $"Healthcalculation: {describeHealthCalculation()}"), delegate (Parameters tp, float f)
			   {
				   tp.HealthCalculation = (int)f;
			   }, null, M.m((Parameters tp) => new ToolTip(healthCalculationTip()))));
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 100, 1, 55,
				M.m((Parameters tp)=>tp.MinimumHealth), "Minimum Health: {0}%", delegate (Parameters tp, float f) {
					tp.MinimumHealth = (int)f;
				}, new ToolTip("Sets the minimum Health below any entry will pickup Penalty time, works best when clean up is \"Off\".")));
			ScreenSegmentStandardHorizontal saveAndLoad = CreateStandardHorizontalSegment();
			saveAndLoad.SpaceAbove = saveAndLoad.SpaceBelow = 5;
			ParametersFolder folder = new ParametersFolder(new FilesystemFolderSource(Get.PermanentPaths.GetSpecificModDir("Tournament").ToString()), false);
			saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quicksave Settings", new ToolTip("Saves the current Parameters into the Mod-Folder."), (t) => t.SaveSettings()));
			saveAndLoad.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, "Save Settings", new ToolTip("Saves the current Parameters into a file of your chosing."), delegate (Parameters tp)
			{
				GuiPopUp.Instance.Add(new PopupTreeViewSave<Parameters>("Save Parameters", FtdGuiUtils.GetFileBrowserFor<ParametersFile, ParametersFolder>(folder), delegate (string s, bool b)
				{
				}, (s) => _focus.Parameters, "Settings"));
			}));
			saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quickload Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) =>
			{
				t.LoadSettings();
				t.MoveCam();
				TriggerScreenRebuild();
			}));
			saveAndLoad.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, "Load Parameters", new ToolTip("Loads new Parameters from a file of your choosing."), delegate (Parameters tp) {
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
			saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) =>
			{
				t.LoadDefaults();
				t.MoveCam();
				TriggerScreenRebuild();
			}));
		}
	}
}