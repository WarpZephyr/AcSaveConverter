namespace AcSaveConverterImGui.IO.Assets
{
    internal static class ImagesPath
    {
        public static readonly string AssetsFolder = Path.Combine(Program.AppFolder, "Assets");
        public static readonly string ImagesFolder = Path.Combine(AssetsFolder, "Images");

        public static string GetImagePath(string image)
        {
            return Path.Combine(ImagesFolder, image);
        }

        public static bool ImageExists(string image)
        {
            return File.Exists(GetImagePath(image));
        }

        public static bool ImagePathExists(string imagePath)
        {
            return !string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath);
        }

        public static void CopyImageTo(string image, string name, string folder)
        {
            string path = GetImagePath(image);
            if (ImagePathExists(path))
            {
                File.Copy(path, Path.Combine(folder, name), true);
            }
        }

        public static void CopyImageTo(string image, string folder)
        {
            string path = GetImagePath(image);
            if (ImagePathExists(path))
            {
                File.Copy(path, Path.Combine(folder, image), true);
            }
        }
    }
}
