using Pfim;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Runtime.CompilerServices;

namespace AcSaveConverterImGui.Graphics
{
    internal static class TextureConverter
    {
        // https://github.com/nickbabcock/Pfim/blob/master/src/Pfim.ImageSharp/Program.cs#L22
        private static Image<Bgra32> ToImageSharp(IImage pfimage)
        {
            byte[] newData;

            // Since image sharp can't handle data with line padding in a stride
            // we create an stripped down array if any padding is detected
            var tightStride = pfimage.Width * pfimage.BitsPerPixel / 8;
            if (pfimage.Stride != tightStride)
            {
                newData = new byte[pfimage.Height * tightStride];
                for (int i = 0; i < pfimage.Height; i++)
                {
                    Buffer.BlockCopy(pfimage.Data, i * pfimage.Stride, newData, i * tightStride, tightStride);
                }
            }
            else
            {
                newData = pfimage.Data;
            }


            return Image.LoadPixelData<Bgra32>(newData, pfimage.Width, pfimage.Height);
        }

        private static unsafe IImage LoadPfim(ReadOnlySpan<byte> buffer)
        {
            fixed (byte* ptr = buffer)
            {
                using UnmanagedMemoryStream stream = new(ptr, buffer.Length);
                return Pfimage.FromStream(stream);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image LoadImageSharp(byte[] bytes)
            => Image.Load(bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image LoadImageSharp(Stream stream)
            => Image.Load(stream);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image LoadImageSharp(string path)
            => Image.Load(path);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image<Bgra32> LoadPfimImageSharp(byte[] bytes)
            => ToImageSharp(LoadPfim(bytes));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image<Bgra32> LoadPfimImageSharp(Stream stream)
            => ToImageSharp(Pfimage.FromStream(stream));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image<Bgra32> LoadPfimImageSharp(string path)
            => ToImageSharp(Pfimage.FromFile(path));
    }
}
