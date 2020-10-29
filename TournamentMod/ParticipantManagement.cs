using BrilliantSkies.Core.Id;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TournamentMod
{
	public class ParticipantManagement
	{
		private HealthType healthType;
		private Dictionary<ObjectId, Dictionary<MainConstruct, Participant>> teamsConstructsParticipants;
		public void SpawnEntries(HealthType healthType, Object teamsEntries)
		{
			this.healthType = healthType;
		}
	}
}