﻿using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Interpretters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Layouts.DropDowns;
using System;
namespace TournamentMod.UI
{
	using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
	using Formations;
	/// <summary>
	/// GUI-Class for Formation-setting of a singular team.
	/// </summary>
	public class FormationConsole : ConsoleUi<CombinedFormation>
	{
		private readonly int teamSize, teamIndex;
		/// <summary>
		/// All the available formations.
		/// </summary>
		private readonly DropDownMenuAltItem<FormationType>[] items;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="focus">The Formation to modify</param>
		/// <param name="teamIndex">The teamindex of the formation, used for the title</param>
		/// <param name="currentTeamSize">The current teamsize, used for calculating maximums.</param>
		public FormationConsole(CombinedFormation focus, int teamIndex, int currentTeamSize) : base(focus)
		{
			teamSize = currentTeamSize;
			this.teamIndex = teamIndex;
			FormationType[] types = (FormationType[]) Enum.GetValues(typeof(FormationType));
			items = new DropDownMenuAltItem<FormationType>[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				FormationType ft = types[i];
				items[i] = new DropDownMenuAltItem<FormationType>()
				{
					Name = ft.GetFormation().Name,
					ToolTip = ft.GetFormation().Description,
					ObjectForAction = ft
				};
			}
		}
		/// <summary>
		/// Builds the Content.
		/// </summary>
		/// <param name="suggestedName"></param>
		/// <returns></returns>
		protected override ConsoleWindow BuildInterface(string suggestedName = "")
		{
			ConsoleWindow window = NewWindow(0, "Manage Formation", WindowSizing.GetRhs());
			window.DisplayTextPrompt = false;
			ConsoleUiScreen screen = window.Screen;
			screen.CreateHeader($"Current Formation List for Team {teamIndex + 1}", new ToolTip("Entries will spawn in these Formation from the top to the bottom. The last formation will act as an Overflow-buffer: " +
				"Taking in any entries, which would go over the last set count, this even works if its count has been set to 0."));
			for (int i = 0; i < _focus.formationData.Count; i++)
			{
				int index = i;
				ScreenSegmentTable formationSegment = screen.CreateTableSegment(3, 3);
				formationSegment.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
				formationSegment.SqueezeTable = false;
				formationSegment.SpaceBelow = formationSegment.SpaceAbove = 5;
				DropDownMenuAlt<FormationType> formationOptions = new DropDownMenuAlt<FormationType>();
				formationOptions.SetItems(items);
				formationSegment.AddInterpretter(new DropDown<CombinedFormation, FormationType>(_focus, formationOptions, (CombinedFormation cf, FormationType ft) => cf.formationData[index].Item1 == ft, delegate (CombinedFormation cf, FormationType ft)
					{
						Tuple<FormationType, int, bool> temp = cf.formationData[index];
						temp = new Tuple<FormationType, int, bool>(ft, temp.Item2, temp.Item3);
						cf.formationData[index] = temp;
					}), 0, 0);
				formationSegment.AddInterpretter(new Empty(), 1, 0);
				formationSegment.AddInterpretter(new Empty(), 2, 0);
				formationSegment.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Move Up", new ToolTip("Move this formation towards the front."), delegate (CombinedFormation cf)
					{
						Tuple<FormationType, int, bool> v2i = cf.formationData[index];
						cf.formationData.RemoveAt(index);
						cf.formationData.Insert(index - 1, v2i);
						TriggerRebuild();
					}), 0, 1).SetConditionalDisplayFunction(() => index != 0);
				formationSegment.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Remove", new ToolTip("Remove this formation."), delegate (CombinedFormation cf)
					{
						cf.formationData.RemoveAt(index);
						TriggerRebuild();
					}), 1, 1);
				formationSegment.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Move Down", new ToolTip("Move this formation towards the back."), delegate (CombinedFormation cf)
				{
					Tuple<FormationType, int, bool> v2i = cf.formationData[index];
					cf.formationData.RemoveAt(index);
					cf.formationData.Insert(index + 1, v2i);
					TriggerRebuild();
				}), 2, 1).SetConditionalDisplayFunction(() => index != _focus.formationData.Count - 1);
				formationSegment.AddInterpretter(new SubjectiveFloatClampedWithBarFromMiddle<CombinedFormation>(M.m<CombinedFormation>(0), new VSA<float, CombinedFormation>(() => DetermineMaximum(index)),
					M.m((CombinedFormation cf) => cf.formationData[index].Item2), M.m<CombinedFormation>(1), M.m<CombinedFormation>(1),
					_focus, M.m((CombinedFormation cf) => $"Entries for Formation: {cf.formationData[index].Item2}"), delegate (CombinedFormation cf, float t)
						  {
							  Tuple<FormationType, int, bool> v2i = cf.formationData[index];
							  v2i = new Tuple<FormationType, int, bool>(v2i.Item1, (int) t, v2i.Item3);
							  cf.formationData[index] = v2i;
						  }, null, M.m<CombinedFormation>(new ToolTip("Select the amount of entries you want in this Formation. 0 means that it will be skipt."))), 0, 2);
				formationSegment.AddInterpretter(new Empty(), 1, 2);
				formationSegment.AddInterpretter(SubjectiveToggle<CombinedFormation>.Quick(_focus, "Fleet", new ToolTip("Turning this on will put entries into a fleet with the designated Flagship."), delegate (CombinedFormation cf, bool set)
				  {
					  Tuple<FormationType, int, bool> v2i = cf.formationData[index];
					  v2i = new Tuple<FormationType, int, bool>(v2i.Item1, v2i.Item2, set);
					  cf.formationData[index] = v2i;
				  }, (cf) => cf.formationData[index].Item3), 2, 2).SetConditionalDisplayFunction(() => { return _focus.formationData[index].Item1.GetFormation().SupportsFleetForming; });
			}
			ScreenSegmentStandardHorizontal buttons = screen.CreateStandardHorizontalSegment();
			buttons.SpaceAbove = buttons.SpaceBelow = 5;
			buttons.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Add Formation", new ToolTip("Adds a new Formation at the end."), delegate (CombinedFormation cf)
				{
					cf.formationData.Add(new Tuple<FormationType, int, bool>(FormationType.GuardLine, 0, false));
					TriggerRebuild();
				}));
			buttons.AddInterpretter(SubjectiveButton<CombinedFormation>.Quick(_focus, "Reset Formation", new ToolTip("Resets the Formation be a single simple line."), delegate (CombinedFormation cf)
				{
					cf.formationData.Clear();
					cf.formationData.Add(new Tuple<FormationType, int, bool>(FormationType.Line, teamSize, false));
					TriggerRebuild();
				}));
			return window;
		}
		/// <summary>
		/// Determines the maximum amount of entries inside a formation depending on the set entries for all other formations.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		private float DetermineMaximum(int index)
		{
			int sum = 0;
			for (int i = 0; i < _focus.formationData.Count; i++)
			{
				sum += i != index ? _focus.formationData[i].Item2 : 0;
			}
			return teamSize - sum;
		}
	}
}