using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ftd.Planets.World;
using TournamentMod.Serialisation;
using UnityEngine;
namespace TournamentMod.UI
{
	public class BaseSettingsTab : AbstractTournamentTab
	{
		private int sectionsNorthSouth, sectionsEastWest, terrainsPerSection;
		private int heightmapRange;
		private float fullGravityHeight, terrainSize;
		public BaseSettingsTab(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(parent, window, focus) {
			Name = new Content("Base Settings", "Setup the basic Parameters of the Fight.");
		}
		public override void Build()
		{
			base.Build();
			CreateHeader("Basic Parameters", new ToolTip("Customize the most basic Parameters here."));
			ScreenSegmentStandard segment = CreateStandardSegment();
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 20000, 1, 1250,
				M.m((Parameters tp)=>tp.StartingDistance), "Starting Distance from center: {0}m", delegate (Parameters tp, float f)
				{
					tp.StartingDistance.Us = (int)f;
				}, new ToolTip("The distance from the center towards the \"Flagship(s)\" of a team.")));
			segment.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, "Team-based Rules are currently in effect.",
				new ToolTip("Press here to enable a global Ruleset for all Teams."), delegate (Parameters tp)
				{
					tp.UniformRules.Us = true;
					tp.MakeUniform();
				})).SetConditionalDisplayFunction(() => !_focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
				M.m((Parameters tp) => tp.SpawngapLR[0]), "Spawngaps Left-Right: {0}m", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.SpawngapLR.Us[i] = (int)f;
					}
				}, new ToolTip("How many meters should entries on the same team be apart in the left-right direction?"))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
				M.m((Parameters tp) => tp.SpawngapFB[0]), "Spawngaps Forward-Backward: {0}m", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.SpawngapFB.Us[i] = (int)f;
					}
				}, new ToolTip("How many meters should entries on the same team be apart in the forward-backward direction?"))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			heightmapRange = WorldSpecification.i.BoardLayout.WorldHeightAndDepth;
			fullGravityHeight = WorldSpecification.i.Physics.SpaceIsFullAgain;
			segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<Parameters>(M.m<Parameters>(-heightmapRange), M.m<Parameters>(fullGravityHeight),
				M.m((Parameters tp) => tp.AltitudeLimits[0].x), M.m<Parameters>(1), M.m((Parameters tp) => tp.AltitudeLimits[0].y),
				_focus.Parameters, M.m((Parameters tp)=>$"Lower Altitude Limit: {tp.AltitudeLimits[0].x}m"), delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						Vector2 v = tp.AltitudeLimits[i];
						v.x = f;
						tp.AltitudeLimits.Us[i] = v;
					}
				}, null, M.m<Parameters>(new ToolTip("What is the minimum altitude for all entries?")))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<Parameters>(M.m<Parameters>(-heightmapRange), M.m<Parameters>(fullGravityHeight),
				M.m((Parameters tp) => tp.AltitudeLimits[0].y), M.m<Parameters>(1), M.m((Parameters tp) => tp.AltitudeLimits[0].x),
				_focus.Parameters, M.m((Parameters tp)=>$"Upper Altitude Limit: {tp.AltitudeLimits[0].y}m"), delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						Vector2 v = tp.AltitudeLimits[i];
						v.y = f;
						tp.AltitudeLimits.Us[i] = v;
					}
				}, null, M.m<Parameters>(new ToolTip("What is the maximum altitude for all entries?")))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 10000, 1, 1500,
				M.m((Parameters tp) => tp.DistanceLimit[0]), "Distance Limit: {0}m", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.DistanceLimit.Us[i] = (int)f;
					}
				}, new ToolTip("What is the maximum distance for all entries towards the nearest enemy?"))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Use projected Distance", new ToolTip("When turned on, a ground projected distance will be used. This is better for fights with a lot of vertical freedom."), delegate (Parameters tp, bool b)
			{
				for (int i = 0; i < 6; i++)
				{
					tp.ProjectedDistance.Us[i] = b;
				}
			}, (tp) => tp.ProjectedDistance[0])).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 3600, 1, 90,
				M.m((Parameters tp) => tp.MaximumPenaltyTime[0]), "Maximum Penalty Time: {0}s", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.MaximumPenaltyTime.Us[i] = (int)f;
					}
				}, new ToolTip("How much penalty time can a participant have, before it self-destructs?"))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 10000, 1, 10000,
				M.m((Parameters tp) => tp.MaximumSpeed[0]), "Maximum Speed: {0}m/s", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.MaximumSpeed.Us[i] = (int)f;
					}
				}, new ToolTip("Going over the maximum Speed will add penalty time. If soft limits are active, it will deplete the buffer first."))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Use soft Limits", new ToolTip("When turned on, entries are given the chance to be considered in bounds under certain conditions. Turned off, entries will pickup penalty time as long as they are outside the bounds."), delegate (Parameters tp, bool b)
			{
				for (int i = 0; i < 6; i++)
				{
					tp.SoftLimits.Us[i] = b;
				}
			}, (tp) => tp.SoftLimits[0])).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 100, 1, 50,
			M.m((Parameters tp) => tp.EnemyAttackPercentage[0]), "Enemy Attack Percentage: {0}%", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.EnemyAttackPercentage.Us[i] = (int) f;
					}
				}, new ToolTip("When determining if an entry is violating the distance limit, this percentage must be reached or it is considered fleeing from too many enemies."))).SetConditionalDisplayFunction(() => _focus.Parameters.UniformRules);
			#region Puffer-Einstellungen
			ScreenSegmentStandard segment2 = CreateStandardSegment();
			segment2.SetConditionalDisplay(() => _focus.Parameters.UniformRules&&_focus.Parameters.SoftLimits[0]);
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 3600, 1, 0,
				M.m((Parameters tp) => tp.MaximumBufferTime[0]), "Maximum Buffer Time: {0}s", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.MaximumBufferTime.Us[i] = (int)f;
					}
				}, new ToolTip("How much buffer time can a participant have, before it gains penalty time? This Buffer will reset, once a participant is considered to be back in bounds.")));
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
				M.m((Parameters tp) => tp.DistanceReverse[0]), "Distance Reversal: {0}m/s", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.DistanceReverse.Us[i] = (int)f;
					}
				}, new ToolTip("A positive value permits a certain fleeing speed, while a negative value requires a certain closing speed. It assumes, that the nearest enemy is stationary.")));
			segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
				M.m((Parameters tp) => tp.AltitudeReverse[0]), "Altitude Reversal: {0}m/s", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.AltitudeReverse.Us[i] = (int)f;
					}
				}, new ToolTip("A positive value allows to move away from the limits at a maximum speed, while a negative value requires to move towards the limit with a certain speed. Recommended is a negative value.")));
			#endregion
			ScreenSegmentStandard segment3 = CreateStandardSegment();
			segment3.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 3600, 1, 900,
				M.m((Parameters tp) => tp.MaximumTime), "Maximum Time: {0}s", delegate (Parameters tp, float f)
				{
					tp.MaximumTime.Us = (int)f;
				}, new ToolTip("What is the maximum battle time? Once reached, the game will be paused. Unpausing it will activate \"Overtime\".")));
			segment3.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 3600, 1, 60,
				M.m((Parameters tp) => tp.Overtime), "Overtime: {0}s", delegate (Parameters tp, float f)
				{
					tp.Overtime.Us = (int)f;
				}, new ToolTip("The length of one Overtime-section. Set it to 0 to only have one infinte long section.")));
			segment3.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Distribute Resources", new ToolTip("The materials set below become the team maximum, which gets distributed along the entries. Any excess goes into team storage."), delegate (Parameters tp, bool b)
			{
				tp.DistributeLocalResources.Us = b;
			}, (Parameters tp) => tp.DistributeLocalResources));
			segment3.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Even Resources", new ToolTip("Give all Teams the same amount of resources of make it uneven."), delegate (Parameters tp, bool b)
			{
				tp.SameMaterials.Us = b;
			}, (tp) => tp.SameMaterials.Us));
			#region Resourcen-Einstellungen
			ScreenSegmentStandard segmentIdenticalMaterials = CreateStandardSegment();
			segmentIdenticalMaterials.SetConditionalDisplay(() => _focus.Parameters.SameMaterials);
			_focus.Parameters.EnsureEnoughData();
			segmentIdenticalMaterials.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Infinte Resources", new ToolTip("Give all Teams infinte Materials. It is unclear how it currently interacts with local Resources."), delegate (Parameters tp, bool b)
			{
				for (int i = 0; i < 6; i++)
				{
					tp.InfinteResourcesPerTeam.Us[i] = b;
				}
			}, (tp) =>
			{
				return tp.InfinteResourcesPerTeam[0];
			}));
			segmentIdenticalMaterials.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 1000000, 1, 10000,
				M.m((Parameters tp) => tp.ResourcesPerTeam[0]), "{0} Materials for all Teams", delegate (Parameters tp, float f)
				{
					for (int i = 0; i < 6; i++)
					{
						tp.ResourcesPerTeam.Us[i] = (int)f;
					}
				}, new ToolTip("This determines with how much Materials a participant can spawn at maximum. Teams with more entries are naturally getting more Resources."))).SetConditionalDisplayFunction(() => _focus.Parameters.SameMaterials && !_focus.Parameters.InfinteResourcesPerTeam[0]);
			ScreenSegmentStandard segmentIndividualMaterials = CreateStandardSegment();
			segmentIndividualMaterials.SetConditionalDisplay(() => !_focus.Parameters.SameMaterials);
			for (int i = 0; i < 6; i++)
			{
				int index = i;
				segmentIndividualMaterials.AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, $"Infinte Resources for Team {index + 1}", new ToolTip($"Give Team {index + 1} infinte Materials. It is unclear how it currently interacts with local Resources."), delegate (Parameters tp, bool b)
					{
						tp.InfinteResourcesPerTeam.Us[index] = b;
					}, (tp) =>
					{
						return tp.InfinteResourcesPerTeam[index];
					})).SetConditionalDisplayFunction(() => index < _focus.Parameters.ActiveFactions);
				segmentIndividualMaterials.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, 1000000, 1, 10000,
					M.m((Parameters tp) => tp.ResourcesPerTeam[index]), $"{{0}} Materials for Team {index + 1}", delegate (Parameters tp, float f)
					  {
						  tp.ResourcesPerTeam.Us[index] = (int) f;
					  }, new ToolTip("This determines with how much Materials a participant of this team can spawn at maximum. Teams with more entries are naturally getting more Resources."))).SetConditionalDisplayFunction(() => index < _focus.Parameters.ActiveFactions && !_focus.Parameters.InfinteResourcesPerTeam[index]);
			}
			#endregion
			sectionsNorthSouth = WorldSpecification.i.BoardLayout.NorthSouthBoardSectionCount - 1;
			sectionsEastWest = WorldSpecification.i.BoardLayout.EastWestBoardSectionCount - 1;
			ScreenSegmentStandardHorizontal horizontal = CreateStandardHorizontalSegment();
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, sectionsEastWest, 1, sectionsEastWest / 2,
				M.m((Parameters tp) => tp.EastWestBoard), "East-West Board {0}", delegate (Parameters tp, float f)
				{
					tp.EastWestBoard.Us = (int)f;
					_focus.MoveCam();
				}, new ToolTip("Change the East-West Board index. In the Map it is the first number, 0 is at the left side.")));
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, 0, sectionsNorthSouth, 1, sectionsNorthSouth / 2,
				M.m((Parameters tp) => tp.NorthSouthBoard), "North-South Board {0}", delegate (Parameters tp, float f)
				{
					tp.NorthSouthBoard.Us = (int)f;
					_focus.MoveCam();
				}, new ToolTip("Change the North-South Board index. In the Map it is the second number, 0 is at the bottom.")));
			horizontal.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<Parameters>(M.m((Parameters tp) => -180f / tp.ActiveFactions), M.m((Parameters tp) => 180f / tp.ActiveFactions),
				M.m((Parameters tp) => tp.Rotation), M.m<Parameters>(1), M.m<Parameters>(0),
				_focus.Parameters, M.m((Parameters tp)=>$"Rotation: {tp.Rotation}°"), delegate (Parameters tp, float f)
				{
					tp.Rotation.Us = (int)f;
				}, null, M.m<Parameters>(new ToolTip("Rotate everything around the center before starting the fight"))));
			horizontal = CreateStandardHorizontalSegment();
			terrainsPerSection = WorldSpecification.i.BoardLayout.TerrainsPerBoard;
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -terrainsPerSection / 2, terrainsPerSection / 2, 1, 0,
			M.m((Parameters tp) => tp.EastWestTerrain), "East-West Terrain {0}", delegate (Parameters tp, float f)
				{
					tp.EastWestTerrain.Us = (int) f;
					_focus.MoveCam();
				}, new ToolTip("Change the offset on the east-west axis, measured in whole terrains. 0 is the center of a map tile.")));
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -terrainsPerSection / 2, terrainsPerSection / 2, 1, 0,
				M.m((Parameters tp) => tp.NorthSouthTerrain), "North-South Terrain {0}", delegate (Parameters tp, float f)
				{
					tp.NorthSouthTerrain.Us = (int) f;
					_focus.MoveCam();
				}, new ToolTip("Change the offset on the north-south axis, measured in whole terrains. 0 is the center of a map tile.")));
			terrainSize = WorldSpecification.i.BoardLayout.TerrainSize;
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -terrainSize / 2, terrainSize / 2, 1, 0,
				M.m((Parameters tp) => tp.EastWestOffset), "East-West Offset {0}m", delegate (Parameters tp, float f)
				{
					tp.EastWestOffset.Us = f;
					_focus.MoveCam();
				}, new ToolTip("Change the offset on the east-west axis, measured in meters. 0 is the center of a terrain.")));
			horizontal.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Parameters>.Quick(_focus.Parameters, -terrainSize / 2, terrainSize / 2, 1, 0,
				M.m((Parameters tp) => tp.NorthSouthOffset), "North-South Offset {0}m", delegate (Parameters tp, float f)
				{
					tp.NorthSouthOffset.Us = f;
					_focus.MoveCam();
				}, new ToolTip("Change the offset on the north-south axis, measured in meters. 0 is the center of a terrain.")));
			CreateStandardSegment().AddInterpretter(SubjectiveToggle<Parameters>.Quick(_focus.Parameters, "Pause on Victory", new ToolTip("When active, the game will be paused once a winner has been determined."), delegate (Parameters tp, bool b)
			{
				tp.PauseOnVictory.Us = b;
			}, (tp) => tp.PauseOnVictory));
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
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quickload Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) =>
				{
					t.LoadSettings();
					t.MoveCam();
					TriggerScreenRebuild();
				}));
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