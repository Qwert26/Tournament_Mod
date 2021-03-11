using System;
using System.Collections.Generic;
using BrilliantSkies.Core.Logger;
namespace TournamentMod
{
	public class FleetBuilder
	{
		private static readonly Dictionary<int, Dictionary<int, MainConstruct>> flagships = new Dictionary<int, Dictionary<int, MainConstruct>>();
		private static readonly Dictionary<int, Dictionary<MainConstruct, int>> fleeters = new Dictionary<int, Dictionary<MainConstruct, int>>();
		public static void RegisterAsFlagship(MainConstruct mc, int factionIndex, int indexInFormation) 
		{
			if (flagships.TryGetValue(factionIndex, out Dictionary<int, MainConstruct> factionFlagships))
			{
				factionFlagships[indexInFormation] = mc;
			}
			else
			{
				flagships[factionIndex] = new Dictionary<int, MainConstruct>
				{
					[indexInFormation] = mc
				};
			}
		}
		public static void RegisterForFlagship(MainConstruct mc, int factionIndex, int indexOfFlagship)
		{
			if (fleeters.TryGetValue(factionIndex, out Dictionary<MainConstruct, int> factionFleeters))
			{
				factionFleeters[mc] = indexOfFlagship;
			}
			else
			{
				fleeters[factionIndex] = new Dictionary<MainConstruct, int>
				{
					[mc] = indexOfFlagship
				};
			}
		}
		public static void BuildFleets()
		{
			foreach (KeyValuePair<int, Dictionary<MainConstruct, int>> fleetPart in fleeters)
			{
				foreach (KeyValuePair<MainConstruct, int> fleeter in fleetPart.Value)
				{
					if (!flagships.TryGetValue(fleetPart.Key, out Dictionary<int, MainConstruct> possibleFlagships))
					{
						AdvLogger.LogError("There are Mainconstructs for building one or multiple fleets, but no flagships are registered.", LogOptions.Popup);
						break;
					}
					else
					{
						if (!possibleFlagships.TryGetValue(fleeter.Value, out MainConstruct flagship))
						{
							AdvLogger.LogWarning("The Flagship has gone missing.", LogOptions.OnlyInDeveloperLog);
						}
						else
						{
							flagship.GetFleet().AddForce(fleeter.Key.GetForce(), enumFlagshipCreationChoice.never);
							foreach (AIMainframe mainframe in fleeter.Key.BlockTypeStorage.MainframeStore.Blocks)
							{
								mainframe.Node.Master.SetFleetMove();
							}
						}
					}
				}
			}
			fleeters.Clear();
			flagships.Clear();
		}
	}
}