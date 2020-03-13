using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using System;
using BrilliantSkies.Core.Types;
namespace TournamentMod.Serialisation
{
	public class Vector4IntList : VarList<Vector4i>
	{
		public override void ByteToEntry(byte[] bytes)
		{
			Vector4i entry;
			entry.x = BitConverter.ToInt32(bytes, 0);
			entry.y = BitConverter.ToInt32(bytes, 4);
			entry.z = BitConverter.ToInt32(bytes, 8);
			entry.w = BitConverter.ToInt32(bytes, 12);
			Add(entry);
		}
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 4 * 4;
			return (uint)Us.Count;
		}
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