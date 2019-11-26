using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ftd.Planets.World;
using Tournament.Serialisation;
using UnityEngine;
namespace Tournament.UI
{
	public class BaseSettingsTab : SuperScreen<Tournament>
	{
		private int sectionsNorthSouth, sectionsEastWest;
		private int heightmapRange;
		private float fullGravityHeight;
		public BaseSettingsTab(ConsoleWindow window, Tournament focus) : base(window, focus) {
			Name = new Content("Base Settings", "Setup the basic Parameters of the Fight.");
		}
		public override void Build()
		{
			base.Build();
			CreateHeader("Basic Parameters", new ToolTip("Customize the most basic Parameters here."));
			ScreenSegmentStandard segment = CreateStandardSegment();
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 20000, 1, 1250,
				M.m((TournamentParameters tp)=>tp.StartingDistance), "Starting Distance from center: {0}m", delegate (TournamentParameters tp, float f)
				{
					tp.StartingDistance.Us = (int)f;
				}, new ToolTip("The distance from the center towards the \"Flagship(s)\" of a team.")));
			segment.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Team-based Rules are currently in effect.",
				new ToolTip("Press here to enable a global Ruleset for all Teams."), delegate (TournamentParameters tp)
				{
					tp.UniformRules.Us = true;
					tp.MakeUniform();
				})).SetConditionalDisplayFunction(() => !_focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
				M.m((TournamentParameters tp) => tp.SpawngapLR[0]), "Spawngaps Left-Right: {0}m", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.SpawngapLR.Us[i] = (int)f;
					}
				}, new ToolTip("How many meters should entries on the same team be apart in the left-right direction?"))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
				M.m((TournamentParameters tp) => tp.SpawngapFB[0]), "Spawngaps Forward-Backward: {0}m", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.SpawngapFB.Us[i] = (int)f;
					}
				}, new ToolTip("How many meters should entries on the same team be apart in the forward-backward direction?"))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			heightmapRange = WorldSpecification.i.BoardLayout.WorldHeightAndDepth;
			fullGravityHeight = WorldSpecification.i.Physics.SpaceIsFullAgain;
			segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>(M.m<TournamentParameters>(-heightmapRange), M.m<TournamentParameters>(fullGravityHeight),
				M.m((TournamentParameters tp) => tp.AltitudeLimits[0].x), M.m<TournamentParameters>(1), M.m((TournamentParameters tp) => tp.AltitudeLimits[0].y),
				_focus.Parameters, M.m((TournamentParameters tp)=>$"Lower Altitude Limit: {tp.AltitudeLimits[0].x}m"), delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						Vector2 v = tp.AltitudeLimits[i];
						v.x = f;
						tp.AltitudeLimits.Us[i] = v;
					}
				}, null, M.m<TournamentParameters>(new ToolTip("What is the minimum altitude for all entries?")))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>(M.m<TournamentParameters>(-heightmapRange), M.m<TournamentParameters>(fullGravityHeight),
				M.m((TournamentParameters tp) => tp.AltitudeLimits[0].y), M.m<TournamentParameters>(1), M.m((TournamentParameters tp) => tp.AltitudeLimits[0].x),
				_focus.Parameters, M.m((TournamentParameters tp)=>$"Upper Altitude Limit: {tp.AltitudeLimits[0].y}m"), delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						Vector2 v = tp.AltitudeLimits[i];
						v.y = f;
						tp.AltitudeLimits.Us[i] = v;
					}
				}, null, M.m<TournamentParameters>(new ToolTip("What is the maximum altitude for all entries?")))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 10000, 1, 1500,
				M.m((TournamentParameters tp) => tp.DistanceLimit[0]), "Distance Limit: {0}m", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.DistanceLimit.Us[i] = (int)f;
					}
				}, new ToolTip("What is the maximum distance for all entries towards the nearest enemy?"))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use projected Distance", new ToolTip("When turned on, a ground projected distance will be used. This is better for fights with a lot of vertical freedom."), delegate (TournamentParameters tp, bool b)
			{
				for (int i = 0; i < 6; i++)
				{
					tp.ProjectedDistance.Us[i] = b;
				}
			}, (tp) => tp.ProjectedDistance[0])).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 90,
				M.m((TournamentParameters tp) => tp.MaximumPenaltyTime[0]), "Maximum Penalty Time: {0}s", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.MaximumPenaltyTime.Us[i] = (int)f;
					}
				}, new ToolTip("How much penalty time can a participant have, before it self-destructs?"))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 10000, 1, 10000,
				M.m((TournamentParameters tp) => tp.MaximumSpeed[0]), "Maximum Speed: {0}m/s", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.MaximumSpeed.Us[i] = (int)f;
					}
				}, new ToolTip("Going over the maximum Speed will add penalty time. If soft limits are active, it will deplete the buffer first."))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use soft Limits", new ToolTip("When turned on, entries are given the chance to be considered in bounds under certain conditions. Turned off, entries will pickup penalty time as long as they are outside the bounds."), delegate (TournamentParameters tp, bool b)
			{
				for (int i = 0; i < 6; i++)
				{
					tp.SoftLimits.Us[i] = b;
				}
			}, (tp) => tp.SoftLimits[0])).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			#region Puffer-Einstellungen
			ScreenSegmentStandard segment2 = CreateStandardSegment();
			segment2.SetConditionalDisplay(() => _focus.Parameters.UniformRules&&_focus.Parameters.SoftLimits[0]);
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 0,
				M.m((TournamentParameters tp) => tp.MaximumBufferTime[0]), "Maximum Buffer Time: {0}s", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.MaximumBufferTime.Us[i] = (int)f;
					}
				}, new ToolTip("How much buffer time can a participant have, before it gains penalty time? This Buffer will reset, once a participant is considered to be back in bounds.")));
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
				M.m((TournamentParameters tp) => tp.DistanceReverse[0]), "Distance Reversal: {0}m/s", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.DistanceReverse.Us[i] = (int)f;
					}
				}, new ToolTip("A positive value permits a certain fleeing speed, while a negative value requires a certain closing speed. It assumes, that the nearest enemy is stationary.")));
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
				M.m((TournamentParameters tp) => tp.AltitudeReverse[0]), "Altitude Reversal: {0}m/s", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.AltitudeReverse.Us[i] = (int)f;
					}
				}, new ToolTip("A positive value allows to move away from the limits at a maximum speed, while a negative value requires to move towards the limit with a certain speed. Recommended is a negative value.")));
			#endregion
			ScreenSegmentStandard segment3 = CreateStandardSegment();
			segment3.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 900,
				M.m((TournamentParameters tp) => tp.MaximumTime), "Maximum Time: {0}s", delegate (TournamentParameters tp, float f)
				{
					tp.MaximumTime.Us = (int)f;
				}, new ToolTip("What is the maximum battle time? Once reached, the game will be paused. Unpausing it will activate \"Overtime\".")));
			segment3.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 60,
				M.m((TournamentParameters tp) => tp.Overtime), "Overtime: {0}s", delegate (TournamentParameters tp, float f)
				{
					tp.Overtime.Us = (int)f;
				}, new ToolTip("The length of one Overtime-section. Set it to 0 to only have one infinte long section.")));
			segment3.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Local Resources", new ToolTip("Enable or Disable local Resources."), delegate (TournamentParameters tp, bool b)
			   {
				   tp.LocalResources.Us = b;
			   }, (tp) => tp.LocalResources.Us));
			segment3.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Distribute local Resources", new ToolTip("The materials set below become the team maximum, which gets distributed along the entries. Any excess goes into team storage."), delegate (TournamentParameters tp, bool b)
			{
				tp.DistributeLocalResources.Us = b;
			}, (TournamentParameters tp) => tp.DistributeLocalResources)).SetConditionalDisplayFunction(() => _focus.Parameters.LocalResources);
			segment3.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Even Resources", new ToolTip("Give all Teams the same amount of resources of make it uneven."), delegate (TournamentParameters tp, bool b)
			{
				tp.SameMaterials.Us = b;
			}, (tp) => tp.SameMaterials.Us));
			#region Resourcen-Einstellungen
			ScreenSegmentStandard segmentIdenticalMaterials = CreateStandardSegment();
			segmentIdenticalMaterials.SetConditionalDisplay(() => _focus.Parameters.SameMaterials);
			_focus.Parameters.EnsureEnoughData();
			segmentIdenticalMaterials.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Infinte Resources", new ToolTip("Give all Teams infinte Materials"), delegate (TournamentParameters tp, bool b)
			{
				for (int i = 0; i < tp.ActiveFactions; i++)
				{
					tp.InfinteResourcesPerTeam.Us[i] = b;
				}
			}, (tp) => {
				return tp.InfinteResourcesPerTeam[0];
				}));
			segmentIdenticalMaterials.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 1000000, 1, 10000,
				M.m((TournamentParameters tp) => tp.ResourcesPerTeam[0]), "{0} Materials for all Teams", delegate (TournamentParameters tp, float f)
				{
					for (int i = 0; i < tp.ActiveFactions; i++)
					{
						tp.ResourcesPerTeam.Us[i] = (int)f;
					}
				}, new ToolTip("For local Resources, this determines with how much Materials a participant can spawn at maximum, for global resources, it determines the amount of Materials in storage."))).SetConditionalDisplayFunction(() => _focus.Parameters.SameMaterials && !_focus.Parameters.InfinteResourcesPerTeam[0]);
			ScreenSegmentStandard segmentIndividualMaterials = CreateStandardSegment();
			segmentIndividualMaterials.SetConditionalDisplay(() => !_focus.Parameters.SameMaterials);
			for (int i = 0; i < _focus.Parameters.ActiveFactions; i++) {
				int index = i + 1;
				segmentIndividualMaterials.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, $"Infinte Resources for Team {index}", new ToolTip($"Give Team {index} infinte Materials."), delegate (TournamentParameters tp, bool b)
				{
					tp.InfinteResourcesPerTeam.Us[index] = b;
				}, (tp) => {
					return tp.InfinteResourcesPerTeam[index];
					}));
				segmentIndividualMaterials.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 1000000, 1, 10000,
					M.m((TournamentParameters tp) => tp.ResourcesPerTeam[index]), $"{{0}} Materials for Team {index}", delegate (TournamentParameters tp, float f)
					{
						tp.ResourcesPerTeam.Us[index] = (int)f;
					}, new ToolTip("For local Resources, this determines with how much Materials a participant of this team can spawn at maximum, for global resources, it determines the amount of Materials in storage for this team."))).SetConditionalDisplayFunction(() => !_focus.Parameters.InfinteResourcesPerTeam[index]);
			}
			#endregion
			sectionsNorthSouth = WorldSpecification.i.BoardLayout.NorthSouthBoardSectionCount - 1;
			sectionsEastWest = WorldSpecification.i.BoardLayout.EastWestBoardSectionCount - 1;
			ScreenSegmentStandardHorizontal horizontal = CreateStandardHorizontalSegment();
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, sectionsEastWest, 1, sectionsEastWest / 2,
				M.m((TournamentParameters tp) => tp.EastWestBoard), "East-West Board {0}", delegate (TournamentParameters tp, float f)
				{
					tp.EastWestBoard.Us = (int)f;
					_focus.MoveCam();
				}, new ToolTip("Change the East-West Board index. In the Map it is the first number, 0 is at the left side.")));
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, sectionsNorthSouth, 1, sectionsNorthSouth / 2,
				M.m((TournamentParameters tp) => tp.NorthSouthBoard), "North-South Board {0}", delegate (TournamentParameters tp, float f)
				{
					tp.NorthSouthBoard.Us = (int)f;
					_focus.MoveCam();
				}, new ToolTip("Change the North-South Board index. In the Map it is the second number, 0 is at the bottom.")));
			horizontal.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>(M.m((TournamentParameters tp) => -180f / tp.ActiveFactions), M.m((TournamentParameters tp) => 180f / tp.ActiveFactions),
				M.m((TournamentParameters tp) => tp.Rotation), M.m<TournamentParameters>(1), M.m<TournamentParameters>(0),
				_focus.Parameters, M.m((TournamentParameters tp)=>$"Rotation: {tp.Rotation}°"), delegate (TournamentParameters tp, float f)
				{
					tp.Rotation.Us = (int)f;
				}, null, M.m<TournamentParameters>(new ToolTip("Rotate everything around the center before starting the fight"))));
			horizontal = CreateStandardHorizontalSegment();
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quicksave Settings", new ToolTip("Saves the current Parameters into the Mod-Folder."), (t) => t.SaveSettings()));
			/*horizontal.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Save Settings", new ToolTip("Saves the current Parameters into a file of your chosing."), delegate (TournamentParameters tp)
			{
				GuiPopUp.Instance.Add(new PopupTreeViewSave<TournamentParameters>("Save Parameters", FtdGuiUtils.GetFileBrowserFor<TournamentParametersFile, TournamentParametersFolder>(new TournamentParametersFolder(new FilesystemFolderSource(Get.PerminentPaths.GetSpecificModDir("Tournament").ToString()))), delegate (string s, bool b)
				{
					if (b) {
						TournamentParametersFile tpf = new TournamentParametersFile(new FilesystemFileSource(s + ".json"));
						tpf.Save(_focus.Parameters);
					}
				}, _focus.Parameters));
			}));*/
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quickload Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) => t.LoadSettings()));
			/*horizontal.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Load Parameters", new ToolTip("Loads new Parameters from a file of your choosing."), delegate (TournamentParameters tp) {
				GuiPopUp.Instance.Add(new PopupTreeView("Load Parameters", FtdGuiUtils.GetFileBrowserFor<TournamentParametersFile, TournamentParametersFolder>(new TournamentParametersFolder(new FilesystemFolderSource(Get.PerminentPaths.GetSpecificModDir("Tournament").ToString()))), delegate (string s, bool b)
				{
					if (b)
					{
						TournamentParametersFile tpf = new TournamentParametersFile(new FilesystemFileSource(s + ".json"));
						_focus.Parameters = tpf.Load();
						TriggerRebuild();
					}
				}));
			}));*/
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) => t.LoadDefaults()));
			horizontal.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use Default Keymap", new ToolTip("Uses the internal fixed keymap instead of your customized keymap."), delegate (TournamentParameters tp, bool b)
			   {
				   tp.DefaultKeys.Us = b;
			   }, (tp) => tp.DefaultKeys.Us));
		}
	}
}
