using AcSaveFormats.Textures;
using System.IO;
using Veldrid;

namespace AcSaveConverter.Graphics
{
    public class GuiTexturePool
    {
        private readonly GraphicsDevice Graphics;
        private readonly ResourceFactory Factory;
        private readonly ImGuiRenderer ImGuiRenderer;
        private readonly DdsLoader DdsLoader;
        public bool CommandListDirty { get; set; }

        internal GuiTexturePool(GraphicsDevice graphics, ResourceFactory factory, CommandList commandList, ImGuiRenderer imGuiRenderer)
        {
            Graphics = graphics;
            Factory = factory;
            ImGuiRenderer = imGuiRenderer;
            DdsLoader = new DdsLoader(graphics, factory, commandList);
        }

        #region Load DDS

        public TextureHandle LoadDDS(byte[] bytes)
        {
            var dds = DDS.Read(bytes);
            var texture = DdsLoader.LoadDds(dds, bytes, string.Empty);
            return LoadVeldridTexture(texture);
        }

        public TextureHandle LoadDDS(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            var dds = DDS.Read(bytes);
            var texture = DdsLoader.LoadDds(dds, bytes, string.Empty);
            return LoadVeldridTexture(texture);
        }

        #endregion

        #region Destroy Texture

        public bool DestroyTexture(TextureHandle texture)
        {
            ImGuiRenderer.RemoveImGuiBinding(texture.Texture);
            CommandListDirty = true;
            return true;
        }

        #endregion

        #region Helpers

        private TextureHandle LoadVeldridTexture(Texture texture)
        {
            nint handle = ImGuiRenderer.GetOrCreateImGuiBinding(Factory, texture);
            return new TextureHandle(this, texture, handle);
        }

        #endregion
    }
}
