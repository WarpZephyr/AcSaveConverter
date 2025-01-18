using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Runtime.CompilerServices;

namespace AcSaveConverter.Graphics.Textures
{
    public static class TextureSave
    {
        public static void ExportDDS(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        public static void ExportPng(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsPng(path);
        }

        public static void ExportJpeg(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsJpeg(path);
        }

        public static void ExportTga(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsTga(path);
        }

        public static void ExportTiff(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsTiff(path);
        }

        public static void ExportWebp(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsWebp(path);
        }

        public static void ExportBmp(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsBmp(path);
        }

        public static void ExportGif(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsGif(path);
        }

        public static void ExportPbm(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsPbm(path);
        }

        public static void ExportQoi(string path, byte[] bytes)
        {
            GetImageSharp(bytes).SaveAsQoi(path);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Image<Bgra32> GetImageSharp(byte[] bytes)
        {
            return TextureConverter.LoadPfimImageSharp(bytes);
        }
    }
}
