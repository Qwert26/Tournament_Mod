using System;
using System.Collections.Generic;
using UnityEngine;
namespace TournamentMod.Formations
{
	public class CombinedFormation
	{
		private List<Vector2Int> formationIndexStartIndex;
		public CombinedFormation() {
			formationIndexStartIndex = new List<Vector2Int>
			{
				new Vector2Int(0, 0)
			};
		}
	}
}