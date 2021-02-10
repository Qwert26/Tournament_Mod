using BrilliantSkies.Core.Timing;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Segments;
using System.Collections.Generic;
using BrilliantSkies.Core.Id;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Tips;
using UnityEngine;
using System;
namespace TournamentMod.UI
{
	public class LiveViewConsole : ConsoleUi<ParticipantManagement>
	{
		private int lastParticipantCount = -1;
		public LiveViewConsole() : base() {}
		protected override ConsoleWindow BuildInterface(string suggestedName = "")
		{
			ConsoleWindow window = NewWindow(1, "Live View", new StandardFractional(0.01f, 0.01f, 0.3f, 0.9f));
			window.DisplayTextPrompt = false;
			window.BackgroundType = BackgroundType.Normal;
			lastParticipantCount = _focus.ParticipantCount();
			ScreenSegmentStandard standard = window.Screen.CreateStandardSegment();
			standard.AddInterpretter(new StringDisplay(M.m(() => $"<b>Elapsed Time: {Mathf.FloorToInt(Tournament._me.timerTotal / 60f)}m {Mathf.FloorToInt(Tournament._me.timerTotal % 60f)}s.</b>"), M.m(new ToolTip("The amount of Time that has already past."))));
			standard.AddInterpretter(StringDisplay.Quick("As long as this Console is visible, you can not unlatch the Cursor: Instead close this console, unlatch the Cursor and reopen it with the specified Keys in the Setup-Console. It is recommended to do this while the Orbit-Camera is following a construct."));
			foreach (KeyValuePair<ObjectId, Dictionary<MainConstruct, Participant>> team in _focus.TCP)
			{
				window.Screen.CreateHeader($"{team.Key.FactionSpec().Name}");
				foreach (KeyValuePair<MainConstruct, Participant> entry in team.Value)
				{
					Participant participant = entry.Value;
					MainConstruct construct = entry.Key;
					ScreenSegmentTable table = window.Screen.CreateTableSegment(3, 1);
					table.SpaceAbove = table.SpaceBelow = 5f;
					table.SqueezeTable = true;
					table.AddInterpretter(new StringDisplay(M.m(() => participant.BlueprintName), M.m(new ToolTip("The name of the participant."))), 0, 0);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Mathf.RoundToInt(participant.HP * 10f) / 10f}%"), M.m(new ToolTip("The current or last known Health value of the participant."))), 0, 1);
					table.AddInterpretter(new StringDisplay(M.m(() => $"<color=#{_focus.PenaltyTimeColor(participant)}>{Mathf.FloorToInt(participant.OoBTime / 60f)}m{Mathf.FloorToInt(10f * (participant.OoBTime % 60f)) / 10f}s{(participant.Scrapped ? $"@{Mathf.FloorToInt(participant.TimeOfDespawn / 60f)}m{Mathf.FloorToInt(10 * (participant.TimeOfDespawn % 60f)) / 10f}s" : "")}</color>"), M.m(new ToolTip("The current penalty- and DQ-time of the participant."))), 0, 2);
					//Extra Info ist hier.
					standard = window.Screen.CreateStandardSegment();
					standard.SetConditionalDisplay(() => ShowExtraInfo() && !construct.Destroyed);
					standard.SpaceAbove = standard.SpaceBelow = 5f;
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Altitude of {Math.Round(construct.SafeAltitude, 1)}m"), M.m(new ToolTip("The current altitude of the participant."))));
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Speed of {Math.Round(construct.TopSpeedAndAltitude.FilteredSpeed, 1)}m/s"), M.m(new ToolTip("The current speed of the participant."))));
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Materials of {Math.Round(construct.GetForce().Material.Quantity, 2)} out of {Mathf.RoundToInt(construct.GetForce().Material.Maximum)}"), M.m(new ToolTip("The current and maximum amount of Materials of the participant."))));
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Energy of {Math.Round(construct.GetForce().Energy.Quantity)} out of {Mathf.RoundToInt(construct.GetForce().Energy.Maximum)}"), M.m(new ToolTip("The current and maximum amount of Energy of the participant."))));
					standard.AddInterpretter(new StringDisplay(M.m(() => $"Current Power of {Mathf.RoundToInt(construct.PowerUsageCreationAndFuel.Power)} out of {Mathf.RoundToInt(construct.PowerUsageCreationAndFuel.MaxPower)}"), M.m(new ToolTip("The current and maximum amount of Power of the participant. Useful for figuring out, if someone has suffered engine damage."))));
					table = window.Screen.CreateTableSegment(8, 1);
					table.SpaceAbove = table.SpaceBelow = 5f;
					table.SqueezeTable = true;
					table.SetConditionalDisplay(standard._fnDisplayIfTrue);
					table.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
					table.SetColumnHeadings("APS", "CRAM", "Laser", "Melee", "Missile", "PAC", "Simple Cannon", "Simple Laser");
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Math.Round(construct.GetApsFirePower(), 2)}"), M.m(new ToolTip("The current APS-FP"))), 0, 0);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Math.Round(construct.GetCramFirePower(), 2)}"), M.m(new ToolTip("The current CRAM-FP"))), 0, 1);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Math.Round(construct.GetLaserFirePower(), 2)}"), M.m(new ToolTip("The current Laser-FP"))), 0, 2);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Math.Round(construct.GetMeleeFirePower(), 2)}"), M.m(new ToolTip("The current Melee-FP"))), 0, 3);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Math.Round(construct.GetMissileFirePower(), 2)}"), M.m(new ToolTip("The current Missile-FP"))), 0, 4);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Math.Round(construct.GetPacFirePower(), 2)}"), M.m(new ToolTip("The current PAC-FP"))), 0, 5);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Math.Round(construct.GetSimpleCannonFirePower(), 2)}"), M.m(new ToolTip("The current Simple Cannon-FP"))), 0, 6);
					table.AddInterpretter(new StringDisplay(M.m(() => $"{Math.Round(construct.GetSimpleLaserFirePower(), 2)}"), M.m(new ToolTip("The current Simple Laser-FP"))), 0, 7);
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
			GuiSettings.DisallowMouseControl = false;
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