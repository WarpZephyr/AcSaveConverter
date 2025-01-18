using BinaryMemory;
using System;
using System.Diagnostics;
using System.IO;

namespace AcSaveConverter.Graphics.Textures
{
    // Credit for this primarily goes to the writer of this DDS class from SoulsFormats.

    /// <summary>
    /// Parser for .dds texture file headers.
    /// </summary>
    public class DDS
    {
        #region Constants

        public static readonly byte[] DX10DXGI = [29, 72, 75, 78, 86, 91, 93, 94, 95, 96, 97, 98, 99];
        public const DDSD HEADER_FLAGS_TEXTURE = DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT;
        public const DDSCAPS2 CUBEMAP_ALLFACES = DDSCAPS2.CUBEMAP | DDSCAPS2.CUBEMAP_POSITIVEX | DDSCAPS2.CUBEMAP_NEGATIVEX
            | DDSCAPS2.CUBEMAP_POSITIVEY | DDSCAPS2.CUBEMAP_NEGATIVEY | DDSCAPS2.CUBEMAP_POSITIVEZ | DDSCAPS2.CUBEMAP_NEGATIVEZ;
        public const int DXT1_BLOCKSIZE = 8;

        #endregion

        #region Properties

        public DDSD Flags;
        public int Height;
        public int Width;
        public int PitchOrLinearSize;
        public int Depth;
        public int MipMapCount;
        public int[] Reserved1;
        public PIXELFORMAT DDSPixelFormat;
        public DDSCAPS Caps;
        public DDSCAPS2 Caps2;
        public int Caps3;
        public int Caps4;
        public int Reserved2;
        public HEADER_DXT10? HeaderDXT10;

        #endregion

        #region Read Properties

        public int DataOffset => DDSPixelFormat.FourCC == "DX10" ? 0x94 : 0x80;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new DDS header with default values and no DX10 header.
        /// </summary>
        public DDS()
        {
            Flags = HEADER_FLAGS_TEXTURE;
            Reserved1 = new int[11];
            DDSPixelFormat = new PIXELFORMAT();
            Caps = DDSCAPS.TEXTURE;
        }

        /// <summary>
        /// Read a DDS header from an array of bytes.
        /// </summary>
        private DDS(BinaryStreamReader br)
        {
            br.AssertASCII("DDS "); // dwMagic
            br.AssertInt32(0x7C); // dwSize
            Flags = (DDSD)br.ReadUInt32();
            Height = br.ReadInt32();
            Width = br.ReadInt32();
            PitchOrLinearSize = br.ReadInt32();
            Depth = br.ReadInt32();
            MipMapCount = br.ReadInt32();
            Reserved1 = br.ReadInt32s(11);
            DDSPixelFormat = new PIXELFORMAT(br);
            Caps = (DDSCAPS)br.ReadUInt32();
            Caps2 = (DDSCAPS2)br.ReadUInt32();
            Caps3 = br.ReadInt32();
            Caps4 = br.ReadInt32();
            Reserved2 = br.ReadInt32();

            if (DDSPixelFormat.FourCC == "DX10")
                HeaderDXT10 = new HEADER_DXT10(br);
            else
                HeaderDXT10 = null;
        }

        #endregion

        #region Read

        public static DDS Read(string path)
        {
            using var br = new BinaryStreamReader(path, false);
            return new DDS(br);
        }

        public static DDS Read(byte[] bytes)
        {
            using var br = new BinaryStreamReader(bytes, false);
            return new DDS(br);
        }

        public static DDS Read(Stream stream)
        {
            using var br = new BinaryStreamReader(stream, false, true);
            return new DDS(br);
        }

        #endregion

        #region Write

        /// <summary>
        /// Write a DDS file from this header object and given pixel data.
        /// </summary>
        public byte[] Write(byte[] pixelData)
        {
            using var bw = new BinaryStreamWriter(false);

            bw.WriteASCII("DDS ");
            bw.WriteInt32(0x7C);
            bw.WriteUInt32((uint)Flags);
            bw.WriteInt32(Height);
            bw.WriteInt32(Width);
            bw.WriteInt32(PitchOrLinearSize);
            bw.WriteInt32(Depth);
            bw.WriteInt32(MipMapCount);
            bw.WriteInt32s(Reserved1);
            DDSPixelFormat.Write(bw);
            bw.WriteUInt32((uint)Caps);
            bw.WriteUInt32((uint)Caps2);
            bw.WriteInt32(Caps3);
            bw.WriteInt32(Caps4);
            bw.WriteInt32(Reserved2);

            if (DDSPixelFormat.FourCC == "DX10")
                if (HeaderDXT10 != null)
                    HeaderDXT10.Write(bw);
                else
                    throw new InvalidOperationException($"{nameof(HeaderDXT10)} was null when format is DX10.");

            bw.WriteBytes(pixelData);
            return bw.ToArray();
        }

        #endregion

        #region Methods

        public DXGI_FORMAT GetDXGIFormat()
        {
            if (HeaderDXT10 != null)
            {
                return HeaderDXT10.dxgiFormat;
            }
            else
            {
                //https://learn.microsoft.com/en-us/windows/uwp/gaming/complete-code-for-ddstextureloader
                switch (DDSPixelFormat.FourCC)
                {
                    case "DXT1":
                    case "DXT2":
                        return DXGI_FORMAT.BC1_UNORM;
                    case "DXT3":
                        return DXGI_FORMAT.BC2_UNORM;
                    case "DXT4":
                    case "DXT5":
                        return DXGI_FORMAT.BC3_UNORM;
                    case "ATI1":
                        return DXGI_FORMAT.BC4_UNORM;
                    case "BC4S":
                        return DXGI_FORMAT.BC4_SNORM;
                    case "ATI2":
                    case "BC5U":
                        return DXGI_FORMAT.BC5_UNORM;
                    case "BC5S":
                        return DXGI_FORMAT.BC5_SNORM;
                    case "RGBG":
                        return DXGI_FORMAT.R8G8_B8G8_UNORM;
                    case "GRGB":
                        return DXGI_FORMAT.G8R8_G8B8_UNORM;
                    case "$\0\0\0": //36
                        return DXGI_FORMAT.R16G16B16A16_UNORM;
                    case "n\0\0\0": //110
                        return DXGI_FORMAT.R16G16B16A16_SNORM;
                    case "o\0\0\0": //111
                        return DXGI_FORMAT.R16_FLOAT;
                    case "p\0\0\0": //112
                        return DXGI_FORMAT.R16G16_FLOAT;
                    case "q\0\0\0": //113
                        return DXGI_FORMAT.R16G16B16A16_FLOAT;
                    case "r\0\0\0": //114
                        return DXGI_FORMAT.R32_FLOAT;
                    case "s\0\0\0": //115
                        return DXGI_FORMAT.R32G32_FLOAT;
                    case "t\0\0\0": //116
                        return DXGI_FORMAT.R32G32B32A32_FLOAT;
                    default:
                        Debug.WriteLine("Unrecognized FourCC, defaulting to R8G8B8A8");
                        return DXGI_FORMAT.R8G8B8A8_UNORM;
                }
            }
        }

        #endregion

        #region Structs

        public class PIXELFORMAT
        {
            public DDPF Flags;
            public string FourCC;
            public int RGBBitCount;
            public uint RBitMask;
            public uint GBitMask;
            public uint BBitMask;
            public uint ABitMask;

            /// <summary>
            /// Create a new <see cref="PIXELFORMAT"/> with default values.
            /// </summary>
            public PIXELFORMAT()
            {
                FourCC = "\0\0\0\0";
            }

            internal PIXELFORMAT(BinaryStreamReader br)
            {
                br.AssertInt32(32); // dwSize
                Flags = (DDPF)br.ReadUInt32();
                FourCC = br.ReadASCII(4);
                RGBBitCount = br.ReadInt32();
                RBitMask = br.ReadUInt32();
                GBitMask = br.ReadUInt32();
                BBitMask = br.ReadUInt32();
                ABitMask = br.ReadUInt32();
            }

            internal void Write(BinaryStreamWriter bw)
            {
                bw.WriteInt32(32);
                bw.WriteUInt32((uint)Flags);
                // Make sure it's 4 characters
                bw.WriteASCII((FourCC ?? "").PadRight(4, '\0').Substring(0, 4));
                bw.WriteInt32(RGBBitCount);
                bw.WriteUInt32(RBitMask);
                bw.WriteUInt32(GBitMask);
                bw.WriteUInt32(BBitMask);
                bw.WriteUInt32(ABitMask);
            }
        }

        public class HEADER_DXT10
        {
            public DXGI_FORMAT dxgiFormat;
            public DIMENSION resourceDimension;
            public RESOURCE_MISC miscFlag;
            public uint arraySize;
            public ALPHA_MODE miscFlags2;

            /// <summary>
            /// Creates a new <see cref="HEADER_DXT10"/> with default values.
            /// </summary>
            public HEADER_DXT10()
            {
                dxgiFormat = DXGI_FORMAT.UNKNOWN;
                resourceDimension = DIMENSION.TEXTURE2D;
                arraySize = 1;
                miscFlags2 = ALPHA_MODE.UNKNOWN;
            }

            internal HEADER_DXT10(BinaryStreamReader br)
            {
                dxgiFormat = br.ReadEnumUInt32<DXGI_FORMAT>();
                resourceDimension = br.ReadEnumUInt32<DIMENSION>();
                miscFlag = (RESOURCE_MISC)br.ReadUInt32();
                arraySize = br.ReadUInt32();
                miscFlags2 = br.ReadEnumUInt32<ALPHA_MODE>();
            }

            internal void Write(BinaryStreamWriter bw)
            {
                bw.WriteUInt32((uint)dxgiFormat);
                bw.WriteUInt32((uint)resourceDimension);
                bw.WriteUInt32((uint)miscFlag);
                bw.WriteUInt32(arraySize);
                bw.WriteUInt32((uint)miscFlags2);
            }
        }

        #endregion

        #region Enums

        [Flags]
        public enum DDSD : uint
        {
            CAPS = 0x1,
            HEIGHT = 0x2,
            WIDTH = 0x4,
            PITCH = 0x8,
            PIXELFORMAT = 0x1000,
            MIPMAPCOUNT = 0x20000,
            LINEARSIZE = 0x80000,
            DEPTH = 0x800000,
        }

        [Flags]
        public enum DDSCAPS : uint
        {
            COMPLEX = 0x8,
            TEXTURE = 0x1000,
            MIPMAP = 0x400000,
        }

        [Flags]
        public enum DDSCAPS2 : uint
        {
            CUBEMAP = 0x200,
            CUBEMAP_POSITIVEX = 0x400,
            CUBEMAP_NEGATIVEX = 0x800,
            CUBEMAP_POSITIVEY = 0x1000,
            CUBEMAP_NEGATIVEY = 0x2000,
            CUBEMAP_POSITIVEZ = 0x4000,
            CUBEMAP_NEGATIVEZ = 0x8000,
            VOLUME = 0x200000,
        }

        [Flags]
        public enum DDPF : uint
        {
            ALPHAPIXELS = 0x1,
            ALPHA = 0x2,
            FOURCC = 0x4,
            RGB = 0x40,
            YUV = 0x200,
            LUMINANCE = 0x20000,
        }

        public enum DIMENSION : uint
        {
            TEXTURE1D = 2,
            TEXTURE2D = 3,
            TEXTURE3D = 4,
        }

        [Flags]
        public enum RESOURCE_MISC : uint
        {
            TEXTURECUBE = 0x4,
        }

        public enum ALPHA_MODE : uint
        {
            UNKNOWN = 0,
            STRAIGHT = 1,
            PREMULTIPLIED = 2,
            OPAQUE = 3,
            CUSTOM = 4,
        }

        public enum DXGI_FORMAT : uint
        {
            UNKNOWN,
            R32G32B32A32_TYPELESS,
            R32G32B32A32_FLOAT,
            R32G32B32A32_UINT,
            R32G32B32A32_SINT,
            R32G32B32_TYPELESS,
            R32G32B32_FLOAT,
            R32G32B32_UINT,
            R32G32B32_SINT,
            R16G16B16A16_TYPELESS,
            R16G16B16A16_FLOAT,
            R16G16B16A16_UNORM,
            R16G16B16A16_UINT,
            R16G16B16A16_SNORM,
            R16G16B16A16_SINT,
            R32G32_TYPELESS,
            R32G32_FLOAT,
            R32G32_UINT,
            R32G32_SINT,
            R32G8X24_TYPELESS,
            D32_FLOAT_S8X24_UINT,
            R32_FLOAT_X8X24_TYPELESS,
            X32_TYPELESS_G8X24_UINT,
            R10G10B10A2_TYPELESS,
            R10G10B10A2_UNORM,
            R10G10B10A2_UINT,
            R11G11B10_FLOAT,
            R8G8B8A8_TYPELESS,
            R8G8B8A8_UNORM,
            R8G8B8A8_UNORM_SRGB,
            R8G8B8A8_UINT,
            R8G8B8A8_SNORM,
            R8G8B8A8_SINT,
            R16G16_TYPELESS,
            R16G16_FLOAT,
            R16G16_UNORM,
            R16G16_UINT,
            R16G16_SNORM,
            R16G16_SINT,
            R32_TYPELESS,
            D32_FLOAT,
            R32_FLOAT,
            R32_UINT,
            R32_SINT,
            R24G8_TYPELESS,
            D24_UNORM_S8_UINT,
            R24_UNORM_X8_TYPELESS,
            X24_TYPELESS_G8_UINT,
            R8G8_TYPELESS,
            R8G8_UNORM,
            R8G8_UINT,
            R8G8_SNORM,
            R8G8_SINT,
            R16_TYPELESS,
            R16_FLOAT,
            D16_UNORM,
            R16_UNORM,
            R16_UINT,
            R16_SNORM,
            R16_SINT,
            R8_TYPELESS,
            R8_UNORM,
            R8_UINT,
            R8_SNORM,
            R8_SINT,
            A8_UNORM,
            R1_UNORM,
            R9G9B9E5_SHAREDEXP,
            R8G8_B8G8_UNORM,
            G8R8_G8B8_UNORM,
            BC1_TYPELESS,
            BC1_UNORM,
            BC1_UNORM_SRGB,
            BC2_TYPELESS,
            BC2_UNORM,
            BC2_UNORM_SRGB,
            BC3_TYPELESS,
            BC3_UNORM,
            BC3_UNORM_SRGB,
            BC4_TYPELESS,
            BC4_UNORM,
            BC4_SNORM,
            BC5_TYPELESS,
            BC5_UNORM,
            BC5_SNORM,
            B5G6R5_UNORM,
            B5G5R5A1_UNORM,
            B8G8R8A8_UNORM,
            B8G8R8X8_UNORM,
            R10G10B10_XR_BIAS_A2_UNORM,
            B8G8R8A8_TYPELESS,
            B8G8R8A8_UNORM_SRGB,
            B8G8R8X8_TYPELESS,
            B8G8R8X8_UNORM_SRGB,
            BC6H_TYPELESS,
            BC6H_UF16,
            BC6H_SF16,
            BC7_TYPELESS,
            BC7_UNORM,
            BC7_UNORM_SRGB,
            AYUV,
            Y410,
            Y416,
            NV12,
            P010,
            P016,
            OPAQUE_420, // DXGI_FORMAT_420_OPAQUE
            YUY2,
            Y210,
            Y216,
            NV11,
            AI44,
            IA44,
            P8,
            A8P8,
            B4G4R4A4_UNORM,
            P208,
            V208,
            V408,
            FORCE_UINT,
        }

        #endregion
    }
}
