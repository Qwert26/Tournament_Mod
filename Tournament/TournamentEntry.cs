using Assets.Scripts;
using Assets.Scripts.Persistence;
using BrilliantSkies.Core.Id;
using System;
using System.Collections.Generic;
using UnityEngine;
using BrilliantSkies.Core.UniverseRepresentation;
using BrilliantSkies.Ftd.Planets.Factions;
using BrilliantSkies.Ftd.Persistence.Inits;

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
                            float material = list[i].CalculateResourceCost(false, true).Material;
                            float material2 = list[i].CalculateResourceCost(true, true).Material;
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
            Team_id = faction.Id;
            BlueprintInitialisation initialisation = new BlueprintInitialisation(val)
            {
                Positioning = new BlueprintPositioning(PlanetList.MainFrame.FramePositionToUniversalPosition(VLoc(gap, gap2, count, pos, dis, Offset)), VDir())
                {
                    PositioningType = SpawnPositioning.OriginOrCentre
                }
            };
            initialisation.Run(Team_id);
            (val.Owner as ConstructableOwner).SetFleetColors(Team_id);
        }

        public Vector3 VLoc(float gap, float gap2, int count, int pos, float dis, float offset)
        {
			Vector3 ret = (IsKing ? Tournament._me.kingFormation : Tournament._me.challengerFormation).DetermineLocalPosition(IsKing, gap, gap2, count, pos, dis, offset, Spawn_location);
            return Tournament._me.Rotation * ret;
        }

        public Quaternion VDir()
        {
            Quaternion ret;
            switch (Spawn_direction)
            {
                case Tournament.SPAWN.DIR.Facing:
                    ret = Quaternion.LookRotation(IsKing ? Vector3.back : Vector3.forward);
                    break;
                case Tournament.SPAWN.DIR.Away:
                    ret = Quaternion.LookRotation(IsKing ? Vector3.forward : Vector3.back);
                    break;
                case Tournament.SPAWN.DIR.Left:
                    ret = Quaternion.LookRotation(IsKing ? Vector3.right : Vector3.left);
                    break;
                case Tournament.SPAWN.DIR.Right:
                    ret = Quaternion.LookRotation(IsKing ? Vector3.left : Vector3.right);
                    break;
                default:
                    ret = Quaternion.LookRotation(IsKing ? Vector3.back : Vector3.forward);
                    break;
            }
            return ret*Tournament._me.Rotation;
        }
    }
}
