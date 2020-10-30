using BrilliantSkies.Core.Timing;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Segments;
using System.Collections.Generic;
using BrilliantSkies.Core.Id;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Tips;
using UnityEngine;
namespace TournamentMod.UI
{
	public class LiveViewConsole : ConsoleUi<ParticipantManagement>
	{
		private int lastParticipantCount = -1;
		public LiveViewConsole() : base() {}
		protected override ConsoleWindow BuildInterface(string suggestedName = "")
		{
			ConsoleWindow window = NewWindow("Live View", new StandardFractional(0.1f, 0.1f, 0.3f, 0.8f));
			window.DisplayTextPrompt = false;
			window.BackgroundType = BackgroundType.Normal;
			lastParticipantCount = _focus.ParticipantCount();
			window.Screen.CreateStandardSegment().AddInterpretter(new StringDisplay(M.m(() => $"<b>Elapsed Time: {Mathf.FloorToInt(Tournament._me.timerTotal / 60f)}m {Mathf.FloorToInt(Tournament._me.timerTotal % 60f)}s.</b>"), M.m(new ToolTip("The amount of Time that has already past."))));
			foreach (KeyValuePair<ObjectId, Dictionary<MainConstruct, Participant>> team in _focus.TCP)
			{
				window.Screen.CreateHeader(team.Key.FactionSpec().Name);
				foreach (KeyValuePair<MainConstruct, Participant> entry in team.Value)
				{
					Participant participant = entry.Value;
					MainConstruct construct = entry.Key;
					ScreenSegmentTable table = window.Screen.CreateTableSegment(3, 1);
					table.SpaceAbove = table.SpaceBelow = 5f;
					table.SqueezeTable = true;
					table.AddInterpretter(new StringDisplay(M.m(() => participant.BlueprintName), M.m(new ToolTip("The name of the participant."))), 0, 0);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Mathf.RoundToInt(participant.HP * 10f) / 10f}%"), M.m(new ToolTip("The current Health value of the participant."))), 0, 1);
					table.AddInterpretter(new StringDisplay(M.m(() => $"<color=#{_focus.PenaltyTimeColor(participant)}>{Mathf.FloorToInt(participant.OoBTime / 60f)}m{Mathf.FloorToInt(10f * (participant.OoBTime % 60f)) / 10f}s{(participant.Scrapped ? $"@{Mathf.FloorToInt(participant.TimeOfDespawn / 60f)}m{Mathf.FloorToInt(10 * (participant.TimeOfDespawn % 60f)) / 10f}s" : "")}</color>"), M.m(new ToolTip("The current penalty time of the participant."))), 0, 2);
					//Extra Info ist hier.
					ScreenSegmentStandard standard = window.Screen.CreateStandardSegment();
					standard.SetConditionalDisplay(() => ShowExtraInfo() && !construct.Destroyed);
					standard.SpaceAbove = standard.SpaceBelow = 5f;
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Altitude of {construct.SafeAltitude}m"), M.m(new ToolTip("The current altitude of the participant."))));
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Speed of {construct.TopSpeedAndAltitude.FilteredSpeed}m/s"), M.m(new ToolTip("The current speed of the participant."))));
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Materials of {construct.GetForce().Material.Quantity} out of {construct.GetForce().Material.Maximum}"), M.m(new ToolTip("The current and maximum amount of Materials of the participant."))));
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Energy of {construct.GetForce().Energy.Quantity} out of {construct.GetForce().Energy.Maximum}"), M.m(new ToolTip("The current and maximum amount of Energy of the participant."))));
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Power of {construct.PowerUsageCreationAndFuel.Power} out of {construct.PowerUsageCreationAndFuel.MaxPower}"), M.m(new ToolTip("The current and maximum amount of Power of the participant. Useful for figuring out, if someone has suffered engine damage."))));

				}
			}
			return window;
		}
		public override void UpdateWhenActive(ITimeStep t)
		{
			base.UpdateWhenActive(t);
			int currentParticipantCount = _focus.ParticipantCount();
			if (currentParticipantCount != lastParticipantCount)
			{
				lastParticipantCount = currentParticipantCount;
				TriggerRebuild();
			}
		}
		public override void EscapePressed()
		{
			base.EscapePressed();
			Tournament._me.showLists = false;
		}
		public override void SetGuiSettings()
		{
			base.SetGuiSettings();
			GuiSettings.DisallowMouseControl = true;
			GuiSettings.PausesMultiplayerPlay = false;
			GuiSettings.PausesPlay = false;
			GuiSettings.QGui = false;
			GuiSettings.RequiresPlayStop = false;
			GuiSettings.AllowElaborateOverlays = true;
		}
		private static bool ShowExtraInfo()
		{
			return Tournament._me.extraInfo;
		}
	}
}