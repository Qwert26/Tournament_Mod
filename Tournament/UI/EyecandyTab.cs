using System;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using UnityEngine;
using Tournament.Serialisation;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective;
namespace Tournament.UI
{
	public class EyecandyTab : AbstractTournamentTab
	{
		private int currentTeam = 0;
		public EyecandyTab(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(parent, window, focus)
		{
			Name = new Content("Eyecandy", "Change the fleet appearence of a Team.");
		}
		public override Action OnSelectTab => base.OnSelectTab;
		public override void Build()
		{
			base.Build();
			_focus.Parameters.EnsureEnoughData();
			for (int i = 0; i < 6; i++) {
				int index = i;
				CreateHeader("Team " + (1 + i), new ToolTip($"Fleetcolors for Team {i + 1}")).SetConditionalDisplay(() => index < _focus.Parameters.ActiveFactions);
				ScreenSegmentStandardHorizontal table = CreateStandardHorizontalSegment();
				table.SetConditionalDisplay(() => index < _focus.Parameters.ActiveFactions);
				table.AddInterpretter(new SubjectiveColorChanger<TournamentParameters>(_focus.Parameters, M.m<TournamentParameters>($"Team {i + 1} Main Color"),
					M.m<TournamentParameters>(new ToolTip($"Set the Main Color for Team {i + 1}")), M.m((TournamentParameters tp) => tp.MainColorsPerTeam[index]), delegate (TournamentParameters tp, Color c)
					  {
						  tp.MainColorsPerTeam.Us[index] = c;
					  }));
				table.AddInterpretter(new SubjectiveColorChanger<TournamentParameters>(_focus.Parameters, M.m<TournamentParameters>($"Team {i + 1} Secondary Color"),
					M.m<TournamentParameters>(new ToolTip($"Set the Secondary Color for Team {i + 1}")), M.m((TournamentParameters tp) => tp.SecondaryColorsPerTeam[index]), delegate (TournamentParameters tp, Color c)
					  {
						  tp.SecondaryColorsPerTeam.Us[index] = c;
					  }));
				table.AddInterpretter(new SubjectiveColorChanger<TournamentParameters>(_focus.Parameters, M.m<TournamentParameters>($"Team {i + 1} Trim Color"),
					M.m<TournamentParameters>(new ToolTip($"Set the Trim Color for Team {i + 1}")), M.m((TournamentParameters tp) => tp.TrimColorsPerTeam[index]), delegate (TournamentParameters tp, Color c)
					  {
						  tp.TrimColorsPerTeam.Us[index] = c;
					  }));
				table.AddInterpretter(new SubjectiveColorChanger<TournamentParameters>(_focus.Parameters, M.m<TournamentParameters>($"Team {i + 1} Detail Color"),
					M.m<TournamentParameters>(new ToolTip($"Set the Main Color for Team {i + 1}")), M.m((TournamentParameters tp) => tp.DetailColorsPerTeam[index]), delegate (TournamentParameters tp, Color c)
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
			saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Quickload Settings", new ToolTip("Loads the last saved Parameters from the Mod-Folder."), (t) => t.LoadSettings()));
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
			saveAndLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Load Defaults", new ToolTip("Reloads all default settings"), (t) => t.LoadDefaults()));
			CreateHeader("Prepared Fleet Colors", new ToolTip("Here you can find prepared fleet colors back from the old days."));
			CreateStandardSegment().AddInterpretter(new SubjectiveFloatClampedWithBar<EyecandyTab>(M.m<EyecandyTab>(0), M.m((EyecandyTab et) => et._focus.Parameters.ActiveFactions - 1), M.m((EyecandyTab et) => et.currentTeam),
				M.m<EyecandyTab>(1), this, M.m((EyecandyTab et) => $"Apply Fleetcolor-Prefeb to Team {et.currentTeam + 1}"), delegate (EyecandyTab et, float f)
				{
					et.currentTeam = (int)f;
				}, null, M.m<EyecandyTab>(new ToolTip("Select the Team to apply one of the prefabs below."))));
			foreach (TournamentFleetColor tfc in TournamentFleetColor.colorSchemes) {
				TournamentFleetColor current = tfc;
				ScreenSegmentStandard standard = CreateStandardSegment();
				standard.AddInterpretter(SubjectiveButton<TournamentParameters>.Quick(_focus.Parameters, current.Name, new ToolTip(current.Description), delegate (TournamentParameters tp)
				{
					tp.MainColorsPerTeam.Us[currentTeam] = current.Main;
					tp.SecondaryColorsPerTeam.Us[currentTeam] = current.Secondary;
					tp.DetailColorsPerTeam.Us[currentTeam] = current.Detail;
					tp.TrimColorsPerTeam.Us[currentTeam] = current.Trim;
				}));
				ScreenSegmentTable table = CreateTableSegment(4, 1);
				table.SqueezeTable = false;
				table.AddInterpretter(new SubjectiveColorDisplay<TournamentFleetColor>(current, M.m<TournamentFleetColor>("Main Color"), M.m<TournamentFleetColor>(new ToolTip($"The Main Color of the \"{current.Name}\"-Prefab.")), M.m((TournamentFleetColor tFC) => tFC.Main)));
				table.AddInterpretter(new SubjectiveColorDisplay<TournamentFleetColor>(current, M.m<TournamentFleetColor>("Secondary Color"), M.m<TournamentFleetColor>(new ToolTip($"The Secondary Color of the \"{current.Name}\"-Prefab.")), M.m((TournamentFleetColor tFC) => tFC.Secondary)));
				table.AddInterpretter(new SubjectiveColorDisplay<TournamentFleetColor>(current, M.m<TournamentFleetColor>("Trim Color"), M.m<TournamentFleetColor>(new ToolTip($"The Trim Color of the \"{current.Name}\"-Prefab.")), M.m((TournamentFleetColor tFC) => tFC.Trim)));
				table.AddInterpretter(new SubjectiveColorDisplay<TournamentFleetColor>(current, M.m<TournamentFleetColor>("Detail Color"), M.m<TournamentFleetColor>(new ToolTip($"The Detail Color of the \"{current.Name}\"-Prefab.")), M.m((TournamentFleetColor tFC) => tFC.Detail)));
				CreateSpace(15);
			}
		}
	}
}