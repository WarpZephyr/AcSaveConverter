using AcSaveConverter.Graphics;
using AcSaveFormats.ArmoredCoreForAnswer.Designs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AcSaveConverter.Editors.AcfaEditor.Utilities
{
    internal static class DesignThumbnailImporter
    {
        internal static bool ImportThumbnail(string path, bool xbox, Thumbnail previous, [NotNullWhen(true)] out Thumbnail? output)
        {
            const int Width = 256;
            const int Height = 128;
            const StringComparison strComp = StringComparison.InvariantCultureIgnoreCase;
            static void CheckResize(Image<Rgba32> image)
            {
                if (image.Width != Width || image.Height != Height)
                    image.Mutate(x => x.Resize(Width, Height));
            }

            if (path.EndsWith(".dds", strComp) || path.EndsWith(".ktx", strComp))
            {
                using var image = TextureConverter.LoadBcImage(path);
                CheckResize(image);

                byte[] bytes = TextureConverter.ToBc1(image);
                previous.SetPixelData(bytes);
                output = previous;
            }
            else if (path.EndsWith(".bin", strComp))
            {
                try
                {
                    output = Thumbnail.Read(path, xbox);
                }
                catch
                {
                    // Was not a thumbnail file
                    output = null;
                    return false;
                }
            }
            else
            {
                using var image = TextureConverter.LoadImageSharp(path);
                CheckResize(image);

                byte[] bytes = TextureConverter.ToBc1(image);
                previous.SetPixelData(bytes);
                output = previous;
            }

            return true;
        }
    }
}
