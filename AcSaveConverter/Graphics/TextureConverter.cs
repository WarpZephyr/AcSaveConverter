using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace AcSaveConverter.Graphics
{
    internal static class TextureConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image<Rgba32> LoadImageSharp(byte[] bytes)
            => Image.Load<Rgba32>(bytes);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image<Rgba32> LoadImageSharp(Stream stream)
            => Image.Load<Rgba32>(stream);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Image<Rgba32> LoadImageSharp(string path)
            => Image.Load<Rgba32>(path);

        public static unsafe Image<Rgba32> LoadBcImage(ReadOnlySpan<byte> bytes, bool hasAlpha = true)
        {
            var decoder = new BcDecoder();
            decoder.InputOptions.DdsBc1ExpectAlpha = hasAlpha;
            fixed (byte* ptr = bytes)
            {
                using UnmanagedMemoryStream stream = new(ptr, bytes.Length);
                return decoder.DecodeToImageRgba32(stream);
            }
        }

        public static Image<Rgba32> LoadBcImage(Stream stream, bool hasAlpha = true)
        {
            var decoder = new BcDecoder();
            decoder.InputOptions.DdsBc1ExpectAlpha = hasAlpha;
            return decoder.DecodeToImageRgba32(stream);
        }

        public static Image<Rgba32> LoadBcImage(string path, bool hasAlpha = true)
        {
            var decoder = new BcDecoder();
            decoder.InputOptions.DdsBc1ExpectAlpha = hasAlpha;
            using var stream = File.OpenRead(path);
            return decoder.DecodeToImageRgba32(stream);
        }

        public static byte[] ToBc1(Image<Rgba32> image, bool hasAlpha = true)
        {
            var encoder = new BcEncoder();
            encoder.OutputOptions.GenerateMipMaps = false;
            encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
            encoder.OutputOptions.Format = hasAlpha ? CompressionFormat.Bc1WithAlpha : CompressionFormat.Bc1;
            encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;
            return encoder.EncodeToRawBytes(image)[0];
        }
    }
}
