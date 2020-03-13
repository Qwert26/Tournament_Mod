using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Tips;
using UnityEngine;
using BrilliantSkies.Ui.Consoles.Interpretters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Layouts.DropDowns;
using System;
namespace TournamentMod.UI
{
    using Formations;
    public class FormationConsole : ConsoleUi<CombinedFormation>
	{
		private ConsoleUiScreen _parentTab;
		private readonly int teamSize, teamIndex;
		private readonly DropDownMenuAlt<FormationType> formationOptions;
		public FormationConsole(CombinedFormation focus, ConsoleUiScreen parentTab, int teamIndex, int currentTeamSize) : base(focus)
		{
			_parentTab = parentTab;
			teamSize = currentTeamSize;
			this.teamIndex = teamIndex;
			formationOptions = new DropDownMenuAlt<FormationType>(TextAnchor.MiddleCenter);
			formationOptions.SetItems(
				new DropDownMenuAltItem<FormationType>()
				{
					Name= FormationType.Line.getFormation().Name,
					ToolTip=FormationType.Line.getFormation().Description,
					ObjectForAction=FormationType.Line
				},
				new DropDownMenuAltItem<FormationType>()
				{
					Name = FormationType.Wedge.getFormation().Name,
					ToolTip = FormationType.Wedge.getFormation().Description,
					ObjectForAction = FormationType.Wedge
				},
				new DropDownMenuAltItem<FormationType>()
				{
					Name = FormationType.DividedWedge.getFormation().Name,
					ToolTip = FormationType.DividedWedge.getFormation().Description,
					ObjectForAction = FormationType.DividedWedge
				},
				new DropDownMenuAltItem<FormationType>()
				{
					Name = FormationType.ParallelColumns.getFormation().Name,
					ToolTip = FormationType.ParallelColumns.getFormation().Description,
					ObjectForAction = FormationType.ParallelColumns
				},
				new DropDownMenuAltItem<FormationType>()
				{
					Name = FormationType.CommandedParallelColumns.getFormation().Name,
					ToolTip = FormationType.CommandedParallelColumns.getFormation().Description,
					ObjectForAction = FormationType.CommandedParallelColumns
				},
				new DropDownMenuAltItem<FormationType>()
				{
					Name = FormationType.RomanManipelBase.getFormation().Name,
					ToolTip = FormationType.RomanManipelBase.getFormation().Description,
					ObjectForAction = FormationType.RomanManipelBase
				},
				new DropDownMenuAltItem<FormationType>()
				{
					Name = FormationType.RomanManipelAttack.getFormation().Name,
					ToolTip = FormationType.RomanManipelAttack.getFormation().Description,
					ObjectForAction = FormationType.RomanManipelAttack
				},
				new DropDownMenuAltItem<FormationType>()
				{
					Name = FormationType.GuardLine.getFormation().Name,
					ToolTip = FormationType.GuardLine.getFormation().Description,
					ObjectForAction = FormationType.GuardLine
				});
		}
		protected override ConsoleWindow BuildInterface(string suggestedName = "")
		{
			ConsoleWindow window = NewWindow("Manage Formation", WindowSizing.GetRhs());
			window.DisplayTextPrompt = false;
			ConsoleUiScreen screen = window.Screen;
			screen.CreateHeader($"Current Formation List for Team {teamIndex + 1}", new ToolTip("Entries will spawn in these Formation from the top to the bottom. The last formation will act as an Overflow-buffer: " +
				"Taking in any entries, which would go over the last set count."));
			for (int i = 0; i < _focus.formationEntrycount.Count; i++)
			{
				int index = i;
				ScreenSegmentTable formationSegment = screen.CreateTableSegment(3, 3);
				formationSegment.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
				formationSegment.SqueezeTable = false;
				formationSegment.AddInterpretter(new DropDown<CombinedFormation, FormationType>(_focus, formationOptions, (CombinedFormation cf, FormationType ft) => cf.formationEntrycount[index].Item1 == ft, delegate (CombinedFormation cf, FormationType ft)
					{
						Tuple<FormationType, int> temp = cf.formationEntrycount[index];
						temp = new Tuple<FormationType, int>(ft, temp.Item2);
						cf.formationEntrycount[index] = temp;
					}), 0, 0);
				formationSegment.AddInterpretter(new Empty(), 1, 0);
				formationSegment.AddInterpretter(new Empty(), 2, 0);
				formationSegment.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Move Up", new ToolTip("Move this formation towards the front."), delegate (CombinedFormation cf)
					{
						Tuple<FormationType,int> v2i = cf.formationEntrycount[index];
						cf.formationEntrycount.RemoveAt(index);
						cf.formationEntrycount.Insert(index - 1, v2i);
						TriggerRebuild();
					}), 0, 1).SetConditionalDisplayFunction(() => index != 0);
				formationSegment.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Remove", new ToolTip("Remove this formation."), delegate (CombinedFormation cf)
					{
						cf.formationEntrycount.RemoveAt(index);
						TriggerRebuild();
					}), 1, 1);
				formationSegment.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Move Down", new ToolTip("Move this formation towards the back."), delegate (CombinedFormation cf)
				{
					Tuple<FormationType, int> v2i = cf.formationEntrycount[index];
					cf.formationEntrycount.RemoveAt(index);
					cf.formationEntrycount.Insert(index + 1, v2i);
					TriggerRebuild();
				}), 2, 1).SetConditionalDisplayFunction(() => index != _focus.formationEntrycount.Count - 1);
				formationSegment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<CombinedFormation>(M.m<CombinedFormation>(0), new VSA<float, CombinedFormation>(() => DetermineMaximum(index)),
					M.m((CombinedFormation cf) => cf.formationEntrycount[index].Item2), M.m<CombinedFormation>(1), M.m<CombinedFormation>(1),
					_focus, M.m((CombinedFormation cf)=>$"Entries for Formation: {cf.formationEntrycount[index].Item2}"), delegate (CombinedFormation cf, float t)
						{
							Tuple<FormationType,int> v2i = cf.formationEntrycount[index];
							v2i = new Tuple<FormationType, int>(v2i.Item1, (int) t);
							cf.formationEntrycount[index] = v2i;
						}, null, M.m<CombinedFormation>(new ToolTip("Select the amount of entries you want in this Formation. 0 means that it will be skipt."))), 0, 2);
				formationSegment.AddInterpretter(new Empty(), 1, 2);
				formationSegment.AddInterpretter(new Empty(), 2, 2);
			}
			ScreenSegmentStandardHorizontal buttons = screen.CreateStandardHorizontalSegment();
			buttons.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Add Formation", new ToolTip("Adds a new Formation at the end."), delegate (CombinedFormation cf)
				{
					cf.formationEntrycount.Add(new System.Tuple<FormationType, int>(FormationType.GuardLine, 0));
					TriggerRebuild();
				}));
			buttons.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Reset Formation", new ToolTip("Resets the Formation be a single simple line."), delegate (CombinedFormation cf)
				{
					cf.formationEntrycount.Clear();
					cf.formationEntrycount.Add(new System.Tuple<FormationType, int>(FormationType.Line, teamSize));
					TriggerRebuild();
				}));
			return window;
		}
		private float DetermineMaximum(int index)
		{
			int sum = 0;
			for (int i = 0; i < _focus.formationEntrycount.Count; i++)
			{
				sum += (i != index ? _focus.formationEntrycount[i].Item2 : 0);
			}
			return teamSize - sum;
		}
	}
}
