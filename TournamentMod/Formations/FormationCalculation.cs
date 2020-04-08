using UnityEngine;
using System;
namespace TournamentMod.Formations
{
	/// <summary>
	/// Contains functions for positional calculations
	/// </summary>
	public static class FormationCalculation {
		/// <summary>
		/// Important value for PC- and CPC-Formation.
		/// </summary>
		public static readonly float factorFor1To1GapRatio = Mathf.Tan(68 * Mathf.Deg2Rad);
		/// <summary>
		/// The final rotation for a team.
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		public static Quaternion FactionRotation(float angle) {
			return Quaternion.Euler(0, angle, 0);
		}
		/// <summary>
		/// Calculates the position of an individual entry inside a line-formation.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		public static Vector3 LineFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			float x = - (count - 1) * gapLeftRight / 2f + index * gapLeftRight;
			float z = distance /*+ (count - 1) * gapForwardBackward / 2f*/ + index * gapForwardBackward;
			return FactionRotation(factionRotation) * new Vector3(x, height, z);
		}
		/// <summary>
		/// Calculates the position of an individual entry inside a guardline-formation. This formation ignores the forward-backward-gap.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="_1">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		public static Vector3 GuardLineFormation(float factionRotation, float gapLeftRight, float _1, int count, int index, float distance, float height) {
			float x = (count - 1) * gapLeftRight / 2f - index * gapLeftRight;
			return FactionRotation(factionRotation) * new Vector3(x, height, distance);
		}
		/// <summary>
		/// Calculates the position of an individual entry inside a "arrow"/"funnel"-formation. As this formation is built from the central pivot point,
		/// it does not need to know the size of the formation.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="_1">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		public static Vector3 WedgeFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int _1, int index, float distance, float height) {
			if (index == 0) //Ist es das Flaggschiff?
			{
				return FactionRotation(factionRotation) * new Vector3(0, height, distance);
			}
			else
			{
				float x, z;
				index--;
				if (index % 2 == 0)
				{
					index /= 2;
					x = (1 + index) * gapLeftRight;
				}
				else
				{
					index = (index - 1) / 2;
					x = -(1 + index) * gapLeftRight;
				}
				z = distance + (1 + index) * gapForwardBackward;
				return FactionRotation(factionRotation) * new Vector3(x, height, z);
			}
		}
		/// <summary>
		/// Calculates the position of an individual entry inside a divided wedge. As the formation is divided into three groups with equal sizes,
		/// it need to know the size of the entire formation.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		public static Vector3 DividedWedgeFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			if (index == 0) //Ist es das Flaggschiff?
			{
				return FactionRotation(factionRotation) * new Vector3(0, height, distance);
			}
			else
			{
				float x, z;
				if (count <= 3) //Sind es 3 oder weniger Schiffe?
				{
					z = distance;
					if (index == 1)
					{
						x = gapLeftRight;
						z += gapForwardBackward;
					}
					else //index=2
					{
						x = -gapLeftRight;
						z += gapForwardBackward;
					}
				}
				else
				{
					count -= 3;
					if (count % 6 != 0)
					{
						count += 6 - (count % 6);
					}
					//Anzahl der Plätze zwischen Flagschiff und Kommandoschiff
					int bufferSpaces = count / 3;
					if (index == 1) //Ist es das linke Kommandoschiff?
					{
						x = (1 + bufferSpaces) * gapLeftRight;
						z = distance + ((1 + bufferSpaces) * gapForwardBackward);
					}
					else if (index == 2) //Ist es das rechte Kommandoschiff?
					{
						x = -(1 + bufferSpaces) * gapLeftRight;
						z = distance + ((1 + bufferSpaces) * gapForwardBackward);
					}
					else //Alle anderen Schiffe
					{
						int groupAndSide = (index - 3) % 6;
						int groupLine = (index - 3) / 6;
						switch (groupAndSide)
						{
							case 0: //Links hinter dem Flagschiff
								x = gapLeftRight;
								z = distance + (groupLine * gapForwardBackward);
								break;
							case 1: //Rechts hinter dem Flagschiff
								x = -gapLeftRight;
								z = distance + (groupLine * gapForwardBackward);
								break;
							case 2: //Links hinter dem linken Kommandoschiff
								x = (2 + bufferSpaces) * gapLeftRight;
								z = distance + ((2 + bufferSpaces + groupLine) * gapForwardBackward);
								break;
							case 3: //Rechts hinter dem rechten Kommandoschiff
								x = -(2 + bufferSpaces) * gapLeftRight;
								z = distance + ((2 + bufferSpaces + groupLine) * gapForwardBackward);
								break;
							case 4: //Rechts hinter dem linken Kommandoschiff
								x = bufferSpaces * gapLeftRight;
								z = distance  + ((2 + bufferSpaces + groupLine) * gapForwardBackward);
								break;
							case 5: //Links hinter dem rechten Kommandoschiff
								x = -bufferSpaces * gapLeftRight;
								z = distance + ((2 + bufferSpaces + groupLine) * gapForwardBackward);
								break;
							default:
								throw new Exception("Someone modified groupAndSide with a debugger...");
						}
					}
				}
				return FactionRotation(factionRotation) * new Vector3(x, height, z);
			}
		}
		/// <summary>
		/// Calculates the position of an individual entry inside a columns-formation.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		public static Vector3 ParallelColumnsFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerColumn = Math.Max(1, Mathf.RoundToInt(currentGapRatio * factorFor1To1GapRatio));
			int columns = (int)Math.Ceiling((double)count / shipsPerColumn);
			float x = (columns - 1) * gapLeftRight / 2 - index % columns * gapLeftRight;
			float z = distance - (shipsPerColumn - 1) * gapForwardBackward / 2 + (index / columns * gapForwardBackward);
			return FactionRotation(factionRotation) * new Vector3(x, height, z);
		}
		/// <summary>
		/// Calculates the position of an individual entry inside a commanded columns-formation.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		public static Vector3 CommandedParallelColumnsFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(2f * currentGapRatio * factorFor1To1GapRatio)); //Schiffe sind doppel so weit voneinander entfernt.
			int groups = Math.Max(1, Mathf.CeilToInt(count / (1f + 2f * shipsPerLine)));
			float x, z;
			if (index < groups) //Platziere Kommandoschiffe
			{
				x = (groups - 1) * 4 * gapLeftRight / 2f - index * 4 * gapLeftRight;
				z = distance;
			}
			else //Platziere Flotte
			{
				index -= groups;
				x = (2 * groups - 1) * 2 * gapLeftRight / 2f - index % (2 * groups) * 2 * gapLeftRight;
				z = distance + (1 + index / (2 * groups)) * gapForwardBackward;
			}
			return FactionRotation(factionRotation) * new Vector3(x, height, z);
		}
		/// <summary>
		/// Calculates the position of an individual entry inside a marching roman manipel formation.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		public static Vector3 ManipelBaseFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			return ManipelFormation(factionRotation, gapLeftRight, gapForwardBackward, count, index, distance, height, false);
		}
		/// <summary>
		/// Calculates the position of an individual entry inside an attacking roman manipel formation.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		public static Vector3 ManipelAttackFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height)
		{
			return ManipelFormation(factionRotation, gapLeftRight, gapForwardBackward, count, index, distance, height, true);
		}
		/// <summary>
		/// Calculates the position of an individual entry inside a marching or attacking roman manipel formation.
		/// </summary>
		/// <param name="factionRotation">The rotation angle for a team</param>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries.</param>
		/// <param name="index">The index of the current entry.</param>
		/// <param name="distance">The distance of the formation to the center of the battlefield.</param>
		/// <param name="height">The spawn-height of the entry.</param>
		/// <param name="attack">If true, the attacking formation will be used, otherwise the marching one.</param>
		/// <returns>The local spawnposition of an entry.</returns>
		private static Vector3 ManipelFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height, bool attack) {
			int groups = Mathf.CeilToInt(count / 6f);
			int line = index / groups;
			int group = index % groups;
			float x, z = distance + line * gapForwardBackward;
			switch (line) {
				case 0: //erste Reihe
				case 4: //fünfte Reihe
				case 5: //sechste Reihe
					x = (2 * groups - 1) * gapLeftRight / 2f - 2 * group * gapLeftRight;
					break;
				case 1: //zweite Reihe
					if (attack) //Fülle die Lücken der ersten Reihe auf.
					{
						z = distance;
						x = (2 * groups - 1) * gapLeftRight / 2f - (2 * group + 1) * gapLeftRight;
					}
					else //Marschiere hinter der ersten Reihe.
					{
						x = (2 * groups - 1) * gapLeftRight / 2f - 2 * group * gapLeftRight;
					}
					break;
				case 2: //dritte Reihe
				case 3: //vierte Reihe
					x = (2 * groups - 1) * gapLeftRight / 2f - (2 * group + 1) * gapLeftRight;
					break;
				default:
					throw new Exception("Someone modified line with a debugger...");
			}
			return FactionRotation(factionRotation) * new Vector3(x, height, z);
		}
	}
}