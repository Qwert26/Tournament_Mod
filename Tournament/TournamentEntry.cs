using Assets.Scripts;
using Assets.Scripts.Persistence;
using BrilliantSkies.Core.Id;
using System;
using System.Collections.Generic;
using UnityEngine;
using BrilliantSkies.Core.UniverseRepresentation;
using BrilliantSkies.Ftd.Planets.Factions;
namespace Tournament
{
    public class TournamentEntry
    {
        private BlueprintFile _bpf;

        public Blueprint bp;

        public bool IsKing {
            get;
            set;
        }

        public Tournament.SPAWN.DIR Spawn_direction {
            get;
            set;
        }

        public Tournament.SPAWN.LOC Spawn_location {
            get;
            set;
        }

        public ObjectId Team_id {
            get;
            set;
        }

        public float Res {
            get;
            set;
        }

        public float Offset {
            get;
            set;
        }

        public BlueprintFile Bpf {
            get {
                return _bpf;
            }
            set {
                _bpf = value;
                bp = Bpf.Load(true);
            }
        }

        public string[] LabelCost {
            get {
                if (bp != null)
                {
                    List<Blueprint> list = bp.SCs.FindAll((Blueprint x) => !x.IsSubConstructable());
                    int count = list.Count;
                    if (count > 0)
                    {
                        string[] array = new string[checked(count + 1)];
                        float num = 0f;
                        for (int i = 0; i < count; i = checked(i + 1))
                        {
                            float material = list[i].CalculateResourceCost(false, true).Material;
                            array[checked(i + 1)] = $"{list[i].blueprintName} <color=cyan>{material}</color>";
                            num += material;
                        }
                        array[0] = $"{bp.blueprintName} <color=cyan>{bp.CalculateResourceCost(false, true).Material - num}</color>";
                        return array;
                    }
                    return new string[1]
                    {
                        $"{bp.blueprintName} <color=cyan>{bp.CalculateResourceCost(false, true).Material}</color>"
                    };
                }
                return null;
            }
        }

        public string[] Label {
            get {
                if (bp != null)
                {
                    List<Blueprint> list = bp.SCs.FindAll((Blueprint x) => !x.IsSubConstructable());
                    int count = list.Count;
                    if (count > 0)
                    {
                        string[] array = new string[checked(count + 1)];
                        float num = 0f;
                        float num2 = 0f;
                        for (int i = 0; i < count; i = checked(i + 1))
                        {
                            float material = (list[i].CalculateResourceCost(false, true)).Material;
                            float material2 = (list[i].CalculateResourceCost(true, true)).Material;
                            array[checked(i + 1)] = $"{list[i].blueprintName} {Math.Round(material2 / material * 100f, 1)}";
                            num += material;
                            num2 += material2;
                        }
                        array[0] = $"{bp.blueprintName} {Math.Round((bp.CalculateResourceCost(true, true).Material - num2) / (bp.CalculateResourceCost(false, true).Material - num) * 100f, 1)}";
                        return array;
                    }
                    return new string[1]
                    {
                        $"{bp.blueprintName} {Math.Round(bp.CalculateResourceCost(true, true).Material / bp.CalculateResourceCost(false, true).Material * 100f, 1)}"
                    };
                }
                return null;
            }
        }

        public void Spawn(float dis, float gap, float gap2, int count, int pos)
        {
            MainConstruct val = BlueprintConverter.Convert(bp, ConversionDamageMode.IgnoreDamage, true);
            FactionSpecificationFaction faction = IsKing ? TournamentPlugin.kingFaction : TournamentPlugin.challengerFaction;
            val.Colors.SetFleetColors(faction.FleetColors);
            Team_id = faction.Id;
            BlueprintConverter.Initiate(val, PlanetList.MainFrame.FramePositionToUniversalPosition(VLoc(gap, gap2, count, pos, dis, Offset)), VDir(), Team_id, null, SpawnPositioning.OriginOrCentre);
        }

        public Vector3 VLoc(float gap, float gap2, int count, int pos, float dis, float offset)
        {
            float x = (count - 1f) * gap / 2f - pos * gap;
            float z = IsKing ? (dis / 2f) + (pos * gap2) : (dis / 2f) - dis - (pos * gap2);

            switch (Spawn_location)
            {
                case Tournament.SPAWN.LOC.Sea:
                    return new Vector3(x, 1f + offset, z);
                case Tournament.SPAWN.LOC.Air:
                    return new Vector3(x, 100f + offset, z);
                case Tournament.SPAWN.LOC.Sub:
                    return new Vector3(x, -20f + offset, z);
                case Tournament.SPAWN.LOC.Land:
                    return new Vector3(x, 51f + offset, z);
                default:
                    return new Vector3(x, 0f + offset, z);
            }
        }

        public Quaternion VDir()
        {

            switch (Spawn_direction)
            {
                case Tournament.SPAWN.DIR.Facing:
                    return Quaternion.LookRotation(new Vector3(0f, 0f, (!IsKing) ? 1 : (-1)));
                case Tournament.SPAWN.DIR.Away:
                    return Quaternion.LookRotation(new Vector3(0f, 0f, IsKing ? 1 : (-1)));
                case Tournament.SPAWN.DIR.Left:
                    return Quaternion.LookRotation(new Vector3(IsKing ? 1 : (-1), 0f, 0f));
                case Tournament.SPAWN.DIR.Right:
                    return Quaternion.LookRotation(new Vector3((!IsKing) ? 1 : (-1), 0f, 0f));
                default:
                    return Quaternion.LookRotation(new Vector3(0f, 0f, (!IsKing) ? 1 : (-1)));
            }
        }
    }
}
