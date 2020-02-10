using BrilliantSkies.Core.Constants;
using BrilliantSkies.Core.FilesAndFolders;
using BrilliantSkies.Core.Id;
using BrilliantSkies.Core.Returns.PositionAndRotation;
using BrilliantSkies.Core.Timing;
using BrilliantSkies.Core.Types;
using BrilliantSkies.Core.UniverseRepresentation;
using BrilliantSkies.Core.UniverseRepresentation.Positioning.Frames.Points;
using BrilliantSkies.Effects.Cameras;
using BrilliantSkies.FromTheDepths.Game;
using BrilliantSkies.Ftd.Avatar;
using BrilliantSkies.Ftd.Planets;
using BrilliantSkies.Ftd.Planets.Instances;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.GridCasts;
using BrilliantSkies.PlayerProfiles;
using BrilliantSkies.Ui.Special.PopUps;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tournament.Serialisation;
using Tournament.UI;
using UnityEngine;
namespace Tournament
{
	public class Tournament : BrilliantSkies.FromTheDepths.Game.UserInterfaces.InteractiveOverlay.InteractiveOverlay
	{
		public static Tournament _me;
		public TournamentConsole _GUI;
		#region GUI-Stil
		private readonly GUIStyle timerStyle;
		private readonly GUIStyle extrainfoLeft;
		private readonly GUIStyle sidelist;
		private readonly GUIStyle extrainfoRight;
		private readonly GUIStyle extrainfoName;
		private static readonly Gradient penaltyTimeColor;
		#endregion
		#region Kamerakontrolle
		private GameObject cam;
		private MouseLook flycam;
		private MouseOrbit orbitcam;
		//private DemoCamera demoCamera;
		private int orbittarget;
		private int orbitMothership;
		private int orbitindex;
		#endregion
		private bool extraInfo;
		private float timerTotal;
		private float timerTotal2;
		private byte overtimeCounter;
		private Vector2 scrollPos = Vector2.zero;
		//Management
		private readonly Dictionary<ObjectId, Dictionary<MainConstruct, Participant>> HUDLog = new Dictionary<ObjectId, Dictionary<MainConstruct, Participant>>();
		private bool showLists = true;
		public List<Formation> teamFormations = new List<Formation>();
		public Parameters Parameters { get; set; } = new Parameters(1u);
		public Dictionary<int, List<Entry>> entries = new Dictionary<int, List<Entry>>();
		private List<int> materials;
		static Tournament() {
			penaltyTimeColor = new Gradient
			{
				colorKeys = new GradientColorKey[] {
				new GradientColorKey(Color.white, 0f),
				new GradientColorKey(Color.blue, 0.25f),
				new GradientColorKey(Color.green, 0.5f),
				new GradientColorKey(new Color(1f, 1f, 0f), 0.75f),
				new GradientColorKey(new Color(1f, 0.5f, 0f), 0.875f),
				new GradientColorKey(Color.red, 1f)
			},
				alphaKeys = new GradientAlphaKey[] {
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			},
				mode = GradientMode.Blend
			};
		}
		public Tournament()
		{
			_me = this;
			LoadSettings();
			timerStyle = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
			{
				alignment = TextAnchor.MiddleCenter,
				richText = true,
				fontSize = 12
			};
			extrainfoLeft = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
			{
				alignment = TextAnchor.UpperLeft,
				richText = true,
				fontSize = 12,
				wordWrap = false,
				clipping = TextClipping.Clip
			};
			sidelist = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
			{
				alignment = TextAnchor.UpperLeft,
				richText = true,
				fontSize = 12,
				wordWrap = false,
				clipping = TextClipping.Clip,
				stretchHeight = false
			};
			sidelist.normal.textColor = Color.white;
			extrainfoRight = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
			{
				alignment = TextAnchor.UpperRight,
				richText = true,
				fontSize = 12,
				wordWrap = false,
				clipping = TextClipping.Clip
			};
			extrainfoName = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
			{
				alignment = TextAnchor.UpperRight,
				richText = true,
				fontSize = 12,
				wordWrap = true,
				clipping = TextClipping.Clip
			};
			_GUI = new TournamentConsole(_me);
		}
		/// <summary>
		/// Loads in all selected crafts and sets their materialstorage or the storage of their team, depending if local resources are enabled.
		/// </summary>
		public void LoadCraft()
		{
			ClearArea();
			HUDLog.Clear();
			materials?.Clear();
			materials = null;
			InstanceSpecification.i.Header.CommonSettings.EnemyBlockDestroyedResourceDrop = Parameters.MaterialConversion / 100f;
			InstanceSpecification.i.Header.CommonSettings.LocalisedResourceMode = Parameters.LocalResources ? LocalisedResourceMode.UseLocalisedStores : LocalisedResourceMode.UseCentralStore;
			if (Parameters.LocalResources)
			{
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
							mc.RawResource.Material.SetQuantity(Parameters.ResourcesPerTeam[team.Key]);
						}
					}
				}
				else
				{
					List<int> materials = new List<int>(Parameters.ResourcesPerTeam.Us);
					Dictionary<int, int> maxMaterials = new Dictionary<int, int>();
					Dictionary<int, List<MainConstruct>> constructs = new Dictionary<int, List<MainConstruct>>();
					foreach (KeyValuePair<int, List<Entry>> team in entries)
					{
						if (team.Key >= Parameters.ActiveFactions)
						{
							break;
						}
						maxMaterials.Add(team.Key, 0);
						constructs.Add(team.Key, new List<MainConstruct>());
						for (int pos = 0; pos < team.Value.Count; pos++)
						{
							MainConstruct mc = team.Value[pos].Spawn(Parameters.StartingDistance, Parameters.SpawngapLR[team.Key], Parameters.SpawngapFB[team.Key], team.Value.Count, pos);
							constructs[team.Key].Add(mc);
							maxMaterials[team.Key] += (int)mc.RawResource.Material.Maximum;
						}
					}
					for (int i = 0; i < Parameters.ActiveFactions; i++)
					{
						if (maxMaterials[i] <= materials[i])
						{
							foreach (MainConstruct mc in constructs[i])
							{
								mc.RawResource.Material.SetQuantity(materials[i]);
								materials[i] -= (int)mc.RawResource.Material.Maximum;
							}
							TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResources(materials[i]);
						}
						else
						{
							double expectedFraction = ((double)materials[i]) / maxMaterials[i];
							foreach (MainConstruct mc in constructs[i])
							{
								mc.RawResource.Material.SetQuantity(mc.RawResource.Material.Maximum * expectedFraction);
							}
							TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResources(0);
						}
					}
				}
			}
			else //Global Store
			{
				materials = new List<int>(Parameters.ResourcesPerTeam.Us);
				for (int i = 0; i < Parameters.ActiveFactions; i++)
				{
					if (Parameters.InfinteResourcesPerTeam[i])
					{
						TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResourcesInfinite();
					}
					else
					{
						TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResources(Parameters.ResourcesPerTeam[i]);
					}
				}
				foreach (KeyValuePair<int, List<Entry>> team in entries)
				{
					for (int pos = 0; pos < team.Value.Count; pos++)
					{
						team.Value[pos].Spawn(Parameters.StartingDistance, Parameters.SpawngapLR[team.Key], Parameters.SpawngapFB[team.Key], team.Value.Count, pos);
						StaticConstructablesManager.constructables[StaticConstructablesManager.constructables.Count - 1].RawResource.Material.SetQuantity(0);
					}
				}
			}
		}
		/// <summary>
		/// Sets Cleanup-Functions, adds all Mainconstruct into the HUDLog and registers its methods.
		/// </summary>
		public void StartMatch()
		{
			overtimeCounter = 0;
			timerTotal = 0f;
			timerTotal2 = Time.timeSinceLevelLoad;
			ConstructableCleanUpSettings ccus = InstanceSpecification.i.Header.CommonSettings.ConstructableCleanUpSettings;
			InstanceSpecification.i.Header.CommonSettings.ConstructableCleanUp = (ConstructableCleanUp)Parameters.CleanUpMode.Us;
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
			flycam.transform.position = new Vector3(-500f, 50f, 0f);
			flycam.transform.rotation = Quaternion.LookRotation(Vector3.right);
			foreach (MainConstruct constructable in StaticConstructablesManager.constructables)
			{
				if (!HUDLog.ContainsKey(constructable.GetTeam()))
				{
					HUDLog.Add(constructable.GetTeam(), new Dictionary<MainConstruct, Participant>());
				}
				if (!HUDLog[constructable.GetTeam()].ContainsKey(constructable))
				{
					bool spawnStick = constructable.BlockTypeStorage.MainframeStore.Count == 0;
					switch (Parameters.HealthCalculation)
					{
						case 0:
							HUDLog[constructable.GetTeam()].Add(constructable, new Participant
							{
								TeamId = constructable.GetTeam(),
								TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
								UniqueId = constructable.UniqueId,
								BlueprintName = constructable.GetName(),
								AICount = constructable.BlockTypeStorage.MainframeStore.Blocks.Count,
								HP = 100,
								HPCUR = (!spawnStick) ? constructable.AllBasics.GetNumberAliveBlocksIncludingSubConstructables() : constructable.AllBasics.GetNumberAliveBlocks(),
								HPMAX = (!spawnStick) ? constructable.AllBasics.GetNumberBlocksIncludingSubConstructables() : constructable.AllBasics.GetNumberBlocks()
							});
							break;
						case 1:
							HUDLog[constructable.GetTeam()].Add(constructable, new Participant
							{
								TeamId = constructable.GetTeam(),
								TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
								UniqueId = constructable.UniqueId,
								BlueprintName = constructable.GetName(),
								AICount = constructable.BlockTypeStorage.MainframeStore.Blocks.Count,
								HP = 100,
								HPCUR = constructable.AllBasics.GetResourceCost(false, true, false).Material,
								HPMAX = constructable.AllBasics.GetResourceCost(false, true, false).Material
							});
							break;
						case 2:
							HUDLog[constructable.GetTeam()].Add(constructable, new Participant
							{
								TeamId = constructable.GetTeam(),
								TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
								UniqueId = constructable.UniqueId,
								BlueprintName = constructable.GetName(),
								AICount = constructable.BlockTypeStorage.MainframeStore.Blocks.Count,
								HP = 100,
								HPCUR = constructable.AllBasics.VolumeAliveUsed,
								HPMAX = constructable.AllBasics.VolumeAliveUsed
							});
							break;
						case 3:
							HUDLog[constructable.GetTeam()].Add(constructable, new Participant
							{
								TeamId = constructable.GetTeam(),
								TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
								UniqueId = constructable.UniqueId,
								BlueprintName = constructable.GetName(),
								AICount = constructable.BlockTypeStorage.MainframeStore.Blocks.Count,
								HP = 100,
								HPCUR = constructable.AllBasics.VolumeOfFullAliveBlocksUsed,
								HPMAX = constructable.AllBasics.VolumeOfFullAliveBlocksUsed
							});
							break;
						default:
							break;
					}
				}
			}
			GameEvents.PreLateUpdate += LateUpdate;
			GameEvents.Twice_Second += SlowUpdate;
			GameEvents.FixedUpdateEvent += FixedUpdate;
			GameEvents.OnGui += OnGUI;
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
			orbittarget = 0;
			orbitindex = 0;
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
		public Vector3 FramePositionOfBoardSection()
		{
			return PlanetList.MainFrame.UniversalPositionToFramePosition(UniversalPositionOfBoardSection()+LocalTerrainOffsetFromSectionCenter());
		}
		public Vector3d UniversalPositionOfBoardSection()
		{
			return StaticCoordTransforms.BoardSectionToUniversalPosition(WorldSpecification.i.BoardLayout.BoardSections[Parameters.EastWestBoard, Parameters.NorthSouthBoard].BoardSectionCoords);
		}
		public Vector3 LocalTerrainOffsetFromSectionCenter() {
			return WorldSpecification.i.BoardLayout.TerrainSize * new Vector3(Parameters.EastWestTerrain, 0, Parameters.NorthSouthTerrain);
		}
		public Vector3 LocalOffsetFromTerrainCenter() {
			return new Vector3(Parameters.EastWestOffset, 0, Parameters.NorthSouthOffset);
		}
		/// <summary>
		/// Saves the current Parameters.
		/// </summary>
		public void SaveSettings()
		{
			string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
			FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder, "parameters.json"));
			settingsFile.SaveData(Parameters, Formatting.Indented);
		}
		/// <summary>
		/// Loads the Parameters-File.
		/// </summary>
		public void LoadSettings()
		{
			string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
			FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder, "parameters.json"));
			if (settingsFile.Exists)
			{
				try
				{
					Parameters = settingsFile.LoadData<Parameters>();
					Parameters.EnsureEnoughData();
				}
				catch (Exception)
				{
					LoadDefaults();
					SaveSettings();
					GuiPopUp.Instance.Add(new PopupError("Could not load Settings", "Something went wrong during the loading of your last settings. This could be because of a corrupt Savefile " +
						"or some of the Datatypes have been changed and can not be loaded. To prevent future Errors, we just saved the default settings into the Savefile."));
				}
			}
			else
			{
				LoadDefaults();
			}
			for (int i = 0; i < Parameters.ActiveFactions; i++)
			{
				entries.Add(i, new List<Entry>());
			}
		}
		/// <summary>
		/// Resets the Parameters and makes sure that there is enough data for the GUI.
		/// </summary>
		public void LoadDefaults()
		{
			Parameters.ResetToDefault();
			Parameters.EnsureEnoughData();
		}
		/// <summary>
		/// Displays the Sidelist and when required the Extra-Info-Panel.
		/// </summary>
		public void OnGUI()
		{
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f * Screen.width / 1280f, 1f * Screen.height / 800f, 1f));
			GUI.backgroundColor = new Color(0f, 0f, 0f, 0.6f);
			GUI.Label(new Rect(590f, 0f, 100f, 30f), $"{Mathf.Floor(timerTotal / 60f)}m {Mathf.Floor(timerTotal % 60f)}s", timerStyle);
			if (showLists)
			{
				GUILayout.BeginArea(new Rect(0, 50, 200, 700), sidelist);
				scrollPos = GUILayout.BeginScrollView(scrollPos);
				float t = Time.realtimeSinceStartup * 30;
				foreach (KeyValuePair<ObjectId, Dictionary<MainConstruct, Participant>> team in HUDLog)
				{
					string teamMaterials = "M: " + team.Key.FactionInst().ResourceStore.Material.ToString();
					float teamMaxHP = 0, teamCurHP = 0;
					teamMaxHP = team.Value.Values.Aggregate(0f, (currentSum, member) => currentSum + member.HPMAX);
					teamCurHP = team.Value.Values.Aggregate(0f, (currentSum, member) => currentSum + member.HPCUR);
					string teamHP = $"{Mathf.RoundToInt(100 * teamCurHP / teamMaxHP)}%";
					GUILayout.Label($"<color=cyan>{team.Key.FactionSpec().Name} @ {teamHP}, {teamMaterials}</color>", sidelist);
					int maxTimeForTeam = Parameters.MaximumPenaltyTime[TournamentPlugin.factionManagement.TeamIndexFromObjectID(team.Key)];
					foreach (KeyValuePair<MainConstruct, Participant> member in team.Value)
					{
						string name = member.Value.BlueprintName;
						string percentHP = $"{Mathf.RoundToInt(member.Value.HP)}%";
						float penaltyFraction = member.Value.OoBTime / maxTimeForTeam;
						Color32 timeColor = penaltyTimeColor.Evaluate(penaltyFraction);
						string penaltyTime = $"<color=#{string.Format("{0:X2}{1:X2}{2:X2}", timeColor.r, timeColor.g, timeColor.b)}>{Mathf.Floor(member.Value.OoBTime / 60)}m{Mathf.Floor(member.Value.OoBTime) % 60}s</color>";
						bool disqualified = member.Value.Disqual || member.Value.Scrapped || (Parameters.CleanUpMode != 0 && member.Value.AICount == 0);
						GUIContent memberContent;
						if (disqualified)
						{
							memberContent = new GUIContent($"<color=red>{name} DQ @{Math.Floor(member.Value.TimeOfDespawn / 60f)}m {Math.Floor(member.Value.TimeOfDespawn % 60f)}s</color>");
						}
						else
						{
							memberContent = new GUIContent($"{name} @ {percentHP}, {penaltyTime}");
						}
						Vector2 size = sidelist.CalcSize(memberContent);
						if (size.x <= 150)
						{
							GUILayout.Label(memberContent, sidelist);
						}
						else
						{
							GUILayout.BeginScrollView(new Vector2(t % (size.x + 50) - 25, 0), false, false, GUIStyle.none, GUIStyle.none);
							GUILayout.Label(memberContent, sidelist);
							GUILayout.EndScrollView();
						}
					}
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndScrollView();
				GUILayout.EndArea();
			}
			// extra info panel
			if (extraInfo)
			{
				IMainConstructBlock target = GetTarget();
				if (target != null)
				{
					MainConstruct targetConstruct = StaticConstructablesManager.constructables.Where(x => x.iMain == target).First();
					string name = targetConstruct.blueprintName;
					string team = targetConstruct.GetTeam().FactionSpec().Name;
					string hp = $"{Math.Round(targetConstruct.AllBasics.GetFractionAliveBlocksIncludingSubConstructables() * 100f, 1).ToString()}%";
					string resources = $"{Math.Round(targetConstruct.RawResource.Material.Quantity, 0)}/{Math.Round(targetConstruct.RawResource.Material.Maximum, 0)}";
					string ammo = $"{Math.Round(targetConstruct.Ammo.Ammo.Quantity, 0)}/{Math.Round(targetConstruct.Ammo.Ammo.Maximum, 0)}";
					string fuel = $"{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Fuel.Quantity, 0)}/{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Fuel.Maximum, 0)}";
					string battery = $"{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Energy.Quantity, 0)}/{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Energy.Maximum, 0)}";
					string power = $"{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Power, 0)} / {Math.Round(targetConstruct.PowerUsageCreationAndFuel.MaxPower, 0)}";
					string speed = $"{Math.Round(targetConstruct.Velocity.magnitude, 1)}m/s";
					string altitude = $"{Math.Round(targetConstruct.CentreOfMass.y, 0)}m";

					float closest = -1f;
					foreach (MainConstruct construct in StaticConstructablesManager.constructables.ToArray())
					{
						if (construct.GetTeam() != targetConstruct.GetTeam())
						{
							float distance = Vector3.Distance(construct.CentreOfMass, targetConstruct.CentreOfMass);
							if (closest < 0f || distance < closest)
							{
								closest = distance;
							}
						}
					}
					string nearest = Math.Round(closest, 0).ToString() + "m";

					GUI.Label(new Rect(980, 0, 90f, 38f), "Name:", extrainfoLeft);
					//GUI.Label(new Rect(980, 38, 90f, 38f), "Team:", extrainfoLeft);
					GUI.Label(new Rect(980, 76, 90f, 38f), "HP:", extrainfoLeft);
					GUI.Label(new Rect(980, 114, 90f, 38f), "Materials:", extrainfoLeft);
					GUI.Label(new Rect(980, 152, 90f, 38f), "Ammo:", extrainfoLeft);
					GUI.Label(new Rect(980, 190, 90f, 38f), "Fuel:", extrainfoLeft);
					GUI.Label(new Rect(980, 228, 90f, 38f), "Battery:", extrainfoLeft);
					GUI.Label(new Rect(980, 266, 90f, 38f), "Power:", extrainfoLeft);
					GUI.Label(new Rect(980, 304, 90f, 38f), "Speed:", extrainfoLeft);
					GUI.Label(new Rect(980, 342, 90f, 38f), "Altitude:", extrainfoLeft);
					GUI.Label(new Rect(980, 380, 90f, 38f), "Nearest Enemy:", extrainfoLeft);

					GUI.Label(new Rect(1070, 0, 110f, 38f), name, extrainfoName);
					GUI.Label(new Rect(980, 38, 200f, 38f), team, extrainfoName);
					GUI.Label(new Rect(1070, 76, 110f, 38f), hp, extrainfoRight);
					GUI.Label(new Rect(1070, 114, 110f, 38f), resources, extrainfoRight);
					GUI.Label(new Rect(1070, 152, 110f, 38f), ammo, extrainfoRight);
					GUI.Label(new Rect(1070, 190, 110f, 38f), fuel, extrainfoRight);
					GUI.Label(new Rect(1070, 228, 110f, 38f), battery, extrainfoRight);
					GUI.Label(new Rect(1070, 266, 110f, 38f), power, extrainfoRight);
					GUI.Label(new Rect(1070, 304, 110f, 38f), speed, extrainfoRight);
					GUI.Label(new Rect(1070, 342, 110f, 38f), altitude, extrainfoRight);
					GUI.Label(new Rect(1070, 380, 110f, 38f), nearest, extrainfoRight);
				}
			}
		}
		/// <summary>
		/// Performes a GridCast against all Constructables starting a the camera position and into the direction its facing. The maximum distance is 1000m.
		/// </summary>
		/// <returns>The Mainconstruct if any was hit, otherwise null.</returns>
		private IMainConstructBlock GetTarget()
		{
			IMainConstructBlock target = null;
			Transform myTransform = flycam.enabled ? flycam.transform : orbitcam.transform;
			GridCastReturn gridCastReturn = GridCasting.GridCastAllConstructables(new GridCastReturn(myTransform.position, myTransform.forward, 1000, 1, true));
			if (gridCastReturn.HitSomething)
			{
				if (gridCastReturn.FirstHit.BlockHit.IsOnSubConstructable)
				{
					target = gridCastReturn.FirstHit.BlockHit.ParentConstruct.iMain;
				}
				else
				{
					target = gridCastReturn.FirstHit.BlockHit.MainConstruct;
				}
			}
			return target;
		}
		/// <summary>
		/// Handles input and moves, rotates and places the camera.
		/// </summary>
		public void LateUpdate()
		{
			FtdKeyMap ftdKeyMap = ProfileManager.Instance.GetModule<FtdKeyMap>();
			bool pause = false;
			bool next = false;
			bool previous = false;
			bool shift = false;
			bool strg = false;
			bool freecamOn = false;
			bool orbitcamOn = false;
			bool changeExtraInfo = false;
			bool changeShowList = false;
			if (Parameters.DefaultKeys.Us)
			{
				pause = Input.GetKeyDown(KeyCode.F11); // default f11
				shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
				strg = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
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
				shift = ftdKeyMap.IsKey(KeyInputsFtd.SpeedUpCamera, KeyInputEventType.Held, ModifierAllows.AllowUnnecessary);
				strg = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
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
			}
			if (Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				if (shift)
				{
					orbitcam.distance = (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") > 0f) ? (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") * 100f) : 0f;
				}
				else if (strg)
				{
					orbitcam.distance = (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") > 0f) ? (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") * 25f) : 0f;
				}
				else
				{
					orbitcam.distance = (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") > 0f) ? (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") * 50f) : 0f;
				}
			}
			if (StaticConstructablesManager.constructables.Count > 0)
			{
				if (orbitindex >= StaticConstructablesManager.constructables.Count)
				{
					orbitindex = 0;
				}
				if (StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId != orbittarget && orbittarget != 0 ||
					(orbitMothership != -1 && StaticConstructablesManager.constructables.ToArray()[orbitindex].Drones.LoadedMothershipC.uniqueID != orbitMothership))
				{
					int index;
					if (orbitMothership == -1)
					{
						index = StaticConstructablesManager.constructables.FindIndex(0, m => m.UniqueId == orbittarget);
					}
					else
					{
						index = StaticConstructablesManager.constructables.FindIndex(0, m => m.UniqueId == orbittarget && m.Drones.LoadedMothershipC.uniqueID == orbitMothership);
					}
					if (index >= 0) { orbitindex = index; }
					else { orbitindex = 0; }
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
					(StaticConstructablesManager.constructables.ToArray()[orbitindex].Drones.LoadedMothershipC != null &&
					 orbitMothership != StaticConstructablesManager.constructables.ToArray()[orbitindex].Drones.LoadedMothershipC.uniqueID))
				{
					orbittarget = StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId;
					if (StaticConstructablesManager.constructables.ToArray()[orbitindex].Drones.LoadedMothershipC != null)
					{
						orbitMothership = StaticConstructablesManager.constructables.ToArray()[orbitindex].Drones.LoadedMothershipC.uniqueID;
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
				if (shift)
				{
					val *= 4; //increase vector with shift
				}
				if (strg)
				{
					val /= 4; //decrease vector with strg
				}
				flycam.transform.position = flycam.transform.position + flycam.transform.localRotation * val;
			}
			if (flycam.enabled && !Parameters.DefaultKeys)
			{
				Vector3 movement = ftdKeyMap.GetMovemementDirection() * (shift ? 4 : 1);
				movement /= strg ? 4 : 1;
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
					Vector3d position = PlanetList.MainFrame.FramePositionToUniversalPosition(StaticConstructablesManager.constructables.ToArray()[orbitindex].CentreOfMass);
					Quaternion rotation = StaticConstructablesManager.constructables.ToArray()[orbitindex].SafeRotation;
					orbitcam.OrbitTarget = new PositionAndRotationReturnUniverseCoord(new UniversalTransform(position, rotation));
				}
			}
		}
		/// <summary>
		/// Gets called once every physics update. Negates material-gain by self-shooting if Lifesteal is -1%. Also determines if a Particiant is currently violating any rules and gives out a penalty.
		/// </summary>
		/// <param name="dt"></param>
		public void FixedUpdate(ITimeStep dt)
		{
			if (!GameSpeedManager.Instance.IsPaused)
			{
				if (Parameters.MaterialConversion == -1 && materials != null) //Verbietet Materialrückgewinnung durch Selbst- und Teambeschuss.
				{
					for (int i = 0; i < Parameters.ActiveFactions; i++)
					{
						if (materials[i] < TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.Material.Quantity)
						{
							TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResources(materials[i]);
						}
						else
						{
							materials[i] = (int)TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.Material.Quantity;
						}
					}
				}
				MainConstruct[] array = StaticConstructablesManager.constructables.ToArray();
				if (array.Count() == 0)
				{
					GameSpeedManager.Instance.TogglePause();
					//Everyone died...
				}
				foreach (MainConstruct currentConstruct in array)
				{
					if (!HUDLog[currentConstruct.GetTeam()].TryGetValue(currentConstruct, out Participant tournamentParticipant))
					{
						UpdateConstructs();
						tournamentParticipant = HUDLog[currentConstruct.GetTeam()][currentConstruct];
					}
					//The participant is still in the game:
					if (!tournamentParticipant.Disqual || !tournamentParticipant.Scrapped)
					{
						int teamIndex = TournamentPlugin.factionManagement.TeamIndexFromObjectID(currentConstruct.GetTeam());
						tournamentParticipant.AICount = currentConstruct.BlockTypeStorage.MainframeStore.Blocks.Count;
						bool violatingRules = false;
						//Is it braindead?
						if (tournamentParticipant.AICount == 0)
						{
							violatingRules = true;
						}
						//Is it outside its altitude limits?
						if (!violatingRules && (currentConstruct.CentreOfMass.y < Parameters.AltitudeLimits[teamIndex].x || currentConstruct.CentreOfMass.y > Parameters.AltitudeLimits[teamIndex].y))
						{
							if (Parameters.SoftLimits[teamIndex])
							{
								if (currentConstruct.CentreOfMass.y < Parameters.AltitudeLimits[teamIndex].x && -currentConstruct.Velocity.y > Parameters.AltitudeReverse[teamIndex]) //Below minimum altitude and still sinking.
								{
									violatingRules = true;
								}
								else if (currentConstruct.CentreOfMass.y > Parameters.AltitudeLimits[teamIndex].y && currentConstruct.Velocity.y > Parameters.AltitudeReverse[teamIndex]) //Above maximum altitude and still rising.
								{
									violatingRules = true;
								}
							}
							else
							{
								violatingRules = true;
							}
						}
						//Does it have too little HP?
						if (!violatingRules && tournamentParticipant.HP < Parameters.MinimumHealth)
						{
							violatingRules = true;
						}
						//Is it too fast?
						if (!violatingRules && currentConstruct.Velocity.magnitude > Parameters.MaximumSpeed[teamIndex])
						{
							violatingRules = true;
						}
						//Is it too far away from enemies?
						if (!violatingRules)
						{
							violatingRules = CheckDistanceAll(currentConstruct, teamIndex, Parameters.EnemyAttackPercentage[teamIndex]/100f, out bool noEnemies);
							//Are there no more enemies?
							if (noEnemies && Parameters.PauseOnVictory)
							{
								GameSpeedManager.Instance.TogglePause();
								break;
								//Checking additional constructs does no longer change the outcome, we still need to update the timer though.
							}
						}
						//Has any rule been violated?
						if (violatingRules)
						{
							AddPenalty(tournamentParticipant, teamIndex);
						}
						else
						{
							//Recover the timebuffer if soft limits are active.
							if (Parameters.SoftLimits[teamIndex])
							{
								tournamentParticipant.OoBTimeBuffer = 0;
							}
						}
						tournamentParticipant.Disqual = tournamentParticipant.OoBTime > Parameters.MaximumPenaltyTime[teamIndex];
					}
				}
				timerTotal += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="current"></param>
		/// <param name="teamIndex"></param>
		/// <param name="percentage"></param>
		/// <param name="noEnemiesFound"></param>
		/// <returns></returns>
		private bool CheckDistanceAll(MainConstruct current, int teamIndex, float percentage, out bool noEnemiesFound)
		{
			MainConstruct[] array2 = StaticConstructablesManager.constructables.ToArray();
			List<MainConstruct> enemies = new List<MainConstruct>(array2.Length);
			foreach (MainConstruct potentialEnemy in array2)
			{
				if (current != potentialEnemy && current.GetTeam() != potentialEnemy.GetTeam())
				{
					enemies.Add(potentialEnemy);
				}
			}
			if (enemies.Count == 0)
			{
				noEnemiesFound = true;
				return false;
			}
			noEnemiesFound = false;
			int rulebreaks = 0;
			foreach (MainConstruct enemy in enemies)
			{
				float currentDistance = Parameters.ProjectedDistance[teamIndex] ? DistanceProjected(current.CentreOfMass, enemy.CentreOfMass) : Vector3.Distance(current.CentreOfMass, enemy.CentreOfMass);
				float futureDistance = Parameters.ProjectedDistance[teamIndex] ? DistanceProjected(current.CentreOfMass + current.Velocity, enemy.CentreOfMass) : Vector3.Distance(current.CentreOfMass + current.Velocity, enemy.CentreOfMass);
				//Too far away?
				if(currentDistance>Parameters.DistanceLimit[teamIndex]){
					if (Parameters.SoftLimits[teamIndex])
					{
						//Going away faster than DistanceReverse allows?
						if (futureDistance > currentDistance + Parameters.DistanceReverse[teamIndex])
						{ rulebreaks++; }
					}
					else
					{ rulebreaks++; }
				}
			}
			return Mathf.Max(1, Mathf.RoundToInt((1 - percentage) * enemies.Count)) <= rulebreaks;
		}
		/// <summary>
		/// Increases the Penalty-Time of a single Participant or uses up the Time-Buffer if Soft-Limits are active for the given Team.
		/// </summary>
		/// <param name="tournamentParticipant">The offending Participant</param>
		/// <param name="teamIndex">The index of its corresponding Team</param>
		private void AddPenalty(Participant tournamentParticipant, int teamIndex)
		{
			if (Parameters.SoftLimits[teamIndex])
			{
				if (tournamentParticipant.OoBTimeBuffer > Parameters.MaximumBufferTime[teamIndex])
				{
					tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
				}
				else
				{
					tournamentParticipant.OoBTimeBuffer += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
				}
			}
			else
			{
				tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
			}
		}
		/// <summary>
		/// Gets called twice a second. Despawns Mainconstructs which have accumulated too much Penalty-time and pauses the Game once the time limit or the end of an overtime-section is reached.
		/// </summary>
		/// <param name="dt"></param>
		public void SlowUpdate(ITimeStep dt)
		{
			UpdateConstructs();
			foreach (KeyValuePair<ObjectId, Dictionary<MainConstruct, Participant>> teamAndMembers in HUDLog)
			{
				foreach (KeyValuePair<MainConstruct, Participant> member in HUDLog[teamAndMembers.Key])
				{
					if (StaticConstructablesManager.constructables.Contains(member.Key))
					{
						if (!HUDLog[teamAndMembers.Key][member.Key].Disqual)
						{
							continue;
						}
						else if (!HUDLog[teamAndMembers.Key][member.Key].Scrapped)
						{
							HUDLog[teamAndMembers.Key][member.Key].HPCUR = 0f;
							HUDLog[teamAndMembers.Key][member.Key].Scrapped = true;
							Vector3 centreOfMass = member.Key.CentreOfMass;
							UnityEngine.Object.Instantiate(Resources.Load("Detonator-MushroomCloud") as GameObject, centreOfMass, Quaternion.identity);
							member.Key.DestroyCompletely(true);
							HUDLog[teamAndMembers.Key][member.Key].TimeOfDespawn = timerTotal;
						}
					}
					else if (!(HUDLog[teamAndMembers.Key][member.Key].Disqual && HUDLog[teamAndMembers.Key][member.Key].Scrapped))
					{
						HUDLog[teamAndMembers.Key][member.Key].Disqual = HUDLog[teamAndMembers.Key][member.Key].Scrapped = true;
						HUDLog[teamAndMembers.Key][member.Key].HPCUR = 0;
						HUDLog[teamAndMembers.Key][member.Key].TimeOfDespawn = timerTotal;
					}
				}
			}
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
		/// Includes any new Mainconstruct into the HUDLog and updates current health-values.
		/// </summary>
		private void UpdateConstructs()
		{
			MainConstruct[] array = StaticConstructablesManager.constructables.ToArray();
			foreach (MainConstruct val in array)
			{
				if (!HUDLog[val.GetTeam()].TryGetValue(val, out Participant tournamentParticipant))
				{
					tournamentParticipant = new Participant
					{
						AICount = val.BlockTypeStorage.MainframeStore.Count,
						TeamId = val.GetTeam(),
						TeamName = val.GetTeam().FactionSpec().Name,
						BlueprintName = val.GetBlueprintName(),
						Disqual = false,
						Scrapped = false,
						UniqueId = val.UniqueId
					};
					switch (Parameters.HealthCalculation)
					{
						case 0:
							tournamentParticipant.HPMAX = val.AllBasics.GetNumberBlocksIncludingSubConstructables();
							break;
						case 1:
							tournamentParticipant.HPMAX = val.AllBasics.GetResourceCost(false, true, false).Material;
							break;
						case 2:
						case 3:
							tournamentParticipant.HPMAX = 0;
							for (int x = val.AllBasics.minx_; x <= val.AllBasics.maxx_; x++)
							{
								for (int y = val.AllBasics.miny_; x <= val.AllBasics.maxy_; y++)
								{
									for (int z = val.AllBasics.minz_; x <= val.AllBasics.maxz_; z++)
									{
										Block b = val.AllBasics[x, y, z];
										if (b != null)
										{
											if (Parameters.HealthCalculation == 2)
											{
												tournamentParticipant.HPMAX += b.item.SizeInfo.VolumeFactor;
											}
											else
											{
												tournamentParticipant.HPMAX += 1;
											}
										}
									}
								}
							}
							break;
						default:
							Debug.LogError("Health calculation of newly spawned in Construct is not available!");
							break;
					}
					HUDLog[tournamentParticipant.TeamId][val] = tournamentParticipant;
				}
				if (!tournamentParticipant.Disqual || !tournamentParticipant.Scrapped)
				{
					switch (Parameters.HealthCalculation)
					{
						case 0:
							tournamentParticipant.HPCUR = val.AllBasics.GetNumberAliveBlocksIncludingSubConstructables();
							break;
						case 1:
							tournamentParticipant.HPCUR = val.AllBasics.GetResourceCost(true, true, false).Material;
							break;
						case 2:
							tournamentParticipant.HPCUR = val.AllBasics.VolumeAliveUsed;
							break;
						case 3:
							tournamentParticipant.HPCUR = val.AllBasics.VolumeOfFullAliveBlocksUsed;
							break;
					}
					tournamentParticipant.HP = 100f * tournamentParticipant.HPCUR / tournamentParticipant.HPMAX;
				}
				else
				{
					tournamentParticipant.HPCUR = 0f;
					tournamentParticipant.HP = 0f;
				}
			}
		}
		public Quaternion Rotation => Quaternion.Euler(0, Parameters.Rotation, 0);
		/// <summary>
		/// Calculates the distance between two points, if they were on the XZ-Plane.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private float DistanceProjected(Vector3 a, Vector3 b)
		{
			a.y = 0;
			b.y = 0;
			return Vector3.Distance(a, b);
		}
		/// <summary>
		/// Retrieves the Formation-struct of a given Team.
		/// </summary>
		/// <param name="index">The index of the Team</param>
		/// <returns></returns>
		public Formation GetFormation(int index)
		{
			return Formation.tournamentFormations[Parameters.FormationIndexPerTeam[index]];
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