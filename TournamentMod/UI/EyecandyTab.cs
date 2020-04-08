using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using UnityEngine;
using TournamentMod.Serialisation;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective;
namespace TournamentMod.UI
{
	/// <summary>
	/// GUI-Class for Eyecandy, currently only fleetcolors of active teams.
	/// </summary>
	public class EyecandyTab : AbstractTournamentTab
	{
		private int currentTeam = 0;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="window"></param>
		/// <param name="focus"></param>
		public EyecandyTab(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(parent, window, focus)
		{
			Name = new Content("Eyecandy", "Change the fleet appearence of a Team.");
		}
		/// <summary>
		/// Builds the Tab.
		/// </summary>
		public override void Build()
		{
			base.Build();
			_focus.Parameters.EnsureEnoughData();
			for (int i = 0; i < 6; i++) {
				int index = i;
				CreateHeader("Team " + (1 + i), new ToolTip($"Fleetcolors for Team {i + 1}")).SetConditionalDisplay(() => index < _focus.Parameters.ActiveFactions);
				ScreenSegmentStandardHorizontal table = CreateStandardHorizontalSegment();
				table.SetConditionalDisplay(() => index < _focus.Parameters.ActiveFactions);
				table.AddInterpretter(new SubjectiveColorChanger<Parameters>(_focus.Parameters, M.m<Parameters>($"Team {i + 1} Main Color"),
					M.m<Parameters>(new ToolTip($"Set the Main Color for Team {i + 1}")), M.m((Parameters tp) => tp.MainColorsPerTeam[index]), delegate (Parameters tp, Color c)
					  {
						  tp.MainColorsPerTeam.Us[index] = c;
					  }));
				table.AddInterpretter(new SubjectiveColorChanger<Parameters>(_focus.Parameters, M.m<Parameters>($"Team {i + 1} Secondary Color"),
					M.m<Parameters>(new ToolTip($"Set the Secondary Color for Team {i + 1}")), M.m((Parameters tp) => tp.SecondaryColorsPerTeam[index]), delegate (Parameters tp, Color c)
					  {
						  tp.SecondaryColorsPerTeam.Us[index] = c;
					  }));
				table.AddInterpretter(new SubjectiveColorChanger<Parameters>(_focus.Parameters, M.m<Parameters>($"Team {i + 1} Trim Color"),
					M.m<Parameters>(new ToolTip($"Set the Trim Color for Team {i + 1}")), M.m((Parameters tp) => tp.TrimColorsPerTeam[index]), delegate (Parameters tp, Color c)
					  {
						  tp.TrimColorsPerTeam.Us[index] = c;
					  }));
				table.AddInterpretter(new SubjectiveColorChanger<Parameters>(_focus.Parameters, M.m<Parameters>($"Team {i + 1} Detail Color"),
					M.m<Parameters>(new ToolTip($"Set the Main Color for Team {i + 1}")), M.m((Parameters tp) => tp.DetailColorsPerTeam[index]), delegate (Parameters tp, Color c)
					  {
						  tp.DetailColorsPerTeam.Us[index] = c;
					  }));
			}
			ScreenSegmentStandardHorizontal saveAndLoad = CreateStandardHorizontalSegment();
			saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quicksave Settings", new ToolTip("Saves the current Parameters into the Mod-Folder."), (t) => t.SaveSettings()));
			/*saveAndLoad.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Save Settings", new ToolTip("Saves the current Parameters into a file of your chosing."), delegate (TournamentParameters tp)
			{
				GuiPopUp.Instance.Add(new PopupTreeViewSave<TournamentParameters>("Save Parameters", FtdGuiUtils.GetFileBrowserFor<TournamentParametersFile, TournamentParametersFolder>(new TournamentParametersFolder(new FilesystemFolderSource(Get.PerminentPaths.GetSpecificModDir("Tournament").ToString()))), delegate (string s, bool b)
				{
					if (b)
					{
						TournamentParametersFile tpf = new TournamentParametersFile(new FilesystemFileSource(s + ".json"));
						tpf.Save(_focus.Parameters);
					}
				}, _focus.Parameters));
			*/
			saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quickload Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) =>
			{
				t.LoadSettings();
				t.MoveCam();
				TriggerScreenRebuild();
			}));
			/*saveAndLoad.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, "Load Parameters", new ToolTip("Loads new Parameters from a file of your choosing."), delegate (TournamentParameters tp) {
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
			saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) =>
			{
				t.LoadDefaults();
				t.MoveCam();
				TriggerScreenRebuild();
			}));
			CreateHeader("Prepared Fleet Colors", new ToolTip("Here you can find prepared fleet colors back from the old days."));
			CreateStandardSegment().AddInterpretter(new SubjectiveFloatClampedWithBar<EyecandyTab>(M.m<EyecandyTab>(0), M.m((EyecandyTab et) => et._focus.Parameters.ActiveFactions - 1), M.m((EyecandyTab et) => et.currentTeam),
				M.m<EyecandyTab>(1), this, M.m((EyecandyTab et) => $"Apply Fleetcolor-Prefeb to Team {et.currentTeam + 1}"), delegate (EyecandyTab et, float f)
				{
					et.currentTeam = (int)f;
				}, null, M.m<EyecandyTab>(new ToolTip("Select the Team to apply one of the prefabs below."))));
			foreach (FleetColor tfc in FleetColor.colorSchemes) {
				FleetColor current = tfc;
				ScreenSegmentStandard standard = CreateStandardSegment();
				standard.AddInterpretter(SubjectiveButton<Parameters>.Quick(_focus.Parameters, current.Name, new ToolTip(current.Description), delegate (Parameters tp)
				{
					tp.MainColorsPerTeam.Us[currentTeam] = current.Main;
					tp.SecondaryColorsPerTeam.Us[currentTeam] = current.Secondary;
					tp.DetailColorsPerTeam.Us[currentTeam] = current.Detail;
					tp.TrimColorsPerTeam.Us[currentTeam] = current.Trim;
				}));
				ScreenSegmentTable table = CreateTableSegment(4, 1);
				table.SqueezeTable = false;
				table.AddInterpretter(new SubjectiveColorDisplay<FleetColor>(current, M.m<FleetColor>("Main Color"), M.m<FleetColor>(new ToolTip($"The Main Color of the \"{current.Name}\"-Prefab.")), M.m((FleetColor tFC) => tFC.Main)));
				table.AddInterpretter(new SubjectiveColorDisplay<FleetColor>(current, M.m<FleetColor>("Secondary Color"), M.m<FleetColor>(new ToolTip($"The Secondary Color of the \"{current.Name}\"-Prefab.")), M.m((FleetColor tFC) => tFC.Secondary)));
				table.AddInterpretter(new SubjectiveColorDisplay<FleetColor>(current, M.m<FleetColor>("Trim Color"), M.m<FleetColor>(new ToolTip($"The Trim Color of the \"{current.Name}\"-Prefab.")), M.m((FleetColor tFC) => tFC.Trim)));
				table.AddInterpretter(new SubjectiveColorDisplay<FleetColor>(current, M.m<FleetColor>("Detail Color"), M.m<FleetColor>(new ToolTip($"The Detail Color of the \"{current.Name}\"-Prefab.")), M.m((FleetColor tFC) => tFC.Detail)));
				CreateSpace(15);
			}
		}
	}
}