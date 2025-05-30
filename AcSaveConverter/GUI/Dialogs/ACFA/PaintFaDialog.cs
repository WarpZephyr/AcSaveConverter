using AcSaveConverter.Graphics;
using AcSaveConverter.GUI.Dialogs.Tabs;
using AcSaveConverter.IO;
using AcSaveConverter.Logging;
using AcSaveFormats.ACFA;
using AcSaveFormats.ACFA.Colors;
using ImGuiNET;
using NativeFileDialogSharp;
using System;
using System.Drawing;
using System.Numerics;

namespace AcSaveConverter.GUI.Dialogs.ACFA
{
    public class PaintFaDialog : IDataTab
    {
        private readonly ImGuiGraphicsContext Graphics;
        private bool disposedValue;
        private static Vector2 PaletteButtonSize = new Vector2(20, 20);

        public string Name { get; set; }
        public string DataType
            => "Paint";

        public Paint Paint { get; set; }

        public PaintFaDialog(string name, ImGuiGraphicsContext graphics, Paint data)
        {
            Graphics = graphics;
            Name = name;

            Paint = data;
        }

        #region Render

        public void Render()
        {
            ImGui.PushID(nameof(PaintFaDialog));
            Render_MenuBar();
            Render_Paint();
            ImGui.PopID();
        }

        void Render_MenuBar()
        {
            if (ImGui.BeginMenuBar())
            {
                Render_FileMenu();
                ImGui.EndMenuBar();
            }
        }

        void Render_FileMenu()
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Open"))
                {
                    string? file = FileDialog.OpenFile();
                    if (FileDialog.ValidFile(file))
                    {
                        Load_Data(file);
                    }
                }

                ImGui.EndMenu();
            }
        }

        void Render_Paint()
        {
            ImGui.SeparatorText("User ColorSets");
            for (int i = 0; i < Paint.ColorSetCount; i++)
            {
                ImGui.PushID(i);
                if ((i % 4) != 0)
                    ImGui.SameLine(0.0f, ImGui.GetStyle().ItemSpacing.Y);

                var colorset = Paint.ColorSets[i];
                Span<Color> colorSpan = [colorset.Main, colorset.Sub, colorset.Support, colorset.Optional, colorset.Joint, colorset.Device];
                if (ImGuiEx.ColorSetEdit4("##colorsets", colorSpan, ImGuiColorEditFlags.NoLabel))
                {
                    UpdateColorSet(ref colorset, colorSpan);
                    Paint.ColorSets[i] = colorset;
                }

                ImGui.PopID();
            }

            ImGui.SeparatorText("User Palette");
            for (int i = 0; i < Paint.PaletteCount; i++)
            {
                ImGui.PushID(i);
                if ((i % 12) != 0)
                    ImGui.SameLine(0.0f, ImGui.GetStyle().ItemSpacing.Y);

                var color = Paint.UserPalette[i];
                if (ImGuiEx.ColorEdit4("##palette", ref color, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel | ImGuiColorEditFlags.NoTooltip))
                {
                    Paint.UserPalette[i] = color;
                }

                ImGui.PopID();
            }
        }

        #endregion

        #region ColorSet

        static void UpdateColorSet(ref ColorSet colorset, Span<Color> colorSpan)
        {
            colorset.Main = colorSpan[0];
            colorset.Sub = colorSpan[1];
            colorset.Support = colorSpan[2];
            colorset.Optional = colorSpan[3];
            colorset.Joint = colorSpan[4];
            colorset.Device = colorSpan[5];
        }

        #endregion

        #region Data

        public void Load_Data(Paint data)
        {
            Paint = data;
        }

        public void Load_Data(string path)
        {
            try
            {
                Log.WriteLine($"Loading {DataType} from path: \"{path}\"");
                Load_Data(Paint.Read(path));
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Failed to load {DataType} from path \"{path}\": {ex}");
            }
        }

        public bool IsData(string path)
        {
            return path.EndsWith("PAINT.DAT");
        }

        #endregion

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
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
