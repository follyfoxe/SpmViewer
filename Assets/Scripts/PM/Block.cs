using System;
using System.IO;
using UnityEngine;

namespace PM
{
    public static class BlockUtility
    {
        public const int Vector3Size = sizeof(float) * 3;

        public static string GetString(char[] chars)
        {
            return new string(chars).Trim('\0');
        }

        public static bool GetBool(uint val) => val > 0;

        public static Vector2 ParseVector2(BigEndianReader reader) => new(reader.ReadSingleBE(), reader.ReadSingleBE());
        public static Vector3 ParseVector3(BigEndianReader reader) => new(reader.ReadSingleBE(), reader.ReadSingleBE(), reader.ReadSingleBE());
        public static Color32 ParseColor32(BigEndianReader reader) => new(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

        public static T[] ParseAll<T>(Func<BigEndianReader, T> func, BigEndianReader reader, uint offset, uint count)
        {
            reader.BaseStream.Position = offset;

            T[] blocks = new T[count];
            for (uint i = 0; i < count; i++)
                blocks[i] = func(reader);
            return blocks;
        }

        public static T[] ParseAll<T>(BigEndianReader reader, uint offset, uint count) where T : struct
        {
            return ParseAll(r => (T)Activator.CreateInstance(typeof(T), r), reader, offset, count);
        }

        public static Texture2D TextureFromBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream stream = new();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            Texture2D tex = new(1, 1);
            tex.LoadImage(stream.ToArray());
            return tex;
        }
    }
}