using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using System;
namespace TournamentMod.Serialisation
{
	/// <summary>
	/// Stores a list of bools for usage with the prototype-system.
	/// </summary>
	[Obsolete("Speichergefahr", true)]
	public class BoolList : VarList<bool>
	{
		/// <summary>
		/// Converts an array of bytes into an entry and adds it to the list.
		/// </summary>
		/// <param name="bytes">The byte array to be converted. It must contain at least 1 byte.</param>
		public override void ByteToEntry(byte[] bytes)
		{
			Us.Add(CountBits(bytes[0]) > 4);
		}
		/// <summary>
		/// Counts the set bits in a byte
		/// </summary>
		/// <param name="b"></param>
		/// <returns>a number between 0 and 8.</returns>
		private static byte CountBits(byte b)
		{
			byte count = 0;
			while (b != 0)
			{
				count++;
				b &= (byte)(b - 1);
			}
			return count;
		}
		/// <summary>
		/// Determines the size of a single, generic entry in bytes as well as the size of the entire list.
		/// </summary>
		/// <param name="entryBytes">The size of an entry in bytes.</param>
		/// <returns>The current size of the list.</returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 1u;
			return (uint)Us.Count;
		}
		/// <summary>
		/// Converts the entry at a given position into an already given byte array.
		/// </summary>
		/// <param name="keyIndex">The index of the entry which should be converted into bytes.</param>
		/// <param name="byteArray">The byte array which should store the converted entry.</param>
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			byteArray = new byte[1] { Us[(int) keyIndex] ? byte.MaxValue : byte.MinValue };
		}
	}
}