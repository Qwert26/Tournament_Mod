using BrilliantSkies.Ftd.Planets;
using BrilliantSkies.Ftd.Planets.Instances;
using BrilliantSkies.Ftd.Planets.Factions;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using BrilliantSkies.Core.Timing;
using BrilliantSkies.Core.Text;
using BrilliantSkies.Modding;
using System;
using System.Collections.Generic;
using BrilliantSkies.Core.Id;
using BrilliantSkies.Ftd.Planets.Instances.Factions;
namespace Tournament
{
    public class TournamentPlugin : GamePlugin
    {
        private static Tournament _t;

        private static InstanceSpecification @is;

        public string name => "Tournament";

        public static string Name => "Tournament";

        public Version version => new Version("2.4.7.16");

        public static FactionSpecificationFaction factionTeam1, factionTeam2, factionTeam3;
        
        public void OnLoad()
        {
            _t = new Tournament();
            GameEvents.StartEvent += OnInstanceChange;
            GameEvents.UniverseChange += OnPlanetChange;
        }

        public void OnSave() { }

        public void OnInstanceChange()
        {
            GameEvents.FixedUpdateEvent -= _t.FixedUpdate;
            GameEvents.OnGui -= _t.OnGUI;
            GameEvents.Twice_Second -= _t.SlowUpdate;
            GameEvents.PreLateUpdate -= _t.LateUpdate;
            GameEvents.UpdateEvent -= _t.UpdateBoardSectionPreview;
            if (@is.Header.Name == InstanceSpecification.i.Header.Name)
            {
                GAMESTATE.MyTeam = ObjectId.NoLinkage;
                _t._GUI.ActivateGui(_t);
            }
        }
        /// <summary>
        /// Creates and Adds the instance and the required factions.
        /// </summary>
        public static void OnPlanetChange()
        {
            @is = new InstanceSpecification();
            @is.GenerateBlankInstance();
            @is.Header.Name = "Tournament Creator";
            @is.Header.Summary = "Create custom tournament style matches.";
            @is.Header.DescriptionParagraphs = new List<HeaderAndParagraph> {
                new HeaderAndParagraph() {
                    Header = "Two Teams",
                    Paragraph = "Add any Vehicle in any Order to the two opposing Teams. For pre-tournament qualications, " +
                    "you can also add a vehicle to both teams for a mirror match. Then let the Fight begin!"
                }, new HeaderAndParagraph() {
                    Header="A lot of Options",
                    Paragraph="Determine the starting Distance, Offsets from teammates, Start-Materials, maximum fighting Distance, maximum and minimum fighting Height, Penalty-Time, " +
                    "tolerated Fleeing-Speed and maximum Match-Time for the fights. For Battles with a lot of vertical Freedom, its better to use the ground-projected Distance."
                }, new HeaderAndParagraph() {
                    Header="Even more Options",
                    Paragraph="Decide to use local or centralised Resources, Give both Teams equal Amounts or make it unbalanced and even enable the advanced Battle Options. " +
                    "Set the Lifesteal-Percentage, change how Vehicles are despawned and how Health is calculated and even set a minimum Health-Percentage under which Penalty-Time is picked up."
                }, new HeaderAndParagraph() {
                    Header="You go there and we fight here",
                    Paragraph="Each Vehicle can be spawned in any orientation and at any altitude. " +
                    "Use the Location sliders to select the perfect Map-Sector for the fight. And finally rotate them around the center point of it."
                }, new HeaderAndParagraph() {
                    Header="I want a Rematch!",
                    Paragraph="Simply use two Buttons to swap both Teams, both orientations or even swap Teams while also swapping the orientation of each Entry, " +
                    "thus keeping the orientations relative to the battlegrounds the same."
                }
            };
            @is.Header.Type = InstanceType.None;
            @is.Header.CommonSettings.AvatarAvailability = AvatarAvailability.None;
            @is.Header.CommonSettings.AvatarDamage = AvatarDamage.Off;
            @is.Header.CommonSettings.ConstructableCleanUp = ConstructableCleanUp.All;
            @is.Header.CommonSettings.HeartStoneRequirement = HeartStoneRequirement.None;
            @is.Header.CommonSettings.BuildModeRules = BuildModeRules.Disabled;
            @is.Header.CommonSettings.SavingOptions = SavingOptions.None;
            @is.Header.CommonSettings.BlueprintSpawningOptions = BlueprintSpawningOptions.NoNewVehicles;
            @is.Header.CommonSettings.EnemyBlockDestroyedResourceDrop = 0f;
            @is.Header.CommonSettings.LocalisedResourceMode = LocalisedResourceMode.UseCentralStore;
            @is.Header.CommonSettings.FogOfWarType = FogOfWarType.None;
            @is.Header.CommonSettings.DesignerOptions = DesignerOptions.Off;
            @is.Header.CommonSettings.LuckyMechanic = LuckyMechanic.Off;
            @is.Territory.SetAllUnowned();
            @is.PostLoadInitiate();
            Planet.i.Designers.AddInstance(@is);
            FactionSpecifications i = FactionSpecifications.i;
            factionTeam1 = new FactionSpecificationFaction
            {
                Name = "Team 1",
                AbreviatedName = "T1",
                FleetColors = TournamentFleetColor.classicYellow.Colors,
            };
            i.AddNew(factionTeam1);
            factionTeam2 = new FactionSpecificationFaction
            {
                Name = "Team 2",
                AbreviatedName = "T2",
                FleetColors = TournamentFleetColor.classicRed.Colors
            };
            i.AddNew(factionTeam2);
            factionTeam3 = new FactionSpecificationFaction
            {
                Name = "Team 3",
                AbreviatedName = "T3",
                FleetColors = TournamentFleetColor.neoBlue.Colors
            };
            i.AddNew(factionTeam3);
            @is.Factions.GetFaction(factionTeam1.Id).eController = FactionController.AI_General;
            @is.Factions.GetFaction(factionTeam2.Id).eController = FactionController.AI_General;
            @is.Factions.GetFaction(factionTeam3.Id).eController = FactionController.AI_General;
        }
    }
}
