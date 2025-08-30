using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace AcSaveConverter.Graphics
{
    public static class TextureExporter
    {
        public static void ExportDds(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public static void ExportKtx(string path, byte[] bytes)
        {
            var encoder = new BcEncoder();

            encoder.OutputOptions.GenerateMipMaps = false;
            encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
            encoder.OutputOptions.FileFormat = OutputFileFormat.Ktx;

            using FileStream fs = File.OpenWrite(path);
            using Image<Rgba32> image = TextureConverter.LoadBcImage(bytes);
            encoder.EncodeToStream(image, fs);
        }

        public static void ExportPng(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsPng(path);
        }

        public static void ExportJpeg(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsJpeg(path);
        }

        public static void ExportTga(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsTga(path);
        }

        public static void ExportTiff(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsTiff(path);
        }

        public static void ExportWebp(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsWebp(path);
        }

        public static void ExportBmp(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsBmp(path);
        }

        public static void ExportGif(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsGif(path);
        }

        public static void ExportPbm(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsPbm(path);
        }

        public static void ExportQoi(string path, byte[] bytes)
        {
            using var image = TextureConverter.LoadBcImage(bytes);
            image.SaveAsQoi(path);
        }
    }
}
