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
using BrilliantSkies.Ftd.Avatar.Build;
using BrilliantSkies.Ftd.Planets;
using BrilliantSkies.Ftd.Planets.Instances;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.GridCasts;
using BrilliantSkies.PlayerProfiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Tournament
{
	public class Tournament : BrilliantSkies.FromTheDepths.Game.UserInterfaces.InteractiveOverlay.InteractiveOverlay
    {
        public static Tournament _me;

        public TournamentGUI _GUI;

        private readonly GUIStyle timerStyle;

        private readonly GUIStyle extrainfoLeft;

        private readonly GUIStyle sidelist;

        private readonly GUIStyle extrainfoRight;

        private readonly GUIStyle extrainfoName;

        private GameObject cam;

        private MouseLook flycam;

        private MouseOrbit orbitcam;

        private int orbittarget;

        private int orbitMothership;

        private int orbitindex;

        private bool extraInfo;

        private float timerTotal;

        private float timerTotal2;

        private byte overtimeCounter;

        private Vector2 scrollPos=Vector2.zero;
        
        //Management

        private Dictionary<ObjectId, SortedDictionary<string, TournamentParticipant>> HUDLog = new Dictionary<ObjectId, SortedDictionary<string, TournamentParticipant>>();

        public List<TournamentEntry> entries_t1 = new List<TournamentEntry>();

        public List<TournamentEntry> entries_t2 = new List<TournamentEntry>();

        public List<TournamentEntry> entries_t3 = new List<TournamentEntry>();

        private bool showLists = true;

		public TournamentFormation formationTeam1, formationTeam2, formationTeam3;

        public TournamentParameters Parameters { get; set; } = new TournamentParameters(1u);

        public Tournament()
        {
            _me = this;
            _GUI = new TournamentGUI(_me);

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
            LoadNewSettings();
        }

        public void LoadCraft()
        {
            ClearArea();
            HUDLog.Clear();
            InstanceSpecification.i.Header.CommonSettings.EnemyBlockDestroyedResourceDrop = Parameters.MaterialConversion / 100f;
            InstanceSpecification.i.Header.CommonSettings.LocalisedResourceMode = Parameters.LocalResources ? LocalisedResourceMode.UseLocalisedStores : LocalisedResourceMode.UseCentralStore;
            foreach (TournamentEntry item in entries_t1)
            {
                item.Spawn(Parameters.StartingDistance, Parameters.SpawngapLR, Parameters.SpawngapFB, entries_t1.Count, entries_t1.IndexOf(item));
                if (Parameters.LocalResources)
                {
                    item.Team_id.FactionInst().ResourceStore.SetResources(0);
                }
                else if(Parameters.InfinteResourcesTeam1)
                {
                    item.Team_id.FactionInst().ResourceStore.SetResourcesInfinite();
                } else
                {
                    item.Team_id.FactionInst().ResourceStore.SetResources(Parameters.ResourcesTeam1);
                }
            }
            foreach (TournamentEntry item2 in entries_t2)
            {
                item2.Spawn(Parameters.StartingDistance, Parameters.SpawngapLR, Parameters.SpawngapFB, entries_t2.Count, entries_t2.IndexOf(item2));
                if (Parameters.LocalResources)
                {
                    item2.Team_id.FactionInst().ResourceStore.SetResources(0);
                }
                else if (Parameters.InfinteResourcesTeam2) {
                    item2.Team_id.FactionInst().ResourceStore.SetResourcesInfinite();
                }
                else
                {
                    item2.Team_id.FactionInst().ResourceStore.SetResources(Parameters.ResourcesTeam2);
                }
            }
            if (Parameters.ActiveFactions >= 3)
            {
                foreach (TournamentEntry item3 in entries_t3)
                {
                    item3.Spawn(Parameters.StartingDistance, Parameters.SpawngapLR, Parameters.SpawngapFB, entries_t3.Count, entries_t3.IndexOf(item3));
                    if (Parameters.LocalResources)
                    {
                        item3.Team_id.FactionInst().ResourceStore.SetResources(0);
                    }
                    else if (Parameters.InfinteResourcesTeam2)
                    {
                        item3.Team_id.FactionInst().ResourceStore.SetResourcesInfinite();
                    }
                    else
                    {
                        item3.Team_id.FactionInst().ResourceStore.SetResources(Parameters.ResourcesTeam2);
                    }
                }
            }
            if (Parameters.LocalResources)
            {
                foreach (MainConstruct construct in StaticConstructablesManager.constructables) {
                    if (construct.GetTeam() == TournamentPlugin.factionTeam1.Id)
                    {
                        construct.RawResource.Material.SetQuantity(Math.Max(construct.RawResource.Material.Maximum, Parameters.ResourcesTeam1));
                    }
                    else if (construct.GetTeam() == TournamentPlugin.factionTeam2.Id)
                    {
                        construct.RawResource.Material.SetQuantity(Math.Max(construct.RawResource.Material.Maximum, Parameters.ResourcesTeam2));
                    }
                    else if (construct.GetTeam() == TournamentPlugin.factionTeam3.Id) {
                        construct.RawResource.Material.SetQuantity(Math.Max(construct.RawResource.Material.Maximum, Parameters.ResourcesTeam3));
                    }
                    else
                    {
                        construct.RawResource.Material.SetQuantity(0);
                    }
                }
            }
        }

        public void StartMatch()
        {
            overtimeCounter = 0;
            timerTotal = 0f;
            timerTotal2 = Time.timeSinceLevelLoad;
            InstanceSpecification.i.Header.CommonSettings.ConstructableCleanUp = (ConstructableCleanUp)Parameters.CleanUpMode.Us;
            if (!GameSpeedManager.Instance.IsPaused) {
                GameSpeedManager.Instance.TogglePause();
            }
            orbitindex = 0;
            orbittarget = 0;

            flycam.transform.position = new Vector3(-500f, 50f, 0f);
            flycam.transform.rotation = Quaternion.LookRotation(Vector3.right);
            foreach (MainConstruct constructable in StaticConstructablesManager.constructables)
            {
                int id = 0;
                if (constructable.Drones.LoadedMothershipC != null)
                {
                    id = constructable.Drones.LoadedMothershipC.UniqueId;
                }
                string key = "" + constructable.UniqueId + "," + id;

                if (!HUDLog.ContainsKey(constructable.GetTeam()))
                {
                    HUDLog.Add(constructable.GetTeam(), new SortedDictionary<string, TournamentParticipant>());
                }

                if (!HUDLog[constructable.GetTeam()].ContainsKey(key))
                {
                    bool spawnStick = constructable.BlockTypeStorage.MainframeStore.Count == 0;
                    switch (Parameters.HealthCalculation)
                    {
                        case 0:
                            HUDLog[constructable.GetTeam()].Add(key, new TournamentParticipant
                            {
                                TeamId = constructable.GetTeam(),
                                TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
                                UniqueId = constructable.UniqueId,
                                MothershipId = id,
                                BlueprintName = constructable.GetName(),
                                AICount = constructable.BlockTypeStorage.MainframeStore.Blocks.Count,
                                HP = 100,
                                HPCUR = (!spawnStick) ? constructable.AllBasics.GetNumberAliveBlocksIncludingSubConstructables() : constructable.AllBasics.GetNumberAliveBlocks(),
                                HPMAX = (!spawnStick) ? constructable.AllBasics.GetNumberBlocksIncludingSubConstructables() : constructable.AllBasics.GetNumberBlocks()
                            });
                            break;
                        case 1:
                            HUDLog[constructable.GetTeam()].Add(key, new TournamentParticipant
                            {
                                TeamId = constructable.GetTeam(),
                                TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
                                UniqueId = constructable.UniqueId,
                                MothershipId = id,
                                BlueprintName = constructable.GetName(),
                                AICount = constructable.BlockTypeStorage.MainframeStore.Blocks.Count,
                                HP = 100,
                                HPCUR = constructable.AllBasics.GetResourceCost(false,true,false).Material,
                                HPMAX = constructable.AllBasics.GetResourceCost(false,true,false).Material
                            });
                            break;
                        case 2:
                            HUDLog[constructable.GetTeam()].Add(key, new TournamentParticipant
                            {
                                TeamId = constructable.GetTeam(),
                                TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
                                UniqueId = constructable.UniqueId,
                                MothershipId = id,
                                BlueprintName = constructable.GetName(),
                                AICount = constructable.BlockTypeStorage.MainframeStore.Blocks.Count,
                                HP = 100,
                                HPCUR = constructable.AllBasics.VolumeAliveUsed,
                                HPMAX = constructable.AllBasics.VolumeAliveUsed
                            });
                            break;
                        case 3:
                            HUDLog[constructable.GetTeam()].Add(key, new TournamentParticipant
                            {
                                TeamId = constructable.GetTeam(),
                                TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
                                UniqueId = constructable.UniqueId,
                                MothershipId = id,
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
            GameEvents.UpdateEvent -= UpdateBoardSectionPreview;
            GameEvents.PreLateUpdate += LateUpdate;
            GameEvents.Twice_Second += SlowUpdate;
            GameEvents.FixedUpdateEvent += FixedUpdate;
            GameEvents.OnGui += OnGUI;
        }

        public void ClearArea()
        {
            ForceManager.Instance.forces.ForEach(delegate (Force t)
            {
                ForceManager.Instance.DeleteForce(t);
            });
        }

        public void ResetCam()
        {
            foreach (PlayerSetupBase @object in Objects.Instance.Players.Objects)
            {
                UnityEngine.Object.Destroy(@object.gameObject);
            }
            if (cam != null) {
                UnityEngine.Object.Destroy(cam);
            }
            cam = R_Avatars.JustOrbitCamera.InstantiateACopy().gameObject;
            cam.gameObject.transform.position = new Vector3(-500f, 50f, 0f);
            cam.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.right);
            cam.AddComponent<MouseLook>();
            flycam = cam.GetComponent<MouseLook>();
            flycam.enabled = true;
            flycam.transform.position = new Vector3(-500f, 50f, 0f);
            flycam.transform.rotation = Quaternion.LookRotation(Vector3.right);
            orbitcam = cam.GetComponent<MouseOrbit>();
            orbitcam.OperateRegardlessOfUiOptions = false;
            orbitcam.distance = 100f;
            orbitcam.enabled = false;
            cBuild build = cam.AddComponent<cBuild>();
            build.team = ObjectId.NoLinkage;
            //orbittarget = StaticConstructablesManager.constructables[0].UniqueId;
            orbittarget = 0;
            orbitindex = 0;
            orbitMothership = -1;
            extraInfo = false;
        }

        public void MoveCam()
        {
            cam.transform.position = FramePositionOfBoardSection() + new Vector3(0, 50, 0);
        }

        public Vector3 FramePositionOfBoardSection()
        {
            return PlanetList.MainFrame.UniversalPositionToFramePosition(UniversalPositionOfBoardSection());
        }

        public Vector3d UniversalPositionOfBoardSection()
        {
            return StaticCoordTransforms.BoardSectionToUniversalPosition(WorldSpecification.i.BoardLayout.BoardSections[Parameters.EastWestBoard, Parameters.NorthSouthBoard].BoardSectionCoords);
        }

        public void SaveSettingsNew() {
            string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
            FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder, "parameters.json"));
            settingsFile.SaveData(Parameters,Formatting.Indented);
        }

        public void LoadNewSettings() {
            string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
            FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder,"parameters.json"));
            if (settingsFile.Exists)
            {
                Parameters = settingsFile.LoadData<TournamentParameters>();
            }
            else {
                LoadOldSettings();
            }
        }

        public void LoadOldSettings()
        {
            string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
            FilesystemFileSource settingsFile = new FilesystemFileSource(Path.Combine(modFolder,"settings.cfg"));
            if (settingsFile.Exists)
            {
                List<float> settingsList = settingsFile.LoadData<List<float>>();
                Parameters.AltitudeLimits.Us.Set(settingsList[0], settingsList[1]);
                Parameters.DistanceLimit.Us = (int)settingsList[2];
                Parameters.MaximumPenaltyTime.Us = (int)settingsList[3];
                Parameters.MaximumTime.Us = (int)settingsList[4];
                Parameters.ResourcesTeam1.Us = (int)settingsList[5];
                Parameters.ResourcesTeam2.Us = (int)settingsList[5];
                Parameters.StartingDistance.Us = (int)settingsList[6];
                Parameters.SpawngapLR.Us = (int)settingsList[7];
                Parameters.Direction.Us = (int)settingsList[9]*90;
                Parameters.SpawnHeight.Us = (int)settingsList[10];
                Parameters.DefaultKeys.Us = settingsList[11] != 0;
                Parameters.EastWestBoard.Us = (int)settingsList[12];
                Parameters.NorthSouthBoard.Us = (int)settingsList[13];
                Parameters.SpawngapFB.Us = (int)settingsList[14];
                Parameters.LocalResources.Us = settingsList[15] != 0;
                Parameters.MaximumBufferTime.Us = (int)settingsList[16];
                Parameters.AltitudeReverse.Us = (int)settingsList[17];
                if (settingsList.Count >= 31)
                {
                    Parameters.ShowAdvancedOptions.Us = settingsList[18] != 0;
                    Parameters.MaterialConversion.Us = (int)settingsList[19];
                    Parameters.CleanUpMode.Us = (int)settingsList[20];
                    Parameters.HealthCalculation.Us = (int)settingsList[21];
                    Parameters.MinimumHealth.Us = (int)settingsList[22];
                    Parameters.Rotation.Us = (int)settingsList[23];
                    Parameters.SameMaterials.Us = settingsList[24] != 0;
                    Parameters.InfinteResourcesTeam1.Us = settingsList[25] != 0;
                    Parameters.InfinteResourcesTeam2.Us = settingsList[26] != 0;
                    Parameters.ProjectedDistance.Us = settingsList[27] != 0;
                    Parameters.SoftLimits.Us = settingsList[28] != 0;
                    Parameters.AltitudeReverse.Us = (int)settingsList[29];
                    Parameters.Overtime.Us = (int)settingsList[30];
                }
                else {
                    Parameters.ShowAdvancedOptions.Reset();
                    Parameters.MaterialConversion.Reset();
                    Parameters.CleanUpMode.Reset();
                    Parameters.HealthCalculation.Reset();
                    Parameters.MinimumHealth.Reset();
                    Parameters.Rotation.Reset();
                    Parameters.SameMaterials.Reset();
                    Parameters.InfinteResourcesTeam1.Reset();
                    Parameters.InfinteResourcesTeam2.Reset();
                    Parameters.ProjectedDistance.Reset();
                    Parameters.SoftLimits.Reset();
                    Parameters.AltitudeReverse.Reset();
                    Parameters.Overtime.Reset();
                    Parameters.ActiveFactions.Reset();
                    Parameters.InfinteResourcesTeam3.Reset();
                    Parameters.ResourcesTeam3.Reset();
                }
            }
            else
            {
                LoadDefaults();
            }
        }


        public void LoadDefaults()
        {
            Parameters.ResetToDefault();
        }

        public void OnGUI()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f * Screen.width / 1280f, 1f * Screen.height / 800f, 1f));
            GUI.backgroundColor = new Color(0f, 0f, 0f, 0.6f);
            GUI.Label(new Rect(590f, 0f, 100f, 30f), $"{Math.Floor(timerTotal / 60f)}m {Math.Floor(timerTotal) % 60.0}s", timerStyle);
            if (showLists)
            {
                GUILayout.BeginArea(new Rect(0, 50, 200, 700),sidelist);
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                float t = Time.realtimeSinceStartup * 30;
                foreach (KeyValuePair<ObjectId, SortedDictionary<string,TournamentParticipant>> team in HUDLog) {
                    string teamMaterials = "M: "+team.Key.FactionInst().ResourceStore.Material.ToString();
                    float teamMaxHP = 0, teamCurHP=0;
                    teamMaxHP = team.Value.Values.Aggregate(0f, (currentSum, member) => currentSum + member.HPMAX);
                    teamCurHP = team.Value.Values.Aggregate(0f, (currentSum, member) => currentSum + member.HPCUR);
                    string teamHP = $"{Math.Round(100*teamCurHP/teamMaxHP,1)}%";
                    GUILayout.Label($"<color=cyan>{team.Key.FactionSpec().Name} @ {teamHP}, {teamMaterials}</color>", sidelist);
                    foreach (KeyValuePair <string,TournamentParticipant> member in team.Value) {
                        string name = member.Value.BlueprintName;
                        string percentHP = $"{Math.Round(member.Value.HP, 1)}%";
                        string penaltyTime = $"{Math.Floor(member.Value.OoBTime / 60)}m{Math.Floor(member.Value.OoBTime) % 60}s";
                        bool disqualified = member.Value.Disqual || member.Value.Scrapping;
                        GUIContent memberContent;
                        if (disqualified) {
                            memberContent = new GUIContent($"{name} DQ");
                        }
                        else
                        {
                            memberContent = new GUIContent($"{name} @ {percentHP}, {penaltyTime}");
                        }
                        Vector2 size= sidelist.CalcSize(memberContent);
                        if (size.x <= 150)
                        {
                            GUILayout.Label(memberContent,sidelist);
                        }
                        else {
                            GUILayout.BeginScrollView(new Vector2(t % (size.x + 50), 0), false, false, GUIStyle.none, GUIStyle.none, GUILayout.ExpandWidth(false));
                            GUILayout.Label(memberContent,sidelist);
                            GUILayout.EndScrollView();
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            // extra info panel
            if (extraInfo)
            {
                //int target = getTarget();
                IMainConstructBlock target = GetTarget();
                if (target != null)
                {
                    //int targetIndex = StaticConstructablesManager.constructables.FindIndex(0, m => m.UniqueId == target);
                    MainConstruct targetConstruct = StaticConstructablesManager.constructables.Where(x => x.iMain == target).First();
                    //MainConstruct targetConstruct = StaticConstructablesManager.constructables[targetIndex];

                    string name = targetConstruct.blueprintName;
                    string team = targetConstruct.GetTeam().FactionSpec().Name;
                    string hp = $"{Math.Round(targetConstruct.AllBasics.GetFractionAliveBlocksIncludingSubConstructables() * 100f, 1).ToString()}%";
                    string resources = $"{Math.Round(targetConstruct.RawResource.Material.Quantity, 0)}/{Math.Round(targetConstruct.RawResource.Material.Maximum, 0)}";
                    string ammo = $"{Math.Round(targetConstruct.Ammo.Ammo.Quantity, 0)}/{Math.Round(targetConstruct.Ammo.Ammo.Maximum, 0)}";
                    string fuel = $"{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Fuel.Quantity, 0)}/{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Fuel.Maximum, 0)}";
                    string battery = $"{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Energy.Quantity, 0)}/{Math.Round(targetConstruct.PowerUsageCreationAndFuel.Energy.Maximum, 0)}";
                    //power as current output / max
                    //string power = $"{Math.Round(StaticConstructablesManager.constructables[targetIndex].PowerUsageCreationAndFuel.MaxPower - StaticConstructablesManager.constructables[targetIndex].PowerUsageCreationAndFuel.Power, 0)}" +
                    //    $"/{Math.Round(StaticConstructablesManager.constructables[targetIndex].PowerUsageCreationAndFuel.MaxPower, 0)}";
                    // power as available /max#
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
                    string nearest = Math.Round(closest, 0).ToString()+"m";

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

        public IMainConstructBlock GetTarget()
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

        public void UpdateBoardSectionPreview(ITimeStep dt)
        {
            cam.transform.Rotate(0, (float)(15 * dt.PhysicalDeltaTime.Seconds), 0);//15° rotation per second
        }

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
            bool changeShowLists = false;

            switch (Parameters.DefaultKeys.Us)
            {
                case false:
                    pause = ftdKeyMap.IsKey(KeyInputsFtd.PauseGame, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
                    shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    strg = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    next = ftdKeyMap.IsKey(KeyInputsFtd.InventoryUi, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
                    previous = ftdKeyMap.IsKey(KeyInputsFtd.Interact, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
                    changeExtraInfo = ftdKeyMap.IsKey(KeyInputsFtd.CharacterSheetUi, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
                    changeShowLists = ftdKeyMap.IsKey(KeyInputsFtd.EnemySpawnUi, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
                    freecamOn = Input.GetMouseButtonDown(1); // technically same as default atm
                    orbitcamOn = Input.GetMouseButtonDown(0); // technically same as default atm
                    break;
                case true:
                    pause = Input.GetKeyDown(KeyCode.F11); // default f11
                    shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                    strg = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    next = Input.GetKeyDown(KeyCode.E); // default e
                    previous = Input.GetKeyDown(KeyCode.Q); // default q
                    changeExtraInfo = Input.GetKeyDown(KeyCode.Z); // default z
                    changeShowLists = Input.GetKeyDown(KeyCode.X); // default x
                    freecamOn = Input.GetMouseButtonDown(1); // default left click
                    orbitcamOn = Input.GetMouseButtonDown(0); // default right click
                    break;
            }
            if (shift)
            {
                orbitcam.xSpeed = 1000;
                orbitcam.ySpeed = 480;
            }
            else if (strg) {
                orbitcam.xSpeed = 63;
                orbitcam.ySpeed = 30;
            } else {
                orbitcam.xSpeed = 250;
                orbitcam.ySpeed = 120;
            }
            if (changeExtraInfo)
            {
                extraInfo = !extraInfo;
            }
            if (changeShowLists) {
                showLists = !showLists;
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                if (shift)
                {
                    orbitcam.distance = (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") > 0f) ? (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") * 100f) : 0f;
                }
                else if (strg) {
                    orbitcam.distance = (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") > 0f) ? (orbitcam.distance - Input.GetAxis("Mouse ScrollWheel") * 25f) : 0f;
                } else {
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
                        //orbittarget = StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId;
                    }
                    else
                    {
                        orbitindex++;
                        //orbittarget = StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId;
                    }
                }
                if (previous)
                {
                    if (orbitindex == 0)
                    {
                        orbitindex = StaticConstructablesManager.constructables.Count - 1;
                        //orbittarget = StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId;
                    }
                    else
                    {
                        orbitindex--;
                        //orbittarget = StaticConstructablesManager.constructables.ToArray()[orbitindex].UniqueId;
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
                //Vector3 val = new Vector3(Input.GetAxisRaw("Sidestep"), Input.GetKey((KeyCode)32) ? 1f : ((Input.GetKey((KeyCode)308) | Input.GetKey((KeyCode)307)) ? (-1f) : 0f), Input.GetAxisRaw("ForwardsBackwards"));
                float x = 0;
                float y = 0;
                float z = 0;
                if (Input.GetKey((KeyCode)32)) //space  = up
                {
                    y += 1;
                }
                if (Input.GetKey((KeyCode)308) | Input.GetKey((KeyCode)307)) // alt = down
                {
                    y -= 1;
                }
                if (Input.GetKey((KeyCode)97)) // a = left
                {
                    x -= 1;
                }
                if (Input.GetKey((KeyCode)100)) // d= right
                {
                    x += 1;
                }
                if (Input.GetKey((KeyCode)119)) // w = forward
                {
                    z += 1;
                }
                if (Input.GetKey((KeyCode)115)) // s = back
                {
                    z -= 1;
                }
                Vector3 val = new Vector3(x, y, z);
                if (shift)
                {
                    val *= 5; //increase vector with shift
                }
                else if (strg) {
                    val /= 5; //decrease vector with strg
                }
                flycam.transform.position = flycam.transform.position + flycam.transform.localRotation * val;
            }
            if (flycam.enabled && !Parameters.DefaultKeys)
            {
                Vector3 movement = ftdKeyMap.GetMovemementDirection() * (shift ? 5 : 1);
                movement /= strg ? 5 : 1;
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

        public void FixedUpdate(ITimeStep dt)
        {
            if (!GameSpeedManager.Instance.IsPaused)
            {
                if (Parameters.MaterialConversion == -1f)
                {
                    if (Parameters.ResourcesTeam1 < entries_t1[0].Team_id.FactionInst().ResourceStore.Material.Quantity)
                    {
                        entries_t1[0].Team_id.FactionInst().ResourceStore.SetResources(Parameters.ResourcesTeam1);
                    }
                    else
                    {
                        Parameters.ResourcesTeam1.Us = (int)entries_t1[0].Team_id.FactionInst().ResourceStore.Material.Quantity;
                    }
                    if (Parameters.ResourcesTeam2 < entries_t2[0].Team_id.FactionInst().ResourceStore.Material.Quantity)
                    {
                        entries_t2[0].Team_id.FactionInst().ResourceStore.SetResources(Parameters.ResourcesTeam2);
                    }
                    else
                    {
                        Parameters.ResourcesTeam2.Us = (int)entries_t2[0].Team_id.FactionInst().ResourceStore.Material.Quantity;
                    }
                }
                MainConstruct[] array = StaticConstructablesManager.constructables.ToArray();
                foreach (MainConstruct val in array)
                {
                    //Debug.Log("FixedUpdate ID: " + val.UniqueId);
                    int id = 0;
                    if (val.Drones.LoadedMothershipC != null)
                    {
                        id = val.Drones.LoadedMothershipC.UniqueId;
                    }
                    string key = "" + val.UniqueId + "," + id;
                    //Debug.Log("FixedUpdate mothership ID: " + val.Drone.loadedMothershipC.UniqueId);
                    TournamentParticipant tournamentParticipant = HUDLog[val.GetTeam()][key];
                    if (!tournamentParticipant.Disqual || !tournamentParticipant.Scrapping)
                    {
                        tournamentParticipant.AICount = val.BlockTypeStorage.MainframeStore.Blocks.Count;
                        if (tournamentParticipant.AICount == 0)
                        {
                            tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                        }
                        else if (val.CentreOfMass.y < Parameters.AltitudeLimits.Lower || val.CentreOfMass.y > Parameters.AltitudeLimits.Upper)
                        {
                            if (Parameters.SoftLimits) {
                                if (val.CentreOfMass.y < Parameters.AltitudeLimits.Lower && -val.Velocity.y > Parameters.AltitudeReverse) //Below minimum altitude and still sinking.
                                {
                                    tournamentParticipant.OoBTimeBuffer += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                    if (tournamentParticipant.OoBTimeBuffer > Parameters.MaximumBufferTime)
                                    {
                                        tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                    }
                                }
                                else if (val.CentreOfMass.y > Parameters.AltitudeLimits.Upper && val.Velocity.y > Parameters.AltitudeReverse) //Above maximum altitude and still rising.
                                {
                                    tournamentParticipant.OoBTimeBuffer += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                    if (tournamentParticipant.OoBTimeBuffer > Parameters.MaximumBufferTime)
                                    {
                                        tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                    }
                                }
                            }
                            else
                            {
                                tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                            }
                        }
                        else if (tournamentParticipant.HP < Parameters.MinimumHealth)
                        {
                            tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                        }
                        else
                        {
                            float num = -1f;
                            float num2 = -1f;
                            MainConstruct[] array2 = StaticConstructablesManager.constructables.ToArray();
                            foreach (MainConstruct val2 in array2)
                            {
                                if (val != val2 && val.GetTeam() != val2.GetTeam())
                                {
                                    float num3 = Parameters.ProjectedDistance ? DistanceProjected(val.CentreOfMass, val2.CentreOfMass) : Vector3.Distance(val.CentreOfMass, val2.CentreOfMass);
                                    if (num < 0f)
                                    {
                                        num = num3;
                                        num2 = Parameters.ProjectedDistance ? DistanceProjected(val.CentreOfMass + val.Velocity, val2.CentreOfMass) : Vector3.Distance(val.CentreOfMass + val.Velocity, val2.CentreOfMass);

                                    }
                                    else if (num3 < num)
                                    {
                                        num = num3;
                                        num2 = Parameters.ProjectedDistance ? DistanceProjected(val.CentreOfMass + val.Velocity, val2.CentreOfMass) : Vector3.Distance(val.CentreOfMass + val.Velocity, val2.CentreOfMass);
                                    }
                                }
                            }
                            if (Parameters.SoftLimits && num > Parameters.DistanceLimit && num < num2 - Parameters.DistanceReverse) //out of bounds and moving away faster than oobReverse m/s
                            {
                                tournamentParticipant.OoBTimeBuffer += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                if (tournamentParticipant.OoBTimeBuffer > Parameters.MaximumBufferTime)
                                {
                                    tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                }
                            }
                            else if (!Parameters.SoftLimits && num > Parameters.DistanceLimit) { //out of bounds
                                tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                            }
                            else
                            {
                                tournamentParticipant.OoBTimeBuffer = 0;
                            }
                        }
                        tournamentParticipant.Disqual = tournamentParticipant.OoBTime > Parameters.MaximumPenaltyTime;
                    }
                }
                timerTotal += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
            }
        }

        public void SlowUpdate(ITimeStep dt)
        {

            MainConstruct[] array = StaticConstructablesManager.constructables.ToArray();
            foreach (MainConstruct val in array)
            {
                int id = 0;
                if (val.Drones.LoadedMothershipC != null)
                {
                    id = val.Drones.LoadedMothershipC.UniqueId;
                }
                string key = val.UniqueId + "," + id;
                if (!HUDLog[val.GetTeam()].TryGetValue(key, out TournamentParticipant tournamentParticipant))
                {
                    tournamentParticipant = new TournamentParticipant
                    {
                        AICount = val.BlockTypeStorage.MainframeStore.Count,
                        TeamId = val.GetTeam(),
                        TeamName = val.GetTeam().FactionSpec().Name,
                        BlueprintName = val.GetBlueprintName(),
                        MothershipId = id,
                        Disqual = false,
                        Scrapping = false,
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
                                        if (b!=null) {
                                            if (Parameters.HealthCalculation == 2)
                                            {
                                                tournamentParticipant.HPMAX += b.item.SizeInfo.VolumeFactor;
                                            }
                                            else {
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
                    HUDLog[tournamentParticipant.TeamId][key] = tournamentParticipant;
                }
                if (!tournamentParticipant.Disqual || !tournamentParticipant.Scrapping)
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
            foreach (KeyValuePair<ObjectId, SortedDictionary<string, TournamentParticipant>> item in HUDLog)
            {
                foreach (KeyValuePair<string, TournamentParticipant> item2 in HUDLog[item.Key])
                {
                    List<MainConstruct> constructables = StaticConstructablesManager.constructables;
                    List<MainConstruct> constructables2 = StaticConstructablesManager.constructables;
                    bool match(MainConstruct c)
                    {
                        ObjectId team3 = c.GetTeam();
                        KeyValuePair<string, TournamentParticipant> keyValuePair9 = item2;
                        if (team3 == keyValuePair9.Value.TeamId)
                        {
                            int id3 = 0;
                            if (c.Drones.LoadedMothershipC != null)
                            {
                                id3 = c.Drones.LoadedMothershipC.UniqueId;
                            }
                            string uniqueId3 = "" + c.UniqueId + "," + id3;
                            KeyValuePair<string, TournamentParticipant> keyValuePair10 = item2;
                            return uniqueId3 == keyValuePair10.Key;
                        }
                        return false;
                    }
                    if (constructables.Contains(constructables2.Find(match)))
                    {
                        SortedDictionary<string, TournamentParticipant> sortedDictionary = HUDLog[item.Key];
                        KeyValuePair<string, TournamentParticipant> keyValuePair = item2;
                        if (!sortedDictionary[keyValuePair.Key].Disqual)
                        {
                            continue;
                        }
                    }
                    SortedDictionary<string, TournamentParticipant> sortedDictionary2 = HUDLog[item.Key];
                    KeyValuePair<string, TournamentParticipant> keyValuePair2 = item2;
                    if (!sortedDictionary2[keyValuePair2.Key].Scrapping)
                    {
                        SortedDictionary<string, TournamentParticipant> sortedDictionary3 = HUDLog[item.Key];
                        KeyValuePair<string, TournamentParticipant> keyValuePair3 = item2;
                        sortedDictionary3[keyValuePair3.Key].HPCUR = 0f;
                        SortedDictionary<string, TournamentParticipant> sortedDictionary4 = HUDLog[item.Key];
                        KeyValuePair<string, TournamentParticipant> keyValuePair4 = item2;
                        sortedDictionary4[keyValuePair4.Key].Scrapping = true;
                        Vector3 centreOfMass = StaticConstructablesManager.constructables.Find(delegate (MainConstruct c)
                        {
                            ObjectId team2 = c.GetTeam();
                            KeyValuePair<string, TournamentParticipant> keyValuePair7 = item2;
                            if (team2 == keyValuePair7.Value.TeamId)
                            {
                                int id2 = 0;
                                if (c.Drones.LoadedMothershipC != null)
                                {
                                    id2 = c.Drones.LoadedMothershipC.UniqueId;
                                }
                                string uniqueId2 = "" + c.UniqueId + "," + id2;
                                KeyValuePair<string, TournamentParticipant> keyValuePair8 = item2;
                                return uniqueId2 == keyValuePair8.Key;
                            }
                            return false;
                        }).CentreOfMass;
                        UnityEngine.Object.Instantiate(Resources.Load("Detonator-MushroomCloud") as GameObject, centreOfMass, Quaternion.identity);
                        StaticConstructablesManager.constructables.Find(delegate (MainConstruct c)
                        {
                            ObjectId team = c.GetTeam();
                            KeyValuePair<string, TournamentParticipant> keyValuePair5 = item2;
                            if (team == keyValuePair5.Value.TeamId)
                            {
                                int id1 = 0;
                                if (c.Drones.LoadedMothershipC != null)
                                {
                                    id1 = c.Drones.LoadedMothershipC.UniqueId;
                                }
                                string uniqueId1 = "" + c.UniqueId + "," + id1;
                                KeyValuePair<string, TournamentParticipant> keyValuePair6 = item2;
                                return uniqueId1 == keyValuePair6.Key;
                            }
                            return false;
                        }).DestroyCompletely(true);
                    }
                }
            }
            if (overtimeCounter == 0&& timerTotal > Parameters.MaximumTime)
            {
                GameSpeedManager.Instance.TogglePause();
                overtimeCounter = 1;
            }
            else if (Parameters.Overtime > 0) {//Verlängerung ist eingeschaltet.
                if (timerTotal > Parameters.MaximumTime + overtimeCounter * Parameters.Overtime) {
                    GameSpeedManager.Instance.TogglePause();
                    overtimeCounter++;
                }
            }
        }
        public Quaternion Rotation => Quaternion.Euler(0, Parameters.Rotation, 0);
        public float DistanceProjected(Vector3 a, Vector3 b) {
            a.y = 0;
            b.y = 0;
            return Vector3.Distance(a, b);
        }
        public TournamentFormation GetFormation(int index) {
            switch (index) {
                case 0:
                    return formationTeam1;
                case 1:
                    return formationTeam2;
                case 2:
                    return formationTeam3;
                default:
                    Debug.LogError("Invalid index!");
                    throw new ArgumentOutOfRangeException("index", index, "Only valid values are 0, 1 and 2!");
            }
        }
    }
}
