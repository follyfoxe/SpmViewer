using System;
using System.Text;
using System.IO;

namespace PM
{
    public class BigEndianReader : BinaryReader
    {
        public bool BigEndian { get; set; } = true;

        public BigEndianReader(Stream stream, Encoding encoding) : base(stream, encoding) { }

        byte[] Reverse(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                if (BigEndian)
                    Array.Reverse(bytes);
            else if (!BigEndian)
                Array.Reverse(bytes);
            return bytes;
        }

        public short ReadInt16BE() => BitConverter.ToInt16(Reverse(ReadBytes(2)), 0);
        public int ReadInt32BE() => BitConverter.ToInt32(Reverse(ReadBytes(4)), 0);
        public long ReadInt64BE() => BitConverter.ToInt64(Reverse(ReadBytes(8)), 0);

        public ushort ReadUInt16BE() => BitConverter.ToUInt16(Reverse(ReadBytes(2)), 0);
        public uint ReadUInt32BE() => BitConverter.ToUInt32(Reverse(ReadBytes(4)), 0);
        public ulong ReadUInt64BE() => BitConverter.ToUInt64(Reverse(ReadBytes(8)), 0);

        public float ReadSingleBE() => BitConverter.ToSingle(Reverse(ReadBytes(4)), 0);
        public double ReadDoubleBE() => BitConverter.ToDouble(Reverse(ReadBytes(8)), 0);

        public int[] ReadInts32BE(int count)
        {
            int[] ints = new int[count];
            for (int i = 0; i < count; i++)
                ints[i] = ReadInt32BE();
            return ints;
        }

        public uint[] ReadUInts32BE(int count)
        {
            uint[] uints = new uint[count];
            for (int i = 0; i < count; i++)
                uints[i] = ReadUInt32BE();
            return uints;
        }
    }
}