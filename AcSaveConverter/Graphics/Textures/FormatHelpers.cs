using System;
using System.Diagnostics;
using Veldrid;

namespace AcSaveConverter.Graphics.Textures
{
    internal static class FormatHelpers
    {
        public static PixelFormat GetPixelFormatFromDXGI(DDS.DXGI_FORMAT fmt)
        {
            switch (fmt)
            {
                case DDS.DXGI_FORMAT.B8G8R8A8_TYPELESS:
                case DDS.DXGI_FORMAT.B8G8R8A8_UNORM:
                case DDS.DXGI_FORMAT.B8G8R8X8_TYPELESS:
                case DDS.DXGI_FORMAT.B8G8R8X8_UNORM:
                    return PixelFormat.B8_G8_R8_A8_UNorm;
                case DDS.DXGI_FORMAT.B8G8R8A8_UNORM_SRGB:
                case DDS.DXGI_FORMAT.B8G8R8X8_UNORM_SRGB:
                    return PixelFormat.B8_G8_R8_A8_UNorm_SRgb;
                case DDS.DXGI_FORMAT.R8G8B8A8_UNORM_SRGB:
                    return PixelFormat.R8_G8_B8_A8_UNorm_SRgb;
                case DDS.DXGI_FORMAT.R8G8B8A8_UNORM:
                case DDS.DXGI_FORMAT.R8G8B8A8_TYPELESS:
                    return PixelFormat.R8_G8_B8_A8_UNorm;
                case DDS.DXGI_FORMAT.BC1_TYPELESS:
                case DDS.DXGI_FORMAT.BC1_UNORM:
                    return PixelFormat.BC1_Rgba_UNorm;
                case DDS.DXGI_FORMAT.BC1_UNORM_SRGB:
                    return PixelFormat.BC1_Rgba_UNorm_SRgb;
                case DDS.DXGI_FORMAT.BC2_TYPELESS:
                case DDS.DXGI_FORMAT.BC2_UNORM:
                    return PixelFormat.BC2_UNorm;
                case DDS.DXGI_FORMAT.BC2_UNORM_SRGB:
                    return PixelFormat.BC2_UNorm_SRgb;
                case DDS.DXGI_FORMAT.BC3_TYPELESS:
                case DDS.DXGI_FORMAT.BC3_UNORM:
                    return PixelFormat.BC3_UNorm;
                case DDS.DXGI_FORMAT.BC3_UNORM_SRGB:
                    return PixelFormat.BC3_UNorm_SRgb;
                case DDS.DXGI_FORMAT.BC4_TYPELESS:
                case DDS.DXGI_FORMAT.BC4_UNORM:
                    return PixelFormat.BC4_UNorm;
                case DDS.DXGI_FORMAT.BC4_SNORM:
                    return PixelFormat.BC4_SNorm;
                case DDS.DXGI_FORMAT.BC5_TYPELESS:
                case DDS.DXGI_FORMAT.BC5_UNORM:
                    return PixelFormat.BC5_UNorm;
                case DDS.DXGI_FORMAT.BC5_SNORM:
                    return PixelFormat.BC5_SNorm;
                case DDS.DXGI_FORMAT.BC7_TYPELESS:
                case DDS.DXGI_FORMAT.BC7_UNORM:
                    return PixelFormat.BC7_UNorm;
                case DDS.DXGI_FORMAT.BC7_UNORM_SRGB:
                    return PixelFormat.BC7_UNorm_SRgb;
                case DDS.DXGI_FORMAT.R16G16B16A16_FLOAT:
                    return PixelFormat.R16_G16_B16_A16_Float;
                default:
                    throw new Exception($"Unimplemented DXGI Type: {fmt}");
            }
        }

        public static bool SupportsDXGI(DDS.DXGI_FORMAT fmt)
        {
            switch (fmt)
            {
                case DDS.DXGI_FORMAT.B8G8R8A8_TYPELESS:
                case DDS.DXGI_FORMAT.B8G8R8A8_UNORM:
                case DDS.DXGI_FORMAT.B8G8R8X8_TYPELESS:
                case DDS.DXGI_FORMAT.B8G8R8X8_UNORM:
                case DDS.DXGI_FORMAT.B8G8R8A8_UNORM_SRGB:
                case DDS.DXGI_FORMAT.B8G8R8X8_UNORM_SRGB:
                case DDS.DXGI_FORMAT.R8G8B8A8_UNORM_SRGB:
                case DDS.DXGI_FORMAT.R8G8B8A8_UNORM:
                case DDS.DXGI_FORMAT.R8G8B8A8_TYPELESS:
                case DDS.DXGI_FORMAT.BC1_TYPELESS:
                case DDS.DXGI_FORMAT.BC1_UNORM:
                case DDS.DXGI_FORMAT.BC1_UNORM_SRGB:
                case DDS.DXGI_FORMAT.BC2_TYPELESS:
                case DDS.DXGI_FORMAT.BC2_UNORM:
                case DDS.DXGI_FORMAT.BC2_UNORM_SRGB:
                case DDS.DXGI_FORMAT.BC3_TYPELESS:
                case DDS.DXGI_FORMAT.BC3_UNORM:
                case DDS.DXGI_FORMAT.BC3_UNORM_SRGB:
                case DDS.DXGI_FORMAT.BC4_TYPELESS:
                case DDS.DXGI_FORMAT.BC4_UNORM:
                case DDS.DXGI_FORMAT.BC4_SNORM:
                case DDS.DXGI_FORMAT.BC5_TYPELESS:
                case DDS.DXGI_FORMAT.BC5_UNORM:
                case DDS.DXGI_FORMAT.BC5_SNORM:
                case DDS.DXGI_FORMAT.BC7_TYPELESS:
                case DDS.DXGI_FORMAT.BC7_UNORM:
                case DDS.DXGI_FORMAT.BC7_UNORM_SRGB:
                case DDS.DXGI_FORMAT.R16G16B16A16_FLOAT:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsCompressedFormat(PixelFormat format)
        {
            return format == PixelFormat.BC1_Rgb_UNorm
                || format == PixelFormat.BC1_Rgb_UNorm_SRgb
                || format == PixelFormat.BC1_Rgba_UNorm
                || format == PixelFormat.BC1_Rgba_UNorm_SRgb
                || format == PixelFormat.BC2_UNorm
                || format == PixelFormat.BC2_UNorm_SRgb
                || format == PixelFormat.BC3_UNorm
                || format == PixelFormat.BC3_UNorm_SRgb
                || format == PixelFormat.BC4_UNorm
                || format == PixelFormat.BC4_SNorm
                || format == PixelFormat.BC5_UNorm
                || format == PixelFormat.BC5_SNorm
                || format == PixelFormat.BC7_UNorm
                || format == PixelFormat.BC7_UNorm_SRgb
                || format == PixelFormat.ETC2_R8_G8_B8_UNorm
                || format == PixelFormat.ETC2_R8_G8_B8_A1_UNorm
                || format == PixelFormat.ETC2_R8_G8_B8_A8_UNorm;
        }

        public static uint GetTexelSize(PixelFormat surfaceFormat)
        {
            if (IsCompressedFormat(surfaceFormat))
            {
                return GetBlockSizeInBytes(surfaceFormat);
            }

            return GetSizeInBytes(surfaceFormat);
        }

        public static uint GetBlockSizeInBytes(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.BC1_Rgb_UNorm:
                case PixelFormat.BC1_Rgb_UNorm_SRgb:
                case PixelFormat.BC1_Rgba_UNorm:
                case PixelFormat.BC1_Rgba_UNorm_SRgb:
                case PixelFormat.BC4_UNorm:
                case PixelFormat.BC4_SNorm:
                case PixelFormat.ETC2_R8_G8_B8_UNorm:
                case PixelFormat.ETC2_R8_G8_B8_A1_UNorm:
                    return 8;
                case PixelFormat.BC2_UNorm:
                case PixelFormat.BC2_UNorm_SRgb:
                case PixelFormat.BC3_UNorm:
                case PixelFormat.BC3_UNorm_SRgb:
                case PixelFormat.BC5_UNorm:
                case PixelFormat.BC5_SNorm:
                case PixelFormat.BC7_UNorm:
                case PixelFormat.BC7_UNorm_SRgb:
                case PixelFormat.ETC2_R8_G8_B8_A8_UNorm:
                    return 16;
                default:
                    throw new NotSupportedException($"Unknown {nameof(PixelFormat)}: {format}");
            }
        }

        public static uint GetSizeInBytes(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.R8_UNorm:
                case PixelFormat.R8_SNorm:
                case PixelFormat.R8_UInt:
                case PixelFormat.R8_SInt:
                    return 1;
                case PixelFormat.R16_UNorm:
                case PixelFormat.R16_SNorm:
                case PixelFormat.R16_UInt:
                case PixelFormat.R16_SInt:
                case PixelFormat.R16_Float:
                case PixelFormat.R8_G8_UNorm:
                case PixelFormat.R8_G8_SNorm:
                case PixelFormat.R8_G8_UInt:
                case PixelFormat.R8_G8_SInt:
                    return 2;
                case PixelFormat.R32_UInt:
                case PixelFormat.R32_SInt:
                case PixelFormat.R32_Float:
                case PixelFormat.R16_G16_UNorm:
                case PixelFormat.R16_G16_SNorm:
                case PixelFormat.R16_G16_UInt:
                case PixelFormat.R16_G16_SInt:
                case PixelFormat.R16_G16_Float:
                case PixelFormat.R8_G8_B8_A8_UNorm:
                case PixelFormat.R8_G8_B8_A8_UNorm_SRgb:
                case PixelFormat.R8_G8_B8_A8_SNorm:
                case PixelFormat.R8_G8_B8_A8_UInt:
                case PixelFormat.R8_G8_B8_A8_SInt:
                case PixelFormat.B8_G8_R8_A8_UNorm:
                case PixelFormat.B8_G8_R8_A8_UNorm_SRgb:
                case PixelFormat.D24_UNorm_S8_UInt:
                    return 4;
                case PixelFormat.D32_Float_S8_UInt:
                    return 5;
                case PixelFormat.R16_G16_B16_A16_UNorm:
                case PixelFormat.R16_G16_B16_A16_SNorm:
                case PixelFormat.R16_G16_B16_A16_UInt:
                case PixelFormat.R16_G16_B16_A16_SInt:
                case PixelFormat.R16_G16_B16_A16_Float:
                case PixelFormat.R32_G32_UInt:
                case PixelFormat.R32_G32_SInt:
                case PixelFormat.R32_G32_Float:
                    return 8;
                case PixelFormat.R32_G32_B32_A32_Float:
                case PixelFormat.R32_G32_B32_A32_UInt:
                case PixelFormat.R32_G32_B32_A32_SInt:
                    return 16;
                case PixelFormat.BC1_Rgb_UNorm:
                case PixelFormat.BC1_Rgb_UNorm_SRgb:
                case PixelFormat.BC1_Rgba_UNorm:
                case PixelFormat.BC1_Rgba_UNorm_SRgb:
                case PixelFormat.BC2_UNorm:
                case PixelFormat.BC2_UNorm_SRgb:
                case PixelFormat.BC3_UNorm:
                case PixelFormat.BC3_UNorm_SRgb:
                case PixelFormat.BC4_UNorm:
                case PixelFormat.BC4_SNorm:
                case PixelFormat.BC5_UNorm:
                case PixelFormat.BC5_SNorm:
                case PixelFormat.BC7_UNorm:
                case PixelFormat.BC7_UNorm_SRgb:
                case PixelFormat.ETC2_R8_G8_B8_UNorm:
                case PixelFormat.ETC2_R8_G8_B8_A1_UNorm:
                case PixelFormat.ETC2_R8_G8_B8_A8_UNorm:
                    Debug.Fail($"{nameof(PixelFormat)} should not be used on a compressed format: {format}");
                    throw new NotSupportedException($"{nameof(PixelFormat)} should not be used on a compressed format: {format}");
                default:
                    Debug.Fail($"Unimplemented {nameof(PixelFormat)}: {format}");
                    throw new NotSupportedException($"Unimplemented {nameof(PixelFormat)}: {format}");
            }
        }

        public static void GetBlockDimensions(PixelFormat format, out int width, out int height)
        {
            switch (format)
            {
                case PixelFormat.BC1_Rgb_UNorm:
                case PixelFormat.BC1_Rgb_UNorm_SRgb:
                case PixelFormat.BC1_Rgba_UNorm:
                case PixelFormat.BC1_Rgba_UNorm_SRgb:
                case PixelFormat.BC2_UNorm:
                case PixelFormat.BC2_UNorm_SRgb:
                case PixelFormat.BC3_UNorm:
                case PixelFormat.BC3_UNorm_SRgb:
                case PixelFormat.BC4_UNorm:
                case PixelFormat.BC4_SNorm:
                case PixelFormat.BC5_UNorm:
                case PixelFormat.BC5_SNorm:
                case PixelFormat.BC7_UNorm:
                case PixelFormat.BC7_UNorm_SRgb:
                    width = 4;
                    height = 4;
                    break;
                default:
                    width = 1;
                    height = 1;
                    break;
            }
        }

        // From MonoGame.Framework/Graphics/Texture2D.cs and MonoGame.Framework/Graphics/TextureCube.cs
        //private static (int ByteCount, Rectangle Rect) GetMipInfo(PixelFormat sf, int width, int height, int mip, bool isCubemap)
        public static int GetMipInfo(PixelFormat sf, int width, int height, int mip, bool isCubemap)
        {
            width = Math.Max(width >> mip, 1);
            height = Math.Max(height >> mip, 1);

            var formatTexelSize = (int)FormatHelpers.GetTexelSize(sf);

            if (isCubemap)
            {
                if (FormatHelpers.IsCompressedFormat(sf))
                {
                    var roundedWidth = width + 3 & ~0x3;
                    var roundedHeight = height + 3 & ~0x3;

                    var byteCount = roundedWidth * roundedHeight * formatTexelSize / 16;

                    //return (byteCount, new Rectangle(0, 0, roundedWidth, roundedHeight));
                    return byteCount;
                }
                else
                {
                    var byteCount = width * height * formatTexelSize;

                    return byteCount;
                    //return (byteCount, new Rectangle(0, 0, width, height));
                }
            }

            if (FormatHelpers.IsCompressedFormat(sf))
            {
                int blockWidth, blockHeight;
                FormatHelpers.GetBlockDimensions(sf, out blockWidth, out blockHeight);

                var blockWidthMinusOne = blockWidth - 1;
                var blockHeightMinusOne = blockHeight - 1;

                var roundedWidth = width + blockWidthMinusOne & ~blockWidthMinusOne;
                var roundedHeight = height + blockHeightMinusOne & ~blockHeightMinusOne;

                Rectangle rect = new(0, 0, roundedWidth, roundedHeight);

                int byteCount;

                byteCount = roundedWidth * roundedHeight * formatTexelSize / (blockWidth * blockHeight);

                //return (byteCount, rect);
                return byteCount;
            }
            else
            {
                var byteCount = width * height * formatTexelSize;

                //return (byteCount, new Rectangle(0, 0, width, height));
                return byteCount;
            }
        }
    }
}
