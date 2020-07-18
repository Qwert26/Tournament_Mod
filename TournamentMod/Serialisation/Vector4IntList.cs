using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using System;
using BrilliantSkies.Core.Types;
namespace TournamentMod.Serialisation
{
	/// <summary>
	/// Stores a list of Vector4I for the prototype system.
	/// </summary>
	public class Vector4IntList : VarList<Vector4i>
	{
		/// <summary>
		/// Converts a byte-array into a Vector4i and adds it to the list.
		/// </summary>
		/// <param name="bytes">The byte-array which must contain at least 16 bytes.</param>
		public override void ByteToEntry(byte[] bytes)
		{
			Vector4i entry;
			entry.x = BitConverter.ToInt32(bytes, 0);
			entry.y = BitConverter.ToInt32(bytes, 4);
			entry.z = BitConverter.ToInt32(bytes, 8);
			entry.w = BitConverter.ToInt32(bytes, 12);
			Add(entry);
		}
		/// <summary>
		/// Computes the number of entries in the list and uses the parameter to give out the size of each entry.
		/// </summary>
		/// <param name="entryBytes">Will be 16 after the call.</param>
		/// <returns>the size of the list</returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 16;
			return (uint)Us.Count;
		}
		/// <summary>
		/// Converts a single entry into a byte-array which has at least 16 bytes.
		/// </summary>
		/// <param name="keyIndex">The index of the entry to convert.</param>
		/// <param name="byteArray">The array to store the entry in, needs to have at least 16 bytes.</param>
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			Vector4i entry = Us[(int)keyIndex];
			Array.Copy(BitConverter.GetBytes(entry.x), 0, byteArray, 0, 4);
			Array.Copy(BitConverter.GetBytes(entry.y), 0, byteArray, 4, 4);
			Array.Copy(BitConverter.GetBytes(entry.z), 0, byteArray, 8, 4);
			Array.Copy(BitConverter.GetBytes(entry.w), 0, byteArray, 12, 4);
		}
	}
}