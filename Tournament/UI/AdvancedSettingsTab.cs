using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
using Tournament.Serialisation;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using System.Collections.Generic;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
namespace Tournament.UI
{
    public class AdvancedSettingsTab : SuperScreen<Tournament>
    {
        public AdvancedSettingsTab(ConsoleWindow window, Tournament focus) : base(window, focus) {
            Name = new Content("Advanced Settings", "Setup advanced Parameters for the fight.");
        }
        public override void Build()
        {
            base.Build();
            CreateHeader("Customize advanced fighting parameters", new ToolTip("Usually hidden, so you need to activate them first."));
            ScreenSegmentStandard segment1 = CreateStandardSegment();
            segment1.AddInterpretter(SubjectiveFloatClampedWithBar<TournamentParameters>.Quick(_focus.Parameters, 2, 6, 1,
                M.m((TournamentParameters tp) => tp.ActiveFactions), "Active Teams: {0}", delegate (TournamentParameters tp, float f)
                {
                    tp.ActiveFactions.Us = (int)f;
                    TournamentPlugin.factionManagement.EnsureFactionCount((int)f);
                    tp.EnsureEnoughData();
                    for (int i = 0; i < f; i++) {
                        if (!_focus.entries.ContainsKey(i)) {
                            _focus.entries.Add(i, new List<TournamentEntry>());
                        }
                    }
                }, new ToolTip("The amount of active Teams.")));
            segment1.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Activate Advanced Options", new ToolTip("Shows the advanced options. Unless you know exactly what you want in here, i suggest leaving it closed."), delegate (TournamentParameters tp, bool b)
               {
                   tp.ShowAdvancedOptions.Us = b;
                   if (!b)
                   {
                       for (int i = 0; i < _focus.Parameters.ActiveFactions; i++)
                       {
                           _focus.Parameters.FormationIndexPerTeam.Us[i] = 0;
                       }
                       _focus.Parameters.MaterialConversion.Reset();
                       _focus.Parameters.CleanUpMode.Reset();
                       _focus.Parameters.HealthCalculation.Reset();
                       _focus.Parameters.MinimumHealth.Reset();
                       _focus.Parameters.CleanUpDelayedByRepairs.Reset();
                       _focus.Parameters.RepairDelayTime.Reset();
                       _focus.Parameters.CleanUpNoAI.Reset();
                       _focus.Parameters.CleanUpSinkingConstructs.Reset();
                       _focus.Parameters.SinkingAltitude.Reset();
                       _focus.Parameters.SinkingHealthFraction.Reset();
                       _focus.Parameters.CleanUpTooDamagedConstructs.Reset();
                       _focus.Parameters.TooDamagedHealthFraction.Reset();
                       _focus.Parameters.CleanUpTooSmallConstructs.Reset();
                       _focus.Parameters.TooSmallBlockCount.Reset();
                   }
               }, (tp) => tp.ShowAdvancedOptions.Us));
            ScreenSegmentStandard segment2 = CreateStandardSegment();
            segment2.SetConditionalDisplay(() => _focus.Parameters.ShowAdvancedOptions.Us);
            for (int i = 0; i < 6; i++) {
                int index = i;
                segment2.AddInterpretter(new SubjectiveFloatClampedWithBar<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(TournamentFormation.tournamentFormations.Length - 1),
                    M.m((TournamentParameters tp) => tp.FormationIndexPerTeam[index]), M.m<TournamentParameters>(1), _focus.Parameters, M.m((TournamentParameters tp) => $"Team {index+1} Formation: {TournamentFormation.tournamentFormations[tp.FormationIndexPerTeam[index]].Name}"), delegate (TournamentParameters tp, float f)
                      {
                          tp.FormationIndexPerTeam.Us[index] = (int)f;
                      }, null, M.m((TournamentParameters tp) => new ToolTip(TournamentFormation.tournamentFormations[tp.FormationIndexPerTeam[index]].Description)))).SetConditionalDisplayFunction(() => index < _focus.Parameters.ActiveFactions);
            }
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1, 100, 1, 0,
                M.m((TournamentParameters tp)=>tp.MaterialConversion), "Material-Conversion: {0}%", delegate (TournamentParameters tp, float f)
                {
                    tp.MaterialConversion.Us = (int)f;
                }, new ToolTip("Set the Material-Conversion-Factor, also known as Lifesteal. The maximum in campaigns is 10% but here you can go up to 100%! The value -1% is special: " +
                "In the case of friendly fire, a team will not get any materials back!")));
            string describeCleanupMode()
            {
                switch (_focus.Parameters.CleanUpMode.Us)
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
            segment2.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(3),
                M.m((TournamentParameters tp) => tp.CleanUpMode), M.m<TournamentParameters>(1), M.m<TournamentParameters>(2),
                _focus.Parameters, M.m((TournamentParameters tp) => $"Constructable-Cleanup set to {(ConstructableCleanUp)(int)tp.CleanUpMode}"), delegate (TournamentParameters tp, float f)
                {
                    tp.CleanUpMode.Us = (int)f;
                }, null, M.m((TournamentParameters tp) => new ToolTip(describeCleanupMode()))));
            segment2.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Enable \"Below and Sinking\"-Cleanup function", new ToolTip("Disable this function to allow submarines to last longer underwater."), delegate (TournamentParameters tp, bool b)
            {
                tp.CleanUpSinkingConstructs.Us = b;
            }, (tp) => tp.CleanUpSinkingConstructs)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 100, 1, 80,
                M.m((TournamentParameters tp) => tp.SinkingHealthFraction), "\"Below and Sinking\"-Healthfraction: {0}%", delegate (TournamentParameters tp, float f)
                {
                    tp.SinkingHealthFraction.Us = (int)f;
                }, new ToolTip("Any construct below a certain altitude and below this healthfraction will be scrapped after 10s"))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpSinkingConstructs);
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 0, 1, -10,
                M.m((TournamentParameters tp) => tp.SinkingAltitude), "\"Below and Sinking\"-Altitude: {0}m", delegate (TournamentParameters tp, float f)
                {
                    tp.SinkingAltitude.Us = (int)f;
                }, new ToolTip("Any construct below this altitude and below a certain healthfraction will be scrapped after 10s"))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpSinkingConstructs);
            segment2.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Enable \"Too Damaged\"-Cleanup function", new ToolTip("Disable this function to allow entries to fight until they are considered \"dead\"."), delegate (TournamentParameters tp, bool b) {
                tp.CleanUpTooDamagedConstructs.Us = b;
            }, (tp) => tp.CleanUpTooDamagedConstructs)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 100, 1, 55,
                M.m((TournamentParameters tp) => tp.TooDamagedHealthFraction), "\"Too Damaged\"-Healthfraction: {0}%", delegate (TournamentParameters tp, float f)
                {
                    tp.TooDamagedHealthFraction.Us = (int)f;
                }, new ToolTip("Any construct below this Healthfraction will be scrapped after 10s."))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpTooDamagedConstructs);
            segment2.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Enable \"Too few Blocks\"-Cleanup function", new ToolTip("Disable this function to allow entries below a certain number of blocks to exist, if they meet all other criteria."), delegate (TournamentParameters tp, bool b)
            {
                tp.CleanUpTooSmallConstructs.Us = b;
            }, (tp) => tp.CleanUpTooSmallConstructs)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 1, 100, 1, 10,
                M.m((TournamentParameters tp) => tp.TooSmallBlockCount), "\"Too few Blocks\"-Block count: {0}", delegate (TournamentParameters tp, float f)
                {
                    tp.TooSmallBlockCount.Us = (int)f;
                }, new ToolTip("Any construct, which is not a drone and has less blocks alive than this amount, will be scrapped after 10s."))).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0 && _focus.Parameters.CleanUpTooSmallConstructs);
            segment2.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Enable \"AI Dead\"-Cleanup function", new ToolTip("Any construct without an AI-Mainframe will be scrapped after 10s"), delegate (TournamentParameters tp, bool b)
            {
                tp.CleanUpNoAI.Us = b;
            }, (tp) => tp.CleanUpNoAI)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
            segment2.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Enable \"Sustained by Repairs\"-Delay function", new ToolTip("Repairs by other constructs delay the scrapping. This is important for freshly spawned in drones."), delegate (TournamentParameters tp, bool b)
            {
                tp.CleanUpDelayedByRepairs.Us = b;
            }, (tp) => tp.CleanUpDelayedByRepairs)).SetConditionalDisplayFunction(() => _focus.Parameters.CleanUpMode != 0);
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 10, 600, 1, 100,
                M.m((TournamentParameters tp) => tp.RepairDelayTime), "\"Sustained by Repairs\"-Delay time: {0}s", delegate (TournamentParameters tp, float f)
                {
                    tp.RepairDelayTime.Us = (int)f;
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
            segment2.AddInterpretter(new SubjectiveFloatClampedWithBar<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(3),
               M.m((TournamentParameters tp) => tp.HealthCalculation), M.m<TournamentParameters>(1), _focus.Parameters,
               M.m((TournamentParameters tp) => $"Healthcalculation: {describeHealthCalculation()}"), delegate (TournamentParameters tp, float f)
               {
                   tp.HealthCalculation.Us = (int)f;
               }, null, M.m((TournamentParameters tp) => new ToolTip(healthCalculationTip()))));
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 100, 1, 55,
                M.m((TournamentParameters tp)=>tp.MinimumHealth), "Minimum Health: {0}%", delegate (TournamentParameters tp, float f) {
                    tp.MinimumHealth.Us = (int)f;
                }, new ToolTip("Sets the minimum Health below any entry will pickup Penalty time, works best when clean up is \"Off\".")));
            ScreenSegmentStandardHorizontal saveAndLoad = CreateStandardHorizontalSegment();
            saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quicksave Settings", new ToolTip("Saves the current Parameters into the Mod-Folder."), (t) => t.SaveSettings()));
            /*saveAndLoad.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Save Settings", new ToolTip("Saves the current Parameters into a file of your chosing."), delegate (TournamentParameters tp)
            {
                GuiPopUp.Instance.Add(new PopupTreeViewSave<TournamentParameters>("Save Parameters", FtdGuiUtils.GetFileBrowserFor<TournamentParametersFile, TournamentParametersFolder>(new TournamentParametersFolder(new FilesystemFolderSource(Get.PerminentPaths.GetSpecificModDir("Tournament").ToString()))), delegate (string s, bool b)
                {
                    if (b)
                    {
                        TournamentParametersFile tpf = new TournamentParametersFile(new FilesystemFileSource(s + ".json"));
                        tpf.Save(_focus.Parameters);
                    }
                }, _focus.Parameters));
            }));*/
            saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quickload Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) => t.LoadSettings()));
            /*saveAndLoad.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Load Parameters", new ToolTip("Loads new Parameters from a file of your choosing."), delegate (TournamentParameters tp) {
                GuiPopUp.Instance.Add(new PopupTreeView("Load Parameters", FtdGuiUtils.GetFileBrowserFor<TournamentParametersFile, TournamentParametersFolder>(new TournamentParametersFolder(new FilesystemFolderSource(Get.PerminentPaths.GetSpecificModDir("Tournament").ToString()))), delegate (string s, bool b)
                {
                    if (b)
                    {
                        TournamentParametersFile tpf = new TournamentParametersFile(new FilesystemFileSource(s + ".json"));
                        _focus.Parameters = tpf.Load();
                        TriggerRebuild();
                    }
                }));
            }));*/
            saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) => t.LoadDefaults()));
        }
        public override Action OnSelectTab => base.OnSelectTab;
    }
}
