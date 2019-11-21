using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using Tournament.Serialisation;
namespace Tournament.UI
{
    public class TeamSplittedRules : SuperScreen<Tournament>
    {
        public TeamSplittedRules(ConsoleWindow window, Tournament focus) : base(window, focus) {
            Name = new Content("Team-splitted DQ-Rules", "");
        }
        public override void Build()
        {
            CreateHeader("Uniform Rules are active", new ToolTip("Team-based DQ-Rules are not currently active.")).SetConditionalDisplay(() => _focus.Parameters.UniformRules);
            ScreenSegmentStandard segment = CreateStandardSegment();
            segment.SetConditionalDisplay(() => _focus.Parameters.UniformRules);
            segment.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Use Team-based DQ-Rules", new ToolTip("Activate the usage of Team-based DQ-Rules."),
                delegate (TournamentParameters tp)
                {
                    tp.UniformRules.Us = false;
                }));
            for (int i = 0; i < 6; i++) {
                int index = i;
                CreateHeader($"Rules for Team {index + 1}", new ToolTip($"These are the DQ-Rules specific to Members of Team {index + 1}.")).SetConditionalDisplay(() => !_focus.Parameters.UniformRules && index < _focus.Parameters.ActiveFactions);
                segment = CreateStandardSegment();
                segment.SetConditionalDisplay(() => !_focus.Parameters.UniformRules && index < _focus.Parameters.ActiveFactions);
            }
        }
    }
}