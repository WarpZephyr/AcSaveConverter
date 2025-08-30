using AcSaveConverter.Editors.Framework;
using AcSaveConverter.Interface;
using AcSaveConverter.Logging;
using AcSaveFormats.ArmoredCoreForAnswer;
using AcSaveFormats.ArmoredCoreForAnswer.Colors;
using ImGuiNET;
using System;
using System.Drawing;

namespace AcSaveConverter.Editors.AcfaEditor.Views
{
    public class PaintView
    {
        private Paint Data;

        public PaintView()
        {
            Data = new Paint();
        }

        public void Display()
        {
            EditorDecorator.SetupWindow();
            if (ImGui.Begin("Paint"))
            {
                ShowProperties(Data);
            }

            ImGui.End();
        }

        private void ShowProperties(Paint data)
        {
            ImGui.SeparatorText("User ColorSets");
            for (int i = 0; i < Paint.ColorSetCount; i++)
            {
                ImGui.PushID(i);
                if ((i % 4) != 0)
                    ImGui.SameLine(0.0f, ImGui.GetStyle().ItemSpacing.Y);

                var colorset = data.ColorSets[i];
                Span<Color> colorSpan = [colorset.Main, colorset.Sub, colorset.Support, colorset.Optional, colorset.Joint, colorset.Device];
                if (ImGuiEx.ColorSetEdit4("##colorsets", colorSpan, ImGuiColorEditFlags.NoLabel))
                {
                    UpdateColorSet(ref colorset, colorSpan);
                    data.ColorSets[i] = colorset;
                }

                ImGui.PopID();
            }

            ImGui.SeparatorText("User Palette");
            for (int i = 0; i < Paint.PaletteCount; i++)
            {
                ImGui.PushID(i);
                if ((i % 12) != 0)
                    ImGui.SameLine(0.0f, ImGui.GetStyle().ItemSpacing.Y);

                var color = data.UserPalette[i];
                if (ImGuiEx.ColorEdit4("##palette", ref color, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel | ImGuiColorEditFlags.NoTooltip))
                {
                    data.UserPalette[i] = color;
                }

                ImGui.PopID();
            }
        }

        private static void UpdateColorSet(ref ColorSet colorset, Span<Color> colorSpan)
        {
            colorset.Main = colorSpan[0];
            colorset.Sub = colorSpan[1];
            colorset.Support = colorSpan[2];
            colorset.Optional = colorSpan[3];
            colorset.Joint = colorSpan[4];
            colorset.Device = colorSpan[5];
        }

        public void Load(string path)
        {
            try
            {
                var data = Paint.Read(path);
                Load(data);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Error: Paint load failed: {ex.Message}");
            }
        }

        public void Load(Paint data)
        {
            Data = data;
        }

        public Paint GetData()
            => Data;
    }
}
