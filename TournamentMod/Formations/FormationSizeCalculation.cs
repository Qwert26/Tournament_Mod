using System;
using UnityEngine;
namespace TournamentMod.Formations
{
	/// <summary>
	/// Contains functions for size calculations.
	/// </summary>
	public static class FormationSizeCalculation
	{
		public static Vector2 Abs(this Vector2 v2) {
			return new Vector2(Mathf.Abs(v2.x), Mathf.Abs(v2.y));
		}
		public static Vector2 LineFormation(float gapLeftRight, float gapForwardBackward, int count)
		{
			return (new Vector2(gapLeftRight, gapForwardBackward) * count).Abs();
		}
		public static Vector2 GuardLineFormation(float gapLeftRight, float _1, int count)
		{
			return new Vector2(gapLeftRight * count, 0).Abs();
		}
		public static Vector2 WedgeFormation(float gapLeftRight, float gapForwardBackward, int count)
		{
			Vector2 ret = new Vector2(gapLeftRight, gapForwardBackward);
			if (count > 1)
			{
				ret.x *= count;
				count /= 2;
				ret.y *= count + 1;
			}
			return ret.Abs();
		}
		public static Vector2 DividedWedgeFormation(float gapLeftRight, float gapForwardBackward, int count)
		{
			if (count <= 3)
			{
				return WedgeFormation(gapLeftRight, gapForwardBackward, count);
			}
			else
			{
				int groupLines = (count - 3) / 6;
				count -= 3;
				if (count % 6 != 0)
				{
					count += 6 - (count % 6);
				}
				int bufferspaces = count / 3;
				float width = gapLeftRight * (4 + 2 * bufferspaces);
				float height = gapForwardBackward * (2 + bufferspaces + groupLines);
				return new Vector2(width, height).Abs();
			}
		}
		public static Vector2 ParallelColumnsFormation(float gapLeftRight, float gapForwardBackward, int count)
		{
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerColumn = Math.Max(1, Mathf.RoundToInt(currentGapRatio * FormationCalculation.factorFor1To1GapRatio));
			int columns = (int) Math.Ceiling((double) count / shipsPerColumn);
			return new Vector2(gapLeftRight * columns, gapForwardBackward * shipsPerColumn).Abs();
		}
		public static Vector2 CommandedParallelColumnsFormation(float gapLeftRight, float gapForwardBackward, int count)
		{
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(2f * currentGapRatio * FormationCalculation.factorFor1To1GapRatio)); //Schiffe sind doppel so weit voneinander entfernt.
			int groups = Math.Max(1, Mathf.CeilToInt(count / (1f + 2f * shipsPerLine)));
			return new Vector2(gapLeftRight * 4 * groups, gapForwardBackward * (1 + 2 * groups)).Abs();
		}
		public static Vector2 ManipelFormation(float gapLeftRight, float gapForwardBackward, int count)
		{
			int groups = Mathf.CeilToInt(count / 6f);
			int lastLine = (count - 1) / groups;
			return new Vector2(gapLeftRight * groups * 2, gapForwardBackward * lastLine).Abs();
		}
	}
}