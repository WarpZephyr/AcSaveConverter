using AcSaveConverter.Interface;
using ImGuiNET;
using System.Numerics;

namespace AcSaveConverter.Editors.Framework
{
    internal class EditorDecorator
    {
        public static void SetupWindow()
        {
            var scale = DPI.Current.GetUIScale();
            ImGui.SetNextWindowSize(new Vector2(300.0f, 200.0f) * scale, ImGuiCond.FirstUseEver);
        }
    }
}
