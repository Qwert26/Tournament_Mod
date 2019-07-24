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
using BrilliantSkies.Ftd.Planets.Factions;
using BrilliantSkies.Ftd.Planets.Instances;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using BrilliantSkies.Ftd.Planets.World;
using BrilliantSkies.GridCasts;
using BrilliantSkies.PlayerProfiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tournament
{
	public class Tournament : BrilliantSkies.FromTheDepths.Game.UserInterfaces.InteractiveOverlay.InteractiveOverlay
    {
        public static class SPAWN
        {
            public enum DIR
            {
                Facing,
                Away,
                Left,
                Right
            }

            public enum LOC
            {
                Air,
                Sea,
                Sub,
                Land
            }
        }

        public static Tournament _me;

        public TournamentGUI _GUI;

        public Font _Font;

        public GUIStyle _Top;

        public GUIStyle _Left;

        public GUIStyle _Left2;

        public GUIStyle _Right;

        public GUIStyle _Right2;

        public GUIStyle _RightWrap;

        public bool started;

        public GameObject cam;

        public MouseLook flycam;

        public MouseOrbit orbitcam;

        public Vector3 lastpos;

        public int orbittarget;

        public int orbitMothership;

        public int orbitindex;

        public bool cammode;

        public bool extraInfo;

        private float timerTotal;

        private float timerTotal2;

        private byte overtimeCounter;

        public float minalt;

        public float maxalt;

        public float maxdis;

        public float maxoob;

        public float maxtime;

        public float maxmat;

        public float matconv = -1f;

        public float spawndis;

        public float spawngap;

        public float spawngap2;

        public float offset;

        public SPAWN.DIR Dir;

        public SPAWN.LOC Loc;

        public int northSouthBoard;

        public int eastWestBoard;

        public int defaultKeys;

        public bool defaultKeysBool;

        public float oobReverse; // out of bounds and maoving away speed limit before dq time

        public float oobMaxBuffer; //out of bounds and moving away too fast buffer time in secs

        //Defaults
        public float minaltD = -50f;

        public float maxaltD = 500f;

        public float maxdisD = 1500f;

        public float maxoobD = 120f;

        public float maxtimeD = 900f;

        public float maxmatD = 10000f;

        public float matconvD = -1f;

        public float spawndisD = 1000f;

        public float spawngapD = 100f;

        public float spawngap2D = 0f;

        public float offsetD = 0f;

        public float timer;

        public SPAWN.DIR DirD = SPAWN.DIR.Facing;

        public SPAWN.LOC LocD = SPAWN.LOC.Sea;

        public int northSouthBoardD = 0;

        public int eastWestBoardD = 0;

        public int defaultKeysD = 1;

        public float oobReverseD = 0; // out of bounds and moving away speed limit before dq time. 0 will add dq time if move away at all, positve increases away speed limit, negative you need to move towards at at least this speed or pick up dq

        public float oobMaxBufferD = 3; //out of bounds and moving away too fast buffer time in secs

        //Management

        private SortedDictionary<int, SortedDictionary<string, TournamentParticipant>> HUDLog = new SortedDictionary<int, SortedDictionary<string, TournamentParticipant>>();

        public float t1_res;

        public float t2_res;

        public List<TournamentEntry> entries_t1 = new List<TournamentEntry>();

        public List<TournamentEntry> entries_t2 = new List<TournamentEntry>();

        public ConstructableCleanUp cleanUp = ConstructableCleanUp.All;

        public ConstructableCleanUp cleanUpD = ConstructableCleanUp.All;

        public float minimumHealth = 0;

        public float minimumHealthD = 0;

        public float rotation = 0;

        public float rotationD = 0;

        public bool showAdvancedOptions = false;

        public bool showAdvancedOptionsD = false;

        public bool sameMaterials = true;

        public bool sameMaterialsD = true;

        public bool localResources = false;

        public bool localResourcesD = false;

        public bool infinteResourcesT1 = false;

        public bool infinteResourcesT2 = false;

        public bool infiniteResourcesD = false;

        public bool project2D = false;

        public bool project2DD = false;

        public bool showLists = true;

		public TournamentFormation kingFormation, challengerFormation;

        public bool softLimits = true;

        public bool softLimitsD = true;

        public float altitudeReverse = 3;

        public float altitudeReverseD = 3;

        public float overtime = 0;

        public float overtimeD = 0;

        public TournamentParameters Parameters { get; set; } = new TournamentParameters(1u);

        public enum HealthCalculation
        {
            NumberOfBlocks,
            ResourceCost,
            Volume,
            ArrayElements
        }
        public HealthCalculation healthCalculation = HealthCalculation.NumberOfBlocks;

        public HealthCalculation healthCalculationD = HealthCalculation.NumberOfBlocks;

        public Tournament()
        {
            _me = this;
            _GUI = new TournamentGUI(_me);

            _Top = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true,
                fontSize = 12
            };

            _Left = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
            {
                alignment = TextAnchor.UpperLeft,
                richText = true,
                fontSize = 12,
                wordWrap = false,
                clipping = TextClipping.Clip
            };

            _Left2 = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft,
                richText = true,
                fontSize = 12
            };
            _Left2.normal.textColor = Color.white;

            _Right2 = new GUIStyle
            {
                alignment = TextAnchor.UpperRight,
                richText = true,
                fontSize = 12
            };
            _Right2.normal.textColor = Color.white;

            _Right = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
            {
                alignment = TextAnchor.UpperRight,
                richText = true,
                fontSize = 12,
                wordWrap = false,
                clipping = TextClipping.Clip
            };

            _RightWrap = new GUIStyle(LazyLoader.HUD.Get().interactionStyle)
            {
                alignment = TextAnchor.UpperRight,
                richText = true,
                fontSize = 12,
                wordWrap = true,
                clipping = TextClipping.Clip
            };

            LoadOldSettings();
        }

        public void LoadCraft()
        {
            ClearArea();
            HUDLog.Clear();
            InstanceSpecification.i.Header.CommonSettings.EnemyBlockDestroyedResourceDrop = matconv / 100f;
            InstanceSpecification.i.Header.CommonSettings.LocalisedResourceMode = localResources ? LocalisedResourceMode.UseLocalisedStores : LocalisedResourceMode.UseCentralStore;
            foreach (TournamentEntry item in entries_t1)
            {
                item.Spawn(spawndis, spawngap, spawngap2, entries_t1.Count, entries_t1.IndexOf(item));
                if (localResources)
                {
                    item.Team_id.FactionInst().ResourceStore.SetResources(0);
                }
                else if(infinteResourcesT1)
                {
                    item.Team_id.FactionInst().ResourceStore.SetResourcesInfinite();
                } else
                {
                    item.Team_id.FactionInst().ResourceStore.SetResources(t1_res);
                }
            }
            foreach (TournamentEntry item2 in entries_t2)
            {
                item2.Spawn(spawndis, spawngap, spawngap2, entries_t2.Count, entries_t2.IndexOf(item2));
                if (localResources)
                {
                    item2.Team_id.FactionInst().ResourceStore.SetResources(0);
                }
                else if (infinteResourcesT2) {
                    item2.Team_id.FactionInst().ResourceStore.SetResourcesInfinite();
                }
                else
                {
                    item2.Team_id.FactionInst().ResourceStore.SetResources(t2_res);
                }
            }
            if (localResources)
            {
                foreach (MainConstruct construct in StaticConstructablesManager.constructables) {
                    if (construct.GetTeam() == TournamentPlugin.kingFaction.Id)
                    {
                        construct.RawResource.Material.SetQuantity(Math.Max(construct.RawResource.Material.Maximum, t1_res));
                    }
                    else if (construct.GetTeam() == TournamentPlugin.challengerFaction.Id)
                    {
                        construct.RawResource.Material.SetQuantity(Math.Max(construct.RawResource.Material.Maximum, t2_res));
                    }
                    else {
                        construct.RawResource.Material.SetQuantity(0);
                    }
                }
            }
        }

        public void StartMatch()
        {
            overtimeCounter = 0;
            timer = 0f;
            timerTotal = 0f;
            timerTotal2 = Time.timeSinceLevelLoad;
            InstanceSpecification.i.Header.CommonSettings.ConstructableCleanUp = cleanUp;
            if (!GameSpeedManager.Instance.IsPaused) {
                GameSpeedManager.Instance.TogglePause();
            }
            orbitindex = 0;
            orbittarget = 0;

            flycam.transform.position = new Vector3(-500f, 50f, 0f);
            flycam.transform.rotation = Quaternion.LookRotation(Vector3.right);
            cammode = false;
            foreach (MainConstruct constructable in StaticConstructablesManager.constructables)
            {
                int id = 0;
                if (constructable.Drones.LoadedMothershipC != null)
                {
                    id = constructable.Drones.LoadedMothershipC.UniqueId;
                }
                string key = "" + constructable.UniqueId + "," + id;

                if (!HUDLog.ContainsKey(constructable.GetTeam().Id))
                {
                    HUDLog.Add(constructable.GetTeam().Id, new SortedDictionary<string, TournamentParticipant>());
                }

                if (!HUDLog[constructable.GetTeam().Id].ContainsKey(key))
                {
                    bool spawnStick = constructable.BlockTypeStorage.MainframeStore.Count == 0;
                    switch (healthCalculation)
                    {
                        case HealthCalculation.NumberOfBlocks:
                            
                            HUDLog[constructable.GetTeam().Id].Add(key, new TournamentParticipant
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
                        case HealthCalculation.ResourceCost:
                            HUDLog[constructable.GetTeam().Id].Add(key, new TournamentParticipant
                            {
                                TeamId = constructable.GetTeam(),
                                TeamName = constructable.GetTeam().FactionSpec().AbreviatedName,
                                UniqueId = constructable.UniqueId,
                                MothershipId = id,
                                BlueprintName = constructable.GetName(),
                                AICount = constructable.BlockTypeStorage.MainframeStore.Blocks.Count,
                                HP = 100,
                                HPCUR = spawnStick ? constructable.AllBasics.GetResourceCost().Material : constructable.AllBasics.GetResourceCostAllNotIncludingSubVehicles().Material,
                                HPMAX = spawnStick ? constructable.AllBasics.GetResourceCost().Material : constructable.AllBasics.GetResourceCostAllNotIncludingSubVehicles().Material
                            });
                            break;
                        case HealthCalculation.Volume:
                            HUDLog[constructable.GetTeam().Id].Add(key, new TournamentParticipant
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
                        case HealthCalculation.ArrayElements:
                            HUDLog[constructable.GetTeam().Id].Add(key, new TournamentParticipant
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
            started = true;
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
            cammode = false;
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
            return StaticCoordTransforms.BoardSectionToUniversalPosition(WorldSpecification.i.BoardLayout.BoardSections[eastWestBoard, northSouthBoard].BoardSectionCoords);
        }

        public void SaveSettingsOld()
        {
            string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
            FilesystemFileSource settingsFile = new FilesystemFileSource(modFolder + "settings.cfg");
            List<float> settingsList = new List<float>
            {
                minalt,
                maxalt,
                maxdis,
                maxoob,
                maxtime,
                maxmat,
                spawndis,
                spawngap,
                offset,
                (float)Dir,
                (float)Loc,
                defaultKeysBool?1:0,
                eastWestBoard,
                northSouthBoard,
                spawngap2,
                localResources ? 1:0,
                oobMaxBuffer,
                oobReverse,
                showAdvancedOptions ? 1 : 0,
                matconv,
                (float)cleanUp,
                (float)healthCalculation,
                minimumHealth,
                rotation,
                sameMaterials ? 1 : 0,
                infinteResourcesT1?1:0,
                infinteResourcesT2?1:0,
                project2D?1:0,
                softLimits?1:0,
                altitudeReverse,
                overtime
            };
            settingsFile.SaveData(settingsList, Formatting.None);
        }

        public void SaveSettingsNew() {
            string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
            FilesystemFileSource settingsFile = new FilesystemFileSource(modFolder + "parameters.json");
            settingsFile.SaveJson(Parameters);
        }

        public void LoadNewSettings() {
            string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
            FilesystemFileSource settingsFile = new FilesystemFileSource(modFolder + "parameters.json");
            if (settingsFile.Exists)
            {
                Parameters = settingsFile.LoadJson<TournamentParameters>();
            }
        }

        public void LoadOldSettings()
        {
            string modFolder = Get.PerminentPaths.GetSpecificModDir("Tournament").ToString();
            FilesystemFileSource settingsFile = new FilesystemFileSource(modFolder + "settings.cfg");
            if (settingsFile.Exists)
            {
                List<float> settingsList = settingsFile.LoadData<List<float>>();

                minalt = settingsList[0];
                maxalt = settingsList[1];
                Parameters.AltitudeLimits.Us.Set(minalt, maxalt);

                maxdis = settingsList[2];
                Parameters.DistanceLimit.Us = (int)maxdis;

                maxoob = settingsList[3];
                Parameters.MaximumPenaltyTime.Us = (int)maxoob;

                maxtime = settingsList[4];
                Parameters.MaximumTime.Us = (int)maxtime;

                maxmat = settingsList[5];
                Parameters.ResourcesTeam1.Us = (int)maxmat;
                Parameters.ResourcesTeam2.Us = (int)maxmat;

                spawndis = settingsList[6];
                Parameters.StartingDistance.Us = (int)spawndis;

                spawngap = settingsList[7];
                Parameters.SpawngapLR.Us = (int)spawngap;

                offset = settingsList[8];
                Parameters.Offset.Us = (int)offset;

                Dir = (SPAWN.DIR)settingsList[9];
                Parameters.Direction.Us = (int)settingsList[0];

                Loc = (SPAWN.LOC)settingsList[10];
                Parameters.Location.Us = (int)settingsList[10];

                defaultKeys = (int)settingsList[11];
                eastWestBoard = (int)settingsList[12];
                Parameters.EastWestBoard.Us = eastWestBoard;

                northSouthBoard = (int)settingsList[13];
                Parameters.NorthSouthBoard.Us = northSouthBoard;

                spawngap2 = settingsList[14];
                Parameters.SpawngapFB.Us = (int)spawngap2;

                localResources = settingsList[15] != 0;
                Parameters.LocalResources.Us = localResources;

                oobMaxBuffer = settingsList[16];
                Parameters.MaximumBufferTime.Us = (int)oobMaxBuffer;

                oobReverse = settingsList[17];
                Parameters.AltitudeReverse.Us = (int)oobReverse;
                if (settingsList.Count >= 31)
                {
                    showAdvancedOptions = settingsList[18] != 0;
                    Parameters.ShowAdvancedOptions.Us = showAdvancedOptions;

                    matconv = settingsList[19];
                    Parameters.MaterialConversion.Us = (int)matconv;

                    cleanUp = (ConstructableCleanUp)settingsList[20];
                    Parameters.CleanUpMode.Us = (int)settingsList[20];

                    healthCalculation = (HealthCalculation)settingsList[21];
                    Parameters.HealthCalculation.Us = (int)settingsList[21];

                    minimumHealth = settingsList[22];
                    Parameters.MinimumHealth.Us = (int)minimumHealth;

                    rotation = settingsList[23];
                    Parameters.Rotation.Us = (int)rotation;

                    sameMaterials = settingsList[24] != 0;
                    Parameters.SameMaterials.Us = sameMaterials;

                    infinteResourcesT1 = settingsList[25] != 0;
                    Parameters.InfinteResourcesTeam1.Us = infinteResourcesT1;

                    infinteResourcesT2 = settingsList[26] != 0;
                    Parameters.InfinteResourcesTeam2.Us = infinteResourcesT2;

                    project2D = settingsList[27] != 0;
                    Parameters.ProjectedDistance.Us = project2D;

                    softLimits = settingsList[28] != 0;
                    Parameters.SoftLimits.Us = softLimits;

                    altitudeReverse = settingsList[29];
                    Parameters.AltitudeReverse.Us = (int)altitudeReverse;

                    overtime = settingsList[30];
                    Parameters.Overtime.Us = (int)overtime;
                }
                else {
                    showAdvancedOptions = showAdvancedOptionsD;
                    matconv = matconvD;
                    cleanUp = cleanUpD;
                    healthCalculation = healthCalculationD;
                    minimumHealth = minimumHealthD;
                    rotation = rotationD;
                    sameMaterials = sameMaterialsD;
                    infinteResourcesT1 = infinteResourcesT2 = infiniteResourcesD;
                    project2D = project2DD;
                    softLimits = softLimitsD;
                    altitudeReverse = altitudeReverseD;
                    overtime = overtimeD;
                }

                if (defaultKeys == 1)
                {
                    defaultKeysBool = true;
                }
                else
                {
                    defaultKeysBool = false;
                }
                Parameters.DefaultKeys.Us = defaultKeysBool;
            }
            else
            {
                LoadDefaults();
            }
        }


        public void LoadDefaults()
        {
            spawndis = spawndisD;
            spawngap = spawngapD;
            spawngap2 = spawngap2D;
            minalt = minaltD;
            maxalt = maxaltD;
            maxdis = maxdisD;
            maxoob = maxoobD;
            maxtime = maxtimeD;
            maxmat = maxmatD;
            Dir = DirD;
            Loc = LocD;
            offset = offsetD;
            defaultKeys = defaultKeysD;
            eastWestBoard = eastWestBoardD;
            northSouthBoard = northSouthBoardD;
            oobMaxBuffer = oobMaxBufferD;
            oobReverse = oobReverseD;
            if (defaultKeys == 1)
            {
                defaultKeysBool = true;
            }
            else
            {
                defaultKeysBool = false;
            }
            showAdvancedOptions = showAdvancedOptionsD;
            matconv = matconvD;
            cleanUp = cleanUpD;
            healthCalculation = healthCalculationD;
            minimumHealth = minimumHealthD;
            rotation = rotationD;
            sameMaterials = sameMaterialsD;
            localResources = localResourcesD;
            infinteResourcesT1 = infinteResourcesT2 = infiniteResourcesD;
            project2D = project2DD;
            softLimits = softLimitsD;
            altitudeReverse = altitudeReverseD;
            overtime = overtimeD;
            Parameters.ResetToDefault();
        }

        public void OnGUI()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f * Screen.width / 1280f, 1f * Screen.height / 800f, 1f));
            GUI.backgroundColor = new Color(0f, 0f, 0f, 0.6f);
            GUI.Label(new Rect(590f, 0f, 100f, 30f), $"{Math.Floor(timerTotal / 60f)}m {Math.Floor(timerTotal) % 60.0}s", _Top);
            if (showLists)
            {
                foreach (KeyValuePair<int, SortedDictionary<string, TournamentParticipant>> team in HUDLog)
                {
                    bool teamOne = entries_t1[0].Team_id.Id == team.Key;

                    float xOffset = 0f;
                    float oobOffset = 2f;
                    float hpOffset = 42f;
                    float nameOffset = 78f;

                    GUIStyle style = _Left2;
                    if (!teamOne)
                    {
                        style = _Right2;
                        xOffset = 1080f;
                        oobOffset = 160f;
                        hpOffset = 128f;
                        nameOffset = 2f;
                    }

                    float height = 38f + 16f * HUDLog[team.Key].Values.Count;

                    float teamCurrentHP = 0f;
                    float teamMaxHP = 0f;

                    string teamName = $"Team {(teamOne ? 1f : 2f)}";
                    string materials = FactionSpecifications.i.Factions.Where(x => x.Id.Id == team.Key).First().InstanceOfFaction.ResourceStore.Material.ToString() + "M";

                    GUILayout.BeginArea(new Rect(xOffset, 0f, 200f, height), "", _Top);

                    int entries = 0;

                    foreach (KeyValuePair<string, TournamentParticipant> entry in team.Value)
                    {
                        string oob = "";
                        string percentHP = "";
                        string nameBP = $"{entry.Value.BlueprintName}";
                        string dq = "DQ";
                        bool dqed = true;

                        teamMaxHP += entry.Value.HPMAX;

                        if (!entry.Value.Disqual && !entry.Value.Scrapping && entry.Value.AICount != 0)
                        {
                            dqed = false;
                            teamCurrentHP += entry.Value.HPCUR;
                            oob = $"{Math.Floor(entry.Value.OoBTime / 60f)}m{Math.Floor(entry.Value.OoBTime) % 60.0}s";
                            percentHP = $"{Math.Round(entry.Value.HP, 1)}%";
                            //text = ((!flag) ? (text + string.Format("\n{2} {1,4} {0,6}", Math.Floor((double)(item2.Value.OoBTime / 60f)) + "m" + Math.Floor((double)item2.Value.OoBTime) % 60.0 + "s", Math.Round((double)item2.Value.HP, 1) + "%", item2.Value.BlueprintName)) : (text + string.Format("\n{0,-6} {1,-4} {2}", Math.Floor((double)(item2.Value.OoBTime / 60f)) + "m" + Math.Floor((double)item2.Value.OoBTime) % 60.0 + "s", Math.Round((double)item2.Value.HP, 1) + "%", item2.Value.BlueprintName)));
                        }

                        // member name, hp + oob time or DQ
                        if (!dqed)
                        {
                            GUI.Label(new Rect(hpOffset, 38f + entries * 16f, 30f, 16f), percentHP, style);
                            GUI.Label(new Rect(oobOffset, 38f + entries * 16f, 38f, 16f), oob, style);
                        }
                        else
                        {
                            GUI.Label(new Rect(oobOffset, 38f + entries * 16f, 30f, 16f), dq, style);
                        }

                        //GUI.Label(new Rect(nameOffset, 38f + entries*16f, 124f, 16f), nameBP, style);

                        float scrollSpeed = 30;
                        float t = Time.realtimeSinceStartup * scrollSpeed;

                        //float dimensions = _Left2.fontSize * nameBP.Length;
                        var dimensions = _Left2.CalcSize(new GUIContent(nameBP));
                        float width = dimensions.x + 120f;

                        if (dimensions.x <= 120f)
                        {
                            GUI.Label(new Rect(nameOffset, 38f + entries * 16f, 120f, 16f), nameBP, style);
                        }
                        else
                        {
                            GUI.BeginScrollView(new Rect(nameOffset, 38f + entries * 16f, 120f, 16f), new Vector2(t % width, 0), new Rect(-width, 0, 2 * dimensions.x + 120f, 16f), GUIStyle.none, GUIStyle.none);
                            GUI.Label(new Rect(-dimensions.x, 0, dimensions.x, 16f), nameBP, style);
                            GUI.EndScrollView();
                        }

                        entries++;
                    }

                    // team name , hp , mats
                    string teamHP = Math.Round(teamCurrentHP / teamMaxHP * 100, 1) + "%";
                    string topLabel = "";
                    if (teamOne)
                    {
                        topLabel = $"{teamName}  <color=#ffa500ff>{teamHP}</color>  <color=cyan>{materials}</color>";
                    }
                    else
                    {
                        topLabel = $"<color=cyan>{materials}</color>  <color=#ffa500ff>{teamHP}</color>  {teamName}";
                    }
                    GUI.Label(new Rect(4f, 6f, 192f, 32f), topLabel, style);

                    GUILayout.EndArea();
                }
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
                    string nearest = Math.Round(closest, 0).ToString();


                    float xOffsetLabel;
                    float xOffsetValue;

                    int kingId = TournamentPlugin.kingFaction.Id.Id;
                    if (targetConstruct.GetTeam().Id == kingId) // team 1
                    {
                        xOffsetLabel = 200;
                        xOffsetValue = 290;
                    }
                    else
                    {
                        xOffsetLabel = 880;
                        xOffsetValue = 970;
                    }

                    GUI.Label(new Rect(xOffsetLabel, 0f, 90f, 38f), "Name:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 38f, 90f, 38f), "HP:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 76, 90, 38), "Materials:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 114, 90f, 38f), "Ammo:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 152, 90f, 38f), "Fuel:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 190, 90f, 38f), "Battery:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 228, 90f, 38f), "Power:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 266, 90f, 38f), "Speed:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 304, 90f, 38f), "Altitude:", _Left);
                    GUI.Label(new Rect(xOffsetLabel, 342, 90f, 38f), "Nearest Enemy:", _Left);

                    GUI.Label(new Rect(xOffsetValue, 0f, 110f, 38f), name, _RightWrap);
                    GUI.Label(new Rect(xOffsetValue, 38f, 110f, 38f), hp, _Right);
                    GUI.Label(new Rect(xOffsetValue, 76, 110, 38), resources, _Right);
                    GUI.Label(new Rect(xOffsetValue, 114, 110f, 38f), ammo, _Right);
                    GUI.Label(new Rect(xOffsetValue, 152, 110f, 38f), fuel, _Right);
                    GUI.Label(new Rect(xOffsetValue, 190, 110f, 38f), battery, _Right);
                    GUI.Label(new Rect(xOffsetValue, 228, 110f, 38f), power, _Right);
                    GUI.Label(new Rect(xOffsetValue, 266, 110f, 38f), speed, _Right);
                    GUI.Label(new Rect(xOffsetValue, 304, 110f, 38f), altitude, _Right);
                    GUI.Label(new Rect(xOffsetValue, 342, 110f, 38f), nearest, _Right);
                }
            }
        }

        public IMainConstructBlock GetTarget()
        {
            IMainConstructBlock target = null;

            Transform myTransform = flycam.enabled ? flycam.transform : orbitcam.transform;
            GridCastReturn gridCastReturn = GridCasting.GridCastAllConstructables(new GridCastReturn(myTransform.position, myTransform.forward, 500.0f, 10, true));
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
            bool freecamOn = false;
            bool orbitcamOn = false;
            bool changeExtraInfo = false;
            bool changeShowLists = false;

            switch (defaultKeysBool)
            {
                case false:
                    pause = ftdKeyMap.IsKey(KeyInputsFtd.PauseGame, KeyInputEventType.Down, ModifierAllows.AllowUnnecessary);
                    shift = Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift);
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
            else
            {
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
            if (flycam.enabled && defaultKeysBool)
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
                    val = Vector3.Scale(val, new Vector3(5f, 5f, 5f)); // increase vector with shift
                }
                flycam.transform.position = flycam.transform.position + flycam.transform.localRotation * val;
            }
            if (flycam.enabled && !defaultKeysBool)
            {
                Vector3 movement = ftdKeyMap.GetMovemementDirection() * (shift ? 5 : 1);
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
                if (matconv == -1f)
                {
                    if (t1_res < entries_t1[0].Team_id.FactionInst().ResourceStore.Material.Quantity)
                    {
                        entries_t1[0].Team_id.FactionInst().ResourceStore.SetResources(t1_res);
                    }
                    else
                    {
                        t1_res = (float)entries_t1[0].Team_id.FactionInst().ResourceStore.Material.Quantity;
                    }
                    if (t2_res < entries_t2[0].Team_id.FactionInst().ResourceStore.Material.Quantity)
                    {
                        entries_t2[0].Team_id.FactionInst().ResourceStore.SetResources(t2_res);
                    }
                    else
                    {
                        t2_res = (float)entries_t2[0].Team_id.FactionInst().ResourceStore.Material.Quantity;
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
                    TournamentParticipant tournamentParticipant = HUDLog[val.GetTeam().Id][key];
                    if (!tournamentParticipant.Disqual || !tournamentParticipant.Scrapping)
                    {
                        tournamentParticipant.AICount = val.BlockTypeStorage.MainframeStore.Blocks.Count;
                        if (tournamentParticipant.AICount == 0)
                        {
                            tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                        }
                        else if (val.CentreOfMass.y < minalt || val.CentreOfMass.y > maxalt)
                        {
                            if (softLimits) {
                                if (val.CentreOfMass.y < minalt && -val.Velocity.y > altitudeReverse) //Below minimum altitude and still sinking.
                                {
                                    tournamentParticipant.OoBTimeBuffer += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                    if (tournamentParticipant.OoBTimeBuffer > oobMaxBuffer)
                                    {
                                        tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                    }
                                }
                                else if (val.CentreOfMass.y > maxalt && val.Velocity.y > altitudeReverse) //Above maximum altitude and still rising.
                                {
                                    tournamentParticipant.OoBTimeBuffer += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                    if (tournamentParticipant.OoBTimeBuffer > oobMaxBuffer)
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
                        else if (tournamentParticipant.HP < minimumHealth)
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
                                    float num3 = project2D ? DistanceProjected(val.CentreOfMass, val2.CentreOfMass) : Vector3.Distance(val.CentreOfMass, val2.CentreOfMass);
                                    if (num < 0f)
                                    {
                                        num = num3;
                                        num2 = project2D ? DistanceProjected(val.CentreOfMass + val.Velocity, val2.CentreOfMass) : Vector3.Distance(val.CentreOfMass + val.Velocity, val2.CentreOfMass);

                                    }
                                    else if (num3 < num)
                                    {
                                        num = num3;
                                        num2 = project2D ? DistanceProjected(val.CentreOfMass + val.Velocity, val2.CentreOfMass) : Vector3.Distance(val.CentreOfMass + val.Velocity, val2.CentreOfMass);
                                    }
                                }
                            }
                            if (softLimits && num > maxdis && num < num2 - oobReverse) //out of bounds and moving away faster than oobReverse m/s
                            {
                                tournamentParticipant.OoBTimeBuffer += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                if (tournamentParticipant.OoBTimeBuffer > oobMaxBuffer)
                                {
                                    tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                                }
                            }
                            else if (!softLimits && num > maxdis) { //out of bounds
                                tournamentParticipant.OoBTime += Time.timeSinceLevelLoad - timerTotal - timerTotal2;
                            }
                            else
                            {
                                tournamentParticipant.OoBTimeBuffer = 0;
                            }
                        }
                        tournamentParticipant.Disqual = tournamentParticipant.OoBTime > maxoob;
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
                if (!HUDLog[val.GetTeam().Id].TryGetValue(key, out TournamentParticipant tournamentParticipant))
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
                    switch (healthCalculation)
                    {
                        case HealthCalculation.NumberOfBlocks:
                            tournamentParticipant.HPMAX = val.AllBasics.GetNumberBlocksIncludingSubConstructables();
                            break;
                        case HealthCalculation.ResourceCost:
                            tournamentParticipant.HPMAX = val.AllBasics.GetResourceCost().Material;
                            break;
                        case HealthCalculation.Volume:
                        case HealthCalculation.ArrayElements:
                            tournamentParticipant.HPMAX = 0;
                            for (int x = val.AllBasics.minx_; x <= val.AllBasics.maxx_; x++)
                            {
                                for (int y = val.AllBasics.miny_; x <= val.AllBasics.maxy_; y++)
                                {
                                    for (int z = val.AllBasics.minz_; x <= val.AllBasics.maxz_; z++)
                                    {
                                        Block b = val.AllBasics[x, y, z];
                                        if (b!=null) {
                                            if (healthCalculation == HealthCalculation.Volume)
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
                    HUDLog[tournamentParticipant.TeamId.Id][key] = tournamentParticipant;
                }
                if (!tournamentParticipant.Disqual || !tournamentParticipant.Scrapping)
                {
                    switch (healthCalculation)
                    {
                        case HealthCalculation.NumberOfBlocks:
                            tournamentParticipant.HPCUR = val.AllBasics.GetNumberAliveBlocksIncludingSubConstructables();
                            break;
                        case HealthCalculation.ResourceCost:
                            tournamentParticipant.HPCUR = val.AllBasics.GetResourceCost().Material;
                            break;
                        case HealthCalculation.Volume:
                            tournamentParticipant.HPCUR = val.AllBasics.VolumeAliveUsed;
                            break;
                        case HealthCalculation.ArrayElements:
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
            foreach (KeyValuePair<int, SortedDictionary<string, TournamentParticipant>> item in HUDLog)
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
            if (overtimeCounter == 0&& timerTotal > maxtime)
            {
                GameSpeedManager.Instance.TogglePause();
                overtimeCounter = 1;
            }
            else if (overtime > 0) {//Verlängerung ist eingeschaltet.
                if (timerTotal > maxtime + overtimeCounter * overtime) {
                    GameSpeedManager.Instance.TogglePause();
                    overtimeCounter++;
                }
            }
        }
        public Quaternion Rotation => Quaternion.Euler(0, rotation, 0);
        public float DistanceProjected(Vector3 a, Vector3 b) {
            a.y = 0;
            b.y = 0;
            return Vector3.Distance(a, b);
        }
    }
}
