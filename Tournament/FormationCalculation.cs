using UnityEngine;
using System;
namespace Tournament
{
	internal static class FormationCalculation {
		public static readonly float factorFor1To1GapRatio = Mathf.Tan(Mathf.Deg2Rad*68);
		public static Quaternion FactionRotation(float angle) {
			return Quaternion.Euler(0, angle, 0);
		}
		public static Vector3 LineFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			float x = (count - 1) * gapLeftRight / 2f - index * gapLeftRight;
			float z = distance + (count - 1) * gapForwardBackward / 2f - index * gapForwardBackward;
			return FactionRotation(factionRotation)*new Vector3(x, height, z);
		}
		public static Vector3 WedgeFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			if (index == 0) //Ist es das Flaggschiff?
			{
				return FactionRotation(factionRotation) * new Vector3(0, height,distance);
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
		public static Vector3 DividedWedgeFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			if (index == 0) //Ist es das Flaggschiff?
			{
				return FactionRotation(factionRotation) * new Vector3(0, height, distance);
			}
			else
			{
				float x = 0, z = 0;
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
		public static Vector3 ParallelColumns(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(currentGapRatio * factorFor1To1GapRatio));
			int lines = (int)Math.Ceiling((double)count / shipsPerLine);
			float x = (lines - 1) * gapLeftRight / 2f - index % lines * gapLeftRight;
			float z = distance + (index / lines * gapForwardBackward);
			return FactionRotation(factionRotation) * new Vector3(x, height, z);
		}
		public static Vector3 CommandedParallelColumns(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
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
		public static Vector3 ManipelBaseFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			return ManipelFormation(factionRotation, gapLeftRight, gapForwardBackward, count, index, distance, height, false);
		}
		public static Vector3 ManipelAttackFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height)
		{
			return ManipelFormation(factionRotation, gapLeftRight, gapForwardBackward, count, index, distance, height, true);
		}
		private static Vector3 ManipelFormation(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height, bool attack) {
			int groups = Mathf.CeilToInt(count / 6f);
			int line = index / groups;
			int group = index % groups;
			float x, z = distance + line * gapForwardBackward;
			switch (line) {
				case 0:
				case 4:
				case 5:
					x = (2 * groups - 1) * gapLeftRight / 2f - 2 * group * gapLeftRight;
					break;
				case 1:
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
				case 2:
				case 3:
					x = (2 * groups - 1) * gapLeftRight / 2f - (2 * group + 1) * gapLeftRight;
					break;
				default:
					throw new Exception("Someone modified line with a debugger...");
			}
			return FactionRotation(factionRotation) * new Vector3(x, height, z);
		}
	}
}