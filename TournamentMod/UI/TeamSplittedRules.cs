﻿using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using TournamentMod.Serialisation;
using BrilliantSkies.Ui.Consoles.Getters;
using UnityEngine;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.Core.FilesAndFolders;
using BrilliantSkies.Core.Constants;
using BrilliantSkies.Ui.Special.PopUps;
using Assets.Scripts.Gui;
using BrilliantSkies.Ui.Displayer;

namespace TournamentMod.UI
{
	/// <summary>
	/// GUI-Class for splitting spawn and penalty rules across teams.
	/// </summary>
	public class TeamSplittedRules : AbstractTournamentTab
	{
		/// <summary>
		/// Used as minimal value for height-limits.
		/// </summary>
		private int heightmapRange;
		/// <summary>
		/// Used as maximal value for height-limits.
		/// </summary>
		private float fullGravityHeight;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="window"></param>
		/// <param name="focus"></param>
		public TeamSplittedRules(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(parent, window, focus) {
			Name = new Content("Team-splitted Rules", "You can set team-specific rules for spawning and penaltytime-pickup here.");
		}
		/// <summary>
		/// Builds the Tab.
		/// </summary>
		public override void Build()
		{
			CreateHeader("Uniform Rules are active", new ToolTip("Team-based Spawn- and Penalty-Rules are not currently active.")).SetConditionalDisplay(() => _focus.Parameters.UniformRules);
			ScreenSegmentStandard segment = CreateStandardSegment();
			segment.SpaceAbove = segment.SpaceBelow = 5;
			segment.SetConditionalDisplay(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, "Use Team-based Spawn- and Penalty-Rules", new ToolTip("Activate the usage of Team-based Spawn- and Penalty-Rules."),
				delegate (Parameters tp)
				{
					tp.UniformRules.Us = false;
				}));
			heightmapRange = WorldSpecification.i.BoardLayout.WorldHeightAndDepth;
			fullGravityHeight = WorldSpecification.i.Physics.SpaceIsFullAgain;
			for (int i = 0; i < 6; i++) {
				int index = i;
				CreateHeader($"Rules for Team {index + 1}", new ToolTip($"These are the DQ-Rules specific to Members of Team {index + 1}.")).SetConditionalDisplay(() => !_focus.Parameters.UniformRules && index < _focus.Parameters.ActiveFactions);
				segment = CreateStandardSegment();
				segment.SpaceAbove = segment.SpaceBelow = 5;
				segment.SetConditionalDisplay(() => !_focus.Parameters.UniformRules && index < _focus.Parameters.ActiveFactions);
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
					M.m((Parameters tp) => tp.SpawngapFB[index]), "Spawngap Forward-Backward: {0}m", delegate (Parameters tp, float f)
					{
						tp.SpawngapFB.Us[index] = (int)f;
					}, new ToolTip("How many meters should the entries on this team be apart on the z-axis?")));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
					M.m((Parameters tp) => tp.SpawngapLR[index]), "Spawngap Left-Right: {0}m", delegate (Parameters tp, float f)
					{
						tp.SpawngapLR.Us[index] = (int)f;
					}, new ToolTip("How many meters should the entries on this team be apart on the x-axis?")));
				segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<Parameters>(M.m<Parameters>(-heightmapRange), M.m<Parameters>(fullGravityHeight),
					M.m((Parameters tp) => tp.AltitudeLimits[index].x), M.m<Parameters>(1), M.m((Parameters tp) => tp.AltitudeLimits[index].y),
					_focus.Parameters, M.m((Parameters tp)=>$"Lower Altitude Limit: {tp.AltitudeLimits[index].x}m"), delegate (Parameters tp, float f)
					{
						Vector2 v = tp.AltitudeLimits[index];
						v.x = f;
						if (v.x > v.y)
						{
							v.y = v.x;
						}
						tp.AltitudeLimits.Us[index] = v;
					}, null, M.m<Parameters>(new ToolTip("What is the maximum depth that entries on this team are allowed to go to?"))));
				segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<Parameters>(M.m<Parameters>(-heightmapRange), M.m<Parameters>(fullGravityHeight),
					M.m((Parameters tp) => tp.AltitudeLimits[index].y), M.m<Parameters>(1), M.m((Parameters tp) => tp.AltitudeLimits[index].x),
					_focus.Parameters, M.m((Parameters tp)=>$"Upper Altitude Limit: {tp.AltitudeLimits[index].y}m"), delegate (Parameters tp, float f)
					{
						Vector2 v = tp.AltitudeLimits[index];
						v.y = f;
						if (v.y < v.x)
						{
							v.x = v.y;
						}
						tp.AltitudeLimits.Us[index] = v;
					}, null, M.m<Parameters>(new ToolTip("What is the maximum height that entries on this team are allowed to go to?"))));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 10000, 1, 1500,
					M.m((Parameters tp) => tp.DistanceLimit[index]), "Distance Limit: {0}m", delegate (Parameters tp, float f)
					{
						tp.DistanceLimit.Us[index] = (int)f;
					}, new ToolTip("How many meters should the entries on this team be apart towards enemies?")));
				segment.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Use projected Distance", new ToolTip("When calculating distances and if this is on, height will be ignored. This is good for fights with a lot of vertical freedom."), delegate (Parameters tp, bool b)
				{
					tp.ProjectedDistance.Us[index] = b;
				}, (Parameters tp) => tp.ProjectedDistance[index]));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 3600, 1, 90,
					M.m((Parameters tp) => tp.MaximumPenaltyTime[index]), "Maximum Penaltytime: {0}s", delegate (Parameters tp, float f)
					{
						tp.MaximumPenaltyTime.Us[index] = (int)f;
					}, new ToolTip("How many seconds of penaltytime is an entry permitted to collect, before it gets removed from the fight?")));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 10000, 1, 10000,
					M.m((Parameters tp) => tp.MaximumSpeed[index]), "Maximum Speed: {0}m/s", delegate (Parameters tp, float f)
					{
						tp.MaximumSpeed.Us[index] = (int)f;
					}, new ToolTip("The maximum speed for entries on this team. Going over it will add penalty time. If soft limits are active, it will deplete the time buffer first.")));
				segment.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Use soft Limits", new ToolTip("With soft Limits, entries outside the boundaries are given the chance to get back into them. Without soft Limits, entries will pickup penalty time as long as they are outside the boundaries."), delegate (Parameters tp, bool b)
				{
					tp.SoftLimits.Us[index] = b;
				}, (Parameters tp) => tp.SoftLimits[index]));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 100, 1, 50,
				M.m((Parameters tp) => tp.EnemyAttackPercentage[index]), "Enemy Attack Percentage: {0}%", delegate (Parameters tp, float f)
					{
						tp.EnemyAttackPercentage.Us[index] = (int) f;
					}, new ToolTip("When determining if an entry is violating the distance limit, this percentage must be reached or it is considered fleeing from too many enemies.")));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 3600, 1, 0,
					M.m((Parameters tp) => tp.MaximumBufferTime[index]), "Maximum Buffertime: {0}s", delegate (Parameters tp, float f)
					{
						tp.MaximumBufferTime.Us[index] = (int)f;
					}, new ToolTip("When an entry is considered out of bounds, a timebuffer will be depleted first. It will reset once an entry is considered back in bounds."))).
					SetConditionalDisplayFunction(() => _focus.Parameters.SoftLimits[index]);
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
					M.m((Parameters tp) => tp.DistanceReverse[index]), "Distance Reversal: {0}m/s", delegate (Parameters tp, float f)
					{
						tp.DistanceReverse.Us[index] = (int)f;
					}, new ToolTip("A positive value permits a certain fleeing speed, while a negative value requires a certain closing speed. It assumes, that the nearest enemy is stationary."))).
					SetConditionalDisplayFunction(() => _focus.Parameters.SoftLimits[index]);
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -500, 500, 1, -3,
					M.m((Parameters tp) => tp.AltitudeReverse[index]), "Altitude Reversal: {0}m/s", delegate (Parameters tp, float f)
					{
						tp.AltitudeReverse.Us[index] = (int)f;
					}, new ToolTip("A positive value allows to move away from the limits at a maximum speed, while a negative value requires to move towards the limit with a certain speed. Recommended is a negative value."))).
					SetConditionalDisplayFunction(() => _focus.Parameters.SoftLimits[index]);
			}
			ScreenSegmentStandardHorizontal horizontal = CreateStandardHorizontalSegment();
			horizontal.SpaceAbove = horizontal.SpaceBelow = 5;
			ParametersFolder folder = new ParametersFolder(new FilesystemFolderSource(Get.PermanentPaths.GetSpecificModDir("Tournament").ToString()), false);
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quicksave Settings", new ToolTip("Saves the current Parameters into the Mod-Folder."), (t) => t.SaveSettings()));
			horizontal.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, "Save Settings", new ToolTip("Saves the current Parameters into a file of your chosing."), delegate (Parameters tp)
			{
				GuiPopUp.Instance.Add(new PopupTreeViewSave<Parameters>("Save Parameters", FtdGuiUtils.GetFileBrowserFor<ParametersFile, ParametersFolder>(folder), delegate (string s, bool b)
				{
				}, (s) => _focus.Parameters, "Settings"));
			}));
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quickload Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) =>
			{
				t.LoadSettings();
				t.MoveCam();
				TriggerScreenRebuild();
			}));
			horizontal.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, "Load Parameters", new ToolTip("Loads new Parameters from a file of your choosing."), delegate (Parameters tp) {
				GuiPopUp.Instance.Add(new PopupTreeView("Load Parameters", FtdGuiUtils.GetFileBrowserFor<ParametersFile, ParametersFolder>(folder), delegate (string s, bool b)
				{
					if (b)
					{
						Parameters parameters = folder.GetFile(s, true).Load();
						_focus.Parameters = parameters;
						DeactivatePopup();
						GuiDisplayer.GetSingleton().CloseAllUis();
						new TournamentConsole(_focus).ActivateGui(GuiActivateType.Stack);
					}
				}));
			}));
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) =>
			{
				t.LoadDefaults();
				t.MoveCam();
				TriggerScreenRebuild();
			}));
			horizontal.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Use Default Keymap", new ToolTip("Uses the internal fixed keymap instead of your customized keymap."), delegate (Parameters tp, bool b)
			{
				tp.DefaultKeys.Us = b;
			}, (tp) => tp.DefaultKeys.Us));
		}
	}
}