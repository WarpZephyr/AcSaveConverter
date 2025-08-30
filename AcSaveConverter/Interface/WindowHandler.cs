using ImGuiNET;

namespace AcSaveConverter.Interface
{
    public class WindowHandler
    {
        private bool ShowImGuiDemoWindow;
        private bool ShowImGuiMetricsWindow;
        private bool ShowImGuiDebugLogWindow;
        private bool ShowImGuiStackToolWindow;
        private bool ShowImGuiAboutWindow;

        public WindowHandler()
        {

        }

        public void OnMenuGui()
        {
            DebugDropdown();
        }

        public void OnGui()
        {
            if (ShowImGuiDemoWindow)
                ImGui.ShowDemoWindow(ref ShowImGuiDemoWindow);

            if (ShowImGuiMetricsWindow)
                ImGui.ShowMetricsWindow(ref ShowImGuiMetricsWindow);

            if (ShowImGuiDebugLogWindow)
                ImGui.ShowDebugLogWindow(ref ShowImGuiDebugLogWindow);

            if (ShowImGuiStackToolWindow)
                ImGui.ShowIDStackToolWindow(ref ShowImGuiStackToolWindow);

            if (ShowImGuiAboutWindow)
                ImGui.ShowAboutWindow(ref ShowImGuiAboutWindow);
        }

        public void DebugDropdown()
        {
            if (ImGui.BeginMenu("Debug"))
            {
                if (ImGui.MenuItem($"ImGui Demo"))
                {
                    ShowImGuiDemoWindow = !ShowImGuiDemoWindow;
                }

                if (ImGui.MenuItem($"ImGui Metrics"))
                {
                    ShowImGuiMetricsWindow = !ShowImGuiMetricsWindow;
                }

                if (ImGui.MenuItem($"ImGui Log"))
                {
                    ShowImGuiDebugLogWindow = !ShowImGuiDebugLogWindow;
                }

                if (ImGui.MenuItem($"ImGui Stack Tool"))
                {
                    ShowImGuiStackToolWindow = !ShowImGuiStackToolWindow;
                }

                if (ImGui.MenuItem($"ImGui About"))
                {
                    ShowImGuiAboutWindow = !ShowImGuiAboutWindow;
                }
                ImGui.EndMenu();
            }
        }
    }
}
