using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using System.Collections.Generic;
using System;
using BrilliantSkies.Ui.Displayer;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Core.FilesAndFolders;
namespace TournamentMod.UI
{
	using Assets.Scripts.Gui;
	using BrilliantSkies.Core.Constants;
	using BrilliantSkies.Ui.Special.PopUps;
	using Serialisation;
	/// <summary>
	/// GUI-Class for managing participants.
	/// </summary>
	public class ParticipantManagementTab : AbstractTournamentTab
	{
		/// <summary>
		/// The Console for adding Participants.
		/// </summary>
		internal ParticipantConsole participantConsole;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="window"></param>
		/// <param name="focus"></param>
		public ParticipantManagementTab(TournamentConsole parent, ConsoleWindow window, Tournament focus) : base(parent, window, focus) {
			Name = new Content("Participants", "View and edit Participants.");
			participantConsole = new ParticipantConsole(focus, this);
		}
		/// <summary>
		/// Builds the Tab.
		/// </summary>
		public override void Build()
		{
			CreateHeader("Modify current Entries", new ToolTip("Modify current Entries from active Teams."));
			ScreenSegmentStandardHorizontal horizontal = CreateStandardHorizontalSegment();
			horizontal.SpaceBelow = horizontal.SpaceAbove = 5;
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Cycle Teams", new ToolTip("Cycles the entries through the currently active Teams. Non-active Teams will be excluded."), delegate (Tournament t)
			{
				List<Entry> temp = t.entries[0];
				int i;
				for (i = 1; i < t.Parameters.ActiveFactions; i++)
				{
					t.entries[i - 1] = t.entries[i];
				}
				t.entries[i - 1] = temp;
				foreach (KeyValuePair<int, List<Entry>> teamList in t.entries)
				{
					teamList.Value.ForEach((Entry te) => te.FactionIndex = teamList.Key);
				}
				TriggerScreenRebuild();
			}));
			for (int i = 0; i < StaticConstants.MAX_TEAMS; i++)
			{
				int factionIndex = i;
				horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Invert Direction\nfor Team {factionIndex + 1}", new ToolTip($"Inverts the direction for Team {factionIndex + 1}, by turning each entry 180°."), delegate (Tournament t)
				{
					t.entries[factionIndex].ForEach((Entry te) => te.Spawn_direction = (te.Spawn_direction + 180) % 360);
					TriggerScreenRebuild();
				})).SetConditionalDisplayFunction(() => factionIndex < _focus.Parameters.ActiveFactions);
			}
			horizontal = CreateStandardHorizontalSegment();
			horizontal.SpaceBelow = horizontal.SpaceAbove = 5;
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Clear all Teams", new ToolTip("Removes all entries from all Teams."), delegate (Tournament t)
			{
				foreach (var team in t.entries) {
					team.Value.Clear();
				}
				TriggerScreenRebuild();
			}));
			for (int i = 0; i < StaticConstants.MAX_TEAMS; i++)
			{
				int factionIndex = i;
				horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Clear Team {factionIndex + 1}", new ToolTip($"Removes all entries for Team {factionIndex + 1}."), delegate (Tournament t)
				{
					t.entries[factionIndex].Clear();
					TriggerScreenRebuild();
				})).SetConditionalDisplayFunction(() => factionIndex < _focus.Parameters.ActiveFactions);
			}
			horizontal = CreateStandardHorizontalSegment();
			horizontal.SpaceBelow = horizontal.SpaceAbove = 5;
			horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Update all Teams", new ToolTip("Updates all entries from all Teams."), delegate (Tournament t)
			{
				foreach (var team in t.entries) {
					foreach (var member in team.Value) {
						member.Spawn_direction = t.Parameters.Direction;
						member.Spawn_height = t.Parameters.SpawnHeight;
					}
				}
				TriggerScreenRebuild();
			}));
			for (int i = 0; i < StaticConstants.MAX_TEAMS; i++)
			{
				int factionIndex = i;
				horizontal.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Update Team {factionIndex + 1}", new ToolTip($"Updates all entries for Team {factionIndex + 1}."), delegate (Tournament t)
				{
					foreach (var member in t.entries[factionIndex])
					{
						member.Spawn_direction = t.Parameters.Direction;
						member.Spawn_height = t.Parameters.SpawnHeight;
					}
					TriggerScreenRebuild();
				})).SetConditionalDisplayFunction(() => factionIndex < _focus.Parameters.ActiveFactions);
			}
			bool ready = true;
			for (int i = 0; i < StaticConstants.MAX_TEAMS; i++) {
				int factionIndex = i;
				int teamSize = 0;
				if (_focus.entries.ContainsKey(i))
				{
					teamSize = _focus.entries[factionIndex].Count;
				}
				ready &= teamSize > 0 || factionIndex >= _focus.Parameters.ActiveFactions;
				CreateHeader("Team " + (factionIndex + 1), new ToolTip($"Current Entries for Team {factionIndex + 1}. The list goes from top to bottom.")).SetConditionalDisplay(() => factionIndex < _focus.Parameters.ActiveFactions);
				ScreenSegmentStandardHorizontal saveLoad = CreateStandardHorizontalSegment();
				saveLoad.SpaceAbove = saveLoad.SpaceBelow = 5;
				saveLoad.SetConditionalDisplay(() => factionIndex < _focus.Parameters.ActiveFactions);
				TCCFolder folder = new TCCFolder(new FilesystemFolderSource(Get.PermanentPaths.GetSpecificModDir("Tournament").ToString()), false);
				saveLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Quicksave Team {factionIndex + 1}", new ToolTip("Saves this team into a predetermined file inside the mod-folder."), delegate (Tournament t)
					 {
						 t.QuicksaveTeam(factionIndex);
					 }));
				saveLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Save Team{factionIndex + 1}", new ToolTip("Save this team into a file or your choosing."), delegate (Tournament t)
					{
						GuiPopUp.Instance.Add(new PopupTreeViewSave<TeamCompositionConfiguration>("Save Team", FtdGuiUtils.GetFileBrowserFor<TCCFile, TCCFolder>(folder), delegate (string s, bool b)
						  { }, _focus.CreateSavefileForTeam(factionIndex), $"Team{factionIndex + 1}"));
					}));
				saveLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Quickload Team {factionIndex + 1}", new ToolTip("Loads a team from a predetermined file inside the mod-folder."), delegate (Tournament t)
					{
						t.QuickloadTeam(factionIndex);
					}));
				saveLoad.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, $"Load Team {factionIndex + 1}", new ToolTip("Load a team from a file of your choosing."), delegate (Tournament t)
					{
						GuiPopUp.Instance.Add(new PopupTreeView("Load Team", FtdGuiUtils.GetFileBrowserFor<TCCFile, TCCFolder>(folder), delegate (string s, bool b)
						{
							if (b)
							{
								TeamCompositionConfiguration tcc = folder.GetFile(s, true).Load();
								_focus.LoadTeam(factionIndex, tcc);
								DeactivatePopup();
							}
						}));
					}));
				for (int j = 0; j < teamSize; j++) {
					int indexInFaction = j;
					Entry entry = _focus.entries[factionIndex][indexInFaction];
					string text = "";
					string[] labelCost = entry.LabelCost;
					foreach (string str in labelCost)
					{
						text = text + "\n" + str;
					}
					ScreenSegmentTable entryControl = CreateTableSegment(3, 2);
					entryControl.eTableOrder = ScreenSegmentTable.TableOrder.Columns;
					entryControl.SqueezeTable = false;
					entryControl.SpaceAbove = entryControl.SpaceBelow = 5;
					entryControl.SetConditionalDisplay(() => factionIndex < _focus.Parameters.ActiveFactions);
					entryControl.AddInterpretter(new StringDisplay(M.m<string>(string.Format(
						"{3}°@{2}m\n" +
						"{0} <color=cyan>{1}</color>\n" +
						"~-------SPAWNS-------~{4}\n" +
						"~--------------------~\n" +
						"~---FORMATION-ROLE---~\n" +
						"{5}",
						entry.Bpf.Name,
						entry.bp.CalculateResourceCost(false, true, false).Material+entry.bp.ContainedMaterialCost,
						entry.Spawn_height,
						entry.Spawn_direction,
						text,
						_focus.GetFormation(factionIndex).DeterminePositionDescription(_focus.Parameters.SpawngapLR[factionIndex],
						_focus.Parameters.SpawngapFB[factionIndex],
						_focus.entries[factionIndex].Count,
						indexInFaction))), M.m<ToolTip>(new ToolTip("Here you can see every important information about the Blueprint."))), 0, 0);
					entryControl.AddInterpretter(SubjectiveFloatClampedWithBarFromMiddle<Entry>.Quick(entry, 0, entry.MaxMaterials, 1, 0, M.m<Entry>((e) => e.CurrentMaterials), "Spawnmaterials: {0}", delegate (Entry e, float f)
							 {
								 e.CurrentMaterials = f;
							 }, new ToolTip("Change the Spawnmaterials for this entry")), 1, 0).SetConditionalDisplayFunction(() => _focus.Parameters.TeamEntryMaterials[factionIndex]);
					entryControl.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Remove", new ToolTip("Removes this entry."), delegate (Tournament t)
					{
						t.entries[factionIndex].Remove(entry);
						TriggerScreenRebuild();
					}), 0, 1);
					entryControl.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Update", new ToolTip("Updates this entry with new values for its direction and height."), delegate (Tournament t)
					{
						entry.Spawn_direction = t.Parameters.Direction;
						entry.Spawn_height = t.Parameters.SpawnHeight;
						TriggerScreenRebuild();
					}), 1, 1);
					entryControl.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Move up", new ToolTip("Moves the entry one place up in its teamlist."), delegate (Tournament t)
					{
						t.entries[factionIndex].RemoveAt(indexInFaction);
						t.entries[factionIndex].Insert(indexInFaction - 1, entry);
						TriggerScreenRebuild();
					}), 0, 2).SetConditionalDisplayFunction(() => indexInFaction != 0);
					entryControl.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "Move Down", new ToolTip("Moves the entry one place down in its teamlist."), delegate (Tournament t)
					{
						t.entries[factionIndex].RemoveAt(indexInFaction);
						t.entries[factionIndex].Insert(indexInFaction + 1, entry);
						TriggerScreenRebuild();
					}), 1, 2).SetConditionalDisplayFunction(() => indexInFaction + 1 != teamSize);
				}
			}
			ScreenSegmentStandard posthead = CreateStandardSegment();
			posthead.SpaceBelow = posthead.SpaceAbove = 5;
			posthead.AddInterpretter(StringDisplay.Quick("During the fight you can use your key for the Charactersheet-GUI to bring up or hide the Extra-Info-Panel for an individual construct. When using the default keymap, it is 'Z'. " +
				"With the Key for the EnemySpawn-GUI you can hide and show the Sidelist. Its default Key is 'X'."));
			posthead.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "START", new ToolTip("Start the fight."), delegate (Tournament t)
			{
				((ConsoleUi<Tournament>)Window._governingUi).DeactivateGui();
				t.ApplyFactionColors();
				t.LoadCraft();
				t.StartMatch();
			})).SetConditionalDisplayFunction(() => ready);
			posthead.AddInterpretter(SubjectiveButton<Tournament>.Quick(_focus, "START & Quicksave Settings", new ToolTip("Saves your current Settings and then starts the fight."), delegate (Tournament t)
			{
				((ConsoleUi<Tournament>) Window._governingUi).DeactivateGui();
				t.SaveSettings();
				t.ApplyFactionColors();
				t.LoadCraft();
				t.StartMatch();
			})).SetConditionalDisplayFunction(() => ready);
			posthead.AddInterpretter(StringDisplay.Quick("It seems at least one Team has no Entry. Reduce the number of Teams or give the Team(s) in question at least one Entry.")).SetConditionalDisplayFunction(() => !ready);
		}
		/// <summary>
		/// Automatically pops up the Participant-Console.
		/// </summary>
		public override Action OnSelectTab => delegate() {
			PopThisUp(participantConsole);
		};
		/// <summary>
		/// Automatically deactivates the current Popup, when the Tab (or the whole Console) get closed.
		/// </summary>
		public override Action<OnDeselectTabSource> OnDeselectTab => delegate(OnDeselectTabSource source) {
			DeactivatePopup();
			GuiDisplayer.GetSingleton().EvenOutUisAcrossTheScreen();
		};
	}
}