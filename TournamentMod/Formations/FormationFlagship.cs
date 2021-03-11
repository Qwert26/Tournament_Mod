using System;
using UnityEngine;
namespace TournamentMod.Formations
{
	public static class FormationFlagship
	{
		/// <summary>
		/// The default method for new formations or for formations which do not support fleetforming.
		/// </summary>
		/// <param name="_0">The distance along the local x-axis</param>
		/// <param name="_1">The distance along the local z-axis</param>
		/// <param name="_2">the entries in the formation</param>
		/// <param name="_3">the current entry</param>
		/// <returns>-1</returns>
		public static int UnknownFormation(float _0,float _1, int _2, int _3)
		{
			return -1;
		}
		/// <summary>
		/// The method for line-formations.
		/// </summary>
		/// <param name="_0">The distance along the local x-axis</param>
		/// <param name="_1">The distance along the local z-axis</param>
		/// <param name="count">the entries in the formation</param>
		/// <param name="index">the current entry</param>
		/// <returns>The center index</returns>
		public static int LineFormation(float _0, float _1, int count, int index)
		{
			if (count % 2 == 0)
			{
				if (index < count / 2)
				{
					return count / 2 - 1;
				}
				else
				{
					return count / 2;
				}
			}
			else
			{
				return count / 2;
			}
		}
		/// <summary>
		/// The method for wedge- and triangle-formations.
		/// </summary>
		/// <param name="_0">The distance along the local x-axis</param>
		/// <param name="_1">The distance along the local z-axis</param>
		/// <param name="_2">the entries in the formation</param>
		/// <param name="_3">the current entry</param>
		/// <returns>0</returns>
		public static int WedgeTriangleFormation(float _0, float _1, int _2, int _3)
		{
			return 0;
		}
		/// <summary>
		/// The method for the divided wedge-formation.
		/// </summary>
		/// <param name="_0">The distance along the local x-axis</param>
		/// <param name="_1">The distance along the local z-axis</param>
		/// <param name="count">the entries in the formation</param>
		/// <param name="index">the current entry</param>
		/// <returns>0, 1 or 2.</returns>
		public static int DividedWedgeFormation(float _0, float _1, int count, int index)
		{
			if (count <= 3)
			{
				return 0;
			}
			else
			{
				if (index < 3)
				{
					return index;
				}
				else
				{
					switch ((index - 3) % 6)
					{
						case 0:
						case 1:
							return 0;
						case 2:
						case 4:
							return 1;
						case 3:
						case 5:
							return 2;
						default:
							throw new Exception("Value for switch compromised!");
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis</param>
		/// <param name="count">the entries in the formation</param>
		/// <param name="index">the current entry</param>
		/// <returns></returns>
		public static int CommandedParallelColumnsFormation(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			float currentGapRatio = Mathf.Abs(gapLeftRight / (1 + Mathf.Abs(gapForwardBackward)));
			int shipsPerLine = Math.Max(1, Mathf.RoundToInt(2f * currentGapRatio * FormationCalculation.factorFor1To1GapRatio)); //Schiffe sind doppel so weit voneinander entfernt.
			int groups = Math.Max(1, Mathf.CeilToInt(count / (1f + 2f * shipsPerLine)));
			if (index < groups)
			{
				return index;
			}
			else
			{
				return ((index - groups) / 2) % groups;
			}
		}
	}
}