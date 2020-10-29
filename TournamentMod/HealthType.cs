using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace TournamentMod
{
	public enum HealthType : int
	{
		Blockcount = 0,
		Resourcecost = 1,
		Volume = 2,
		ArrayElements = 3
	}
	public static class HealthTypeExtensions
	{
		public static string DescribeHealthtype(this HealthType healthType)
		{
			switch (healthType)
			{
				case HealthType.Blockcount:
					return "Blockcount";
				case HealthType.Resourcecost:
					return "Materialcost";
				case HealthType.Volume:
					return "Volume";
				case HealthType.ArrayElements:
					return "Array-Elements";
				default:
					return "How did you manage to go out of bounds here?";
			}
		}
	}
}