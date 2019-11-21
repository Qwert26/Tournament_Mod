using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using UnityEngine;
using System;
namespace Tournament.Serialisation
{
    public class Vector2List : VarList<Vector2>
    {
        public override void ByteToEntry(byte[] bytes)
        {
            float x = BitConverter.ToSingle(bytes, 0);
            float y = BitConverter.ToSingle(bytes, 4);
            Add(new Vector2(x, y));
        }
        public override uint EntriesAndBytesPerEntry(out uint entryBytes)
        {
            entryBytes = 8;
            return (uint)Us.Count;
        }
        public override void EntryToByte(uint keyIndex, ref byte[] byteArray)
        {
            Vector2 v = Us[(int)keyIndex];
            byteArray = new byte[8];
            Array.Copy(BitConverter.GetBytes(v.x), 0, byteArray, 0, 4);
            Array.Copy(BitConverter.GetBytes(v.y), 0, byteArray, 4, 4);
        }
    }
}