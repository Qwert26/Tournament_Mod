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
                M.m((TournamentParameters tp) => tp.ActiveFactions), "Active Teams", delegate (TournamentParameters tp, float f)
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
            segment1.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters,"Activate Advanced Options",new ToolTip(""),delegate (TournamentParameters tp,bool b)
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
                }
            },(tp)=>tp.ShowAdvancedOptions.Us));
            ScreenSegmentStandard segment2 = CreateStandardSegment();
            segment2.SetConditionalDisplay(() => _focus.Parameters.ShowAdvancedOptions.Us);
            for (int i = 0; i < 6; i++) {
                int index = i;
                segment2.AddInterpretter(new SubjectiveFloatClampedWithBar<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(TournamentFormation.tournamentFormations.Length - 1),
                    M.m((TournamentParameters tp) => tp.FormationIndexPerTeam[index]), M.m<TournamentParameters>(1), _focus.Parameters, M.m((TournamentParameters tp) => $"Team {index} Formation: {TournamentFormation.tournamentFormations[tp.FormationIndexPerTeam[index]].Name}"), delegate (TournamentParameters tp, float f)
                      {
                          tp.FormationIndexPerTeam.Us[index] = (int)f;
                      }, null, M.m((TournamentParameters tp) => new ToolTip(TournamentFormation.tournamentFormations[tp.FormationIndexPerTeam[index]].Description)))).SetConditionalDisplayFunction(() => index < _focus.Parameters.ActiveFactions);
            }
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1, 100, 1, 0,
                M.m((TournamentParameters tp)=>tp.MaterialConversion), "Material-Conversion", delegate (TournamentParameters tp, float f)
                {
                    tp.MaterialConversion.Us = (int)f;
                }, new ToolTip("Set the Material-Conversion-Factor, also known as Lifesteal. The maximum in campaigns is 10% but here you can go up to 100%! The value -1% is a special case:" +
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
               M.m((TournamentParameters tp) => "Healthcalculation: " + describeHealthCalculation()), delegate (TournamentParameters tp, float f)
               {
                   tp.HealthCalculation.Us = (int)f;
               }, null, M.m((TournamentParameters tp) => new ToolTip(healthCalculationTip()))));
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 100, 1, 55,
                M.m((TournamentParameters tp)=>tp.MinimumHealth), "Minimum Health", delegate (TournamentParameters tp, float f) {
                    tp.MinimumHealth.Us = (int)f;
                }, new ToolTip("Sets the minimum Health below any entry will pickup Penalty time, works best when clean up is \"Off\".")));
            ScreenSegmentStandardHorizontal saveAndLoad = CreateStandardHorizontalSegment();
            saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Save Settings", new ToolTip("Saves the current Parameters into the Mod-Folder."), (t) => t.SaveSettings()));
            saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) => t.LoadSettings()));
            saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) => t.LoadDefaults()));
        }
        public override Action OnSelectTab => base.OnSelectTab;
    }
}
