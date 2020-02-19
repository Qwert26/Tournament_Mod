using System;
using UnityEngine;
namespace TournamentMod.Formations
{
	public static class FormationPositionDescription
	{
		/// <summary>
		/// Für neue Formationen, die noch keine eigene Positions-Beschreibungen haben.
		/// </summary>
		/// <param name="gapLeftRight"></param>
		/// <param name="gapForwardBackward"></param>
		/// <param name="count"></param>
		/// <param name="index"></param>
		/// <returns></returns>
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
						return $"{(signLR == 1 ? "Right " : signLR == -1 ? "Left " : "")}{(signFB == 1 ? "Back " : signFB == -1 ? "Front " : "")}Scout";
					else if (index == count - 1)
						return $"{(signLR == 1 ? "Left " : signLR == -1 ? "Right " : "")}{(signFB == 1 ? "Front " : signFB == -1 ? "Back " : "")}Scout";
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
					return $"{(signLR == 1 ? "Right " : signLR == -1 ? "Left " : "")}{(signFB == 1 ? "Back " : signFB == -1 ? "Front " : "")}Scout";
				else if (index == count - 1)//index gerade
					return $"{(signLR == 1 ? "Left " : signLR == -1 ? "Right " : "")}{(signFB == 1 ? "Back " : signFB == -1 ? "Front " : "")}Scout";
				else
					return "None";
			}
			else
			{
				if (index == count - 1)//index ungerade
					return $"{(signLR == 1 ? "Right " : signLR == -1 ? "Left " : "")}{(signFB == 1 ? "Back " : signFB == -1 ? "Front " : "")}Scout";
				else if (index == count - 2)//index gerade
					return $"{(signLR == 1 ? "Left " : signLR == -1 ? "Right " : "")}{(signFB == 1 ? "Back " : signFB == -1 ? "Front " : "")}Scout";
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
					return $"{(signLR == 1 ? "Left " : signLR == -1 ? "Right " : "")}Commandship";
				else if (index == 2)
					return $"{(signLR == 1 ? "Right " : signLR == -1 ? "Left " : "")}Commandship";
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
		public static string ParallelColumns(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(currentGapRatio * FormationCalculation.factorFor1To1GapRatio));
			int lines = (int) Math.Ceiling((double) count / shipsPerLine);
			return $"Column {(index % lines) + 1}, Row {(index / lines) + 1}";
		}
		public static string ManipelBaseFormation(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			return ManipelFormation(gapLeftRight, gapForwardBackward, count, index, false);
		}
		public static string ManipelAttackFormation(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			return ManipelFormation(gapLeftRight, gapForwardBackward, count, index, true);
		}
		private static string ManipelFormation(float gapLeftRight, float gapForwardBackward, int count, int index,bool attack)
		{
			int groups = Mathf.CeilToInt(count / 6f);
			int line = index / groups;
			int group = index % groups;
			switch (line)
			{
				case 0:
				case 4:
				case 5:
					return $"Group {group}, Line {line+1}, left side of group";
				case 1:
					if (attack)
					{
						return $"Group {group}, Line {line}, right side of group";
					}
					else
					{
						return $"Group {group}, Line {line + 1}, left side of group";
					}
				case 2:
				case 3:
					return $"Group {group}, Line {line + 1}, right side of group";
				default:
					throw new Exception("Someone modified line with a debugger...");
			}
		}
	}
}