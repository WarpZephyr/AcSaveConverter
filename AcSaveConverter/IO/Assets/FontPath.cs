﻿using System.IO;

namespace AcSaveConverter.IO.Assets
{
    internal static class FontPath
    {
        public static readonly string AssetsFolder = Path.Combine(Program.AppDataFolder, "Assets");
        public static readonly string FontsFolder = Path.Combine(AssetsFolder, "Fonts");

        public static string GetFontPath(string font)
        {
            return Path.Combine(FontsFolder, font);
        }

        public static bool FontExists(string font)
        {
            return File.Exists(GetFontPath(font));
        }

        public static bool FontPathExists(string fontPath)
        {
            return !string.IsNullOrWhiteSpace(fontPath) && File.Exists(fontPath);
        }
    }
}
