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
			formationEntrycount = new List<Tuple<FormationType, int>>
			{
				new Tuple<FormationType, int>(FormationType.Line, 0)
			};
		}
		/// <summary>
		/// Exports the entire list into an array of Vector4i.
		/// </summary>
		/// <param name="teamIndex">The future x-value for the Vector4i, used for sorting when loading in.</param>
		/// <returns></returns>
		public Vector4i[] Export(int teamIndex)
		{
			Vector4i[] ret = new Vector4i[formationEntrycount.Count];
			for (int index = 0; index < ret.Length; index++)
			{
				ret[index] = new Vector4i(teamIndex, index, (int)formationEntrycount[index].Item1, formationEntrycount[index].Item2);
			}
			return ret;
		}
		/// <summary>
		/// Imports a Vector4i, it assumes that the original list was already sorted by teamindex and position of the formation.
		/// </summary>
		/// <param name="v4i"></param>
		public void Import(Vector4i v4i)
		{
			formationEntrycount.Add(new Tuple<FormationType, int>((FormationType)v4i.z, v4i.w));
		}
		/// <summary>
		/// Determines the local position of an entry using the information provided, as well as its internal list of formations and entry-counts.
		/// </summary>
		/// <param name="factionRotation">The final Rotationangle to apply after the position has been determined</param>
		/// <param name="gapLeftRight">The distance along the local x-axis</param>
		/// <param name="gapForwardBackward">The distance along the local z-axis</param>
		/// <param name="count">The amount of total entries in the team</param>
		/// <param name="index">The index of the current entry</param>
		/// <param name="distance">The distance from the choosen center</param>
		/// <param name="height">the set height of the entry</param>
		/// <returns>The spawn position for the entry</returns>
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
		/// Determines the role of an entry inside the formation it will fall into.
		/// </summary>
		/// <param name="gapLeftRight">The distance along the local x-axis</param>
		/// <param name="gapForwardBackward">the distance along the local z-axis</param>
		/// <param name="count">The amount of entries in the team</param>
		/// <param name="index">The index of the current entry</param>
		/// <returns>The role description</returns>
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