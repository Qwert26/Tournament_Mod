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
                M.m<TournamentParameters>(_focus.Parameters.StartingDistance), "Starting Distance", delegate (TournamentParameters tp, float f)
                {
                    tp.StartingDistance.Us = (int)f;
                }, new ToolTip("The distance from the center towards the \"Flagship(s)\" of a team.")));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
                M.m<TournamentParameters>(_focus.Parameters.SpawngapLR), "Spawngap Left-Right", delegate (TournamentParameters tp, float f)
                {
                    tp.SpawngapLR.Us = (int)f;
                }, new ToolTip("How many meters should entries on the same team be apart in the left-right direction?")));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
                M.m<TournamentParameters>(_focus.Parameters.SpawngapFB), "Spawngap Forward-Backward", delegate (TournamentParameters tp, float f)
                {
                    tp.SpawngapFB.Us = (int)f;
                }, new ToolTip("How many meters should entries on the same team be apart in the forward-backward direction?")));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 3000, 1, _focus.Parameters.AltitudeLimits.Upper,
                M.m<TournamentParameters>(_focus.Parameters.AltitudeLimits.Lower), "Lower Altitude Limit", delegate (TournamentParameters tp, float f)
                {
                    tp.AltitudeLimits.Lower = f;
                }, new ToolTip("What is the minimum altitude for all entries?")));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 3000, 1, _focus.Parameters.AltitudeLimits.Lower,
                M.m<TournamentParameters>(_focus.Parameters.AltitudeLimits.Upper), "Upper Altitude Limit", delegate (TournamentParameters tp, float f)
                {
                    tp.AltitudeLimits.Upper = f;
                }, new ToolTip("What is the maximum altitude for all entries?")));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters,0,10000,1,1500,
                M.m<TournamentParameters>(_focus.Parameters.DistanceLimit),"Distance Limit",delegate(TournamentParameters tp,float f) {
                    tp.DistanceLimit.Us = (int)f;
                },new ToolTip("What is the maximum distance for all entries towards the nearest enemy?")));
            segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use projected Distance", new ToolTip("When turned on, a ground projected distance will be used. This is better for fights with a lot of vertical freedom."), delegate (TournamentParameters tp, bool b)
            {
                tp.ProjectedDistance.Us = b;
            }, (tp) => tp.ProjectedDistance.Us));
            segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 90,
                M.m<TournamentParameters>(_focus.Parameters.MaximumPenaltyTime), "Maximum Penalty Time", delegate (TournamentParameters tp, float f)
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
                M.m<TournamentParameters>(_focus.Parameters.MaximumBufferTime), "Maximum Buffer Time", delegate (TournamentParameters tp, float f)
                {
                    tp.MaximumBufferTime.Us = (int)f;
                }, new ToolTip("How much buffer time can a participant have, before it gains penalty time? This Buffer will reset, once a participant is considered to be back in bounds.")));
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
                M.m<TournamentParameters>(_focus.Parameters.DistanceReverse), "Distance Reversal", delegate (TournamentParameters tp, float f)
                {
                    tp.DistanceReverse.Us = (int)f;
                }, new ToolTip(_focus.Parameters.DistanceReverse>0?$"Participants are allowed to move up to {_focus.Parameters.DistanceReverse}m/s away from the nearest, assumed to be stationary, enemy.":
                _focus.Parameters.DistanceReverse<0?$"Participants are required to move at least {-_focus.Parameters.DistanceReverse}m/s towards the nearest, assumed to be stationary, enemy.":
                "Participants are allowed to hold their distance to the nearest, assumed to be stationary, enemy, even if it is outside the limit.")));
            segment2.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
                M.m<TournamentParameters>(_focus.Parameters.AltitudeReverse), "Altitude Reversal", delegate (TournamentParameters tp, float f)
                {
                    tp.AltitudeReverse.Us = (int)f;
                }, new ToolTip(_focus.Parameters.AltitudeReverse > 0 ? $"Participants are allowed to move up to {_focus.Parameters.AltitudeReverse}m/s away from the altitude limits." :
                _focus.Parameters.AltitudeReverse < 0 ? $"Participants are required to move at least {-_focus.Parameters.AltitudeReverse}m/s towards the altitude limits." :
                "Participants are allowed to hold their altitude, even if it is outside the limits.")));
            #endregion
            ScreenSegmentStandard segment3 = CreateStandardSegment();
            segment3.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(3600),
                M.m<TournamentParameters>(_focus.Parameters.MaximumTime), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Match Time"), delegate (TournamentParameters tp, float f)
                {
                    tp.MaximumBufferTime.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip("What is the expected maximum match time? The game will pause once this many seconds have passed. You can unpause it manually to activate \"Overtime\"."))));
            segment3.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(3600),
                M.m<TournamentParameters>(_focus.Parameters.Overtime), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Overtime"), delegate (TournamentParameters tp, float f)
                {
                    tp.Overtime.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip("How long is one overtime-section? After each section the game will once again pause. Set to 0 to only have a single section without any end."))));
            segment3.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Local Resources", new ToolTip("Enable or Disable local Resources."), delegate (TournamentParameters tp, bool b)
               {
                   tp.LocalResources.Us = b;
               }, (tp) => tp.LocalResources.Us));
            segment3.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Even Resources", new ToolTip("Give all Teams the same amount of resources of make it uneven."), delegate (TournamentParameters tp, bool b)
            {
                tp.SameMaterials.Us = b;
            }, (tp) => tp.SameMaterials.Us));
            #region Resourcen-Einstellungen
            ScreenSegmentStandard segmentIdenticalMaterials1 = CreateStandardSegment();
            segmentIdenticalMaterials1.SetConditionalDisplay(() => _focus.Parameters.SameMaterials);
            _focus.Parameters.EnsureEnoughData();
            segmentIdenticalMaterials1.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Infinte Resources", new ToolTip("Give all Teams infinte Materials"), delegate (TournamentParameters tp, bool b)
            {
                for (int i = 0; i < tp.ActiveFactions; i++)
                {
                    tp.InfinteResourcesPerTeam.Us[i] = b;
                }
            }, (tp) => tp.InfinteResourcesPerTeam[0]));
            ScreenSegmentStandard segmentIdenticalMaterials2 = CreateStandardSegment();
            segmentIdenticalMaterials2.SetConditionalDisplay(() => _focus.Parameters.SameMaterials && _focus.Parameters.InfinteResourcesPerTeam[0]);
            segmentIdenticalMaterials2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(1000000),
                M.m<TournamentParameters>(_focus.Parameters.ResourcesPerTeam[0]), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Resources for all Teams"), delegate (TournamentParameters tp, float f)
                {
                    for (int i = 0; i < tp.ActiveFactions; i++)
                    {
                        tp.ResourcesPerTeam.Us[i] = (int)f;
                    }
                }, null, M.m<TournamentParameters>(new ToolTip(_focus.Parameters.LocalResources ? "Give each entry this amount of Materials to start with." : "Give all Teams this amount of Materials to start with."))));
            ScreenSegmentStandard segmentIndividualMaterials = CreateStandardSegment();
            segmentIndividualMaterials.SetConditionalDisplay(() => !_focus.Parameters.SameMaterials);
            for (int i = 0; i < _focus.Parameters.ActiveFactions; i++) {
                segmentIndividualMaterials.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, $"Infinte Resources for Team {i}", new ToolTip($"Give Team {i} infinte Materials."), delegate (TournamentParameters tp, bool b)
                {
                    tp.InfinteResourcesPerTeam.Us[i] = b;
                    TriggerScreenRebuild();
                }, (tp) => tp.InfinteResourcesPerTeam[i]));
                if (!_focus.Parameters.InfinteResourcesPerTeam[i]) {
                    segmentIndividualMaterials.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(1000000),
                        M.m<TournamentParameters>(_focus.Parameters.ResourcesPerTeam[i]), M.m<TournamentParameters>(1), _focus.Parameters,
                        M.m<TournamentParameters>($"Resources for Team {i}"), delegate (TournamentParameters tp, float f)
                        {
                            tp.ResourcesPerTeam.Us[i] = (int)f;
                        }, null, M.m<TournamentParameters>(new ToolTip(_focus.Parameters.LocalResources ? $"Give each entry on Team {i} this amount of Materials to start with." : $"Give Team {i} this amount of Materials to start with."))));
                }
            }
            #endregion
            ScreenSegmentTable table1 = CreateTableSegment(3,2);
            table1.eTableOrder = ScreenSegmentTable.TableOrder.Rows;
            table1.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, sectionsEastWest, 1, sectionsEastWest / 2,
                M.m<TournamentParameters>(_focus.Parameters.EastWestBoard), "East-West Board", delegate (TournamentParameters tp, float f)
                {
                    tp.EastWestBoard.Us = (int)f;
                }, new ToolTip("Change the East-West Board index. In the Map it is the first number, 0 is at the left side.")));
            table1.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, sectionsNorthSouth, 1, sectionsNorthSouth / 2,
                M.m<TournamentParameters>(_focus.Parameters.NorthSouthBoard), "North-South Board", delegate (TournamentParameters tp, float f)
                {
                    tp.NorthSouthBoard.Us = (int)f;
                }, new ToolTip("Change the North-South Board index. In the Map it is the second number, 0 is at the bottom.")));
            table1.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -180/_focus.Parameters.ActiveFactions, 180/_focus.Parameters.ActiveFactions, 1, 0,
                M.m<TournamentParameters>(_focus.Parameters.Rotation), "Rotation", delegate (TournamentParameters tp, float f)
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
        public override Action OnSelectTab => () =>
        {
            base.OnSelectTab();
            sectionsNorthSouth = WorldSpecification.i.BoardLayout.NorthSouthBoardSectionCount - 1;
            sectionsEastWest = WorldSpecification.i.BoardLayout.EastWestBoardSectionCount - 1;
        };
    }
}
