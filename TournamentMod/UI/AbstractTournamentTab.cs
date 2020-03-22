using BrilliantSkies.Ui.Consoles;
namespace TournamentMod.UI
{
	/// <summary>
	/// Baseclass for all Tabs, some of them have Popups.
	/// </summary>
	public abstract class AbstractTournamentTab : SuperScreen<Tournament>
	{
		private readonly TournamentConsole parentUI;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="window"></param>
		/// <param name="focus"></param>
		public AbstractTournamentTab(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(window, focus) {
			parentUI = parent;
		}
		/// <summary>
		/// Pops up an ConsoleUI, closes any active Popups.
		/// </summary>
		/// <param name="ui"></param>
		protected void PopThisUp(IConsoleUi ui) {
			parentUI.PopThisUp(ui);
		}
		/// <summary>
		/// Deactivates any current Popups.
		/// </summary>
		protected void DeactivatePopup() {
			parentUI.DeactivatePopup();
		}
		/// <summary>
		/// Rebuilds a currently active Popup.
		/// </summary>
		protected void RebuildPopup() {
			parentUI.RebuildPopup();
		}
		/// <summary>
		/// Rebuilds the Popup and the current Screen.
		/// </summary>
		protected void RebuildScreenAndPopup() {
			RebuildPopup();
			TriggerScreenRebuild();
		}
	}
}
