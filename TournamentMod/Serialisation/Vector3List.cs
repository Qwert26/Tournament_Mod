using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using UnityEngine;
using System;
namespace TournamentMod.Serialisation
{
	public class Vector3List : VarList<Vector3>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		public override void ByteToEntry(byte[] bytes)
		{
			Vector3 entry;
			entry.x = BitConverter.ToSingle(bytes, 0);
			entry.y = BitConverter.ToSingle(bytes, 4);
			entry.z = BitConverter.ToSingle(bytes, 8);
			Add(entry);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryBytes"></param>
		/// <returns></returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 12;
			return (uint) Us.Count;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyIndex"></param>
		/// <param name="byteArray"></param>
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			Vector3 entry = Us[(int) keyIndex];
			Array.Copy(BitConverter.GetBytes(entry.x), 0, byteArray, 0, 4);
			Array.Copy(BitConverter.GetBytes(entry.y), 0, byteArray, 4, 4);
			Array.Copy(BitConverter.GetBytes(entry.z), 0, byteArray, 8, 4);
		}
	}
}