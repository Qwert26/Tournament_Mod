using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using Tournament.Serialisation;
using BrilliantSkies.Ui.Consoles.Getters;
using UnityEngine;
using BrilliantSkies.Ftd.Planets.World;
namespace Tournament.UI
{
	public class TeamSplittedRules : SuperScreen<Tournament>
	{
		private int heightmapRange;
		private float fullGravityHeight;
		public TeamSplittedRules(ConsoleWindow window, Tournament focus) : base(window, focus) {
			Name = new Content("Team-splitted Rules", "You can set team-specific rules for spawning and penaltytime-pickup here.");
		}
		public override void Build()
		{
			CreateHeader("Uniform Rules are active", new ToolTip("Team-based Spawn- and Penalty-Rules are not currently active.")).SetConditionalDisplay(() => _focus.Parameters.UniformRules);
			ScreenSegmentStandard segment = CreateStandardSegment();
			segment.SetConditionalDisplay(() => _focus.Parameters.UniformRules);
			segment.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Use Team-based Spawn- and Penalty-Rules", new ToolTip("Activate the usage of Team-based Spawn- and Penalty-Rules."),
				delegate (TournamentParameters tp)
				{
					tp.UniformRules.Us = false;
				}));
			heightmapRange = WorldSpecification.i.BoardLayout.WorldHeightAndDepth;
			fullGravityHeight = WorldSpecification.i.Physics.SpaceIsFullAgain;
			for (int i = 0; i < 6; i++) {
				int index = i;
				CreateHeader($"Rules for Team {index + 1}", new ToolTip($"These are the DQ-Rules specific to Members of Team {index + 1}.")).SetConditionalDisplay(() => !_focus.Parameters.UniformRules && index < _focus.Parameters.ActiveFactions);
				segment = CreateStandardSegment();
				segment.SetConditionalDisplay(() => !_focus.Parameters.UniformRules && index < _focus.Parameters.ActiveFactions);
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
					M.m((TournamentParameters tp) => tp.SpawngapFB[index]), "Spawngap Forward-Backward: {0}m", delegate (TournamentParameters tp, float f)
					{
						tp.SpawngapFB.Us[index] = (int)f;
					}, new ToolTip("How many meters should the entries on this team be apart on the z-axis?")));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -1000, 1000, 1, 0,
					M.m((TournamentParameters tp) => tp.SpawngapLR[index]), "Spawngap Left-Right: {0}m", delegate (TournamentParameters tp, float f)
					{
						tp.SpawngapLR.Us[index] = (int)f;
					}, new ToolTip("How many meters should the entries on this team be apart on the x-axis?")));
				segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>(M.m<TournamentParameters>(-heightmapRange), M.m<TournamentParameters>(fullGravityHeight),
					M.m((TournamentParameters tp) => tp.AltitudeLimits[index].x), M.m<TournamentParameters>(1), M.m((TournamentParameters tp) => tp.AltitudeLimits[index].y),
					_focus.Parameters, M.m((TournamentParameters tp)=>$"Lower Altitude Limit: {tp.AltitudeLimits[index].x}m"), delegate (TournamentParameters tp, float f)
					{
						Vector2 v = tp.AltitudeLimits[index];
						v.x = f;
						if (v.x > v.y)
						{
							v.y = v.x;
						}
						tp.AltitudeLimits.Us[index] = v;
					}, null, M.m<TournamentParameters>(new ToolTip("What is the maximum depth that entries on this team are allowed to go to?"))));
				segment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>(M.m<TournamentParameters>(-heightmapRange), M.m<TournamentParameters>(fullGravityHeight),
					M.m((TournamentParameters tp) => tp.AltitudeLimits[index].y), M.m<TournamentParameters>(1), M.m((TournamentParameters tp) => tp.AltitudeLimits[index].x),
					_focus.Parameters, M.m((TournamentParameters tp)=>$"Upper Altitude Limit: {tp.AltitudeLimits[index].y}m"), delegate (TournamentParameters tp, float f)
					{
						Vector2 v = tp.AltitudeLimits[index];
						v.y = f;
						if (v.y < v.x)
						{
							v.x = v.y;
						}
						tp.AltitudeLimits.Us[index] = v;
					}, null, M.m<TournamentParameters>(new ToolTip("What is the maximum height that entries on this team are allowed to go to?"))));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 10000, 1, 1500,
					M.m((TournamentParameters tp) => tp.DistanceLimit[index]), "Distance Limit: {0}m", delegate (TournamentParameters tp, float f)
					{
						tp.DistanceLimit.Us[index] = (int)f;
					}, new ToolTip("How many meters should the entries on this team be apart towards enemies?")));
				segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use projected Distance", new ToolTip("When calculating distances and if this is on, height will be ignored. This is good for fights with a lot of vertical freedom."), delegate (TournamentParameters tp, bool b)
				{
					tp.ProjectedDistance.Us[index] = b;
				}, (TournamentParameters tp) => tp.ProjectedDistance[index]));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 90,
					M.m((TournamentParameters tp) => tp.MaximumPenaltyTime[index]), "Maximum Penaltytime: {0}s", delegate (TournamentParameters tp, float f)
					{
						tp.MaximumPenaltyTime.Us[index] = (int)f;
					}, new ToolTip("How many seconds of penaltytime is an entry permitted to collect, before it gets removed from the fight?")));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 10000, 1, 10000,
					M.m((TournamentParameters tp) => tp.MaximumSpeed[index]), "Maximum Speed: {0}m/s", delegate (TournamentParameters tp, float f)
					{
						tp.MaximumSpeed.Us[index] = (int)f;
					}, new ToolTip("The maximum speed for entries on this team. Going over it will add penalty time. If soft limits are active, it will deplete the time buffer first.")));
				segment.AddInterpretter(SubjectiveToggle<TournamentParameters>.Quick(_focus.Parameters, "Use soft Limits", new ToolTip("With soft Limits, entries outside the boundaries are given the chance to get back into them. Without soft Limits, entries will pickup penalty time as long as they are outside the boundaries."), delegate (TournamentParameters tp, bool b)
				{
					tp.SoftLimits.Us[index] = b;
				}, (TournamentParameters tp) => tp.SoftLimits[index]));
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, 0, 3600, 1, 0,
					M.m((TournamentParameters tp) => tp.MaximumBufferTime[index]), "Maximum Buffertime: {0}s", delegate (TournamentParameters tp, float f)
					{
						tp.MaximumBufferTime.Us[index] = (int)f;
					}, new ToolTip("When an entry is considered out of bounds, a timebuffer will be depleted first. It will reset once an entry is considered back in bounds."))).
					SetConditionalDisplayFunction(() => _focus.Parameters.SoftLimits[index]);
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 500, 1, 3,
					M.m((TournamentParameters tp) => tp.DistanceReverse[index]), "Distance Reversal: {0}m/s", delegate (TournamentParameters tp, float f)
					{
						tp.DistanceReverse.Us[index] = (int)f;
					}, new ToolTip("A positive value permits a certain fleeing speed, while a negative value requires a certain closing speed. It assumes, that the nearest enemy is stationary."))).
					SetConditionalDisplayFunction(() => _focus.Parameters.SoftLimits[index]);
				segment.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<TournamentParameters>.Quick(_focus.Parameters, -500, 500, 1, -3,
					M.m((TournamentParameters tp) => tp.AltitudeReverse[index]), "Altitude Reversal: {0}m/s", delegate (TournamentParameters tp, float f)
					{
						tp.AltitudeReverse.Us[index] = (int)f;
					}, new ToolTip("A positive value allows to move away from the limits at a maximum speed, while a negative value requires to move towards the limit with a certain speed. Recommended is a negative value."))).
					SetConditionalDisplayFunction(() => _focus.Parameters.SoftLimits[index]);
			}
		}
	}
}