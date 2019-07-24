using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
namespace Tournament.UI
{
    public class BaseSettingsTab : SuperScreen<Tournament>
    {
        public BaseSettingsTab(ConsoleWindow window, Tournament focus) : base(window, focus) {
            Name = new Content("Base Settings", "Setup the basic Parameters of the Fight.");
        }
        public override void Build()
        {
            base.Build();
            CreateHeader("Customize basic fighting parameters.", new ToolTip(""));
            ScreenSegmentStandard segment = CreateStandardSegment();
            //segment.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>());
        }
        public override Action OnSelectTab => base.OnSelectTab;
    }
}
