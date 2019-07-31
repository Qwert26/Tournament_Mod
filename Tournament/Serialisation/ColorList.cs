using BrilliantSkies.Core.Serialisation.Parameters.Prototypes;
using UnityEngine;
using System;
namespace Tournament.Serialisation
{
    public class ColorList : VarList<Color>
    {
        public override void ByteToEntry(byte[] bytes)
        {
            float r, g, b, a;
            r = BitConverter.ToSingle(bytes, 0);
            g = BitConverter.ToSingle(bytes, 4);
            b = BitConverter.ToSingle(bytes, 8);
            a = BitConverter.ToSingle(bytes, 12);
            Us.Add(new Color(r, g, b, a));
        }
        public override uint EntriesAndBytesPerEntry(out uint entryBytes)
        {
            entryBytes = 4 * 4;
            return (uint)Us.Count;
        }
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
