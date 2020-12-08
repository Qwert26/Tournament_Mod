using System;
using UnityEngine;
namespace TournamentMod.Formations
{
	/// <summary>
	/// Contains functions for positional descriptions
	/// </summary>
	public static class FormationPositionDescription
	{
		/// <summary>
		/// For new Formation, which don't have descriptions yet.
		/// </summary>
		/// <param name="_1">The distance along the local x-axis.</param>
		/// <param name="_2">The distance along the local z-axis.</param>
		/// <param name="_3">The amount of entries inside the formation</param>
		/// <param name="_4">The current entry.</param>
		/// <returns>A description of the current position.</returns>
		public static string UnknownFormation(float _1 = 0, float _2 = 0, int _3 = 0, int _4 = 0)
		{
			return "None";
		}
		/// <summary>
		/// Gives out upto 4 unique position descriptions.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
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
		/// <summary>
		/// Uses the line-formation with a forward-backward-gap of 0.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="_1">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
		public static string GuardLineFormation(float gapLeftRight, float _1, int count, int index)
		{
			return LineFormation(gapLeftRight, 0, count, index);
		}
		/// <summary>
		/// Gives out upto 3 unique position descriptions.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
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
		/// <summary>
		/// Gives out upto 3 unique position descriptions.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="_1">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
		public static string DividedWedgeFormation(float gapLeftRight, float _1, int count, int index)
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
		/// <summary>
		/// Marks the right entries as "Commandships" of column-pairs.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
		public static string CommandedParallelColumnsFormation(float gapLeftRight, float gapForwardBackward, int count, int index) {
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(2f * currentGapRatio * FormationCalculation.factorFor1To1GapRatio)); //Schiffe sind doppel so weit voneinander entfernt.
			int groups = Math.Max(1, Mathf.CeilToInt(count / (1f + 2f * shipsPerLine)));
			int lines = (int) Math.Ceiling((double) (count - groups) / shipsPerLine);
			if (index < groups)
			{
				return $"Commandship {index + 1}";
			}
			else
				return $"Column {((index - groups) % lines) + 1}, Row {((index - groups) / lines) + 1}";
		}
		/// <summary>
		/// Gives each entry a column and row position.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis.</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
		public static string ParallelColumnsFormation(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(currentGapRatio * FormationCalculation.factorFor1To1GapRatio));
			int lines = (int) Math.Ceiling((double) count / shipsPerLine);
			return $"Column {(index % lines) + 1}, Row {(index / lines) + 1}";
		}
		/// <summary>
		/// Gives each entry a Group, a Line and a Side descriptions.
		/// </summary>
		/// <param name="_1">The distance along the local x-axis.</param>
		/// <param name="_2">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
		public static string ManipelBaseFormation(float _1, float _2, int count, int index)
		{
			return ManipelFormation(count, index, false);
		}
		/// <summary>
		/// Gives each entry a Group, a Line and a Side descriptions.
		/// </summary>
		/// <param name="_1">The distance along the local x-axis.</param>
		/// <param name="_2">The distance along the local z-axis.</param>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
		public static string ManipelAttackFormation(float _1, float _2, int count, int index)
		{
			return ManipelFormation(count, index, true);
		}
		/// <summary>
		/// Gives each entry a Group, a Line and a Side descriptions.
		/// </summary>
		/// <param name="count">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <param name="attack">If true, the attacking version will be used, otherwise the marching one.</param>
		/// <returns>A description of the current position.</returns>
		private static string ManipelFormation(int count, int index, bool attack)
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
		/// <summary>
		/// Marks the Flagship in a triangle-like formation.
		/// </summary>
		/// <param name="_1">The distance along the local x-axis.</param>
		/// <param name="_2">The distance along the local z-axis.</param>
		/// <param name="_3">The amount of entries inside the formation</param>
		/// <param name="index">The current entry.</param>
		/// <returns>A description of the current position.</returns>
		public static string Triangle(float _1, float _2, int _3, int index)
		{
			if (index == 0)
			{
				return "Flagship";
			}
			else
			{
				int group = 1;
				for (int s = 1; s < index - 1; s++)
				{
					group++;
					index -= s;
				}
				return $"Line-Group {group}";
			}
		}
	}
}