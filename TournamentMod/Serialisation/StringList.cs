using System;
using System.Text;
using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
namespace TournamentMod.Serialisation
{
	[Obsolete("Can only store between a single 254 byte long encoded string and 127 single ascii-character \"string\"s. Not very useful in any case.", true)]
	public class StringList : VarList<string>
	{
		/// <summary>
		/// Converts a byte-array with UTF8-Encoding back into a string. The first index is a length indicator, so the string starts at the second index.
		/// </summary>
		/// <param name="bytes">The length of the string is in the first byte, afterwards the string itself in UTF8-encoding.</param>
		public override void ByteToEntry(byte[] bytes)
		{
			byte run = bytes[0];
			Add(Encoding.UTF8.GetString(bytes, 1, run));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryBytes">Will be 255 after the call, which is one byte for the length and 254 data-bytes.
		/// A longer Filepath can not be saved without exlcuding its length in an UTF8-Format.</param>
		/// <returns>Amount of strings in the list.</returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 255;
			return (uint) Us.Count;
		}
		/// <summary>
		/// Saves the selected entry into the given byte array. If the string in UTF-Encoding is too long, it will be shortened in order to fit inside 254 bytes.
		/// </summary>
		/// <param name="keyIndex">The indes of the string to save</param>
		/// <param name="byteArray">The array to write the string to.</param>
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			string toSave = Us[(int)keyIndex];
			if (Encoding.UTF8.GetByteCount(toSave) > 254)
			{
				int lastSaveableIndex = 0;
				for (int i = 0; i < toSave.Length; i++)
				{
					lastSaveableIndex += Encoding.UTF8.GetByteCount(new char[1] { toSave[i] });
					if (lastSaveableIndex > 254)
					{
						toSave = toSave.Substring(0, i);
						break;
					}
				}
			}
			byte[] stringBytes = Encoding.UTF8.GetBytes(toSave);
			byteArray[0] = (byte) stringBytes.Length;
			Array.Copy(stringBytes, 0, byteArray, 1, byteArray[0]);
		}
	}
}