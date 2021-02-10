using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Displayer;
namespace TournamentMod.UI
{
	/// <summary>
	/// GUI-Class for the Main-Console.
	/// </summary>
	public class TournamentConsole : ConsoleUi<Tournament>
	{
		/// <summary>
		/// The current popup.
		/// </summary>
		private IConsoleUi poppedUp = null;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="focus"></param>
		public TournamentConsole(Tournament focus) : base(focus)
		{
			_focus.tournamentConsole = this;
		}
		/// <summary>
		/// GUI-Settings of this Console.
		/// </summary>
		public override void SetGuiSettings()
		{
			base.SetGuiSettings();
			GuiSettings.QGui = false;
			GuiSettings.PausesMultiplayerPlay = false;
			GuiSettings.PausesPlay = false;
		}
		/// <summary>
		/// Builds the multi-tab Content.
		/// </summary>
		/// <param name="suggestedName"></param>
		/// <returns></returns>
		protected override ConsoleWindow BuildInterface(string suggestedName = "")
		{
			ConsoleWindow window = NewWindow(3, "Tournament Setup", WindowSizing.GetCentralHuge());
			window.DisplayTextPrompt = false;
			window.SetMultipleTabs(new BaseSettingsTab(this, window, _focus),
				new TeamSplittedRules(this, window, _focus),
				new PenaltiesWeightingTab(this, window, _focus),
				new AdvancedSettingsTab(this, window, _focus),
				new EyecandyTab(this, window, _focus),
				new ParticipantManagementTab(this, window, _focus));
			return window;
		}
		/// <summary>
		/// Moves the camera towards the selected center.
		/// </summary>
		public override void OnActivateGui()
		{
			base.OnActivateGui();
			_focus.ResetCam();
			_focus.MoveCam();
		}
		/// <summary>
		/// Deactivates any popups.
		/// </summary>
		public override void OnDeactivateGui()
		{
			base.OnDeactivateGui();
			DeactivatePopup();
		}
		/// <summary>
		/// Shows a new popup. if another one is active, it will be closed first.
		/// </summary>
		/// <param name="ui">The new popup</param>
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
		/// <summary>
		/// Deactivates any currently active popup.
		/// </summary>
		public void DeactivatePopup() {
			if (poppedUp != null)
			{
				poppedUp.DeactivateGui(GuiDeactivateType.Standard);
				poppedUp = null;
			}
		}
		/// <summary>
		/// Rebuilds the popup.
		/// </summary>
		public void RebuildPopup() {
			if (poppedUp != null)
			{
				poppedUp.TriggerRebuild();
			}
		}
	}
}