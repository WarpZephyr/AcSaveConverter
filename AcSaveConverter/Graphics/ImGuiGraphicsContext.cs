using AcSaveConverter.Graphics.Fonts;
using AcSaveConverter.IO.Assets;
using AcSaveConverter.Logging;
using ImGuiNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace AcSaveConverter.Graphics
{
    public class ImGuiGraphicsContext : IDisposable
    {
        private readonly GraphicsDevice GraphicsDevice;
        private readonly CommandList CommandList;
        private readonly ImGuiRenderer ImGuiRenderer;
        public readonly ImGuiTexturePool TexturePool;
        public readonly Window Window;
        public readonly UI UI;
        public readonly DPI DPI;
        private bool disposedValue;

        public event Action? Render;

        public ImGuiGraphicsContext(int x, int y, int width, int height, string title)
        {
            var wci = new WindowCreateInfo(x, y, width, height, WindowState.Normal, title);
            VeldridStartup.CreateWindowAndGraphicsDevice(wci, out Sdl2Window sdl2Window, out GraphicsDevice);
            var swapchain = GraphicsDevice.MainSwapchain;
            var framebuffer = swapchain.Framebuffer;
            var outDesc = framebuffer.OutputDescription;
            int bwidth = (int)framebuffer.Width;
            int bheight = (int)framebuffer.Height;

            ImGuiRenderer = new ImGuiRenderer(GraphicsDevice, outDesc, bwidth, bheight);
            CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();
            TexturePool = new ImGuiTexturePool(GraphicsDevice, GraphicsDevice.ResourceFactory, CommandList, ImGuiRenderer);
            Window = new Window(sdl2Window);
            UI = new UI();
            DPI = new DPI(UI);
            DPI.UpdateDPI(Window.GetDisplayDPI());

            sdl2Window.Resized += () =>
            {
                ImGuiRenderer.WindowResized(sdl2Window.Width, sdl2Window.Height);
                GraphicsDevice.MainSwapchain.Resize((uint)sdl2Window.Width, (uint)sdl2Window.Height);
            };

            SetupFonts();
        }

        // Credit for this code goes to Smithbox and by extension DSMapStudio
        private unsafe void SetupFonts()
        {
            Log.DirectWriteLine("Setting up fonts for graphics.");

            ImGui.EndFrame();
            string engFontPath = FontPath.GetFontPath("RobotoMono-Light.ttf");
            string otherFontPath = FontPath.GetFontPath("NotoSansCJKtc-Light.otf");

            string uiEngFontPath = FontPath.GetFontPath(UI.FontEnglish);
            string uiOtherFontPath = FontPath.GetFontPath(UI.FontOther);
            if (FontPath.FontPathExists(uiEngFontPath))
                engFontPath = uiEngFontPath;
            if (FontPath.FontPathExists(uiOtherFontPath))
                otherFontPath = uiOtherFontPath;

            ImFontAtlasPtr fonts = ImGui.GetIO().Fonts;
            var fontEn = File.ReadAllBytes(engFontPath);
            var fontEnNative = ImGui.MemAlloc((uint)fontEn.Length);
            Marshal.Copy(fontEn, 0, fontEnNative, fontEn.Length);

            var fontOther = File.ReadAllBytes(otherFontPath);
            var fontOtherNative = ImGui.MemAlloc((uint)fontOther.Length);
            Marshal.Copy(fontOther, 0, fontOtherNative, fontOther.Length);

            var iconFontPath = FontPath.GetFontPath("forkawesome-webfont.ttf");
            var fontIcon = File.ReadAllBytes(iconFontPath);
            var fontIconNative = ImGui.MemAlloc((uint)fontIcon.Length);
            Marshal.Copy(fontIcon, 0, fontIconNative, fontIcon.Length);
            fonts.Clear();

            var scale = DPI.GetUIScale();

            // English fonts
            {
                ImFontConfig* ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                ImFontConfigPtr cfg = new(ptr);
                cfg.GlyphMinAdvanceX = 5.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;
                fonts.AddFontFromMemoryTTF(fontEnNative, fontIcon.Length, (float)Math.Round(UI.InterfaceFontSize * scale), cfg,
                    fonts.GetGlyphRangesDefault());
            }

            // Other language fonts
            {
                ImFontConfig* ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                ImFontConfigPtr cfg = new(ptr);
                cfg.MergeMode = true;
                cfg.GlyphMinAdvanceX = 7.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;

                ImFontGlyphRangesBuilderPtr glyphRanges =
                    new(ImGuiNative.ImFontGlyphRangesBuilder_ImFontGlyphRangesBuilder());
                glyphRanges.AddRanges(fonts.GetGlyphRangesJapanese());
                Array.ForEach(FontGlyphRangeUtil.SpecialCharsJP, c => glyphRanges.AddChar(c));

                if (UI.UseFontChinese)
                {
                    glyphRanges.AddRanges(fonts.GetGlyphRangesChineseFull());
                }

                if (UI.UseFontKorean)
                {
                    glyphRanges.AddRanges(fonts.GetGlyphRangesKorean());
                }

                if (UI.UseFontThai)
                {
                    glyphRanges.AddRanges(fonts.GetGlyphRangesThai());
                }

                if (UI.UseFontVietnamese)
                {
                    glyphRanges.AddRanges(fonts.GetGlyphRangesVietnamese());
                }

                if (UI.UseFontCyrillic)
                {
                    glyphRanges.AddRanges(fonts.GetGlyphRangesCyrillic());
                }

                glyphRanges.BuildRanges(out ImVector glyphRange);
                fonts.AddFontFromMemoryTTF(fontOtherNative, fontOther.Length, (float)Math.Round((UI.InterfaceFontSize + 2) * scale), cfg, glyphRange.Data);
                glyphRanges.Destroy();
            }

            // Icon fonts
            {
                ushort[] ranges = { ForkAwesomeFont.IconMin, ForkAwesomeFont.IconMax, 0 };
                ImFontConfig* ptr = ImGuiNative.ImFontConfig_ImFontConfig();
                ImFontConfigPtr cfg = new(ptr);
                cfg.MergeMode = true;
                cfg.GlyphMinAdvanceX = 12.0f;
                cfg.OversampleH = 5;
                cfg.OversampleV = 5;

                fixed (ushort* r = ranges)
                {
                    ImFontPtr f = fonts.AddFontFromMemoryTTF(fontIconNative, fontIcon.Length, (float)Math.Round((UI.InterfaceFontSize + 2) * scale), cfg,
                        (IntPtr)r);
                }
            }

            ImGuiRenderer.RecreateFontDeviceTexture(GraphicsDevice);
            ImGui.NewFrame();
        }

        public void Run()
        {
            long previousFrameTicks = 0;
            Stopwatch sw = new();
            sw.Start();
            while (Window.Exists)
            {
                var input = Window.PumpEvents();
                if (!Window.Exists)
                    break;

                DPI.UpdateDPI(Window.GetDisplayDPI());
                var currentFrameTicks = sw.ElapsedTicks;
                var deltaSeconds = (currentFrameTicks - previousFrameTicks) / (float)Stopwatch.Frequency;
                previousFrameTicks = currentFrameTicks;
                ImGuiRenderer.Update(deltaSeconds, input);

                // Draw GUI
                Render?.Invoke();

                CommandList.Begin();
                CommandList.SetFramebuffer(GraphicsDevice.MainSwapchain.Framebuffer);
                CommandList.ClearColorTarget(0, Window.BgColorInternal);

                // This feels hacky
                if (!TexturePool.CommandListDirty)
                {
                    ImGuiRenderer.Render(GraphicsDevice, CommandList);
                }
                else
                {
                    TexturePool.CommandListDirty = false;
                }

                CommandList.End();
                GraphicsDevice.SubmitCommands(CommandList);
                GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
            }
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ImGuiRenderer.Dispose();
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
