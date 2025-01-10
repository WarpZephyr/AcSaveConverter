using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using System.Runtime.CompilerServices;
using Veldrid;
using Veldrid.ImageSharp;

namespace AcSaveConverterImGui.Graphics
{
    public class ImGuiTexturePool
    {
        private readonly GraphicsDevice Graphics;
        private readonly ImGuiRenderer ImGuiRenderer;

        internal ImGuiTexturePool(GraphicsDevice graphics, ImGuiRenderer imGuiRenderer)
        {
            Graphics = graphics;
            ImGuiRenderer = imGuiRenderer;
        }

        #region Load DDS

        public ImGuiTexture LoadDDS(byte[] bytes)
        {
            var simage = TextureConverter.LoadPfimImageSharp(bytes);
            return LoadImageSharpTexture(simage.CloneAs<Rgba32>());
        }

        public ImGuiTexture LoadDDS(Stream stream)
        {
            var simage = TextureConverter.LoadPfimImageSharp(stream);
            return LoadImageSharpTexture(simage.CloneAs<Rgba32>());
        }

        public ImGuiTexture LoadDDS(string path)
        {
            var simage = TextureConverter.LoadPfimImageSharp(path);
            return LoadImageSharpTexture(simage.CloneAs<Rgba32>());
        }

        #endregion

        #region Load Common

        public ImGuiTexture LoadCommon(byte[] bytes)
        {
            var simage = TextureConverter.LoadImageSharp(bytes);
            return LoadImageSharpTexture(simage.CloneAs<Rgba32>());
        }

        public ImGuiTexture LoadCommon(Stream stream)
        {
            var simage = TextureConverter.LoadImageSharp(stream);
            return LoadImageSharpTexture(simage.CloneAs<Rgba32>());
        }

        public ImGuiTexture LoadCommon(string path)
        {
            var simage = TextureConverter.LoadImageSharp(path);
            return LoadImageSharpTexture(simage.CloneAs<Rgba32>());
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
            return true;
        }

        #endregion

        #region Helpers

        private ImGuiTexture CreateVeldridTexture(uint width, uint height, TextureUsage usage)
        {
            var desc = TextureDescription.Texture2D(width, height, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, usage);
            var dimage = Graphics.ResourceFactory.CreateTexture(desc);
            return LoadVeldridTexture(dimage);
        }

        private ImGuiTexture LoadImageSharpTexture(Image<Rgba32> texture)
        {
            var vsimage = new ImageSharpTexture(texture);
            var dimage = vsimage.CreateDeviceTexture(Graphics, Graphics.ResourceFactory);
            return LoadVeldridTexture(dimage);
        }

        private ImGuiTexture LoadVeldridTexture(Texture texture)
        {
            nint handle = ImGuiRenderer.GetOrCreateImGuiBinding(Graphics.ResourceFactory, texture);
            return new ImGuiTexture(this, texture, handle);
        }

        #endregion
    }
}
