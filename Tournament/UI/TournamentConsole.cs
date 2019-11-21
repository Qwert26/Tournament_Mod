using BrilliantSkies.Core.Timing;
using BrilliantSkies.Ui.Consoles;
namespace Tournament.UI
{
    public class TournamentConsole : ConsoleUi<Tournament>
    {
        public TournamentConsole(Tournament focus) : base(focus) { }
        public override void SetGuiSettings()
        {
            base.SetGuiSettings();
            GuiSettings.QGui = false;
            GuiSettings.PausesMultiplayerPlay = false;
            GuiSettings.PausesPlay = false;
        }
        protected override ConsoleWindow BuildInterface(string suggestedName = "")
        {
            ConsoleWindow window = NewWindow("Tournament Setup", WindowSizing.GetCentralHuge());
            window.DisplayTextPrompt = false;
            window.SetMultipleTabs(new BaseSettingsTab(window, _focus), new TeamSplittedRules(window, _focus), new AdvancedSettingsTab(window, _focus), new EyecandyTab(window, _focus), new ParticipantManagementTab(window, _focus));
            return window;
        }
        public override void OnActivateGui()
        {
            base.OnActivateGui();
            _focus.ResetCam();
            _focus.MoveCam();
        }
        public override void FixedUpdateWhenActive(ITimeStep t)
        {
            base.FixedUpdateWhenActive(t);
        }
        protected override void OnRisenOutOfStack()
        {
            base.OnRisenOutOfStack();
            TriggerScreenRebuild();
        }
    }
}