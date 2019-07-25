using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ftd.Planets.World;
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
            segment.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(40000),
                M.m<TournamentParameters>(_focus.Parameters.StartingDistance), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Starting Distance"), delegate (TournamentParameters tp, float f)
                {
                    tp.StartingDistance.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip("How many meters apart should the two teams or \"flagships\" be at the start of the fight?"))));
            segment.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(-1000), M.m<TournamentParameters>(1000),
                M.m<TournamentParameters>(_focus.Parameters.SpawngapLR), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Spawngap Left-Right"), delegate (TournamentParameters tp, float f)
                {
                    tp.SpawngapLR.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip("How many meters should entries on the same team be apart in the left-right direction?"))));
            segment.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(-1000), M.m<TournamentParameters>(1000),
                M.m<TournamentParameters>(_focus.Parameters.SpawngapFB), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Spawngap Forward-Backward"), delegate (TournamentParameters tp, float f)
                {
                    tp.SpawngapFB.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip("How many meters should entries on the same team be apart in the forward-backward direction?"))));
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
            segment.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(15000),
                M.m<TournamentParameters>(_focus.Parameters.DistanceLimit), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Maximum Distance"), delegate (TournamentParameters tp, float f)
                {
                    tp.DistanceLimit.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip("What is the maximum distance that enemies can be apart?"))));
            segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use projected Distance", new ToolTip("When turned on, a ground projected distance will be used. This is better for fights with a lot of vertical freedom."), delegate (TournamentParameters tp, bool b)
            {
                tp.ProjectedDistance.Us = b;
            }, (tp) => tp.ProjectedDistance.Us));
            segment.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(3600),
                M.m<TournamentParameters>(_focus.Parameters.DistanceLimit), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Penalty Time"), delegate (TournamentParameters tp, float f)
                {
                    tp.MaximumPenaltyTime.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip("What is the maximum penalty for entries before self-destruct?"))));
            segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use soft Limits", new ToolTip("When turned on, entries are given the chance to be considered in bounds under certain conditions. Turned off, entries will pickup penalty time as long as they are outside the bounds."), delegate (TournamentParameters tp, bool b)
            {
                tp.SoftLimits.Us = b;
            }, (tp) => tp.SoftLimits.Us));
            #region Puffer-Einstellungen
            ScreenSegmentStandard segment2 = CreateStandardSegment();
            segment2.SetConditionalDisplay(() => _focus.Parameters.SoftLimits.Us);
            segment2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(360),
                M.m<TournamentParameters>(_focus.Parameters.MaximumBufferTime), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Buffer Time"), delegate (TournamentParameters tp, float f)
                {
                    tp.MaximumBufferTime.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip("When considered out of bounds, the time buffer must deplete first before penalty time is added. Will be reset once an entry is considered back in bounds."))));
            segment2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(-300), M.m<TournamentParameters>(300),
                M.m<TournamentParameters>(_focus.Parameters.DistanceReverse), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Distance Reverse"), delegate (TournamentParameters tp, float f)
                {
                    tp.DistanceReverse.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip(_focus.Parameters.DistanceReverse.Us > 0 ? $"Entries are currently allowed to move at most {_focus.Parameters.DistanceReverse.Us}m/s away from the nearest enemy." :
                _focus.Parameters.DistanceReverse.Us < 0 ? $"Entries must move at least {-_focus.Parameters.DistanceReverse.Us}m/s towards the nearest enemy." : "Entries are allowed to hold the distance, even if it is above the limit."))));
            segment2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(-300), M.m<TournamentParameters>(300),
               M.m<TournamentParameters>(_focus.Parameters.AltitudeReverse), M.m<TournamentParameters>(1), _focus.Parameters,
               M.m<TournamentParameters>("Altitude Reverse"), delegate (TournamentParameters tp, float f)
               {
                   tp.DistanceReverse.Us = (int)f;
               }, null, M.m<TournamentParameters>(new ToolTip(_focus.Parameters.AltitudeReverse.Us > 0 ? $"Entries are currently allowed to move at most {_focus.Parameters.DistanceReverse.Us}m/s away from the altitude limits." :
               _focus.Parameters.DistanceReverse.Us < 0 ? $"Entries must move at least {-_focus.Parameters.AltitudeReverse.Us}m/s towards the altitude limits." : "Entries are allowed to hold their altitude, even if it is outside the limits."))));
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
            segment3.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Even Resources", new ToolTip("Give both Teams the same amount of resources of make it uneven."), delegate (TournamentParameters tp, bool b)
            {
                tp.SameMaterials.Us = b;
            }, (tp) => tp.SameMaterials.Us));
            #region Resourcen-Einstellungen
            ScreenSegmentStandard segment4_1 = CreateStandardSegment();
            segment4_1.SetConditionalDisplay(() => _focus.Parameters.SameMaterials.Us);
            segment4_1.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Infinte Resources both Teams", new ToolTip("Enable or Disable Infinte Resources for both teams"), delegate (TournamentParameters tp, bool b)
            {
                tp.InfinteResourcesTeam1.Us = tp.InfinteResourcesTeam2.Us = b;
            }, (tp) => tp.InfinteResourcesTeam1.Us));

            ScreenSegmentStandard segment4_2 = CreateStandardSegment();
            segment4_2.SetConditionalDisplay(() => !_focus.Parameters.InfinteResourcesTeam1.Us);
            segment4_2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(1000000),
                M.m<TournamentParameters>(_focus.Parameters.ResourcesTeam1), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Starting Materials both Teams"), delegate (TournamentParameters tp, float f)
                 {
                     tp.ResourcesTeam1.Us = tp.ResourcesTeam2.Us = (int)f;
                 }, null, M.m<TournamentParameters>(new ToolTip(_focus.Parameters.LocalResources.Us ? "Amount of localised Materials for each entry on both teams." : "Amount of centralised Materials for both teams."))));

            ScreenSegmentStandard segment4_3_1 = CreateStandardSegment();
            segment4_3_1.SetConditionalDisplay(() => !_focus.Parameters.SameMaterials.Us);
            segment4_3_1.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Infinte Resources Team 1", new ToolTip("Enable or Disable Infinte Resources for both teams"), delegate (TournamentParameters tp, bool b)
               {
                   tp.InfinteResourcesTeam1.Us = b;
               }, (tp) => tp.InfinteResourcesTeam1.Us));

            ScreenSegmentStandard segment4_3_2 = CreateStandardSegment();
            segment4_3_2.SetConditionalDisplay(() => !_focus.Parameters.InfinteResourcesTeam1.Us);
            segment4_3_2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(1000000),
                M.m<TournamentParameters>(_focus.Parameters.ResourcesTeam1), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Starting Materials Team 1"), delegate (TournamentParameters tp, float f)
                {
                    tp.ResourcesTeam1.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip(_focus.Parameters.LocalResources.Us ? "Amount of localised Materials for each entry on Team 1." : "Amount of centralised Materials for Team 1."))));

            ScreenSegmentStandard segment4_4_1 = CreateStandardSegment();
            segment4_4_1.SetConditionalDisplay(() => !_focus.Parameters.SameMaterials.Us);
            segment4_4_1.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Infinte Resources Team 2", new ToolTip("Enable or Disable Infinte Resources for both teams"), delegate (TournamentParameters tp, bool b)
            {
                tp.InfinteResourcesTeam2.Us = b;
            }, (tp) => tp.InfinteResourcesTeam2.Us));

            ScreenSegmentStandard segment4_4_2 = CreateStandardSegment();
            segment4_4_2.SetConditionalDisplay(() => !_focus.Parameters.InfinteResourcesTeam2.Us);
            segment4_4_2.AddInterpretter(new SubjectiveFloatClamped<TournamentParameters>(M.m<TournamentParameters>(0), M.m<TournamentParameters>(1000000),
                M.m<TournamentParameters>(_focus.Parameters.ResourcesTeam1), M.m<TournamentParameters>(1), _focus.Parameters,
                M.m<TournamentParameters>("Starting Materials Team 1"), delegate (TournamentParameters tp, float f)
                {
                    tp.ResourcesTeam2.Us = (int)f;
                }, null, M.m<TournamentParameters>(new ToolTip(_focus.Parameters.LocalResources.Us ? "Amount of localised Materials for each entry on Team 2." : "Amount of centralised Materials for Team 2."))));
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
            table1.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -90, 90, 1, 0,
                M.m<TournamentParameters>(_focus.Parameters.Rotation), "Rotation", delegate (TournamentParameters tp, float f)
                {
                    tp.Rotation.Us = (int)f;
                }, new ToolTip("Rotate the entire Field before the fight starts.")));
            table1.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Save Settings", new ToolTip("Saves the current Parameters into the Mod-Folder"), (t) => t.SaveSettingsNew()));
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
