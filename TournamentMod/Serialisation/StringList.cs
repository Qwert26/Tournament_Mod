using System;
using System.Text;
using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
namespace TournamentMod.Serialisation
{
	public class StringList : VarList<string>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytes"></param>
		public override void ByteToEntry(byte[] bytes)
		{
			Add(Encoding.UTF8.GetString(bytes).Trim());
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryBytes">Will be 260 after the call</param>
		/// <returns></returns>
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 260;
			return (uint) Us.Count;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyIndex"></param>
		/// <param name="byteArray"></param>
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			byte[] stringBytes = Encoding.UTF8.GetBytes(Us[(int) keyIndex]);
			Array.Copy(stringBytes, 0, byteArray, 0, Math.Min(260, stringBytes.Length));
		}
	}
}