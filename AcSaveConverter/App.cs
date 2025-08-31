using AcSaveConverter.Graphics;
using AcSaveConverter.Interface;
using AcSaveConverter.Resources;
using ImGuiNET;
using System;
using System.Diagnostics;
using System.Numerics;

namespace AcSaveConverter
{
    public class App : IDisposable
    {
        private readonly GraphicsContext Graphics;
        private readonly ResourceHandler ResourceHandler;
        private readonly EditorHandler EditorHandler;
        private readonly WindowHandler WindowHandler;
        private int FrameCount;
        private bool disposedValue;

        public App()
        {
            int x = AppConfig.Current.WindowX;
            int y = AppConfig.Current.WindowY;
            int width = AppConfig.Current.WindowWidth;
            int height = AppConfig.Current.WindowHeight;
            Graphics = new GraphicsContext(x, y, width, height, AppInfo.AppName);
            ResourceHandler = new ResourceHandler(Graphics.TexturePool);
            EditorHandler = new EditorHandler(ResourceHandler);
            WindowHandler = new WindowHandler();
        }

        public void Run()
        {
            long previousFrameTicks = 0;
            Stopwatch sw = new();
            sw.Start();
            while (Graphics.Exists)
            {
                var currentFrameTicks = sw.ElapsedTicks;
                var deltaSeconds = (currentFrameTicks - previousFrameTicks) / (float)Stopwatch.Frequency;
                previousFrameTicks = currentFrameTicks;
                Update(deltaSeconds);
                if (!Graphics.Exists)
                    break;

                Draw();
            }
        }

        private void Update(float deltaTime)
        {
            Graphics.Update(deltaTime);
            EditorHandler.Update(deltaTime);
            OnGui();
        }

        private void Draw()
        {
            Graphics.Draw();
        }

        private void OnGui()
        {
            ImGuiViewportPtr vp = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(vp.Pos);
            ImGui.SetNextWindowSize(vp.Size);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGuiWindowFlags flags =
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoResize |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoDocking |
                ImGuiWindowFlags.MenuBar |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoNavFocus |
                ImGuiWindowFlags.NoBackground;

            ImGui.Begin("DockSpace_W", flags);
            var dsid = ImGui.GetID("DockSpace");
            ImGui.DockSpace(dsid, Vector2.Zero, ImGuiDockNodeFlags.NoDockingSplit);
            ImGui.PopStyleVar();
            ImGui.End();

            if (ImGui.BeginMainMenuBar())
            {
                EditorHandler.OnMenuGui();
                WindowHandler.OnMenuGui();
                ImGui.EndMainMenuBar();
            }

            if (FrameCount < 2)
            {
                // Why is ImGui so deadset on focusing the last window here no matter what unless I do this
                ImGui.SetNextWindowFocus();
            }

            WindowHandler.OnGui();
            foreach (var editor in EditorHandler.Editors)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
                if (ImGui.Begin(editor.EditorName))
                {
                    ImGui.PopStyleVar(1);
                    editor.OnGui();
                    ImGui.End();
                    EditorHandler.FocusedEditor = editor;
                }
                else
                {
                    ImGui.PopStyleVar(1);
                    ImGui.End();
                }
            }

            if (FrameCount < 2)
            {
                FrameCount++;
            }
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EditorHandler.Dispose();
                    ResourceHandler.Dispose();
                    Graphics.Dispose();
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
