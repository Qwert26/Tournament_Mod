using BrilliantSkies.Core.Constants;
using BrilliantSkies.Core.FilesAndFolders;
using BrilliantSkies.Core.Timing;
using BrilliantSkies.Core.Types;
using BrilliantSkies.Core.UniverseRepresentation;
using BrilliantSkies.Effects.Cameras;
using BrilliantSkies.Ftd.Avatar;
using BrilliantSkies.Ftd.Planets;
using BrilliantSkies.Ftd.Planets.Instances;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.PlayerProfiles;
using BrilliantSkies.Ui.Special.PopUps;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using BrilliantSkies.Core.Logger;
namespace TournamentMod
{
	using UI;
	using Serialisation;
	using Formations;
	/// <summary>
	/// GUI-Class for the Overlay and general Mangement.
	/// </summary>
	public class Tournament
	{
		/// <summary>
		/// The Singleton-instance.
		/// </summary>
		public static Tournament _me;
		/// <summary>
		/// The Console-Window for setting things up.
		/// </summary>
		public TournamentConsole tournamentConsole;
		private LiveViewConsole liveViewConsole;
		private ParticipantManagement participantManagement;
		#region Kamerakontrolle
		private GameObject cam;
		private MouseLook flycam;
		private MouseOrbit orbitcam;
		private int orbittarget;
		private int orbitMothership;
		private int orbitindex;
		#endregion
		/// <summary>
		/// Should the LiveView display additional Information?
		/// </summary>
		public bool extraInfo = false;
		/// <summary>
		/// Time since battle start.
		/// </summary>
		public float timerTotal;
		/// <summary>
		/// Amount of Overtimes the battles had.
		/// </summary>
		private byte overtimeCounter = 0;
		/// <summary>
		/// When true, the Sidelist is shown.
		/// </summary>
		public bool showLists = true;
		/// <summary>
		/// The Formation for each Team. For saving, the entire list gets converted into a list of Vector4Int.
		/// </summary>
		public List<CombinedFormation> teamFormations;
		/// <summary>
		/// The Parameters used for the fight.
		/// </summary>
		public Parameters Parameters { get; set; } = new Parameters();
		/// <summary>
		/// The Blueprints to spawn in at the beginning.
		/// </summary>
		public Dictionary<int, List<Entry>> entries = new Dictionary<int, List<Entry>>(StaticConstants.MAX_TEAMS);
		/// <summary>
		/// For distributed Materials.
		/// </summary>
		private List<int> materials;
		/// <summary>
		/// The penalty weight settings for each team. For saving, the entire list gets converted into a list of Vector3.
		/// </summary>
		public List<Dictionary<PenaltyType, float>> teamPenaltyWeights;
		/// <summary>
		/// Creates a new Tournament-instance.
		/// </summary>
		public Tournament()
		{
			_me = this;
			teamFormations = new List<CombinedFormation>(StaticConstants.MAX_TEAMS);
			teamPenaltyWeights = new List<Dictionary<PenaltyType, float>>(StaticConstants.MAX_TEAMS);
			for (int t = 0; t < StaticConstants.MAX_TEAMS; t++)
			{
				teamFormations.Add(new CombinedFormation());
				Dictionary<PenaltyType, float> presets = new Dictionary<PenaltyType, float>();
				foreach (PenaltyType pt in Enum.GetValues(typeof(PenaltyType)))
				{
					presets.Add(pt, 1f);
				}
				teamPenaltyWeights.Add(presets);
			}
			LoadSettings();
		}
		/// <summary>
		/// Loads in all selected crafts and sets their materialstorage.
		/// </summary>
		public void LoadCraft()
		{
			ClearArea();
			materials?.Clear();
			materials = null;
			InstanceSpecification.i.Header.CommonSettings.EnemyBlockDestroyedResourceDrop = Parameters.MaterialConversion / 100f;
			if (!Parameters.DistributeLocalResources)
			{
				for (int i = 0; i < Parameters.ActiveFactions; i++)
				{
					TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResources(0);
				}
				foreach (KeyValuePair<int, List<Entry>> team in entries)
				{
					if (team.Key >= Parameters.ActiveFactions)
					{
						break;
					}
					for (int pos = 0; pos < team.Value.Count; pos++)
					{
						MainConstruct mc = team.Value[pos].Spawn(Parameters.StartingDistance, Parameters.SpawngapLR[team.Key], Parameters.SpawngapFB[team.Key], team.Value.Count, pos);
						mc.GetForce().ResourceStore.iMaterial.SetQuantity(Parameters.ResourcesPerTeam[team.Key]);
					}
				}
			}
			else
			{
				List<int> materials = new List<int>(Parameters.ResourcesPerTeam);
				Dictionary<int, int> maxMaterials = new Dictionary<int, int>();
				Dictionary<int, List<MainConstruct>> constructs = new Dictionary<int, List<MainConstruct>>();
				foreach (KeyValuePair<int, List<Entry>> team in entries)
				{
					if (team.Key >= Parameters.ActiveFactions)
					{
						break;
					}
					else
					{
						maxMaterials.Add(team.Key, 0);
						constructs.Add(team.Key, new List<MainConstruct>());
						for (int pos = 0; pos < team.Value.Count; pos++)
						{
							MainConstruct mc = team.Value[pos].Spawn(Parameters.StartingDistance, Parameters.SpawngapLR[team.Key], Parameters.SpawngapFB[team.Key], team.Value.Count, pos);
							constructs[team.Key].Add(mc);
							if (Parameters.TeamEntryMaterials[team.Key])
							{
								mc.GetForce().ResourceStore.iMaterial.Quantity = team.Value[pos].CurrentMaterials;
							}
							else
							{
								maxMaterials[team.Key] += (int) mc.GetForce().ResourceStore.iMaterial.Maximum;
							}
						}
					}
				}
				for (int i = 0; i < Parameters.ActiveFactions; i++)
				{
					if (Parameters.TeamEntryMaterials[i])
					{
						continue;
					}
					else if (maxMaterials[i] <= materials[i])
					{
						foreach (MainConstruct mc in constructs[i])
						{
							mc.GetForce().ResourceStore.iMaterial.SetQuantity(materials[i]);
							materials[i] -= (int) mc.GetForce().ResourceStore.iMaterial.Maximum;
						}
						TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResources(materials[i]);
					}
					else
					{
						double expectedFraction = ((double) materials[i]) / maxMaterials[i];
						foreach (MainConstruct mc in constructs[i])
						{
							mc.GetForce().ResourceStore.iMaterial.SetQuantity(mc.GetForce().ResourceStore.iMaterial.Maximum * expectedFraction);
						}
						TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResources(0);
					}
				}
			}
		}
		/// <summary>
		/// Sets Cleanup-Functions, adds all Mainconstructs into the HUDLog and registers its methods. Gets called after <see cref="LoadCraft"/>.
		/// </summary>
		public void StartMatch()
		{
			overtimeCounter = 0;
			timerTotal = 0f;
			ConstructableCleanUpSettings ccus = InstanceSpecification.i.Header.CommonSettings.ConstructableCleanUpSettings;
			InstanceSpecification.i.Header.CommonSettings.ConstructableCleanUp = (ConstructableCleanUp)Parameters.CleanUpMode;
			ccus.BelowAndSinking = Parameters.CleanUpSinkingConstructs;
			ccus.BelowAndSinkingAltitude = Parameters.SinkingAltitude;
			ccus.BelowAndSinkingHealthFraction = Parameters.SinkingHealthFraction / 100f; ;
			ccus.TooDamaged = Parameters.CleanUpTooDamagedConstructs;
			ccus.TooDamagedHealthFraction = Parameters.TooDamagedHealthFraction / 100f; ;
			ccus.TooFewBlocks = Parameters.CleanUpTooSmallConstructs;
			ccus.TooFewBlocksCount = Parameters.TooSmallBlockCount;
			ccus.DamagedInEnemyTerritory = false;
			ccus.AIDead = Parameters.CleanUpNoAI;
			ccus.SustainedByRepairs = Parameters.CleanUpDelayedByRepairs;
			ccus.SustainedByRepairsTime = Parameters.RepairDelayTime;
			if (!GameSpeedManager.Instance.IsPaused)
			{
				GameSpeedManager.Instance.TogglePause();
			}
			orbitindex = 0;
			orbittarget = 0;
			flycam.transform.position = new Vector3(-500f, 50f, 0f) + LocalOffsetFromTerrainCenter();
			flycam.transform.rotation = Quaternion.LookRotation(Vector3.right);
			participantManagement = new ParticipantManagement(Parameters, teamPenaltyWeights);
			liveViewConsole = new LiveViewConsole();
			liveViewConsole.ActivateGui(participantManagement);
			GameEvents.PreLateUpdate.RegWithEvent(LateUpdate);
			GameEvents.FixedUpdateEvent.RegWithEvent(HandleTime);
		}
		public void UnregisterEvents()
		{
			GameEvents.PreLateUpdate.UnregWithEvent(LateUpdate);
			GameEvents.FixedUpdateEvent.UnregWithEvent(HandleTime);
			participantManagement?.UnregisterEvents();
		}
		public void HandleTime(ITimeStep dt)
		{
			timerTotal += dt.DeltaTime;
			if (overtimeCounter == 0 && timerTotal > Parameters.MaximumTime)
			{
				GameSpeedManager.Instance.TogglePause();
				overtimeCounter = 1;
			}
			else if (Parameters.Overtime > 0)
			{//Verlängerung ist eingeschaltet.
				if (timerTotal > Parameters.MaximumTime + overtimeCounter * Parameters.Overtime)
				{
					GameSpeedManager.Instance.TogglePause();
					overtimeCounter++;
				}
			}
		}
		/// <summary>
		/// Deletes every Force which there currently is.
		/// </summary>
		public void ClearArea()
		{
			ForceManager.Instance.forces.ForEach(delegate (Force t)
			{
				ForceManager.Instance.DeleteForce(t);
			});
		}
		/// <summary>
		/// Destroys and recreates the camera object.
		/// </summary>
		public void ResetCam()
		{
			foreach (PlayerSetupBase @object in Objects.Instance.Players.Objects)
			{
				UnityEngine.Object.Destroy(@object.gameObject);
			}
			if (cam != null)
			{
				UnityEngine.Object.Destroy(cam);
			}
			cam = R_Avatars.JustOrbitCamera.InstantiateACopy().gameObject;
			cam.gameObject.transform.position = new Vector3(-500f, 50f, 0f);
			cam.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.right);
			cam.AddComponent<MouseLook>();
			flycam = cam.GetComponent<MouseLook>();
			flycam.enabled = true;
			flycam.transform.position = new Vector3(0f, 50f, 0f);
			flycam.transform.rotation = Quaternion.LookRotation(Vector3.right);
			orbitcam = cam.GetComponent<MouseOrbit>();
			orbitcam.OperateRegardlessOfUiOptions = false;
			orbitcam.distance = 100f;
			orbitcam.enabled = false;
			//orbittarget = StaticConstructablesManager.constructables[0].UniqueId;
			orbittarget = -1;
			orbitindex = -1;
			orbitMothership = -1;
			extraInfo = false;
		}
		/// <summary>
		/// Moves the camera into the center of the map-tile, 50m above sea level.
		/// </summary>
		public void MoveCam()
		{
			cam.transform.position = FramePositionOfBoardSection() + new Vector3(0, 50, 0) + LocalOffsetFromTerrainCenter();
		}
		/// <summary>
		/// Calculates the frame position and moves the frame when necessary.
		/// </summary>
		/// <returns></returns>
		private Vector3 FramePositionOfBoardSection()
		{
			return PlanetList.MainFrame.UniversalPositionToFramePosition(UniversalPositionOfBoardSection()+LocalTerrainOffsetFromSectionCenter());
		}
		/// <summary>
		/// Calculates the universal position of the selected map-sector.
		/// </summary>
		/// <returns></returns>
		private Vector3d UniversalPositionOfBoardSection()
		{
			return StaticCoordTransforms.BoardSectionToUniversalPosition(WorldSpecification.i.BoardLayout.BoardSections[Parameters.EastWestBoard, Parameters.NorthSouthBoard].BoardSectionCoords);
		}
		/// <summary>
		/// Uses the selected Terrain-tile and its size to get the offset from the center of a map-sector.
		/// </summary>
		/// <returns></returns>
		private Vector3 LocalTerrainOffsetFromSectionCenter() {
			return WorldSpecification.i.BoardLayout.TerrainSize * new Vector3(Parameters.EastWestTerrain, 0, Parameters.NorthSouthTerrain);
		}
		/// <summary>
		/// Packs the terrain-based offsets into a Vector3.
		/// </summary>
		/// <returns></returns>
		public Vector3 LocalOffsetFromTerrainCenter() {
			return new Vector3(Parameters.EastWestOffset, 0, Parameters.NorthSouthOffset);
		}
		/// <summary>
		/// Uses the loaded list to reconstruct the combined formations and penalty settings for each team.
		/// </summary>
		private void PopulateData() {
			foreach (CombinedFormation cf in teamFormations) {
				cf.formationEntrycount.Clear();
			}
			Parameters.TeamFormations.Sort(delegate (Vector4i x, Vector4i y)
			{
				if (x.x == y.x) //Selbes Team: Der Positionsindex entscheidet.
				{
					return x.y - y.y;
				}
				else //Unterschiedliche Teams
				{
					return x.x - y.x;
				}
			});
			foreach (Vector4i v4i in Parameters.TeamFormations)
			{
				teamFormations[v4i.x].Import(v4i);
			}
			foreach (Vector3 data in Parameters.PenaltyWeights)
			{
				teamPenaltyWeights[(int)data.x][(PenaltyType)data.y] = data.z;
			}
		}
		/// <summary>
		/// Saves the formations and penalty-weigths into lists in the parameter-object.
		/// </summary>
		private void PopulateParameters() {
			Parameters.TeamFormations.Clear();
			Parameters.PenaltyWeights.Clear();
			for (int team = 0; team < teamFormations.Count; team++)
			{
				Parameters.TeamFormations.AddRange(teamFormations[team].Export(team));
			}
			for (int team = 0; team < teamPenaltyWeights.Count; team++)
			{
				foreach(var penaltyweight in teamPenaltyWeights[team])
				{
					Parameters.PenaltyWeights.Add(new Vector3(team, (int)penaltyweight.Key, penaltyweight.Value));
				}
			}
		}
		/// <summary>
		/// Saves the current Parameters.
		/// </summary>
		public void SaveSettings()
		{
			string modFolder = Get.PermanentPaths.GetSpecificModDir("Tournament").ToString();
			FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder, "default.battlesettings"));
			PopulateParameters();
			settingsFile.SaveData(Parameters, Formatting.Indented);
		}
		/// <summary>
		/// Saves a team given by its index.
		/// </summary>
		/// <param name="index"></param>
		public void QuicksaveTeam(int index) {
			string modFolder = Get.PermanentPaths.GetSpecificModDir("Tournament").ToString();
			FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder, $"team{index + 1}.teamsettings"));
			settingsFile.SaveData(CreateSavefileForTeam(index), Formatting.Indented);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public TeamCompositionConfiguration CreateSavefileForTeam(int index)
		{
			TeamCompositionConfiguration tcc = new TeamCompositionConfiguration
			{
				AltitudeLimits = Parameters.AltitudeLimits[index],
				AltitudeReversal = Parameters.AltitudeReverse[index],
				DistanceLimit = Parameters.DistanceLimit[index],
				DistanceReversal = Parameters.DistanceReverse[index],
				EnemyAttackPercentage = Parameters.EnemyAttackPercentage[index],
				IndividualEntryMaterials = Parameters.TeamEntryMaterials[index],
				Resources = Parameters.ResourcesPerTeam[index],
				SpawngapFB = Parameters.SpawngapFB[index],
				SpawngapLR = Parameters.SpawngapLR[index],
				UsesProjectedDistance = Parameters.ProjectedDistance[index],
				UsesSoftLimits = Parameters.SoftLimits[index],
				MaximumBufferTime = Parameters.MaximumBufferTime[index],
				MaximumPenaltyTime = Parameters.MaximumPenaltyTime[index],
				MaximumSpeed = Parameters.MaximumSpeed[index]
			};
			tcc.TeamColors.Add(Parameters.MainColorsPerTeam[index]);
			tcc.TeamColors.Add(Parameters.SecondaryColorsPerTeam[index]);
			tcc.TeamColors.Add(Parameters.TrimColorsPerTeam[index]);
			tcc.TeamColors.Add(Parameters.DetailColorsPerTeam[index]);
			int i = 0;
			foreach (Tuple<FormationType, int> f in teamFormations[index].formationEntrycount)
			{
				tcc.Formation.Add(new Vector3Int(i++, (int) f.Item1, f.Item2));
			}
			foreach (Entry entry in entries[index])
			{
				tcc.EntryInformation.Add(new Vector3(entry.Spawn_direction, entry.Spawn_height, entry.CurrentMaterials));
				tcc.EntryFiles.Add(entry.FilePath.Replace(Get.ProfilePaths.PlayerName,"#0#"));
			}
			foreach (KeyValuePair<PenaltyType, float> penaltyWeigth in teamPenaltyWeights[index])
			{
				tcc.PenaltyWeights.Add((int) penaltyWeigth.Key, penaltyWeigth.Value);
			}
			return tcc;
		}
		/// <summary>
		/// Loads the Parameters-File.
		/// </summary>
		public void LoadSettings()
		{
			string modFolder = Get.PermanentPaths.GetSpecificModDir("Tournament").ToString();
			FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder, "default.battlesettings"));
			if (settingsFile.Exists)
			{
				try
				{
					Parameters = settingsFile.LoadData<Parameters>();
					Parameters.EnsureEnoughData();
					PopulateData();
				}
				catch (Exception e)
				{
					LoadDefaults();
					SaveSettings();
					GuiPopUp.Instance.Add(new PopupError("Could not load Settings", "Something went wrong during the loading of your last settings. This could be because of a corrupt Savefile " +
						"manual edits or some of the Datatypes have been changed and can not be loaded. To prevent future Errors, we just saved the default settings into the Savefile.", e.StackTrace, e));
				}
			}
			else
			{
				LoadDefaults();
			}
			for (int i = 0; i < Parameters.ActiveFactions; i++)
			{
				if (!entries.ContainsKey(i))
				{
					entries.Add(i, new List<Entry>());
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public void QuickloadTeam(int index)
		{
			string modFolder = Get.PermanentPaths.GetSpecificModDir("Tournament").ToString();
			FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder, $"team{index + 1}.teamsettings"));
			if (settingsFile.Exists)
			{
				try
				{
					LoadTeam(index, settingsFile.LoadData<TeamCompositionConfiguration>());
				}
				catch (Exception e)
				{
					settingsFile.Delete();
					GuiPopUp.Instance.Add(new PopupError($"Default Savefile for Team {index + 1} corrupted", "The default savefile for the given team got corrputed. This could be because of manual edits " +
					"or some variables changed their type and can no longer be loaded. In order to prevent future errors, the savefile has been deleted.", e.StackTrace, e));
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="tcc"></param>
		public void LoadTeam(int index, TeamCompositionConfiguration tcc)
		{
			if (!Parameters.UniformRules)
			{
				Parameters.SpawngapFB[index] = tcc.SpawngapFB;
				Parameters.SpawngapLR[index] = tcc.SpawngapLR;
				Parameters.AltitudeLimits[index] = tcc.AltitudeLimits;
				Parameters.AltitudeReverse[index] = tcc.AltitudeReversal;
				Parameters.DistanceLimit[index] = tcc.DistanceLimit;
				Parameters.DistanceReverse[index] = tcc.DistanceReversal;
				Parameters.ProjectedDistance[index] = tcc.UsesProjectedDistance;
				Parameters.SoftLimits[index] = tcc.UsesSoftLimits;
				Parameters.TeamEntryMaterials[index] = tcc.IndividualEntryMaterials;
				Parameters.MaximumBufferTime[index] = tcc.MaximumBufferTime;
				Parameters.MaximumPenaltyTime[index] = tcc.MaximumPenaltyTime;
				Parameters.MaximumSpeed[index] = tcc.MaximumSpeed;
				Parameters.EnemyAttackPercentage[index] = tcc.EnemyAttackPercentage;
			}
			if (!Parameters.SameMaterials)
			{
				Parameters.ResourcesPerTeam[index] = tcc.Resources;
				Parameters.TeamEntryMaterials[index] = tcc.IndividualEntryMaterials;
			}
			entries[index].Clear();
			for (int i = 0; i < tcc.EntryInformation.Count; i++)
			{
				Vector3 info = tcc.EntryInformation[i];
				Entry entry = new Entry()
				{
					Spawn_direction = info.x,
					Spawn_height = info.y,
					CurrentMaterials = info.z,
					FilePath = tcc.EntryFiles[i].Replace("#0#", Get.ProfilePaths.PlayerName),
					FactionIndex = index
				};
				if (entry.LoadBlueprintFile())
				{
					entries[index].Add(entry);
				}
				else
				{
					AdvLogger.LogWarning($"The entry was successfully loaded from file, but the blueprint could not be loaded! Fileplath was \"{entry.FilePath}\". The entry was not added to the Team.", LogOptions.Popup);
				}
			}
			teamFormations[index].formationEntrycount.Clear();
			for (int i = 0; i < tcc.Formation.Count; i++)
			{
				Vector3Int info = tcc.Formation[i];
				teamFormations[index].formationEntrycount.Add(new Tuple<FormationType, int>((FormationType) info.y, info.z));
			}
			foreach (KeyValuePair<int,float> penaltyWeight in tcc.PenaltyWeights)
			{
				teamPenaltyWeights[index][(PenaltyType) penaltyWeight.Key] = penaltyWeight.Value;
			}
		}
		/// <summary>
		/// Resets the Parameters and makes sure that there is enough data for the GUI.
		/// </summary>
		public void LoadDefaults()
		{
			Parameters = new Parameters();
			Parameters.EnsureEnoughData();
			foreach (CombinedFormation cf in teamFormations)
			{
				cf.formationEntrycount.Clear();
				cf.formationEntrycount.Add(new Tuple<FormationType, int>(FormationType.Line, 0));
			}
			foreach (Dictionary<PenaltyType, float> teamPenalties in teamPenaltyWeights)
			{
				foreach (PenaltyType pt in Enum.GetValues(typeof(PenaltyType)))
				{
					teamPenalties[pt] = 1;
				}
			}
		}
		/// <summary>
		/// Handles input and moves, rotates and places the camera.
		/// </summary>
		public void LateUpdate()
		{
			FtdKeyMap ftdKeyMap = ProfileManager.Instance.GetModule<FtdKeyMap>();
			MViewAndControl viewAndControl = ProfileManager.Instance.GetModule<MViewAndControl>();
			bool pause = false;
			bool next = false;
			bool previous = false;
			bool speedUp = false;
			bool slowDown = false;
			bool freecamOn = false;
			bool orbitcamOn = false;
			bool changeExtraInfo = false;
			bool changeShowList = false;
			int oldIndex = orbitindex, oldTarget = orbittarget;
			if (Parameters.DefaultKeys)
			{
				pause = Input.GetKeyDown(KeyCode.F11); // default f11
				speedUp = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
				slowDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				next = Input.GetKeyDown(KeyCode.E); // default e
				previous = Input.GetKeyDown(KeyCode.Q); // default q
				changeExtraInfo = Input.GetKeyDown(KeyCode.Z); // default z
				changeShowList = Input.GetKeyDown(KeyCode.X); // default x
				freecamOn = Input.GetMouseButtonDown(1); // default left click
				orbitcamOn = Input.GetMouseButtonDown(0); // default right click
			}
			else
			{
				pause = ftdKeyMap.IsKey(KeyInputsFtd.PauseGame, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
				speedUp = ftdKeyMap.IsKey(KeyInputsFtd.SpeedUpCamera, KeyInputEventType.Held, ModifierAllows.AllowUnnecessary);
				slowDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				next = ftdKeyMap.IsKey(KeyInputsFtd.InventoryUi, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
				previous = ftdKeyMap.IsKey(KeyInputsFtd.Interact, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
				changeExtraInfo = ftdKeyMap.IsKey(KeyInputsFtd.CharacterSheetUi, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
				changeShowList = ftdKeyMap.IsKey(KeyInputsFtd.EnemySpawnUi, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
				freecamOn = Input.GetMouseButtonDown(1); // technically same as default atm
				orbitcamOn = Input.GetMouseButtonDown(0); // technically same as default atm
			}
			if (changeExtraInfo)
			{
				extraInfo = !extraInfo;
			}
			if (changeShowList)
			{
				showLists = !showLists;
				if (showLists)
				{
					liveViewConsole.ActivateGui(participantManagement);
				}
			}
			if (StaticConstructablesManager.constructables.Count > 0)
			{
				if (orbitindex >= StaticConstructablesManager.constructables.Count || oldIndex == -1)
				{
					orbitindex = 0;
					orbittarget = 0;
				}
				if (StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId != orbittarget && orbittarget != 0 ||
					(orbitMothership != -1 && StaticConstructablesManager.constructables.ToArray()[orbitindex].GetForce().MothershipAndDrone.MothershipLatch.Them?.LinkedCorrectly?.OurForce.Id.Id != orbitMothership))
				{
					int index;
					if (orbitMothership == -1)
					{
						index = StaticConstructablesManager.constructables.FindIndex(0, m => m.UniqueId == orbittarget);
					}
					else
					{
						index = StaticConstructablesManager.constructables.FindIndex(0, m => m.UniqueId == orbittarget && m.GetForce().MothershipAndDrone.MothershipLatch.Them?.LinkedCorrectly?.OurForce.Id.Id == orbitMothership);
					}
					if (index >= 0)
					{
						orbitindex = index;
					}
					else
					{
						orbitindex = 0;
					}
				}
				if (next)
				{
					if (orbitindex + 1 >= StaticConstructablesManager.constructables.Count)
					{
						orbitindex = 0;
					}
					else
					{
						orbitindex++;
					}
				}
				if (previous)
				{
					if (orbitindex == 0)
					{
						orbitindex = StaticConstructablesManager.constructables.Count - 1;
					}
					else
					{
						orbitindex--;
					}
				}
				if (orbittarget != StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId ||
					(StaticConstructablesManager.constructables.ToArray()[orbitindex].GetForce().MothershipAndDrone.MothershipLatch.Them != null &&
					 orbitMothership != StaticConstructablesManager.constructables.ToArray()[orbitindex].GetForce().MothershipAndDrone.MothershipLatch.Them.LinkedCorrectly?.OurForce.Id.Id))
				{
					orbittarget = StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId;
					if (StaticConstructablesManager.constructables.ToArray()[orbitindex].GetForce().MothershipAndDrone.MothershipLatch.Them != null)
					{
						orbitMothership = StaticConstructablesManager.constructables.ToArray()[orbitindex].GetForce().MothershipAndDrone.MothershipLatch.Them.LinkedCorrectly?.OurForce.Id.Id ?? -1;
					}
					else
					{
						orbitMothership = -1;
					}
				}
			}
			if (orbitcamOn && StaticConstructablesManager.constructables.Count != 0)
			{
				flycam.enabled = false;
				orbitcam.enabled = true;
			}
			else if (freecamOn)
			{
				orbitcam.enabled = false;
				flycam.enabled = true;
				flycam.transform.rotation = orbitcam.transform.rotation;
			}
			if (flycam.enabled && Parameters.DefaultKeys)
			{
				float x = 0;
				float y = 0;
				float z = 0;
				if (Input.GetKey(KeyCode.Space))
				{
					y += 1;
				}
				if (Input.GetKey(KeyCode.LeftAlt) | Input.GetKey(KeyCode.RightAlt))
				{
					y -= 1;
				}
				if (Input.GetKey(KeyCode.A))
				{
					x -= 1;
				}
				if (Input.GetKey(KeyCode.D))
				{
					x += 1;
				}
				if (Input.GetKey(KeyCode.W))
				{
					z += 1;
				}
				if (Input.GetKey(KeyCode.S))
				{
					z -= 1;
				}
				Vector3 val = new Vector3(x, y, z);
				if (speedUp)
				{
					val *= 4; //increase vector with shift
				}
				if (slowDown)
				{
					val /= 4; //decrease vector with strg
				}
				flycam.transform.position = flycam.transform.position + flycam.transform.localRotation * val;
			}
			if (flycam.enabled && !Parameters.DefaultKeys)
			{
				Vector3 movement = ftdKeyMap.GetMovemementDirection() * viewAndControl.ExternalCameraSpeed;
				float speedMultiplicator = viewAndControl.BoostCameraSpeed / viewAndControl.ExternalCameraSpeed;
				movement *= speedUp ? speedMultiplicator : 1;
				movement /= slowDown ? speedMultiplicator : 1;
				flycam.transform.position += flycam.transform.localRotation * movement;
			}
			else if (orbitcam.enabled)
			{
				if (StaticConstructablesManager.constructables.Count == 0)
				{
					flycam.enabled = true;
					orbitcam.enabled = false;
				}
				else
				{
					if (oldIndex != orbitindex || orbittarget != oldTarget)
					{
						orbitcam.OrbitTarget = new PositionAndRotationReturnConstruct(StaticConstructablesManager.constructables[orbitindex], BrilliantSkies.Core.Returns.PositionReturnConstructReferenceSelection.CenterOfMass);
					}
				}
			}
			else
			{
				AdvLogger.LogError("How can both Monobehaviors be disabled?", LogOptions.OnlyInDeveloperLog);
			}
		}
		public Quaternion Rotation => Quaternion.Euler(0, Parameters.Rotation, 0);
		/// <summary>
		/// Retrieves the CombinedFormation of a given Team.
		/// </summary>
		/// <param name="index">The index of the Team</param>
		/// <returns></returns>
		public CombinedFormation GetFormation(int index)
		{
			return teamFormations[index];
		}
		/// <summary>
		/// Overrides the default team colors with the ones set in the Parameters.
		/// </summary>
		public void ApplyFactionColors()
		{
			TournamentPlugin.factionManagement.EnsureFactionCount(Parameters.ActiveFactions);
			for (int i = 0; i < Parameters.ActiveFactions; i++)
			{
				TournamentPlugin.factionManagement.factions[i].OverrideFleetColors(new Color[] { Parameters.MainColorsPerTeam[i], Parameters.SecondaryColorsPerTeam[i], Parameters.TrimColorsPerTeam[i], Parameters.DetailColorsPerTeam[i] });
			}
		}
	}
}