using AcSaveConverter.Graphics.Textures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;
using Veldrid.ImageSharp;

namespace AcSaveConverter.Graphics
{
    public class ImGuiTexturePool
    {
        private readonly GraphicsDevice Graphics;
        private readonly ResourceFactory Factory;
        private readonly ImGuiRenderer ImGuiRenderer;
        private readonly DdsLoader DdsLoader;
        public bool CommandListDirty { get; set; }

        internal ImGuiTexturePool(GraphicsDevice graphics, ResourceFactory factory, CommandList commandList, ImGuiRenderer imGuiRenderer)
        {
            Graphics = graphics;
            Factory = factory;
            ImGuiRenderer = imGuiRenderer;
            DdsLoader = new DdsLoader(graphics, factory, commandList);
        }

        #region Load DDS

        public ImGuiTexture LoadDDS(byte[] bytes)
        {
            var dds = DDS.Read(bytes);
            var texture = DdsLoader.LoadDDS(dds, bytes, string.Empty);
            return LoadVeldridTexture(texture);
        }

        public ImGuiTexture LoadDDS(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            var dds = DDS.Read(bytes);
            var texture = DdsLoader.LoadDDS(dds, bytes, string.Empty);
            return LoadVeldridTexture(texture);
        }

        #endregion

        #region Load Common

        public ImGuiTexture LoadCommon(byte[] bytes)
        {
            var simage = TextureConverter.LoadImageSharp(bytes);
            return LoadImageSharpTexture(simage);
        }

        public ImGuiTexture LoadCommon(Stream stream)
        {
            var simage = TextureConverter.LoadImageSharp(stream);
            return LoadImageSharpTexture(simage);
        }

        public ImGuiTexture LoadCommon(string path)
        {
            var simage = TextureConverter.LoadImageSharp(path);
            return LoadImageSharpTexture(simage);
        }

        #endregion

        #region Create Sampled Texture

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImGuiTexture CreateSampledTexture(uint width, uint height)
            => CreateVeldridTexture(width, height, TextureUsage.Sampled);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImGuiTexture CreateSampledTexture(Vector2 size)
            => CreateVeldridTexture((uint)size.X, (uint)size.Y, TextureUsage.Sampled);

        #endregion

        #region Destroy Texture

        public bool DestroyTexture(ImGuiTexture texture)
        {
            ImGuiRenderer.RemoveImGuiBinding(texture.Texture);
            CommandListDirty = true;
            return true;
        }

        #endregion

        #region Helpers

        private ImGuiTexture CreateVeldridTexture(uint width, uint height, TextureUsage usage)
        {
            var desc = TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, usage);
            var dimage = Factory.CreateTexture(desc);
            return LoadVeldridTexture(dimage);
        }

        private ImGuiTexture LoadImageSharpTexture(Image<Rgba32> texture)
        {
            var vsimage = new ImageSharpTexture<Rgba32>(texture);
            var dimage = vsimage.CreateDeviceTexture(Graphics, Factory);
            return LoadVeldridTexture(dimage);
        }

        private ImGuiTexture LoadVeldridTexture(Texture texture)
        {
            nint handle = ImGuiRenderer.GetOrCreateImGuiBinding(Factory, texture);
            return new ImGuiTexture(this, texture, handle);
        }

        #endregion
    }
}
