using SixLabors.ImageSharp.PixelFormats;
using System;
using Veldrid;
using Veldrid.ImageSharp;

namespace AcSaveConverter.Graphics.Textures
{
    internal class DdsLoader
    {
        private readonly GraphicsDevice Graphics;
        private readonly ResourceFactory Factory;
        private readonly CommandList CommandList;

        public DdsLoader(GraphicsDevice graphics, ResourceFactory factory, CommandList commandList)
        {
            Graphics = graphics;
            Factory = factory;
            CommandList = commandList;
        }

        // Credit for the bulk of the direct veldrid loading code goes to Smithbox
        public unsafe Texture LoadDDS(DDS dds, Memory<byte> bytes, string name)
        {
            PixelFormat pixelFormat;
            if (dds.HeaderDXT10 == null
             && dds.DDSPixelFormat.Flags == (DDS.DDPF.RGB | DDS.DDPF.ALPHAPIXELS)
             && dds.DDSPixelFormat.RGBBitCount == 32)
            {
                pixelFormat = PixelFormat.R8_G8_B8_A8_UNorm_SRgb;
            }
            else
            {
                var dxgi = dds.GetDXGIFormat();
                if (!FormatHelpers.SupportsDXGI(dxgi))
                {
                    return LoadViaImageSharp(bytes);
                }

                pixelFormat = FormatHelpers.GetPixelFormatFromDXGI(dxgi);
            }

            var width = (uint)dds.Width;
            var height = (uint)dds.Height;

            bool compressedFormat = FormatHelpers.IsCompressedFormat(pixelFormat);
            width = compressedFormat ? (uint)(width + 3 & ~0x3) : width;
            height = compressedFormat ? (uint)(height + 3 & ~0x3) : height;

            var isCubemap = (dds.Caps2 & DDS.DDSCAPS2.CUBEMAP) > 0;
            var arrayCount = isCubemap ? 6u : 1;

            TextureDescription desc = new()
            {
                Width = width,
                Height = height,
                MipLevels = (uint)dds.MipMapCount,
                SampleCount = TextureSampleCount.Count1,
                ArrayLayers = arrayCount,
                Depth = 1,
                Type = TextureType.Texture2D,
                Usage = TextureUsage.Staging,
                Format = pixelFormat
            };

            using var staging = Factory.CreateTexture(desc);
            var copyOffset = dds.DataOffset;

            for (var slice = 0; slice < arrayCount; slice++)
            {
                for (uint level = 0; level < dds.MipMapCount; level++)
                {
                    MappedResource map = Graphics.Map(staging, MapMode.Write,
                        (uint)slice * (uint)dds.MipMapCount + level);
                    var mipInfo = FormatHelpers.GetMipInfo(pixelFormat, dds.Width, dds.Height, (int)level, false);
                    //int paddedSize = mipInfo.ByteCount;
                    int paddedSize = mipInfo;
                    Span<byte> dest = new(map.Data.ToPointer(), paddedSize);
                    bytes.Span.Slice(copyOffset, paddedSize).CopyTo(dest);
                    copyOffset += paddedSize;
                }
            }

            desc.Usage = TextureUsage.Sampled;
            desc.ArrayLayers = 1;

            var texture = Factory.CreateTexture(desc);
            texture.Name = name;
            CommandList.CopyTexture(staging, texture);

            return texture;
        }

        private Texture LoadViaImageSharp(Memory<byte> bytes)
        {
            var simage = TextureConverter.LoadPfimImageSharp(bytes.Span);
            var vsimage = new ImageSharpTexture<Bgra32>(simage, false);
            return vsimage.CreateDeviceTexture(Graphics, Factory);
        }
    }
}
