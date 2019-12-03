using System;
using UnityEngine;
namespace Tournament
{
	internal static class FormationPositionDescription
	{
		public static string UnknownFormation(float gapLeftRight=0, float gapForwardBackward=0, int count=0, int index=0) {
			return "None";
		}
		public static string LineFormation(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			int signLR = Math.Sign(Mathf.RoundToInt(gapLeftRight));
			int signFB = Math.Sign(Mathf.RoundToInt(gapForwardBackward));
			switch (count) {
				case 0:
					return "None";
				case 1:
					return "Flagship";
				case 2:
					if (index == 0)
						return "Flagship A";
					else
						return "Flagship B";
				default:
					if (index == 0)
						return $"{(signLR == 1 ? "Right" : signLR == -1 ? "Left" : "")} {(signFB == 1 ? "Back" : signFB == -1 ? "Front" : "")} Scout".Trim();
					else if (index == count - 1)
						return $"{(signLR == 1 ? "Left" : signLR == -1 ? "Right" : "")} {(signFB == 1 ? "Front" : signFB == -1 ? "Back" : "")} Scout".Trim();
					else if (count % 2 == 0)
					{
						if (index == count / 2 - 1)
							return "Flagship A";
						else if (index == count / 2)
							return "Flagship B";
					}
					else
					{
						if (index == count / 2)
							return "Flagship";
					}
					return "None";
			}
		}
		public static string WedgeFormation(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			int signLR = Math.Sign(Mathf.RoundToInt(gapLeftRight));
			int signFB = Math.Sign(Mathf.RoundToInt(gapForwardBackward));
			if (index == 0)
				return "Flagship";
			else if (count == 2)
				return "Scout";
			else if (count % 2 == 1)
			{
				if (index == count - 2)//index ungerade
					return $"{(signLR == 1 ? "Right" : signLR == -1 ? "Left" : "")} {(signFB == 1 ? "Back" : signFB == -1 ? "Front" : "")} Scout".Trim();
				else if (index == count - 1)//index gerade
					return $"{(signLR == 1 ? "Left" : signLR == -1 ? "Right" : "")} {(signFB == 1 ? "Back" : signFB == -1 ? "Front" : "")} Scout".Trim();
				else
					return "None";
			}
			else
			{
				if (index == count - 1)//index ungerade
					return $"{(signLR == 1 ? "Right" : signLR == -1 ? "Left" : "")} {(signFB == 1 ? "Back" : signFB == -1 ? "Front" : "")} Scout".Trim();
				else if (index == count - 2)//index gerade
					return $"{(signLR == 1 ? "Left" : signLR == -1 ? "Right" : "")} {(signFB == 1 ? "Back" : signFB == -1 ? "Front" : "")} Scout".Trim();
				else
					return "None";
			}
		}
		public static string DividedWedgeFormation(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			int signLR = Math.Sign(Mathf.RoundToInt(gapLeftRight));
			//int signFB = Math.Sign(Mathf.RoundToInt(gapForwardBackward));
			if (index == 0)
				return "Flagship";
			else if (count >= 4)
			{
				if (index == 1)
					return $"{(signLR == 1 ? "Left" : signLR == -1 ? "Right" : "")} Commandship".Trim();
				else if (index == 2)
					return $"{(signLR == 1 ? "Right" : signLR == -1 ? "Left" : "")} Commandship".Trim();
				else
					return "None";
			}
			else
				return "None";
		}
		public static string CommandedParallelColumns(float gapLeftRight, float gapForwardBackward, int count, int index) {
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(2f * currentGapRatio * FormationCalculation.factorFor1To1GapRatio)); //Schiffe sind doppel so weit voneinander entfernt.
			int groups = Math.Max(1, Mathf.CeilToInt(count / (1f + 2f * shipsPerLine)));
			if (index < groups)
			{
				return $"Commandship {index + 1}";
			}
			else
				return "None";
		}
	}
}