using AcSaveConverterImGui.Graphics;
using AcSaveConverterImGui.GUI.Dialogs.ACFA;
using AcSaveConverterImGui.GUI.Dialogs.Popups;
using AcSaveConverterImGui.GUI.Dialogs.Tabs;
using ImGuiNET;
using System.Numerics;

namespace AcSaveConverterImGui.GUI.Windows
{
    public class ConverterWindow : IDisposable
    {
        private readonly ImGuiGraphicsContext Graphics;
        private readonly Window Window;

        private readonly List<ISaveTab> FileTabs;
        private readonly NewTabPopup NewTabPopup;
        private bool disposedValue;
        public bool IsDisposed => disposedValue;

        private bool ShowImGuiAbout;
        private bool ShowImGuiDebugLog;
        private bool ShowImGuiDemo;
        private bool ShowImGuiMetrics;

        public ConverterWindow(ImGuiGraphicsContext graphics, Window window)
        {
            Graphics = graphics;
            Graphics.Render += Render;
            Window = window;

            FileTabs = [];
            NewTabPopup = new NewTabPopup(FileTabs, Graphics);
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(ConverterWindow));

            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(Window.Size);
            if (ImGui.Begin("Converter", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.MenuBar))
            {
                Render_MenuBar();
                Render_FileTabBar();

                NewTabPopup.Render();

                Render_ImGuiDebug();
                ImGui.End();
            }

            ImGui.PopID();
        }

        void Render_MenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                Render_FileMenu();
                Render_ImGuiMenu();
                ImGui.EndMenuBar();
            }
        }

        void Render_FileMenu()
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New Tab"))
                {
                    NewTabPopup.OpenPopup = true;
                }

                ImGui.EndMenu();
            }
        }

        void Render_ImGuiMenu()
        {
            if (ImGui.BeginMenu("ImGui"))
            {
                if (ImGui.MenuItem("ImGui About"))
                {
                    ShowImGuiAbout = !ShowImGuiAbout;
                }

                if (ImGui.MenuItem("ImGui Debug Log"))
                {
                    ShowImGuiDebugLog = !ShowImGuiDebugLog;
                }

                if (ImGui.MenuItem("ImGui Demo"))
                {
                    ShowImGuiDemo = !ShowImGuiDemo;
                }

                if (ImGui.MenuItem("ImGui Metrics"))
                {
                    ShowImGuiMetrics = !ShowImGuiMetrics;
                }

                ImGui.EndMenu();
            }
        }

        void Render_ImGuiDebug()
        {
            if (ShowImGuiAbout)
            {
                ImGui.ShowAboutWindow(ref ShowImGuiAbout);
            }

            if (ShowImGuiDebugLog)
            {
                ImGui.ShowDebugLogWindow(ref ShowImGuiDebugLog);
            }

            if (ShowImGuiDemo)
            {
                ImGui.ShowDemoWindow(ref ShowImGuiDemo);
            }

            if (ShowImGuiMetrics)
            {
                ImGui.ShowMetricsWindow(ref ShowImGuiMetrics);
            }
        }

        void Render_FileTabBar()
        {
            if (ImGui.BeginTabBar("File Tab Bar", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.NoCloseWithMiddleMouseButton))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
                if (ImGui.TabItemButton("?", ImGuiTabItemFlags.Leading))
                {
                    ImGui.OpenPopup("TabHelpMenu");
                }
                ImGui.PopStyleVar();

                if (ImGui.BeginPopup("TabHelpMenu"))
                {
                    ImGui.Text("These are save tabs.");
                    ImGui.Text("The type of the tab is determined when you create it.");
                    ImGui.Text("Adding a tab via the tab bar will use your current settings for making tabs.");
                    ImGui.Text("Set tab settings or make a tab in File->New Tab");
                    ImGui.EndPopup();
                }

                for (int i = FileTabs.Count - 1; i >= 0; i--)
                {
                    if (FileTabs[i].Open == false)
                    {
                        FileTabs[i].Dispose();
                        FileTabs.RemoveAt(i);
                        continue;
                    }

                    ImGui.PushID(i);
                    FileTabs[i].Render();
                    ImGui.PopID();
                }

                ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3.0f);
                if (ImGui.TabItemButton("+", ImGuiTabItemFlags.Trailing))
                {
                    NewTabPopup.Press_Create();
                }
                ImGui.PopStyleVar();

                ImGui.EndTabBar();
            }
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Graphics.Render -= Render;
                    foreach (var tab in FileTabs)
                    {
                        tab.Dispose();
                    }

                    DesignFaDialog.InvalidateDefaultThumbnailCache();
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
