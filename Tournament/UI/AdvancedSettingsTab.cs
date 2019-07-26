using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
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
            segment1.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters,"Activate Advanced Options",new ToolTip(""),delegate (TournamentParameters tp,bool b)
            {
                tp.ShowAdvancedOptions.Us = b;
            },(tp)=>tp.ShowAdvancedOptions.Us));
            ScreenSegmentStandard segment2 = CreateStandardSegment();
            segment2.SetConditionalDisplay(() => _focus.Parameters.ShowAdvancedOptions.Us);
            segment2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0),M.m<TournamentParameters>(TournamentFormation.tournamentFormations.GetLength(0)-1),
                M.m<TournamentParameters>(_focus.Parameters.Team1FormationIndex),M.m<TournamentParameters>(1),_focus.Parameters,
                M.m<TournamentParameters>($"Team 1 Formation: {TournamentFormation.tournamentFormations[_focus.Parameters.Team1FormationIndex].Name}"),delegate (TournamentParameters tp,float f)
                {
                    tp.Team1FormationIndex.Us = (int)f;
                },null,M.m<TournamentParameters>(new ToolTip(TournamentFormation.tournamentFormations[_focus.Parameters.Team1FormationIndex].Description))));
            segment2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(TournamentFormation.tournamentFormations.GetLength(0) - 1),
                M.m<TournamentParameters>(_focus.Parameters.Team2FormationIndex), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>($"Team 2 Formation: {TournamentFormation.tournamentFormations[_focus.Parameters.Team2FormationIndex].Name}"), delegate (TournamentParameters tp, float f)
                {
                    tp.Team2FormationIndex.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip(TournamentFormation.tournamentFormations[_focus.Parameters.Team2FormationIndex].Description))));
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1, 100, 1, 0,
                M.m<TournamentParameters>(_focus.Parameters.MaterialConversion), "Material-Conversion", delegate (TournamentParameters tp, float f)
                {
                    tp.MaterialConversion.Us = (int)f;
                }, new ToolTip("Set the Material-Conversion-Factor, also known as Lifesteal. The maximum in campaigns is 10% but here you can go up to 100%!")));
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
            segment2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(3),
                M.m<TournamentParameters>(_focus.Parameters.CleanUpMode), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Construct-Cleanup: " + (BrilliantSkies.Ftd.Planets.Instances.Headers.ConstructableCleanUp)_focus.Parameters.CleanUpMode.Us), delegate (TournamentParameters tp, float f) {
                    tp.CleanUpMode.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip(describeCleanupMode()))));
            string describeHealthCalculation()
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
            segment2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(3),
               M.m<TournamentParameters>(_focus.Parameters.HealthCalculation), M.m<TournamentParameters>(1), _focus.Parameters,
               M.m<TournamentParameters>("Healthcalculation: " + (Tournament.HealthCalculation)_focus.Parameters.HealthCalculation.Us), delegate (TournamentParameters tp, float f) {
                   tp.HealthCalculation.Us = (int)f;
               }, null, M.m<TournamentParameters>(new ToolTip(describeHealthCalculation()))));
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 100, 1, 55,
                M.m<TournamentParameters>(_focus.Parameters.MinimumHealth), "Minimum Health", delegate (TournamentParameters tp, float f) {
                    tp.MinimumHealth.Us = (int)f;
                }, new ToolTip("Sets the minimum Health below any entry will pickup Penalty time, works best when clean up is \"Off\".")));
            if (!_focus.Parameters.ShowAdvancedOptions.Us) {
                _focus.Parameters.Team1FormationIndex.Reset();
                _focus.Parameters.Team2FormationIndex.Reset();
                _focus.Parameters.MaterialConversion.Reset();
                _focus.Parameters.CleanUpMode.Reset();
                _focus.Parameters.HealthCalculation.Reset();
                _focus.Parameters.MinimumHealth.Reset();
            }
        }
        public override Action OnSelectTab => base.OnSelectTab;
    }
}
