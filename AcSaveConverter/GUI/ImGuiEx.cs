using AcSaveConverterImGui.Drawing;
using ImGuiNET;
using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AcSaveConverterImGui.GUI
{
    internal static class ImGuiEx
    {
        public static unsafe void DisableIni()
        {
            var io = ImGui.GetIO();
            io.NativePtr->IniFilename = null;
        }

        #region SimpleVerticalTab

        public static bool SimpleVerticalTab(string label, int tabIndex, ref int selectedTabIndex)
        {
            if (ImGui.Selectable(label, selectedTabIndex == tabIndex))
            {
                selectedTabIndex = tabIndex;
                return true;
            }

            return false;
        }

        public static bool SimpleVerticalTab(ReadOnlySpan<char> label, int tabIndex, ref int selectedTabIndex)
        {
            if (ImGui.Selectable(label, selectedTabIndex == tabIndex))
            {
                selectedTabIndex = tabIndex;
                return true;
            }

            return false;
        }

        #endregion

        #region ComboEnum

        public static bool ComboEnum<TEnum>(string label, ref TEnum selectedEnum) where TEnum : struct, Enum
        {
            string[] enumNames = EnumCache<TEnum>.GetEnumNames();
            int selectedIndex = EnumCache<TEnum>.GetEnumIndex(selectedEnum);
            if (ImGui.Combo(label, ref selectedIndex, enumNames, enumNames.Length))
            {
                selectedEnum = EnumCache<TEnum>.GetEnumValue(selectedIndex);
                return true;
            }

            return false;
        }

        public static bool ComboEnum<TEnum>(ReadOnlySpan<char> label, ref TEnum selectedEnum) where TEnum : struct, Enum
        {
            string[] enumNames = EnumCache<TEnum>.GetEnumNames();
            int selectedIndex = EnumCache<TEnum>.GetEnumIndex(selectedEnum);
            if (ImGui.Combo(label, ref selectedIndex, enumNames, enumNames.Length))
            {
                selectedEnum = EnumCache<TEnum>.GetEnumValue(selectedIndex);
                return true;
            }

            return false;
        }

        #endregion

        #region ColorEdit3

        public static bool ColorEdit3(string label, ref Color col)
        {
            var vec = col.ToRgbaVector3();
            if (ImGui.ColorEdit3(label, ref vec))
            {
                col = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorEdit3(string label, ref Color col, ImGuiColorEditFlags flags)
        {
            var vec = col.ToRgbaVector3();
            if (ImGui.ColorEdit3(label, ref vec, flags))
            {
                col = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorEdit3(ReadOnlySpan<char> label, ref Color col)
        {
            var vec = col.ToRgbaVector3();
            if (ImGui.ColorEdit3(label, ref vec))
            {
                col = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorEdit3(ReadOnlySpan<char> label, ref Color col, ImGuiColorEditFlags flags)
        {
            var vec = col.ToRgbaVector3();
            if (ImGui.ColorEdit3(label, ref vec, flags))
            {
                col = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        #endregion

        #region ColorEdit4

        public static bool ColorEdit4(string label, ref Color col, ImGuiColorEditFlags flags)
        {
            var vec = col.ToRgbaVector4();
            if (ImGui.ColorEdit4(label, ref vec, flags))
            {
                col = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorEdit4(string label, ref Color col)
        {
            var vec = col.ToRgbaVector4();
            if (ImGui.ColorEdit4(label, ref vec))
            {
                col = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorEdit4(ReadOnlySpan<char> label, ref Color col)
        {
            var vec = col.ToRgbaVector4();
            if (ImGui.ColorEdit4(label, ref vec))
            {
                col = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorEdit4(ReadOnlySpan<char> label, ref Color col, ImGuiColorEditFlags flags)
        {
            var vec = col.ToRgbaVector4();
            if (ImGui.ColorEdit4(label, ref vec, flags))
            {
                col = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        #endregion

        #region ColorButton

        public static bool ColorButton(string desc_id, Color col)
        {
            if (ImGui.ColorButton(desc_id, col.ToRgbaVector4()))
            {
                return true;
            }

            return false;
        }

        public static bool ColorButton(string desc_id, Color col, ImGuiColorEditFlags flags)
        {
            if (ImGui.ColorButton(desc_id, col.ToRgbaVector4(), flags))
            {
                return true;
            }

            return false;
        }

        public static bool ColorButton(string desc_id, Color col, ImGuiColorEditFlags flags, Vector2 size)
        {
            if (ImGui.ColorButton(desc_id, col.ToRgbaVector4(), flags, size))
            {
                return true;
            }

            return false;
        }

        public static bool ColorButton(ReadOnlySpan<char> desc_id, Color col)
        {
            if (ImGui.ColorButton(desc_id, col.ToRgbaVector4()))
            {
                return true;
            }

            return false;
        }

        public static bool ColorButton(ReadOnlySpan<char> desc_id, Color col, ImGuiColorEditFlags flags)
        {
            if (ImGui.ColorButton(desc_id, col.ToRgbaVector4(), flags))
            {
                return true;
            }

            return false;
        }

        public static bool ColorButton(ReadOnlySpan<char> desc_id, Color col, ImGuiColorEditFlags flags, Vector2 size)
        {
            if (ImGui.ColorButton(desc_id, col.ToRgbaVector4(), flags, size))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region ColorPicker3

        public static bool ColorPicker3(string label, ref Color color)
        {
            var vec = color.ToRgbaVector3();
            if (ImGui.ColorPicker3(label, ref vec))
            {
                color = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorPicker3(string label, ref Color color, ImGuiColorEditFlags flags)
        {
            var vec = color.ToRgbaVector3();
            if (ImGui.ColorPicker3(label, ref vec, flags))
            {
                color = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorPicker3(ReadOnlySpan<char> label, ref Color color)
        {
            var vec = color.ToRgbaVector3();
            if (ImGui.ColorPicker3(label, ref vec))
            {
                color = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorPicker3(ReadOnlySpan<char> label, ref Color color, ImGuiColorEditFlags flags)
        {
            var vec = color.ToRgbaVector3();
            if (ImGui.ColorPicker3(label, ref vec, flags))
            {
                color = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        #endregion

        #region ColorPicker4

        public static bool ColorPicker4(string label, ref Color color)
        {
            var vec = color.ToRgbaVector4();
            if (ImGui.ColorPicker4(label, ref vec))
            {
                color = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorPicker4(string label, ref Color color, ImGuiColorEditFlags flags)
        {
            var vec = color.ToRgbaVector4();
            if (ImGui.ColorPicker4(label, ref vec, flags))
            {
                color = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorPicker4(ReadOnlySpan<char> label, ref Color color)
        {
            var vec = color.ToRgbaVector4();
            if (ImGui.ColorPicker4(label, ref vec))
            {
                color = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        public static bool ColorPiColorPicker4cker3(ReadOnlySpan<char> label, ref Color color, ImGuiColorEditFlags flags)
        {
            var vec = color.ToRgbaVector4();
            if (ImGui.ColorPicker4(label, ref vec, flags))
            {
                color = vec.ToRgbaColor();
                return true;
            }

            return false;
        }

        #endregion

        #region ColorSetButton

        public static bool ColorSetButton(string label, Span<Color> colors, ImGuiColorEditFlags flags, Vector2 colorSize)
        {
            ImGui.PushID(label);
            ImGui.BeginGroup();

            bool buttonPressed = false;
            for (int i = 0; i < colors.Length; i++)
            {
                ImGui.PushID(i);
                if (ColorButton("##colorset", colors[i], flags, colorSize))
                {
                    buttonPressed = true;
                }

                ImGui.SameLine(0.0f, 0.0f);
                ImGui.PopID();
            }

            ImGui.EndGroup();
            ImGui.PopID();
            return buttonPressed;
        }

        #endregion

        #region ColorSetEdit4

        public static bool ColorSetEdit4(string label, Span<Color> colors, ImGuiColorEditFlags flags)
        {
            ImGui.PushID(label);
            ImGui.BeginGroup();

            bool valueChanged = false;
            for (int i = 0; i < colors.Length; i++)
            {
                ImGui.PushID(i);

                var color = colors[i];
                if (ColorEdit4("##colorset", ref color, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel | ImGuiColorEditFlags.NoTooltip))
                {
                    colors[i] = color;
                    valueChanged = true;
                }

                ImGui.SameLine(0.0f, 0.0f);
                ImGui.PopID();
            }

            ImGui.SameLine();

            if ((flags & ImGuiColorEditFlags.NoLabel) == 0)
            {
                ImGui.Text(label);
            }

            ImGui.EndGroup();
            ImGui.PopID();
            return valueChanged;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ColorSetEdit4(string label, Span<Color> colors)
            => ColorSetEdit4(label, colors, ImGuiColorEditFlags.None);

        #endregion
    }
}
