using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Veldrid.ImageSharp
{
    public class ImageSharpTexture<TPixel> where TPixel : unmanaged, IPixel<TPixel>
    {
        /// <summary>
        /// An array of images, each a single element in the mipmap chain.
        /// The first element is the largest, most detailed level, and each subsequent element
        /// is half its size, down to 1x1 pixel.
        /// </summary>
        public Image<TPixel>[] Images { get; }

        /// <summary>
        /// The width of the largest image in the chain.
        /// </summary>
        public uint Width => (uint)Images[0].Width;

        /// <summary>
        /// The height of the largest image in the chain.
        /// </summary>
        public uint Height => (uint)Images[0].Height;

        /// <summary>
        /// The pixel format of all images.
        /// </summary>
        public PixelFormat Format { get; }

        /// <summary>
        /// The size of each pixel, in bytes.
        /// </summary>
        public uint PixelSizeInBytes { get; private init; }

        /// <summary>
        /// The number of levels in the mipmap chain. This is equal to the length of the Images array.
        /// </summary>
        public uint MipLevels => (uint)Images.Length;

        public ImageSharpTexture(string path) : this(Image.Load<TPixel>(path), true) { }
        public ImageSharpTexture(string path, bool mipmap) : this(Image.Load<TPixel>(path), mipmap) { }
        public ImageSharpTexture(string path, bool mipmap, bool srgb) : this(Image.Load<TPixel>(path), mipmap, srgb) { }
        public ImageSharpTexture(Stream stream) : this(Image.Load<TPixel>(stream), true) { }
        public ImageSharpTexture(Stream stream, bool mipmap) : this(Image.Load<TPixel>(stream), mipmap) { }
        public ImageSharpTexture(Stream stream, bool mipmap, bool srgb) : this(Image.Load<TPixel>(stream), mipmap, srgb) { }
        public ImageSharpTexture(Image<TPixel> image, bool mipmap = true) : this(image, mipmap, false) { }
        public ImageSharpTexture(Image<TPixel> image, bool mipmap, bool srgb)
        {
            Format = GetPixelFormatFromTPixel(srgb, out uint pixelByteSize);
            PixelSizeInBytes = pixelByteSize;

            if (mipmap)
            {
                Images = MipmapHelper.GenerateMipmaps(image);
            }
            else
            {
                Images = new Image<TPixel>[] { image };
            }
        }

        public unsafe Texture CreateDeviceTexture(GraphicsDevice gd, ResourceFactory factory)
        {
            return CreateTextureViaUpdate(gd, factory);
        }

        private unsafe Texture CreateTextureViaStaging(GraphicsDevice gd, ResourceFactory factory)
        {
            Texture staging = factory.CreateTexture(
                TextureDescription.Texture2D(Width, Height, MipLevels, 1, Format, TextureUsage.Staging));

            Texture ret = factory.CreateTexture(
                TextureDescription.Texture2D(Width, Height, MipLevels, 1, Format, TextureUsage.Sampled));

            CommandList cl = gd.ResourceFactory.CreateCommandList();
            cl.Begin();
            for (uint level = 0; level < MipLevels; level++)
            {
                Image<TPixel> image = Images[level];
                if (!image.DangerousTryGetSinglePixelMemory(out Memory<TPixel> pixelMemory))
                {
                    throw new VeldridException("Unable to get image pixelmemory.");
                }

                using MemoryHandle pin = pixelMemory.Pin();
                var ptr = pin.Pointer;
                MappedResource map = gd.Map(staging, MapMode.Write, level);
                uint rowWidth = (uint)(image.Width * 4);
                if (rowWidth == map.RowPitch)
                {
                    Unsafe.CopyBlock(map.Data.ToPointer(), ptr, (uint)(image.Width * image.Height * 4));
                }
                else
                {
                    for (uint y = 0; y < image.Height; y++)
                    {
                        byte* dstStart = (byte*)map.Data.ToPointer() + y * map.RowPitch;
                        byte* srcStart = (byte*)ptr + y * rowWidth;
                        Unsafe.CopyBlock(dstStart, srcStart, rowWidth);
                    }
                }
                gd.Unmap(staging, level);

                cl.CopyTexture(
                    staging, 0, 0, 0, level, 0,
                    ret, 0, 0, 0, level, 0,
                    (uint)image.Width, (uint)image.Height, 1, 1);
            }
            cl.End();

            gd.SubmitCommands(cl);
            staging.Dispose();
            cl.Dispose();

            return ret;
        }

        private unsafe Texture CreateTextureViaUpdate(GraphicsDevice gd, ResourceFactory factory)
        {
            Texture tex = factory.CreateTexture(TextureDescription.Texture2D(
                Width, Height, MipLevels, 1, Format, TextureUsage.Sampled));
            for (int level = 0; level < MipLevels; level++)
            {
                Image<TPixel> image = Images[level];
                if (!image.DangerousTryGetSinglePixelMemory(out Memory<TPixel> pixelMemory))
                {
                    throw new VeldridException("Unable to get image pixelmemory.");
                }

                using MemoryHandle pin = pixelMemory.Pin();
                gd.UpdateTexture(
                    tex,
                    (IntPtr)pin.Pointer,
                    (uint)(PixelSizeInBytes * image.Width * image.Height),
                    0,
                    0,
                    0,
                    (uint)image.Width,
                    (uint)image.Height,
                    1,
                    (uint)level,
                    0);
            }

            return tex;
        }

        private PixelFormat GetPixelFormatFromTPixel(bool srgb, out uint pixelByteSize)
        {
            var type = typeof(TPixel);
            if (type == typeof(Rgba32))
            {
                pixelByteSize = (uint)Unsafe.SizeOf<Rgba32>();
                return srgb ? PixelFormat.R8_G8_B8_A8_UNorm_SRgb : PixelFormat.R8_G8_B8_A8_UNorm;
            }

            if (type == typeof(Bgra32))
            {
                pixelByteSize = (uint)Unsafe.SizeOf<Bgra32>();
                return srgb ? PixelFormat.B8_G8_R8_A8_UNorm_SRgb : PixelFormat.B8_G8_R8_A8_UNorm;
            }

            if (type == typeof(RgbaVector))
            {
                pixelByteSize = (uint)Unsafe.SizeOf<RgbaVector>();
                return PixelFormat.R32_G32_B32_A32_Float;
            }

            throw new VeldridException($"Unsupported {nameof(IPixel)} type: {type.Name}");
        }
    }
}
