using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using UnityEngine;
using System;
namespace TournamentMod.Serialisation
{
	/// <summary>
	/// Stores a list of colors for usage with the prototype-system.
	/// </summary>
	public class ColorList : VarList<Color>
	{
		/// <summary>
		/// Converts an array of bytes into an entry and adds it to the list.
		/// </summary>
		/// <param name="bytes">The byte array to be converted. It must contain at least 16 bytes.</param>
		public override void ByteToEntry(byte[] bytes)
		{
			float r, g, b, a;
			r = BitConverter.ToSingle(bytes, 0);
			g = BitConverter.ToSingle(bytes, 4);
			b = BitConverter.ToSingle(bytes, 8);
			a = BitConverter.ToSingle(bytes, 12);
			Us.Add(new Color(r, g, b, a));
		}
		/// <summary>
		/// Determines the size of a single, generic entry in bytes as well as the size of the entire list.
		/// </summary>
		/// <param name="entryBytes">>The size of an entry in bytes.</param>
		/// <returns>The current size of the list.</returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 4 * 4;
			return (uint)Us.Count;
		}
		/// <summary>
		/// Converts the entry at a given position into an already given byte array.
		/// </summary>
		/// <param name="keyIndex">The index of the entry which should be converted into bytes.</param>
		/// <param name="byteArray">The byte array which should store the converted entry.</param>
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			Color c = Us[(int)keyIndex];
			byte[] redBytes = BitConverter.GetBytes(c.r);
			byte[] greenBytes = BitConverter.GetBytes(c.g);
			byte[] blueBytes = BitConverter.GetBytes(c.b);
			byte[] alphaBytes = BitConverter.GetBytes(c.a);
			byteArray = new byte[16];
			Array.Copy(redBytes, 0, byteArray, 0, 4);
			Array.Copy(greenBytes, 0, byteArray, 4, 4);
			Array.Copy(blueBytes, 0, byteArray, 8, 4);
			Array.Copy(alphaBytes, 0, byteArray, 12, 4);
		}
	}
}
