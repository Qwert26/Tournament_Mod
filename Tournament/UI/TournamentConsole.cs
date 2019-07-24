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
        }
        protected override ConsoleWindow BuildInterface(string suggestedName = "")
        {
            ConsoleWindow window = NewWindow("Tournament Setup", new ScaledRectangle(10,10,620,780));
            window.DisplayTextPrompt = false;
            window.SetMultipleTabs(new BaseSettingsTab(window, _focus));
            return window;
        }
    }
}
