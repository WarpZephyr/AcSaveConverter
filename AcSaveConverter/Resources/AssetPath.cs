using System.IO;
using System.Runtime.CompilerServices;

namespace AcSaveConverter.Resources
{
    internal static class AssetPath
    {
        private static readonly string AssetsFolder = Path.Combine(AppInfo.AppDirectory, "Assets");
        private static readonly string FontsFolder = Path.Combine(AssetsFolder, "Fonts");
        private static readonly string ImagesFolder = Path.Combine(AssetsFolder, "Images");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFontPath(string name)
            => Path.Combine(FontsFolder, name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetImagePath(string image)
            => Path.Combine(ImagesFolder, image);

        public static void CopyImageTo(string image, string name, string folder)
        {
            string path = GetImagePath(image);
            if (PathExists(path))
            {
                File.Copy(path, Path.Combine(folder, name), true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PathExists(string path)
            => !string.IsNullOrWhiteSpace(path) && File.Exists(path);
    }
}
