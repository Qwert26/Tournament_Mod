using System;
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
        public BaseSettingsTab(ConsoleWindow window, Tournament focus) : base(window, focus) {
            Name = new Content("Base Settings", "Setup the basic Parameters of the Fight.");
        }
        public override void Build()
        {
            base.Build();
            CreateHeader("Customize basic fighting parameters:", new ToolTip(""));
            ScreenSegmentStandard segment = CreateStandardSegment();
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 20000, 1, 2500,
                M.m((TournamentParameters tp)=>tp.StartingDistance), "Starting Distance", delegate (TournamentParameters tp, float f)
                {
                    tp.StartingDistance.Us = (int)f;
                }, new ToolTip("The distance from the center towards the \"Flagship(s)\" of a team.")));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
                M.m((TournamentParameters tp) => tp.SpawngapLR), "Spawngap Left-Right", delegate (TournamentParameters tp, float f)
                {
                    tp.SpawngapLR.Us = (int)f;
                }, new ToolTip("How many meters should entries on the same team be apart in the left-right direction?")));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
                M.m((TournamentParameters tp) => tp.SpawngapFB), "Spawngap Forward-Backward", delegate (TournamentParameters tp, float f)
                {
                    tp.SpawngapFB.Us = (int)f;
                }, new ToolTip("How many meters should entries on the same team be apart in the forward-backward direction?")));
            segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>(M.m<TournamentParameters>(-500), M.m<TournamentParameters>(3000),
                M.m((TournamentParameters tp) => tp.AltitudeLimits.Lower), M.m<TournamentParameters>(1), M.m((TournamentParameters tp) => tp.AltitudeLimits.Upper),
                _focus.Parameters, M.m<TournamentParameters>("Lower Altitude Limit"), delegate (TournamentParameters tp, float f)
                {
                    tp.AltitudeLimits.Lower = f;
                }, null, M.m<TournamentParameters>(new ToolTip("What is the minimum altitude for all entries?"))));
            segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>(M.m<TournamentParameters>(-500), M.m<TournamentParameters>(3000),
                M.m((TournamentParameters tp) => tp.AltitudeLimits.Upper), M.m<TournamentParameters>(1), M.m((TournamentParameters tp) => tp.AltitudeLimits.Lower),
                _focus.Parameters, M.m<TournamentParameters>("Upper Altitude Limit"), delegate (TournamentParameters tp, float f)
                {
                    tp.AltitudeLimits.Upper = f;
                }, null, M.m<TournamentParameters>(new ToolTip("What is the maximum altitude for all entries?"))));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 10000, 1, 1500,
                M.m((TournamentParameters tp) => tp.DistanceLimit), "Distance Limit", delegate (TournamentParameters tp, float f)
                {
                    tp.DistanceLimit.Us = (int)f;
                }, new ToolTip("What is the maximum distance for all entries towards the nearest enemy?")));
            segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use projected Distance", new ToolTip("When turned on, a ground projected distance will be used. This is better for fights with a lot of vertical freedom."), delegate (TournamentParameters tp, bool b)
            {
                tp.ProjectedDistance.Us = b;
            }, (tp) => tp.ProjectedDistance.Us));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 90,
                M.m((TournamentParameters tp) => tp.MaximumPenaltyTime), "Maximum Penalty Time", delegate (TournamentParameters tp, float f)
                {
                    tp.MaximumPenaltyTime.Us = (int)f;
                }, new ToolTip("How much penalty time can a participant have, before it self-destructs?")));
            segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use soft Limits", new ToolTip("When turned on, entries are given the chance to be considered in bounds under certain conditions. Turned off, entries will pickup penalty time as long as they are outside the bounds."), delegate (TournamentParameters tp, bool b)
            {
                tp.SoftLimits.Us = b;
            }, (tp) => tp.SoftLimits.Us));
            #region Puffer-Einstellungen
            ScreenSegmentStandard segment2 = CreateStandardSegment();
            segment2.SetConditionalDisplay(() => _focus.Parameters.SoftLimits.Us);
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 90,
                M.m((TournamentParameters tp) => tp.MaximumBufferTime), "Maximum Buffer Time", delegate (TournamentParameters tp, float f)
                {
                    tp.MaximumBufferTime.Us = (int)f;
                }, new ToolTip("How much buffer time can a participant have, before it gains penalty time? This Buffer will reset, once a participant is considered to be back in bounds.")));
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
                M.m((TournamentParameters tp) => tp.DistanceReverse), "Distance Reversal", delegate (TournamentParameters tp, float f)
                {
                    tp.DistanceReverse.Us = (int)f;
                }, new ToolTip("A positive value permits a certain fleeing speed, while a negative value requires a certain closing speed. It assumes, that the nearest enemy is stationary.")));
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
                M.m((TournamentParameters tp) => tp.AltitudeReverse), "Altitude Reversal", delegate (TournamentParameters tp, float f)
                {
                    tp.AltitudeReverse.Us = (int)f;
                }, new ToolTip("A positive value permits a certain fleeing speed, while a negative value requires a certain closing speed.")));
            #endregion
            ScreenSegmentStandard segment3 = CreateStandardSegment();
            segment3.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 900,
                M.m((TournamentParameters tp) => tp.MaximumTime), "Maximum Time", delegate (TournamentParameters tp, float f)
                {
                    tp.MaximumTime.Us = (int)f;
                }, new ToolTip("What is the maximum battle time? Once reached, the game will be paused. Unpausing it will activate \"Overtime\".")));
            segment3.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 60,
                M.m((TournamentParameters tp) => tp.Overtime), "Overtime", delegate (TournamentParameters tp, float f)
                {
                    tp.Overtime.Us = (int)f;
                }, new ToolTip("The length of one Overtime-section. Set it to 0 to only have one infinte long section.")));
            segment3.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Local Resources", new ToolTip("Enable or Disable local Resources."), delegate (TournamentParameters tp, bool b)
               {
                   tp.LocalResources.Us = b;
               }, (tp) => tp.LocalResources.Us));
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
                M.m((TournamentParameters tp) => tp.ResourcesPerTeam[0]), "Materials for all Teams", delegate (TournamentParameters tp, float f)
                {
                    for (int i = 0; i < tp.ActiveFactions; i++)
                    {
                        tp.ResourcesPerTeam.Us[i] = (int)f;
                    }
                }, new ToolTip("For local Resources, this determines with how much Materials a participant can spawn at maximum, for global resources, it determines the amount of Materials in storage."))).SetConditionalDisplayFunction(() => _focus.Parameters.SameMaterials && !_focus.Parameters.InfinteResourcesPerTeam[0]);
            ScreenSegmentStandard segmentIndividualMaterials = CreateStandardSegment();
            segmentIndividualMaterials.SetConditionalDisplay(() => !_focus.Parameters.SameMaterials);
            for (int i = 0; i < _focus.Parameters.ActiveFactions; i++) {
                int index = i;
                segmentIndividualMaterials.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, $"Infinte Resources for Team {index}", new ToolTip($"Give Team {index} infinte Materials."), delegate (TournamentParameters tp, bool b)
                {
                    tp.InfinteResourcesPerTeam.Us[index] = b;
                }, (tp) => {
                    return tp.InfinteResourcesPerTeam[index];
                    }));
                segmentIndividualMaterials.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 1000000, 1, 10000,
                    M.m((TournamentParameters tp) => tp.ResourcesPerTeam[index]), $"Materials for Team {index}", delegate (TournamentParameters tp, float f)
                    {
                        tp.ResourcesPerTeam.Us[index] = (int)f;
                    }, new ToolTip("For local Resources, this determines with how much Materials a participant of this team can spawn at maximum, for global resources, it determines the amount of Materials in storage for this team."))).SetConditionalDisplayFunction(() => !_focus.Parameters.InfinteResourcesPerTeam[index]);
            }
            #endregion
            sectionsNorthSouth = WorldSpecification.i.BoardLayout.NorthSouthBoardSectionCount - 1;
            sectionsEastWest = WorldSpecification.i.BoardLayout.EastWestBoardSectionCount - 1;
            ScreenSegmentTable table1 = CreateTableSegment(3,2);
            table1.eTableOrder = ScreenSegmentTable.TableOrder.Rows;
            table1.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, sectionsEastWest, 1, sectionsEastWest / 2,
                M.m((TournamentParameters tp) => tp.EastWestBoard), "East-West Board", delegate (TournamentParameters tp, float f)
                {
                    tp.EastWestBoard.Us = (int)f;
                    _focus.MoveCam();
                }, new ToolTip("Change the East-West Board index. In the Map it is the first number, 0 is at the left side.")));
            table1.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, sectionsNorthSouth, 1, sectionsNorthSouth / 2,
                M.m((TournamentParameters tp) => tp.NorthSouthBoard), "North-South Board", delegate (TournamentParameters tp, float f)
                {
                    tp.NorthSouthBoard.Us = (int)f;
                    _focus.MoveCam();
                }, new ToolTip("Change the North-South Board index. In the Map it is the second number, 0 is at the bottom.")));
            table1.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -180/_focus.Parameters.ActiveFactions, 180/_focus.Parameters.ActiveFactions, 1, 0,
                M.m((TournamentParameters tp) => tp.Rotation), "Rotation", delegate (TournamentParameters tp, float f)
                {
                    tp.Rotation.Us = (int)f;
                }, new ToolTip("Rotate the entire Field before the fight starts.")));
            table1.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Save Settings", new ToolTip("Saves the current Parameters into the Mod-Folder"), (t) => t.SaveSettings()));
            table1.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) => t.LoadDefaults()));
            table1.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use Default Keymap", new ToolTip("Uses the internal fixed keymap instead of your customized keymap."), delegate (TournamentParameters tp, bool b)
               {
                   tp.DefaultKeys.Us = b;
               }, (tp) => tp.DefaultKeys.Us));
        }
    }
}
