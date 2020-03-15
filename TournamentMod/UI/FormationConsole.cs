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
		private readonly int teamSize, teamIndex;
		private readonly DropDownMenuAlt<FormationType> formationOptions;
		public FormationConsole(CombinedFormation focus, int teamIndex, int currentTeamSize) : base(focus)
		{
			teamSize = currentTeamSize;
			this.teamIndex = teamIndex;
			formationOptions = new DropDownMenuAlt<FormationType>(TextAnchor.MiddleCenter);
			FormationType[] types = (FormationType[]) Enum.GetValues(typeof(FormationType));
			DropDownMenuAltItem<FormationType>[] items = new DropDownMenuAltItem<FormationType>[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				FormationType ft = types[i];
				items[i] = new DropDownMenuAltItem<FormationType>()
				{
					Name = ft.getFormation().Name,
					ToolTip = ft.getFormation().Description,
					ObjectForAction = ft
				};
			}
			formationOptions.SetItems(items);
		}
		protected override ConsoleWindow BuildInterface(string suggestedName = "")
		{
			ConsoleWindow window = NewWindow("Manage Formation", WindowSizing.GetRhs());
			window.DisplayTextPrompt = false;
			ConsoleUiScreen screen = window.Screen;
			screen.CreateHeader($"Current Formation List for Team {teamIndex + 1}", new ToolTip("Entries will spawn in these Formation from the top to the bottom. The last formation will act as an Overflow-buffer: " +
				"Taking in any entries, which would go over the last set count, this even works if its count has been set to 0."));
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
				sum += i != index ? _focus.formationEntrycount[i].Item2 : 0;
			}
			return teamSize - sum;
		}
	}
}
