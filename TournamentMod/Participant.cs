using BrilliantSkies.Core.Id;
namespace TournamentMod
{
	/// <summary>
	/// A battle-participant, it could be an original entry or something that was constructed during a battle.
	/// </summary>
	public class Participant
	{
		/// <summary>
		/// The ID of its Team.
		/// </summary>
		public ObjectId TeamId {
			get;
			set;
		}
		/// <summary>
		/// The Name of the Team.
		/// </summary>
		public string TeamName {
			get;
			set;
		}
		/// <summary>
		/// The unique ID of its construct.
		/// </summary>
		public int UniqueId {
			get;
			set;
		}
		/// <summary>
		/// The Name of the Blueprint.
		/// </summary>
		public string BlueprintName {
			get;
			set;
		}
		/// <summary>
		/// The current health in percent.
		/// </summary>
		public float HP {
			get;
			set;
		}
		/// <summary>
		/// The current health in absolute units.
		/// </summary>
		public float HPCUR {
			get;
			set;
		}
		/// <summary>
		/// The maximum health in absolute units.
		/// </summary>
		public float HPMAX {
			get;
			set;
		}
		/// <summary>
		/// The penalty time.
		/// </summary>
		public float OoBTime {
			get;
			set;
		}
		/// <summary>
		/// The time buffer before penalty time is added.
		/// </summary>
		public float OoBTimeBuffer {
			get;
			set;
		}
		/// <summary>
		/// Set to true, if enough penalty time has been accumulated.
		/// </summary>
		public bool Disqual {
			get;
			set;
		}
		/// <summary>
		/// Set to true, if the participant has been removed from the battlefield.
		/// </summary>
		public bool Scrapped {
			get;
			set;
		}
		/// <summary>
		/// Current amount of AI-Mainframes.
		/// </summary>
		public int AICount {
			get;
			set;
		}
		/// <summary>
		/// The time since battle start, when this participant was removed.
		/// </summary>
		public float TimeOfDespawn {
			get;
			set;
		}
	}
}
