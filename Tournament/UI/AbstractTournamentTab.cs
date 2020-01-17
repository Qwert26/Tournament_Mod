using BrilliantSkies.Ui.Consoles;
namespace Tournament.UI
{
	public abstract class AbstractTournamentTab : SuperScreen<Tournament>
	{
		private TournamentConsole parentUI;
		public AbstractTournamentTab(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(window, focus) {
			parentUI = parent;
		}
		protected void PopThisUp(IConsoleUi ui) {
			parentUI.PopThisUp(ui);
		}
		protected void DeactivatePopup() {
			parentUI.DeactivatePopup();
		}
		protected void RebuildPopup() {
			parentUI.RebuildPopup();
		}
		protected void RebuildScreenAndPopup() {
			RebuildPopup();
			TriggerScreenRebuild();
		}
	}
}
