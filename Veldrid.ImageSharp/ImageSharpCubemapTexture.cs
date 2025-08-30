using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace Veldrid.ImageSharp
{
    public class ImageSharpCubemapTexture<TPixel> where TPixel : unmanaged, IPixel<TPixel>
    {
        /// <summary>
        /// An array of images, each face of a cubemap.
        /// Access of CubemapTextures[2][3] means face 2 with mipmap level 3
        /// </summary>
        public Image<TPixel>[][] CubemapTextures { get; }

        /// <summary>
        /// The width of a cubemap texture.
        /// </summary>
        public uint Width => (uint)CubemapTextures[0][0].Width;

        /// <summary>
        /// The height of a cubemap texture.
        /// </summary>
        public uint Height => (uint)CubemapTextures[0][0].Height;

        /// <summary>
        /// The pixel format cubemap textures.
        /// </summary>
        public PixelFormat Format { get; private init; }

        /// <summary>
        /// The size of each pixel, in bytes.
        /// </summary>
        public uint PixelSizeInBytes { get; private init; }

        /// <summary>
        /// The number of levels in the mipmap chain. This is equal to the length of the Images array.
        /// </summary>
        public uint MipLevels => (uint)CubemapTextures[0].Length;

        /// <summary>
        /// Provides standardized access to the cubemap texture array
        /// </summary>
        private const int PositiveXArrayLayer = 0;
        private const int NegativeXArrayLayer = 1;
        private const int PositiveYArrayLayer = 2;
        private const int NegativeYArrayLayer = 3;
        private const int PositiveZArrayLayer = 4;
        private const int NegativeZArrayLayer = 5;

        public ImageSharpCubemapTexture(
            string positiveXPath,
            string negativeXPath,
            string positiveYPath,
            string negativeYPath,
            string positiveZPath,
            string negativeZPath) : this(
                Image.Load<TPixel>(positiveXPath),
                Image.Load<TPixel>(negativeXPath),
                Image.Load<TPixel>(positiveYPath),
                Image.Load<TPixel>(negativeYPath),
                Image.Load<TPixel>(positiveZPath),
                Image.Load<TPixel>(negativeZPath),
                true)
        { }

        public ImageSharpCubemapTexture(
            string positiveXPath,
            string negativeXPath,
            string positiveYPath,
            string negativeYPath,
            string positiveZPath,
            string negativeZPath,
            bool mipmap) : this(
                Image.Load<TPixel>(positiveXPath),
                Image.Load<TPixel>(negativeXPath),
                Image.Load<TPixel>(positiveYPath),
                Image.Load<TPixel>(negativeYPath),
                Image.Load<TPixel>(positiveZPath),
                Image.Load<TPixel>(negativeZPath),
                mipmap)
        { }

        public ImageSharpCubemapTexture(
            Stream positiveXStream,
            Stream negativeXStream,
            Stream positiveYStream,
            Stream negativeYStream,
            Stream positiveZStream,
            Stream negativeZStream,
            bool mipmap) : this(
                Image.Load<TPixel>(positiveXStream),
                Image.Load<TPixel>(negativeXStream),
                Image.Load<TPixel>(positiveYStream),
                Image.Load<TPixel>(negativeYStream),
                Image.Load<TPixel>(positiveZStream),
                Image.Load<TPixel>(negativeZStream),
                mipmap)
        { }

        public ImageSharpCubemapTexture(
            Image<TPixel> positiveX,
            Image<TPixel> negativeX,
            Image<TPixel> positiveY,
            Image<TPixel> negativeY,
            Image<TPixel> positiveZ,
            Image<TPixel> negativeZ,
            bool mipmap = true)
        {
            Format = GetPixelFormatFromTPixel(out uint pixelByteSize);
            PixelSizeInBytes = pixelByteSize;

            CubemapTextures = new Image<TPixel>[6][];
            if (mipmap)
            {
                CubemapTextures[0] = MipmapHelper.GenerateMipmaps(positiveX);
                CubemapTextures[1] = MipmapHelper.GenerateMipmaps(negativeX);
                CubemapTextures[2] = MipmapHelper.GenerateMipmaps(positiveY);
                CubemapTextures[3] = MipmapHelper.GenerateMipmaps(negativeY);
                CubemapTextures[4] = MipmapHelper.GenerateMipmaps(positiveZ);
                CubemapTextures[5] = MipmapHelper.GenerateMipmaps(negativeZ);
            }

            else
            {
                CubemapTextures[0] = new Image<TPixel>[1] { positiveX };
                CubemapTextures[1] = new Image<TPixel>[1] { negativeX };
                CubemapTextures[2] = new Image<TPixel>[1] { positiveY };
                CubemapTextures[3] = new Image<TPixel>[1] { negativeY };
                CubemapTextures[4] = new Image<TPixel>[1] { positiveZ };
                CubemapTextures[5] = new Image<TPixel>[1] { negativeZ };
            }
        }

        public ImageSharpCubemapTexture(
            Image<TPixel>[] positiveX,
            Image<TPixel>[] negativeX,
            Image<TPixel>[] positiveY,
            Image<TPixel>[] negativeY,
            Image<TPixel>[] positiveZ,
            Image<TPixel>[] negativeZ)
        {
            Format = GetPixelFormatFromTPixel(out uint pixelByteSize);
            PixelSizeInBytes = pixelByteSize;

            CubemapTextures = new Image<TPixel>[6][];
            if (positiveX.Length == 0)
            {
                throw new ArgumentException("Texture should have at least one mip level.");
            }
            if (positiveX.Length != negativeX.Length ||
                positiveX.Length != positiveY.Length ||
                positiveX.Length != negativeY.Length ||
                positiveX.Length != positiveZ.Length ||
                positiveX.Length != negativeZ.Length)
            {
                throw new ArgumentException("Mip count doesn't match.");
            }
            CubemapTextures[0] = positiveX;
            CubemapTextures[1] = negativeX;
            CubemapTextures[2] = positiveY;
            CubemapTextures[3] = negativeY;
            CubemapTextures[4] = positiveZ;
            CubemapTextures[5] = negativeZ;
        }

        public unsafe Texture CreateDeviceTexture(GraphicsDevice gd, ResourceFactory factory)
        {
            Texture cubemapTexture = factory.CreateTexture(TextureDescription.Texture2D(
                        Width,
                        Height,
                        MipLevels,
                        1,
                        Format,
                        TextureUsage.Sampled | TextureUsage.Cubemap));

            for (int level = 0; level < MipLevels; level++)
            {
                if (!CubemapTextures[PositiveXArrayLayer][level].DangerousTryGetSinglePixelMemory(out Memory<TPixel> pixelMemoryPosX))
                {
                    throw new VeldridException("Unable to get positive x pixelmemory.");
                }
                if (!CubemapTextures[NegativeXArrayLayer][level].DangerousTryGetSinglePixelMemory(out Memory<TPixel> pixelMemoryNegX))
                {
                    throw new VeldridException("Unable to get negative x pixelmemory.");
                }
                if (!CubemapTextures[PositiveYArrayLayer][level].DangerousTryGetSinglePixelMemory(out Memory<TPixel> pixelMemoryPosY))
                {
                    throw new VeldridException("Unable to get positive y pixelmemory.");
                }
                if (!CubemapTextures[NegativeYArrayLayer][level].DangerousTryGetSinglePixelMemory(out Memory<TPixel> pixelMemoryNegY))
                {
                    throw new VeldridException("Unable to get negative y pixelmemory.");
                }
                if (!CubemapTextures[PositiveZArrayLayer][level].DangerousTryGetSinglePixelMemory(out Memory<TPixel> pixelMemoryPosZ))
                {
                    throw new VeldridException("Unable to get positive z pixelmemory.");
                }
                if (!CubemapTextures[NegativeZArrayLayer][level].DangerousTryGetSinglePixelMemory(out Memory<TPixel> pixelMemoryNegZ))
                {
                    throw new VeldridException("Unable to get negative z pixelmemory.");
                }

                using MemoryHandle positiveXPin = pixelMemoryPosX.Pin();
                using MemoryHandle negativeXPin = pixelMemoryNegX.Pin();
                using MemoryHandle positiveYPin = pixelMemoryPosY.Pin();
                using MemoryHandle negativeYPin = pixelMemoryNegY.Pin();
                using MemoryHandle positiveZPin = pixelMemoryPosZ.Pin();
                using MemoryHandle negativeZPin = pixelMemoryNegZ.Pin();
                Image<TPixel> image = CubemapTextures[0][level];
                uint width = (uint)image.Width;
                uint height = (uint)image.Height;
                uint faceSize = width * height * PixelSizeInBytes;
                gd.UpdateTexture(cubemapTexture, (IntPtr)positiveXPin.Pointer, faceSize, 0, 0, 0, width, height, 1, (uint)level, PositiveXArrayLayer);
                gd.UpdateTexture(cubemapTexture, (IntPtr)negativeXPin.Pointer, faceSize, 0, 0, 0, width, height, 1, (uint)level, NegativeXArrayLayer);
                gd.UpdateTexture(cubemapTexture, (IntPtr)positiveYPin.Pointer, faceSize, 0, 0, 0, width, height, 1, (uint)level, PositiveYArrayLayer);
                gd.UpdateTexture(cubemapTexture, (IntPtr)negativeYPin.Pointer, faceSize, 0, 0, 0, width, height, 1, (uint)level, NegativeYArrayLayer);
                gd.UpdateTexture(cubemapTexture, (IntPtr)positiveZPin.Pointer, faceSize, 0, 0, 0, width, height, 1, (uint)level, PositiveZArrayLayer);
                gd.UpdateTexture(cubemapTexture, (IntPtr)negativeZPin.Pointer, faceSize, 0, 0, 0, width, height, 1, (uint)level, NegativeZArrayLayer);
            }
            return cubemapTexture;
        }

        private PixelFormat GetPixelFormatFromTPixel(out uint pixelByteSize)
        {
            var type = typeof(TPixel);
            if (type == typeof(Rgba32))
            {
                pixelByteSize = (uint)Unsafe.SizeOf<Rgba32>();
                return PixelFormat.R8_G8_B8_A8_UNorm;
            }

            if (type == typeof(Bgra32))
            {
                pixelByteSize = (uint)Unsafe.SizeOf<Bgra32>();
                return PixelFormat.B8_G8_R8_A8_UNorm;
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

