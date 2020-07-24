using System;
using System.Text;
using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
namespace TournamentMod.Serialisation
{
	public class StringList : VarList<string>
	{
		/// <summary>
		/// Converts a byte-array with UTF8-Encoding back into a string. The first index is a length indicator.
		/// </summary>
		/// <param name="bytes"></param>
		public override void ByteToEntry(byte[] bytes)
		{
			byte run = bytes[0];
			Add(Encoding.UTF8.GetString(bytes, 0, run));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryBytes">Will be 255 after the call. A longer Filepath can not be saved without exlcuding its length in an UTF8-Format.</param>
		/// <returns></returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 255;
			return (uint) Us.Count;
		}
		/// <summary>
		/// Saves the selected entry into the given byte array. If the string in UTF-Encoding is too long, it will be shortened in order to fit inside 254 bytes.
		/// </summary>
		/// <param name="keyIndex"></param>
		/// <param name="byteArray"></param>
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