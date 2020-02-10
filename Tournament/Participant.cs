using BrilliantSkies.Core.Id;
namespace Tournament
{
	public class Participant
	{
		public ObjectId TeamId {
			get;
			set;
		}
		public string TeamName {
			get;
			set;
		}
		public int UniqueId {
			get;
			set;
		}
		public string BlueprintName {
			get;
			set;
		}
		public float HP {
			get;
			set;
		}
		public float HPCUR {
			get;
			set;
		}
		public float HPMAX {
			get;
			set;
		}
		public float OoBTime {
			get;
			set;
		}
		public float OoBTimeBuffer {
			get;
			set;
		}
		public bool Disqual {
			get;
			set;
		}
		public bool Scrapped {
			get;
			set;
		}
		public int AICount {
			get;
			set;
		}
		public float TimeOfDespawn {
			get;
			set;
		}
	}
}
