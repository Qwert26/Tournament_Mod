using UnityEngine;
namespace Tournament
{
	internal static class FormationCalculation {
		static readonly float factorFor1To1GapRation = Mathf.Tan(Mathf.Deg2Rad*68);
		public static float CalculateYComponent(float offset, Tournament.SPAWN.LOC spawnLocation)
		{
			switch (spawnLocation)
			{
				case Tournament.SPAWN.LOC.Air:
					return 100 + offset;
				case Tournament.SPAWN.LOC.Sea:
					return 1 + offset;
				case Tournament.SPAWN.LOC.Sub:
					return offset - 20;
				case Tournament.SPAWN.LOC.Land:
					return 51 + offset;
				default:
					return offset;
			}
		}
		public static Vector3 LineFormation(bool isKing, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float offset, Tournament.SPAWN.LOC spawnLocation) {
			float x = (count - 1f) * gapLeftRight / 2f - index * gapLeftRight;
			float z = isKing ? (distance / 2f) + (index * gapForwardBackward) : (distance / -2f) - (index * gapForwardBackward);
			return new Vector3(x, CalculateYComponent(offset, spawnLocation), z);
		}
		public static Vector3 WedgeFormation(bool isKing, float gapLeftRight, float gapForwardBackward, int count, int index, float distance, float offset, Tournament.SPAWN.LOC spawnLocation) {
			if (index == 0) //Ist es das Flaggschiff?
			{
				return new Vector3(0, CalculateYComponent(offset, spawnLocation), isKing ? (distance / 2f) : (distance / -2f));
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
				return new Vector3(x, CalculateYComponent(offset, spawnLocation), z);
			}
		}
	}
}