using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Displayer;
namespace Tournament.UI
{
	public class TournamentConsole : ConsoleUi<Tournament>
	{
		private IConsoleUi poppedUp = null;
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
			window.SetMultipleTabs(new BaseSettingsTab(this, window, _focus),
				new TeamSplittedRules(this, window, _focus),
				new AdvancedSettingsTab(this, window, _focus),
				new EyecandyTab(this, window, _focus),
				new ParticipantManagementTab(this, window, _focus));
			return window;
		}
		public override void OnActivateGui()
		{
			base.OnActivateGui();
			_focus.ResetCam();
			_focus.MoveCam();
		}
		public override void OnDeactivateGui()
		{
			base.OnDeactivateGui();
			DeactivatePopup();
		}
		public void PopThisUp(IConsoleUi ui)
		{
			if (poppedUp != null)
			{
				poppedUp.DeactivateGui(GuiDeactivateType.Standard);
			}
			poppedUp = ui;
			ui.ActivateGui(GuiActivateType.Add);
			GuiDisplayer.GetSingleton().EvenOutUisAcrossTheScreen();
		}
		public void DeactivatePopup() {
			if (poppedUp != null)
			{
				poppedUp.DeactivateGui(GuiDeactivateType.Standard);
				poppedUp = null;
			}
		}
		public void RebuildPopup() {
			if (poppedUp != null)
			{
				poppedUp.TriggerRebuild();
			}
		}
	}
}