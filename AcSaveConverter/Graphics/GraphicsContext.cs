using AcSaveConverter.Fonts;
using AcSaveConverter.Interface;
using AcSaveConverter.Logging;
using AcSaveConverter.Resources;
using ImGuiNET;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Point = System.Drawing.Point;

namespace AcSaveConverter.Graphics
{
    public class GraphicsContext : IDisposable
    {
        private readonly static RgbaFloat BgColor;
        private readonly GraphicsDevice GraphicsDevice;
        private readonly CommandList CommandList;
        private readonly ImGuiRenderer GuiRenderer;
        private readonly Sdl2Window Window;
        public readonly GuiTexturePool TexturePool;
        private bool disposedValue;

        public bool Exists
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Window.Exists;
        }

        static GraphicsContext()
        {
            BgColor = new RgbaFloat(0f, 0f, 0f, 1f);
        }

        public GraphicsContext(int x, int y, int width, int height, string title)
        {
            var wci = new WindowCreateInfo(x, y, width, height, WindowState.Normal, title);
            VeldridStartup.CreateWindowAndGraphicsDevice(wci, out Sdl2Window sdl2Window, out GraphicsDevice);
            var swapchain = GraphicsDevice.MainSwapchain;
            var framebuffer = swapchain.Framebuffer;
            var outDesc = framebuffer.OutputDescription;
            int bwidth = (int)framebuffer.Width;
            int bheight = (int)framebuffer.Height;

            GuiRenderer = new ImGuiRenderer(GraphicsDevice, outDesc, bwidth, bheight);
            SetupFonts();
            GuiRenderer.Begin();

            CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();
            TexturePool = new GuiTexturePool(GraphicsDevice, GraphicsDevice.ResourceFactory, CommandList, GuiRenderer);

            Window = sdl2Window;
            sdl2Window.Moved += p =>
            {
                AppConfig.Current.WindowX = p.X;
                AppConfig.Current.WindowY = p.Y;
            };

            sdl2Window.Resized += () =>
            {
                GuiRenderer.WindowResized(sdl2Window.Width, sdl2Window.Height);
                GraphicsDevice.MainSwapchain.Resize((uint)sdl2Window.Width, (uint)sdl2Window.Height);
                AppConfig.Current.WindowWidth = sdl2Window.Width;
                AppConfig.Current.WindowHeight = sdl2Window.Height;
            };
        }

        public GraphicsContext(Point location, Size size, string title) : this(location.X, location.Y, size.Width, size.Height, title) { }

        public void Update(float deltaTime)
        {
            var input = Window.PumpEvents();
            DPI.Current.UpdateDPI(GetDisplayDPI());
            GuiRenderer.Update(deltaTime, input);
        }

        public void Draw()
        {
            CommandList.Begin();
            CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
            CommandList.ClearColorTarget(0, BgColor);

            // This feels hacky
            if (!TexturePool.CommandListDirty)
            {
                GuiRenderer.Render(GraphicsDevice, CommandList);
            }
            else
            {
                TexturePool.CommandListDirty = false;
            }

            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);
            GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
        }

        #region Helpers

        // Credit for this function goes to Smithbox and by extension DSMapStudio
        private unsafe void SetupFonts()
        {
            Log.DirectWriteLine("Setting up fonts for graphics.");

            string engFontPath = AssetPath.GetFontPath("RobotoMono-Light.ttf");
            string otherFontPath = AssetPath.GetFontPath("NotoSansCJKtc-Light.otf");

            string uiEngFontPath = AssetPath.GetFontPath(UI.Current.FontEnglish);
            string uiOtherFontPath = AssetPath.GetFontPath(UI.Current.FontOther);
            if (AssetPath.PathExists(uiEngFontPath))
                engFontPath = uiEngFontPath;
            if (AssetPath.PathExists(uiOtherFontPath))
                otherFontPath = uiOtherFontPath;

            ImFontAtlasPtr fonts = ImGui.GetIO().Fonts;
            var fontEn = File.ReadAllBytes(engFontPath);
            var fontEnNative = ImGui.MemAlloc((uint)fontEn.Length);
            Marshal.Copy(fontEn, 0, fontEnNative, fontEn.Length);

            var fontOther = File.ReadAllBytes(otherFontPath);
            var fontOtherNative = ImGui.MemAlloc((uint)fontOther.Length);
            Marshal.Copy(fontOther, 0, fontOtherNative, fontOther.Length);

            var iconFontPath = AssetPath.GetFontPath(FontAwesome7.FontIconFileNameSolid);
            var fontIcon = File.ReadAllBytes(iconFontPath);
            var fontIconNative = ImGui.MemAlloc((uint)fontIcon.Length);
            Marshal.Copy(fontIcon, 0, fontIconNative, fontIcon.Length);
            fonts.Clear();

            var scale = DPI.Current.GetUIScale();
            var scaleFine = (float)Math.Round(UI.Current.InterfaceFontSize * scale);
            var scaleLarge = (float)Math.Round((UI.Current.InterfaceFontSize + 2) * scale);

            // English fonts
            {
                ImFontConfig* ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                ImFontConfigPtr cfg = new(ptr);
                cfg.GlyphMinAdvanceX = 5.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;
                fonts.AddFontFromMemoryTTF(fontEnNative, fontIcon.Length, scaleFine, cfg,
                    fonts.GetGlyphRangesDefault());
            }

            // Other language fonts
            {
                ImFontConfig* ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                ImFontConfigPtr cfg = new(ptr)
                {
                    MergeMode = true,
                    GlyphMinAdvanceX = 7.0f,
                    OversampleH = 5,
                    OversampleV = 5
                };

                ImFontGlyphRangesBuilderPtr glyphRanges =
                    new(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
                glyphRanges.AddRanges(fonts.GetGlyphRangesJapanese());
                Array.ForEach(FontGlyphRanges.SpecialCharsJP, c => glyphRanges.AddChar(c));

                if (UI.Current.UseFontChinese)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesChineseFull());

                if (UI.Current.UseFontKorean)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesKorean());

                if (UI.Current.UseFontThai)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesThai());

                if (UI.Current.UseFontVietnamese)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesVietnamese());

                if (UI.Current.UseFontCyrillic)
                    glyphRanges.AddRanges(fonts.GetGlyphRangesCyrillic());

                glyphRanges.BuildRanges(out ImVector glyphRange);
                fonts.AddFontFromMemoryTTF(fontOtherNative, fontOther.Length, scaleFine, cfg, glyphRange.Data);
                glyphRanges.Destroy();
            }

            // Icon fonts
            {
                ushort[] ranges = { FontAwesome7.IconMin, FontAwesome7.IconMax, 0 };
                ImFontConfig* ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                ImFontConfigPtr cfg = new(ptr)
                {
                    MergeMode = true,
                    GlyphMinAdvanceX = 7.0f,
                    OversampleH = 5,
                    OversampleV = 5,
                    GlyphOffset = new Vector2(0f, 2f)
                };

                fixed (ushort* r = ranges)
                {
                    ImFontPtr f = fonts.AddFontFromMemoryTTF(fontIconNative, fontIcon.Length, scaleLarge, cfg, (nint)r);
                }
            }

            GuiRenderer.RecreateFontDeviceTexture(GraphicsDevice);
        }

        public float GetDisplayDPI()
        {
            float ddpi = 96f;
            float _ = 0f;
            Sdl2Ex.GetDisplayDPI(Window, ref ddpi, ref _, ref _);
            return ddpi;
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    GuiRenderer.Dispose();
                    CommandList.Dispose();
                    GraphicsDevice.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
