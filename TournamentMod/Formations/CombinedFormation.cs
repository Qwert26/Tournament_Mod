using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BrilliantSkies.Core.Types;
namespace TournamentMod.Formations
{
	public class CombinedFormation
	{
		public List<Tuple<FormationType,int>> formationEntrycount;
		public CombinedFormation() {
			formationEntrycount = new List<Tuple<FormationType,int>>
			{
				new Tuple<FormationType,int>(FormationType.Line, int.MaxValue)
			};
		}
		public Vector4i[] Export(int teamIndex)
		{
			Vector4i[] ret = new Vector4i[formationEntrycount.Count];
			for (int index = 0; index < ret.Length; index++)
			{
				ret[index] = new Vector4i(teamIndex, index, (int)formationEntrycount[index].Item1, formationEntrycount[index].Item2);
			}
			return ret;
		}
		public void Import(Vector4i v4i)
		{
			formationEntrycount.Add(new Tuple<FormationType, int>((FormationType)v4i.z, v4i.w));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="factionRotation"></param>
		/// <param name="gapLeftRight"></param>
		/// <param name="gapForwardBackward"></param>
		/// <param name="count"></param>
		/// <param name="index"></param>
		/// <param name="distance"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public Vector3 DetermineLocalPosition(float factionRotation, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float height)
		{
			int formationSpecificIndex = index, formationSpecificCount = count;
			foreach (Tuple<FormationType,int> indexAndCount in formationEntrycount)
			{
				if (indexAndCount.Item2 < formationSpecificIndex+1) //Teilnehmer zu spät für Formation.
				{
					formationSpecificCount -= indexAndCount.Item2;
					formationSpecificIndex -= indexAndCount.Item2;
					distance += indexAndCount.Item1.getFormation().DetermineSize(gapLeftRight, gapForwardBackward, indexAndCount.Item2).y + gapForwardBackward;
				}
				else
				{
					return indexAndCount.Item1.getFormation().DetermineLocalPosition(factionRotation, gapLeftRight, gapForwardBackward, indexAndCount.Item2, formationSpecificIndex, distance, height);
				}
			}
			//Die letzte Formation gilt als Fänger.
			Tuple<FormationType, int> last = formationEntrycount.Last();
			return last.Item1.getFormation().DetermineLocalPosition(factionRotation, gapLeftRight, gapForwardBackward, formationSpecificCount, formationSpecificIndex, distance, height);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gapLeftRight"></param>
		/// <param name="gapForwardBackward"></param>
		/// <param name="count"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public string DeterminePositionDescription(float gapLeftRight, float gapForwardBackward, int count, int index)
		{
			int formationSpecificIndex = index, formationSpecificCount = count;
			foreach (Tuple<FormationType,int> indexAndCount in formationEntrycount)
			{
				if (indexAndCount.Item2 < formationSpecificIndex+1) //Teilnehmer zu spät für Formation.
				{
					formationSpecificCount -= indexAndCount.Item2;
					formationSpecificIndex -= indexAndCount.Item2;
				}
				else
				{
					return indexAndCount.Item1.getFormation().DeterminePositionDescription(gapLeftRight, gapForwardBackward, indexAndCount.Item2, formationSpecificIndex);
				}
			}
			//Die letzte Formation gilt als Fänger.
			Tuple<FormationType, int> last = formationEntrycount.Last();
			return last.Item1.getFormation().DeterminePositionDescription(gapLeftRight, gapForwardBackward, formationSpecificCount, formationSpecificIndex);
		}
	}
}