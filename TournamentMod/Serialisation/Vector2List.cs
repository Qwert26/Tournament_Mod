using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using UnityEngine;
using System;
namespace TournamentMod.Serialisation
{
	/// <summary>
	/// Stores a list of Vector2 for the prototype system.
	/// </summary>
	public class Vector2List : VarList<Vector2>
	{
		/// <summary>
		/// Convertes a byte-array into a vector2 and adds it to the list.
		/// </summary>
		/// <param name="bytes">the byte array which needs to contain at least 8 entries.</param>
		public override void ByteToEntry(byte[] bytes)
		{
			float x = BitConverter.ToSingle(bytes, 0);
			float y = BitConverter.ToSingle(bytes, 4);
			Add(new Vector2(x, y));
		}
		/// <summary>
		/// Computes the number of entries in the list and uses the parameter to give out the size of each entry.
		/// </summary>
		/// <param name="entryBytes">will be 8 after the call</param>
		/// <returns>the size of the list</returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 8;
			return (uint)Us.Count;
		}
		/// <summary>
		/// Converts the entry at the given index and stores it into the given byte array
		/// </summary>
		/// <param name="keyIndex">Index of the Vector2 to convert.</param>
		/// <param name="byteArray">The byte array to copy the converted entry into.</param>
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			Vector2 v = Us[(int)keyIndex];
			byteArray = new byte[8];
			Array.Copy(BitConverter.GetBytes(v.x), 0, byteArray, 0, 4);
			Array.Copy(BitConverter.GetBytes(v.y), 0, byteArray, 4, 4);
		}
	}
}