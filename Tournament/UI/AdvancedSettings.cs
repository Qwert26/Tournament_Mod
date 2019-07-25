using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
namespace Tournament.UI
{
    public class AdvancedSettings : SuperScreen<Tournament>
    {
        public AdvancedSettings(ConsoleWindow window, Tournament focus) : base(window, focus) {
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
        }
        public override Action OnSelectTab => base.OnSelectTab;
    }
}
