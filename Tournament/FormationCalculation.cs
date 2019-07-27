using UnityEngine;
using System;
namespace Tournament
{
	internal static class FormationCalculation {
		static readonly float factorFor1To1GapRatio = Mathf.Tan(Mathf.Deg2Rad*68);
		public static Vector3 LineFormation(bool isKing, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			float x = (count - 1f) * gapLeftRight / 2f - index * gapLeftRight;
			float z = isKing ? (distance / 2f) + (index * gapForwardBackward) : (distance / -2f) - (index * gapForwardBackward);
			return new Vector3(x, height, z);
		}
		public static Vector3 WedgeFormation(bool isKing, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			if (index == 0) //Ist es das Flaggschiff?
			{
				return new Vector3(0, height, isKing ? (distance / 2f) : (distance / -2f));
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
				z = isKing ? (distance / 2f) + (1 + index) * gapForwardBackward : (distance / -2f) - (1 + index) * gapForwardBackward;
				return new Vector3(x, height, z);
			}
		}
		public static Vector3 DividedWedgeFormation(bool isKing, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			if (index == 0) //Ist es das Flaggschiff?
			{
				return new Vector3(0, height, isKing ? (distance / 2f) : (distance / -2f));
			}
			else
			{
				float x=0, z=0;
				if (count <= 3)
				{
					z = isKing ? (distance / 2f) + gapForwardBackward : (distance / -2f) - gapForwardBackward;
					if (index == 1)
					{
						x = gapLeftRight;
					}
					else //index=2
					{
						x = -gapLeftRight;
					}
				}
				else
				{
					count -= 3;
					count += 6 - (count % 6);
					//Anzahl der Plätze zwischen Flagschiff und Kommandoschiff
					int bufferSpaces = count / 3;
					if (index == 1)
					{
						x = (1 + bufferSpaces) * gapLeftRight;
						z = isKing ? (distance / 2f) + ((1 + bufferSpaces) * gapForwardBackward) : (distance / -2f) - ((1 + bufferSpaces) * gapForwardBackward);
					}
					else if (index == 2)
					{
						x = -(1 + bufferSpaces) * gapLeftRight;
						z = isKing ? (distance / 2f) + ((1 + bufferSpaces) * gapForwardBackward) : (distance / -2f) - ((1 + bufferSpaces) * gapForwardBackward);
					}
					else //Alle anderen Schiffe
					{
						int groupAndSide = (index - 3) % 6;
						int groupLine = (index - 3) / 6;
						switch (groupAndSide)
						{
							case 0:
								x = gapLeftRight;
								z = isKing ? (distance / 2f) + (groupLine * gapForwardBackward) : (distance / -2f) - (groupLine * gapForwardBackward);
								break;
							case 1:
								x = -gapLeftRight;
								z = isKing ? (distance / 2f) + (groupLine * gapForwardBackward) : (distance / -2f) - (groupLine * gapForwardBackward);
								break;
							case 2:
								x = (2 + bufferSpaces) * gapLeftRight;
								z = isKing ? (distance / 2f) + ((2 + bufferSpaces + groupLine) * gapForwardBackward) : (distance / -2f) - ((2 + bufferSpaces + groupLine) * gapForwardBackward);
								break;
							case 3:
								x = -(2 + bufferSpaces) * gapLeftRight;
								z = isKing ? (distance / 2f) + ((2 + bufferSpaces + groupLine) * gapForwardBackward) : (distance / -2f) - ((2 + bufferSpaces + groupLine) * gapForwardBackward);
								break;
							case 4:
								x = bufferSpaces * gapLeftRight;
								z = isKing ? (distance / 2f) + ((2 + bufferSpaces + groupLine) * gapForwardBackward) : (distance / -2f) - ((2 + bufferSpaces + groupLine) * gapForwardBackward);
								break;
							case 5:
								x = -bufferSpaces * gapLeftRight;
								z = isKing ? (distance / 2f) + ((2 + bufferSpaces + groupLine) * gapForwardBackward) : (distance / -2f) - ((2 + bufferSpaces + groupLine) * gapForwardBackward);
								break;
							default:
								throw new Exception("Someone modified groupAndSide with a debugger...");
						}
					}
				}
				return new Vector3(x,height, z);
			}
		}
		public static Vector3 ParallelColumns(bool isKing, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(currentGapRatio * factorFor1To1GapRatio));
			int lines = (int)Math.Ceiling((double)count / shipsPerLine);
			float x = (lines - 1) * gapLeftRight / 2f - index % lines * gapLeftRight;
			float z = isKing ? (distance / 2f) + (index / lines * gapForwardBackward) : (distance / -2f) - (index / lines * gapForwardBackward);
			return new Vector3(x, height, z);
		}
		public static Vector3 CommandedParallelColumns(bool isKing, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height) {
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(2f * currentGapRatio * factorFor1To1GapRatio)); //Schiffe sind doppel so weit voneinander entfernt.
			int groups = Math.Max(1, Mathf.CeilToInt(count / (1f + 2f * shipsPerLine)));
			float x, z;
			if (index < groups) //Platziere Kommandoschiffe
			{
				x = (groups - 1) * 4 * gapLeftRight / 2f - index * 4 * gapLeftRight;
				z = isKing ? (distance / 2f) : (distance / -2f);
			}
			else //Platziere Flotte
			{
				index -= groups;
				x = (2 * groups - 1) * 2 * gapLeftRight / 2f - index % (2 * groups) * 2 * gapLeftRight;
				z = isKing ? (distance / 2f) + (index / (2 * groups) * gapForwardBackward) : (distance / -2f) - (index / (2 * groups) * gapForwardBackward);
			}
			return new Vector3(x, height, z);
		}
	}
}