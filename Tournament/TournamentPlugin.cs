using BrilliantSkies.Ftd.Planets;
using BrilliantSkies.Ftd.Planets.Instances;
using BrilliantSkies.Ftd.Planets.Factions;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using BrilliantSkies.Core.Timing;
using BrilliantSkies.Modding;
using System;
using UnityEngine;

namespace Tournament
{
    public class TournamentPlugin : GamePlugin
    {
        private static Tournament _t;

        private static InstanceSpecification @is;

        public string name => "Tournament";

        public static string Name => "Tournament";

        public Version version => new Version("0.1.1");

        public void OnLoad()
        {
            _t = new Tournament();
            GameEvents.StartEvent += OnInstanceChange;
            GameEvents.UniverseChange += OnPlanetChange;
        }

        public void OnSave()
        {
        }

        public static void OnInstanceChange()
        {
            GameEvents.FixedUpdateEvent -= _t._me.FixedUpdate;
            GameEvents.OnGui -= _t._me.OnGUI;
            GameEvents.Twice_Second -= _t._me.SlowUpdate;
            GameEvents.PreLateUpdate -= _t._me.LateUpdate;
            GameEvents.UpdateEvent -= _t.UpdateBoardSectionPreview;
            if (@is.Header.Name == InstanceSpecification.i.Header.Name)
            {
                _t._me._GUI.ActivateGui(_t._me, 0);
            }
        }

        public static void OnPlanetChange()
        {
            @is = new InstanceSpecification();
            @is.Header.Name = "Tournament Creator";
            @is.Header.Summary = "Create custom tournament style matches.";
            @is.Header.Type = InstanceType.None;
            @is.Header.CommonSettings.AvatarAvailability = AvatarAvailability.None;
            @is.Header.CommonSettings.AvatarDamage = AvatarDamage.Off;
            @is.Header.CommonSettings.ConstructableCleanUp = 0;
            @is.Header.CommonSettings.HeartStoneRequirement = 0;
            @is.Header.CommonSettings.BuildModeRules = BuildModeRules.Disabled;
            @is.Header.CommonSettings.SavingOptions = 0;
            @is.Header.CommonSettings.BlueprintSpawningOptions = BlueprintSpawningOptions.NoNewVehicles;
            @is.Header.CommonSettings.EnemyBlockDestroyedResourceDrop = 0f;
            @is.Header.CommonSettings.LocalisedResourceMode = 0;
            @is.Header.CommonSettings.FogOfWarType = FogOfWarType.None;
            @is.Header.CommonSettings.DesignerOptions = 0;
            @is.Header.CommonSettings.LuckyMechanic = LuckyMechanic.Off;
            Planet.i.Designers.AddInstance(@is);
            FactionSpecifications i = FactionSpecifications.i;
            FactionSpecificationFaction val = new FactionSpecificationFaction
            {
                Name = "King",
                AbreviatedName = "K",
                FleetColors = (new Color[4]
            {
                new Color(1f, 0.84f, 0f, 0.75f),
                new Color(0.85f, 0.65f, 0.13f, 0.75f),
                new Color(1f, 0.65f, 0f, 0.75f),
                new Color(0.85f, 0.55f, 0f, 0.75f)
            })
            };
            i.AddNew(val);
            val = new FactionSpecificationFaction
            {
                Name = "Challenger",
                AbreviatedName = "C",
                FleetColors = (new Color[4]
            {
                new Color(1f, 0f, 0f, 0.75f),
                new Color(0.55f, 0f, 0f, 0.75f),
                new Color(0.7f, 0.15f, 0.15f, 0.75f),
                new Color(1f, 0.4f, 0.3f, 0.75f)
            })
            };
            i.AddNew(val); //get id ?
        }
    }
}
