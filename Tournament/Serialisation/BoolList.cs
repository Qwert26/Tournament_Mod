using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
namespace Tournament.Serialisation
{
	public class BoolList : VarList<bool>
	{
		public override void ByteToEntry(byte[] bytes)
		{
			Us.Add(bytes[0] != 0);
		}
		public override uint EntriesAndBytesPerEntry(out uint entryBytes)
		{
			entryBytes = 1u;
			return (uint)Us.Count;
		}
		public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
		{
			byteArray = new byte[1] { (byte)(Us[(int)keyIndex] ? 255 : 0) };
		}
	}
}