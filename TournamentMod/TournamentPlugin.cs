using BrilliantSkies.Ftd.Planets;
using BrilliantSkies.Ftd.Planets.Instances;
using BrilliantSkies.Ftd.Planets.Instances.Headers;
using BrilliantSkies.Core.Timing;
using BrilliantSkies.Core.Text;
using BrilliantSkies.Modding;
using System;
using System.Collections.Generic;
using BrilliantSkies.Core.Id;
namespace TournamentMod
{
	/// <summary>
	/// The Plugin, necessary to hook up the Mod into the Game.
	/// </summary>
	public class TournamentPlugin : GamePlugin
	{
		private static Tournament _t;
		private static InstanceSpecification @is;
		public string name => "Tournament";
		public static string Name => "Tournament";
		public Version version => new Version(2, 7, 3, 6);
		internal static FactionManagement factionManagement;
		/// <summary>
		/// Only gets called once during a game session.
		/// </summary>
		public void OnLoad()
		{
			_t = new Tournament();
			factionManagement = new FactionManagement();
			GameEvents.StartEvent += OnInstanceChange;
			GameEvents.StartEvent += factionManagement.OnInstanceChange;
			GameEvents.UniverseChange += OnPlanetChange;
			GameEvents.UniverseChange += factionManagement.OnUniverseChange;
		}
		/// <summary>
		/// Doesn't get called by the game yet, when it shuts down.
		/// </summary>
		public void OnSave() { }
		/// <summary>
		/// Adds and removes the instance based on the gametype.
		/// </summary>
		public void OnInstanceChange()
		{
			GameEvents.FixedUpdateEvent -= _t.FixedUpdate;
			GameEvents.OnGui -= _t.OnGUI;
			GameEvents.Twice_Second -= _t.SlowUpdate;
			GameEvents.PreLateUpdate -= _t.LateUpdate;
			if (GAMESTATE.GetGameType() == enumGameType.worldeditor)
			{
				Planet.i.Designers.RemoveInstance(@is);
			}
			else if(!Planet.i.Designers.Instances.Contains(@is)) {
				Planet.i.Designers.AddInstance(@is);
			}
			if (@is.Header.Name == InstanceSpecification.i.Header.Name) {
				GAMESTATE.MyTeam = ObjectId.NoLinkage;
				_t._GUI.ActivateGui(_t, BrilliantSkies.Ui.Displayer.GuiActivateType.Standard);
			}
		}
		/// <summary>
		/// Creates the instance and adds it to the list of designers.
		/// </summary>
		public void OnPlanetChange()
		{
			@is = new InstanceSpecification();
			@is.GenerateBlankInstance(Planet.i);
			Planet.i.Designers.AddInstance(@is);
			@is.Header.Name = "Tournament Creator";
			@is.Header.Summary = "Create custom tournament style matches.";
			@is.Header.DescriptionParagraphs = new List<HeaderAndParagraph> {
				new HeaderAndParagraph() {
					Header = "Two to Six Teams",
					Paragraph = "Go anywhere from a Duel to a six-way Brawl. The Teams spawn a fixed distance from the center. Deactivating a team does not cause it to forget its entries."
				}, new HeaderAndParagraph() {
					Header="A ton of Options",
					Paragraph="Determine the starting Distance, Offsets from teammates, Start-Materials, maximum fighting Distance, maximum and minimum fighting Height, Penalty-Time, " +
					"tolerated Fleeing-Speed and maximum Match-Time for the fights. For Battles with a lot of vertical Freedom, its better to use the ground-projected Distance. "+
					"Give each entry the same amount or distribute a total amount, give both Teams equal Amounts or make it unbalanced and even enable the advanced Battle Options. " +
					"Set the Lifesteal-Percentage, use a naval march formation, change how Vehicles are despawned, how Health is calculated and even set a minimum Health-Percentage under which Penalty-Time is picked up."
				}, new HeaderAndParagraph() {
					Header="You go there and we fight here",
					Paragraph="Each Vehicle can be spawned in any orientation and at any altitude. If you don't like the spawn order, you can also change it." +
					"Use the Location sliders to select the perfect Map-Sector and Terrain-piece for the fight. The background-camera can be rotated around by either holding 'e' or pressing the middle mousebutton. " +
					"And finally rotate the teams around the center point of it."
				}, new HeaderAndParagraph() {
					Header="I want a Rematch!",
					Paragraph="Simply use two Buttons to cycle through Teams or swap orientations of a single Team. Deactivating a team excludes it from the rotation, " +
					"but it will not forget its entries."
				}, new HeaderAndParagraph() {
					Header="So many Colors!",
					Paragraph="Control the fleet colors of every single team, create your own schema or use one of the pre-made ones for a team. They even get safed when you press the \"Safe\"-Button."
				}
			};
			@is.Header.Type = InstanceType.Designer;
			@is.Header.CommonSettings.AvatarAvailability = AvatarAvailability.None;
			@is.Header.CommonSettings.AvatarDamage = AvatarDamage.Off;
			@is.Header.CommonSettings.ConstructableCleanUp = ConstructableCleanUp.All;
			@is.Header.CommonSettings.HeartStoneRequirement = HeartStoneRequirement.None;
			@is.Header.CommonSettings.BuildModeRules = BuildModeRules.Disabled;
			@is.Header.CommonSettings.SavingOptions = SavingOptions.None;
			@is.Header.CommonSettings.BlueprintSpawningOptions = BlueprintSpawningOptions.NoNewVehicles;
			@is.Header.CommonSettings.EnemyBlockDestroyedResourceDrop = 0f;
			//@is.Header.CommonSettings.LocalisedResourceMode = LocalisedResourceMode.UseCentralStore;
			@is.Header.CommonSettings.FogOfWarType = FogOfWarType.None;
			@is.Header.CommonSettings.DesignerOptions = DesignerOptions.Off;
			@is.Header.CommonSettings.LuckyMechanic = LuckyMechanic.Off;
			@is.Territory.SetAllUnowned(Planet.i.World.BoardLayout);
			@is.PostLoadInitiate(Planet.i, PostLoadInitiateType.New);
		}
	}
}