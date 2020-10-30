using BrilliantSkies.Core.Id;
using System;
using System.Collections.Generic;
using BrilliantSkies.Core.Logger;
using BrilliantSkies.Core.Enumerations;
using BrilliantSkies.Core.Timing;
using UnityEngine;
namespace TournamentMod
{
	using Serialisation;
	using System.Linq;

	public class ParticipantManagement
	{
		private HealthType healthType;
		private Parameters parameters;
		/// <summary>
		/// Calculates the mean in the case of multiple violations at once.
		/// </summary>
		private HoelderMean meanCalculation = null;
		/// <summary>
		/// The penalty weight settings for each team.
		/// </summary>
		private List<Dictionary<PenaltyType, float>> teamPenaltyWeights;
		private float timerTotal;
		/// <summary>
		/// A 2D-Table to Map Teams/Factions to their Mainconstructs which map to their Participant-Object.
		/// </summary>
		public Dictionary<ObjectId, Dictionary<MainConstruct, Participant>> TCP { get; private set; }
		public Gradient PenaltyTimeGradient { get; private set; }
		public ParticipantManagement(Parameters parameters, List<Dictionary<PenaltyType, float>> tpw)
		{
			this.parameters = parameters;
			healthType = (HealthType)parameters.HealthCalculation;
			meanCalculation = new HoelderMean((WeightCombinerType) parameters.WeightCombiner);
			teamPenaltyWeights = tpw;
			timerTotal = 0f;
			PenaltyTimeGradient = ((GradientType) parameters.PenaltyTimeGradient).GetGradient();
			TCP = new Dictionary<ObjectId, Dictionary<MainConstruct, Participant>>();
			GameEvents.FixedUpdateEvent.RegWithEvent(FixedUpdate);
			GameEvents.Four_Second.RegWithEvent(SlowUpdate);
			UpdateConstructs();
		}
		public void UnregisterEvents()
		{
			GameEvents.FixedUpdateEvent.UnregWithEvent(FixedUpdate);
			GameEvents.Four_Second.UnregWithEvent(SlowUpdate);
		}
		/// <summary>
		/// Gets called once every physics update. Negates material-gain by self-shooting if Lifesteal is -1%. Also determines if a Particiant is currently violating any rules and gives out a penalty.
		/// </summary>
		/// <param name="dt"></param>
		public void FixedUpdate(ITimeStep dt)
		{
			if (!GameSpeedManager.Instance.IsPaused)
			{
				/*if (parameters.MaterialConversion == -1 && materials != null) //Verbietet Materialrückgewinnung durch Selbst- und Teambeschuss.
				{
					for (int i = 0; i < parameters.ActiveFactions; i++)
					{
						if (materials[i] < TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.Material.Quantity)
						{
							TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.SetResources(materials[i]);
						}
						else
						{
							materials[i] = (int) TournamentPlugin.factionManagement.factions[i].InstanceOfFaction.ResourceStore.Material.Quantity;
						}
					}
				}*/
				MainConstruct[] array = StaticConstructablesManager.constructables.ToArray();
				if (array.Length == 0)
				{
					GameSpeedManager.Instance.TogglePause();
					return;
					//Everyone died...
				}
				foreach (MainConstruct currentConstruct in array)
				{
					if (!TCP[currentConstruct.GetTeam()].TryGetValue(currentConstruct, out Participant tournamentParticipant))
					{
						UpdateConstructs();
						tournamentParticipant = TCP[currentConstruct.GetTeam()][currentConstruct];
					}
					//The participant is still in the game:
					if (!tournamentParticipant.Disqual || !tournamentParticipant.Scrapped)
					{
						int teamIndex = TournamentPlugin.factionManagement.TeamIndexFromObjectID(currentConstruct.GetTeam());
						tournamentParticipant.AICount = currentConstruct.BlockTypeStorage.MainframeStore.Blocks.Count;
						bool violatingRules = false;
						//Is it braindead and is the corresponding cleanup-function turned off?
						if (((ConstructableCleanUp) parameters.CleanUpMode == ConstructableCleanUp.Off || !parameters.CleanUpNoAI) && tournamentParticipant.AICount == 0)
						{
							violatingRules = true;
							meanCalculation.AddValue(teamPenaltyWeights[teamIndex][PenaltyType.NoAi]);
						}
						//Is it below the lower altitude limit?
						if (currentConstruct.CentreOfMass.y < parameters.AltitudeLimits[teamIndex].x)
						{
							if (parameters.SoftLimits[teamIndex])
							{
								if (-currentConstruct.Velocity.y > parameters.AltitudeReverse[teamIndex]) //Still sinking.
								{
									violatingRules = true;
									meanCalculation.AddValue(teamPenaltyWeights[teamIndex][PenaltyType.UnderAltitude]);
								}
							}
							else
							{
								violatingRules = true;
								meanCalculation.AddValue(teamPenaltyWeights[teamIndex][PenaltyType.UnderAltitude]);
							}
						}
						else if (currentConstruct.CentreOfMass.y > parameters.AltitudeLimits[teamIndex].y) //Is it above the upper altitude limit?
						{
							if (parameters.SoftLimits[teamIndex])
							{
								if (currentConstruct.Velocity.y > parameters.AltitudeReverse[teamIndex]) //Still raising.
								{
									violatingRules = true;
									meanCalculation.AddValue(teamPenaltyWeights[teamIndex][PenaltyType.AboveAltitude]);
								}
							}
							else
							{
								violatingRules = true;
								meanCalculation.AddValue(teamPenaltyWeights[teamIndex][PenaltyType.AboveAltitude]);
							}
						}
						//Does it have too little HP and is the corresponding cleanup-function turned off?
						if (((ConstructableCleanUp) parameters.CleanUpMode == ConstructableCleanUp.Off || !parameters.CleanUpTooDamagedConstructs) && tournamentParticipant.HP < parameters.MinimumHealth)
						{
							violatingRules = true;
							meanCalculation.AddValue(teamPenaltyWeights[teamIndex][PenaltyType.TooMuchDamage]);
						}
						//Is it too fast?
						if (currentConstruct.Velocity.magnitude > parameters.MaximumSpeed[teamIndex])
						{
							violatingRules = true;
							meanCalculation.AddValue(teamPenaltyWeights[teamIndex][PenaltyType.TooFast]);
						}
						//Is it too far away from enemies?
						if (CheckDistanceAll(currentConstruct, teamIndex, parameters.EnemyAttackPercentage[teamIndex] / 100f, out bool noEnemies))
						{
							violatingRules = true;
							meanCalculation.AddValue(teamPenaltyWeights[teamIndex][PenaltyType.FleeingFromBattle]);
						}
						//Are there no more enemies?
						if (noEnemies)
						{
							if (parameters.PauseOnVictory)
							{
								GameSpeedManager.Instance.TogglePause();
							}
							break;
							//Checking additional constructs does no longer change the outcome, we still need to update the timer though.
						}
						//Has any rule been violated?
						if (violatingRules)
						{
							AddPenalty(tournamentParticipant, teamIndex, dt, meanCalculation.GetMean());
						}
						else
						{
							//Recover the timebuffer if soft limits are active.
							if (parameters.SoftLimits[teamIndex])
							{
								tournamentParticipant.OoBTimeBuffer = 0;
							}
						}
						tournamentParticipant.Disqual = tournamentParticipant.OoBTime > parameters.MaximumPenaltyTime[teamIndex];
					}
				}
				timerTotal += dt.DeltaTime;
			}
		}
		/// <summary>
		/// Performs a distance-check for a given Construct against all its enemies to determine, if it is currently violating the distance-rule.
		/// </summary>
		/// <param name="current">The current construct to check distances.</param>
		/// <param name="teamIndex">Its team index</param>
		/// <param name="percentage">The relative value for attacking.</param>
		/// <param name="noEnemiesFound">Set to true, if no enemies were found this pass.</param>
		/// <returns>True, if the current participant is considered as "fleeing from battle" and has to accumulate penalty time.</returns>
		private bool CheckDistanceAll(MainConstruct current, int teamIndex, float percentage, out bool noEnemiesFound)
		{
			MainConstruct[] array = StaticConstructablesManager.constructables.ToArray();
			List<MainConstruct> enemies = new List<MainConstruct>(array.Length);
			foreach (MainConstruct potentialEnemy in array)
			{
				if (current != potentialEnemy && current.GetTeam() != potentialEnemy.GetTeam())
				{
					enemies.Add(potentialEnemy);
				}
			}
			if (enemies.Count == 0)
			{
				noEnemiesFound = true;
				return false;
			}
			noEnemiesFound = false;
			int rulebreaks = 0, beatenButNotDestroyed = 0;
			foreach (MainConstruct enemy in enemies)
			{
				if (TCP[enemy.GetTeam()].TryGetValue(enemy, out Participant participant))
				{
					if (participant.Disqual || participant.Scrapped || ((ConstructableCleanUp) parameters.CleanUpMode != ConstructableCleanUp.Off && parameters.CleanUpNoAI && participant.AICount == 0))
					{
						beatenButNotDestroyed++;
						continue;
					}
					float currentDistance = parameters.ProjectedDistance[teamIndex] ? DistanceProjected(current.CentreOfMass, enemy.CentreOfMass) : Vector3.Distance(current.CentreOfMass, enemy.CentreOfMass);
					float futureDistance = parameters.ProjectedDistance[teamIndex] ? DistanceProjected(current.CentreOfMass + current.Velocity, enemy.CentreOfMass) : Vector3.Distance(current.CentreOfMass + current.Velocity, enemy.CentreOfMass);
					//Too far away?
					if (currentDistance > parameters.DistanceLimit[teamIndex])
					{
						if (parameters.SoftLimits[teamIndex])
						{
							//Going away faster than DistanceReverse allows?
							if (futureDistance > currentDistance + parameters.DistanceReverse[teamIndex])
							{
								rulebreaks++;
							}
						}
						else
						{
							rulebreaks++;
						}
					}
				}
				else
				{
					beatenButNotDestroyed++;
					//We need to wait for the next update, so for now it doesn't get considered.
				}
			}
			return Mathf.Max(1, Mathf.RoundToInt((1f - percentage) * (enemies.Count - beatenButNotDestroyed))) <= rulebreaks;
		}
		/// <summary>
		/// Includes any new Mainconstruct into the HUDLog and updates current health-values.
		/// </summary>
		private void UpdateConstructs()
		{
			MainConstruct[] array = StaticConstructablesManager.constructables.ToArray();
			foreach (MainConstruct val in array)
			{
				if (!TCP.ContainsKey(val.GetTeam()))
				{
					TCP[val.GetTeam()] = new Dictionary<MainConstruct, Participant>();
				}
				if (!TCP[val.GetTeam()].TryGetValue(val, out Participant tournamentParticipant))
				{
					tournamentParticipant = new Participant
					{
						AICount = val.BlockTypeStorage.MainframeStore.Count,
						TeamId = val.GetTeam(),
						TeamName = val.GetTeam().FactionSpec().Name,
						BlueprintName = val.GetBlueprintName(),
						Disqual = false,
						Scrapped = false,
						UniqueId = val.UniqueId
					};
					switch (healthType)
					{
						case HealthType.Blockcount:
							tournamentParticipant.HPMAX = val.AllBasics.GetNumberBlocksIncludingSubConstructables();
							break;
						case HealthType.Resourcecost:
							tournamentParticipant.HPMAX = val.AllBasics.GetResourceCost(ValueQueryType.IncludeContents).Material;
							break;
						case HealthType.Volume:
						case HealthType.ArrayElements:
							tournamentParticipant.HPMAX = 0;
							for (int x = val.AllBasics.minx_; x <= val.AllBasics.maxx_; x++)
							{
								for (int y = val.AllBasics.miny_; x <= val.AllBasics.maxy_; y++)
								{
									for (int z = val.AllBasics.minz_; x <= val.AllBasics.maxz_; z++)
									{
										Block b = val.AllBasics[x, y, z];
										if (b != null)
										{
											if (healthType == HealthType.Volume)
											{
												tournamentParticipant.HPMAX += b.item.SizeInfo.VolumeFactor;
											}
											else
											{
												tournamentParticipant.HPMAX += 1;
											}
										}
									}
								}
							}
							break;
						default:
							AdvLogger.LogError("Health calculation of newly spawned in Construct is not available!", LogOptions.OnlyInDeveloperLog);
							break;
					}
					TCP[tournamentParticipant.TeamId][val] = tournamentParticipant;
				}
				if (!tournamentParticipant.Disqual || !tournamentParticipant.Scrapped)
				{
					switch (healthType)
					{
						case HealthType.Blockcount:
							tournamentParticipant.HPCUR = val.AllBasics.GetNumberAliveBlocksIncludingSubConstructables();
							break;
						case HealthType.Resourcecost:
							tournamentParticipant.HPCUR = val.AllBasics.GetResourceCost(ValueQueryType.AliveOnly | ValueQueryType.IncludeContents).Material;
							break;
						case HealthType.Volume:
							tournamentParticipant.HPCUR = val.AllBasics.VolumeAliveUsed;
							break;
						case HealthType.ArrayElements:
							tournamentParticipant.HPCUR = val.AllBasics.VolumeOfFullAliveBlocksUsed;
							break;
						default:
							AdvLogger.LogError("Health calculation of Construct is not available!", LogOptions.OnlyInDeveloperLog);
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
		}
		/// <summary>
		/// Increases the Penalty-Time of a single Participant or uses up the Time-Buffer if Soft-Limits are active for the given Team.
		/// </summary>
		/// <param name="tournamentParticipant">The offending Participant</param>
		/// <param name="teamIndex">The index of its corresponding Team</param>
		private void AddPenalty(Participant tournamentParticipant, int teamIndex, ITimeStep dt, float multiplier = 1)
		{
			float increment = dt.DeltaTime * multiplier;
			if (parameters.SoftLimits[teamIndex])
			{
				if (tournamentParticipant.OoBTimeBuffer > parameters.MaximumBufferTime[teamIndex])
				{
					tournamentParticipant.OoBTime += increment;
				}
				else
				{
					tournamentParticipant.OoBTimeBuffer += increment;
				}
			}
			else
			{
				tournamentParticipant.OoBTime += increment;
			}
		}
		/// <summary>
		/// Gets called twice a second. Despawns Mainconstructs which have accumulated too much Penalty-time and pauses the Game once the time limit or the end of an overtime-section is reached.
		/// </summary>
		/// <param name="dt"></param>
		public void SlowUpdate(ITimeStep dt)
		{
			UpdateConstructs();
			foreach (KeyValuePair<ObjectId, Dictionary<MainConstruct, Participant>> teamAndMembers in TCP)
			{
				foreach (KeyValuePair<MainConstruct, Participant> member in TCP[teamAndMembers.Key])
				{
					if (StaticConstructablesManager.constructables.Contains(member.Key))
					{
						if (!TCP[teamAndMembers.Key][member.Key].Disqual)
						{
							continue;
						}
						else if (!TCP[teamAndMembers.Key][member.Key].Scrapped)
						{
							TCP[teamAndMembers.Key][member.Key].HPCUR = 0f;
							TCP[teamAndMembers.Key][member.Key].Scrapped = true;
							Vector3 centreOfMass = member.Key.CentreOfMass;
							UnityEngine.Object.Instantiate(Resources.Load("Detonator-MushroomCloud") as GameObject, centreOfMass, Quaternion.identity);
							member.Key.DestroyCompletely(DestroyReason.Wiped, true);
							TCP[teamAndMembers.Key][member.Key].TimeOfDespawn = timerTotal;
						}
					}
					else if (!(TCP[teamAndMembers.Key][member.Key].Disqual && TCP[teamAndMembers.Key][member.Key].Scrapped))
					{
						TCP[teamAndMembers.Key][member.Key].Disqual = TCP[teamAndMembers.Key][member.Key].Scrapped = true;
						TCP[teamAndMembers.Key][member.Key].HPCUR = 0;
						TCP[teamAndMembers.Key][member.Key].TimeOfDespawn = timerTotal;
					}
				}
			}
		}
		/// <summary>
		/// Calculates the distance between two points, if they were on the XZ-Plane.
		/// </summary>
		/// <param name="a">Point a</param>
		/// <param name="b">Point b</param>
		/// <returns>The distance in the horizontal plane only.</returns>
		private float DistanceProjected(Vector3 a, Vector3 b)
		{
			a.y = 0;
			b.y = 0;
			return Vector3.Distance(a, b);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public int ParticipantCount()
		{
			int ret = 0;
			foreach (var teams in TCP)
			{
				ret += teams.Value.Count();
			}
			return ret;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public string PenaltyTimeColor(Participant p)
		{
			int maxTime=parameters.MaximumPenaltyTime[TournamentPlugin.factionManagement.TeamIndexFromObjectID(p.TeamId)];
			return ColorUtility.ToHtmlStringRGB(PenaltyTimeGradient.Evaluate(p.OoBTime / maxTime));
		}
	}
}