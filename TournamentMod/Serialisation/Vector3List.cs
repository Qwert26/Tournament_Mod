using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using UnityEngine;
using System;
namespace TournamentMod.Serialisation
{
	/// <summary>
	/// Stores a List of Vector3 for the prototype system.
	/// </summary>
	[Obsolete("Can only store 21 vectors",true)]
	public class Vector3List : VarList<Vector3>
	{
		/// <summary>
		/// Converts a byte-array into a Vector3 and adds it.
		/// </summary>
		/// <param name="bytes">The byte-array which must contain at least 12 bytes.</param>
		public override void ByteToEntry(byte[] bytes)
		{
			Vector3 entry;
			entry.x = BitConverter.ToSingle(bytes, 0);
			entry.y = BitConverter.ToSingle(bytes, 4);
			entry.z = BitConverter.ToSingle(bytes, 8);
			Add(entry);
		}
		/// <summary>
		/// Computes the number of entries in the list and uses the parameter to give out the size of each entry.
		/// </summary>
		/// <param name="entryBytes">The amount of bytes to store a complete Vector3. Will be 12 after the call.</param>
		/// <returns>the size of the list</returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 12;
			return (uint) Us.Count;
		}
		/// <summary>
		/// Converts a single entry into a byte-array which has at least 12 bytes.
		/// </summary>
		/// <param name="keyIndex">The index of the entry to convert.</param>
		/// <param name="byteArray">The array to store the entry in, needs to have at least 12 bytes.</param>
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			Vector3 entry = Us[(int) keyIndex];
			Array.Copy(BitConverter.GetBytes(entry.x), 0, byteArray, 0, 4);
			Array.Copy(BitConverter.GetBytes(entry.y), 0, byteArray, 4, 4);
			Array.Copy(BitConverter.GetBytes(entry.z), 0, byteArray, 8, 4);
		}
	}
}