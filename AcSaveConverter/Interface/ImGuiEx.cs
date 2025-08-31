using AcSaveConverter.Utilities;
using ImGuiNET;
using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AcSaveConverter.Interface
{
    internal static class ImGuiEx
    {
        public static unsafe void DisableIni()
        {
            var io = ImGui.GetIO();
            io.NativePtr->IniFilename = null;
        }

        #region InputNumeric

        public static bool InputNumeric(string label, ref sbyte value)
        {
            int usable = value;
            bool result = ImGui.InputInt(label, ref usable);
            Math.Clamp(usable, sbyte.MinValue, sbyte.MaxValue);
            value = (sbyte)usable;
            return result;
        }

        public static bool InputNumeric(ReadOnlySpan<char> label, ref sbyte value)
        {
            int usable = value;
            bool result = ImGui.InputInt(label, ref usable);
            Math.Clamp(usable, sbyte.MinValue, sbyte.MaxValue);
            value = (sbyte)usable;
            return result;
        }

        public static bool InputNumeric(string label, ref byte value)
        {
            int usable = value;
            bool result = ImGui.InputInt(label, ref usable);
            Math.Clamp(usable, byte.MinValue, byte.MaxValue);
            value = (byte)usable;
            return result;
        }

        public static bool InputNumeric(ReadOnlySpan<char> label, ref byte value)
        {
            int usable = value;
            bool result = ImGui.InputInt(label, ref usable);
            Math.Clamp(usable, byte.MinValue, byte.MaxValue);
            value = (byte)usable;
            return result;
        }

        public static bool InputNumeric(string label, ref short value)
        {
            int usable = value;
            bool result = ImGui.InputInt(label, ref usable);
            Math.Clamp(usable, short.MinValue, short.MaxValue);
            value = (short)usable;
            return result;
        }

        public static bool InputNumeric(ReadOnlySpan<char> label, ref short value)
        {
            int usable = value;
            bool result = ImGui.InputInt(label, ref usable);
            Math.Clamp(usable, short.MinValue, short.MaxValue);
            value = (short)usable;
            return result;
        }

        public static bool InputNumeric(string label, ref ushort value)
        {
            int usable = value;
            bool result = ImGui.InputInt(label, ref usable);
            Math.Clamp(usable, ushort.MinValue, ushort.MaxValue);
            value = (ushort)usable;
            return result;
        }

        public static bool InputNumeric(ReadOnlySpan<char> label, ref ushort value)
        {
            int usable = value;
            bool result = ImGui.InputInt(label, ref usable);
            Math.Clamp(usable, ushort.MinValue, ushort.MaxValue);
            value = (ushort)usable;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InputNumeric(string label, ref int value)
            => ImGui.InputInt(label, ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InputNumeric(ReadOnlySpan<char> label, ref int value)
            => ImGui.InputInt(label, ref value);

        #endregion

        #region InputSByte

        public static unsafe bool InputSByte(string label, ref sbyte value)
        {
            sbyte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S8, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputSByte(string label, ref sbyte value, sbyte step)
        {
            sbyte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S8, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputSByte(string label, ref sbyte value, sbyte step, sbyte step_fast)
        {
            sbyte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S8, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputSByte(string label, ref sbyte value, sbyte step, sbyte step_fast, string format)
        {
            sbyte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S8, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputSByte(string label, ref sbyte value, sbyte step, sbyte step_fast, string format, ImGuiInputTextFlags flags)
        {
            sbyte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S8, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputByte

        public static unsafe bool InputByte(string label, ref byte value)
        {
            byte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U8, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputByte(string label, ref byte value, byte step)
        {
            byte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U8, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputByte(string label, ref byte value, byte step, byte step_fast)
        {
            byte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U8, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputByte(string label, ref byte value, byte step, byte step_fast, string format)
        {
            byte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U8, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputByte(string label, ref byte value, byte step, byte step_fast, string format, ImGuiInputTextFlags flags)
        {
            byte copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U8, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputInt16

        public static unsafe bool InputInt16(string label, ref short value)
        {
            short copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S16, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt16(string label, ref short value, short step)
        {
            short copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S16, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt16(string label, ref short value, short step, short step_fast)
        {
            short copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S16, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt16(string label, ref short value, short step, short step_fast, string format)
        {
            short copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S16, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputInt16(string label, ref short value, short step, short step_fast, string format, ImGuiInputTextFlags flags)
        {
            short copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S16, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputUInt16

        public static unsafe bool InputUInt16(string label, ref ushort value)
        {
            ushort copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U16, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt16(string label, ref ushort value, ushort step)
        {
            ushort copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U16, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt16(string label, ref ushort value, ushort step, ushort step_fast)
        {
            ushort copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U16, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt16(string label, ref ushort value, ushort step, ushort step_fast, string format)
        {
            ushort copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U16, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt16(string label, ref ushort value, ushort step, ushort step_fast, string format, ImGuiInputTextFlags flags)
        {
            ushort copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U16, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputInt32

        public static unsafe bool InputInt32(string label, ref int value)
        {
            int copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S32, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt32(string label, ref int value, int step)
        {
            int copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S32, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt32(string label, ref int value, int step, int step_fast)
        {
            int copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S32, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt32(string label, ref int value, int step, int step_fast, string format)
        {
            int copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S32, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputInt32(string label, ref int value, int step, int step_fast, string format, ImGuiInputTextFlags flags)
        {
            int copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S32, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputUInt32

        public static unsafe bool InputUInt32(string label, ref uint value)
        {
            uint copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U32, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt32(string label, ref uint value, uint step)
        {
            uint copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U32, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt32(string label, ref uint value, uint step, uint step_fast)
        {
            uint copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U32, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt32(string label, ref uint value, uint step, uint step_fast, string format)
        {
            uint copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U32, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt32(string label, ref uint value, uint step, uint step_fast, string format, ImGuiInputTextFlags flags)
        {
            uint copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U32, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputInt64

        public static unsafe bool InputInt64(string label, ref long value)
        {
            long copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt64(string label, ref long value, long step)
        {
            long copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt64(string label, ref long value, long step, long step_fast)
        {
            long copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputInt64(string label, ref long value, long step, long step_fast, string format)
        {
            long copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputInt64(string label, ref long value, long step, long step_fast, string format, ImGuiInputTextFlags flags)
        {
            long copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputUInt64

        public static unsafe bool InputUInt64(string label, ref ulong value)
        {
            ulong copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt64(string label, ref ulong value, ulong step)
        {
            ulong copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt64(string label, ref ulong value, ulong step, ulong step_fast)
        {
            ulong copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt64(string label, ref ulong value, ulong step, ulong step_fast, string format)
        {
            ulong copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputUInt64(string label, ref ulong value, ulong step, ulong step_fast, string format, ImGuiInputTextFlags flags)
        {
            ulong copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputSingle

        public static unsafe bool InputSingle(string label, ref float value)
        {
            float copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputSingle(string label, ref float value, float step)
        {
            float copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputSingle(string label, ref float value, float step, float step_fast)
        {
            float copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputSingle(string label, ref float value, float step, float step_fast, string format)
        {
            float copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputSingle(string label, ref float value, float step, float step_fast, string format, ImGuiInputTextFlags flags)
        {
            float copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.S64, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputDouble

        public static unsafe bool InputDouble(string label, ref double value)
        {
            double copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy));
            value = copy;
            return result;
        }

        public static unsafe bool InputDouble(string label, ref double value, double step)
        {
            double copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy), new nint(&step));
            value = copy;
            return result;
        }

        public static unsafe bool InputDouble(string label, ref double value, double step, double step_fast)
        {
            double copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy), new nint(&step), new nint(&step_fast));
            value = copy;
            return result;
        }

        public static unsafe bool InputDouble(string label, ref double value, double step, double step_fast, string format)
        {
            double copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy), new nint(&step), new nint(&step_fast), format);
            value = copy;
            return result;
        }

        public static unsafe bool InputDouble(string label, ref double value, double step, double step_fast, string format, ImGuiInputTextFlags flags)
        {
            double copy = value;
            bool result = ImGui.InputScalar(label, ImGuiDataType.U64, new nint(&copy), new nint(&step), new nint(&step_fast), format, flags);
            value = copy;
            return result;
        }

        #endregion

        #region InputColorRgba

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
        private struct ImColorRgba
        {
            [FieldOffset(0)]
            public byte R;

            [FieldOffset(1)]
            public byte G;

            [FieldOffset(2)]
            public byte B;

            [FieldOffset(3)]
            public byte A;

            public ImColorRgba(Color color)
            {
                R = color.R;
                G = color.G;
                B = color.B;
                A = color.A;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Color ToColor()
                => Color.FromArgb(A, R, G, B);
        }

        public static unsafe bool InputColorRgba(string label, ref Color value)
        {
            var copy = new ImColorRgba(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4);
            value = copy.ToColor();
            return result;
        }

        public static unsafe bool InputColorRgba(string label, ref Color value, byte step)
        {
            var copy = new ImColorRgba(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4, step);
            value = copy.ToColor();
            return result;
        }

        public static unsafe bool InputColorRgba(string label, ref Color value, byte step, byte step_fast)
        {
            var copy = new ImColorRgba(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4, step, step_fast);
            value = copy.ToColor();
            return result;
        }

        public static unsafe bool InputColorRgba(string label, ref Color value, byte step, byte step_fast, string format)
        {
            var copy = new ImColorRgba(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4, step, step_fast, format);
            value = copy.ToColor();
            return result;
        }

        public static unsafe bool InputColorRgba(string label, ref Color value, byte step, byte step_fast, string format, ImGuiInputTextFlags flags)
        {
            var copy = new ImColorRgba(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4, step, step_fast, format, flags);
            value = copy.ToColor();
            return result;
        }

        #endregion

        #region InputColorArgb

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 4)]
        private struct ImColorArgb
        {
            [FieldOffset(0)]
            public byte A;

            [FieldOffset(1)]
            public byte R;

            [FieldOffset(2)]
            public byte G;

            [FieldOffset(3)]
            public byte B;

            public ImColorArgb(Color color)
            {
                A = color.A;
                R = color.R;
                G = color.G;
                B = color.B;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly Color ToColor()
                => Color.FromArgb(A, R, G, B);
        }

        public static unsafe bool InputColorArgb(string label, ref Color value)
        {
            var copy = new ImColorArgb(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4);
            value = copy.ToColor();
            return result;
        }

        public static unsafe bool InputColorArgb(string label, ref Color value, byte step)
        {
            var copy = new ImColorArgb(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4, step);
            value = copy.ToColor();
            return result;
        }

        public static unsafe bool InputColorArgb(string label, ref Color value, byte step, byte step_fast)
        {
            var copy = new ImColorArgb(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4, step, step_fast);
            value = copy.ToColor();
            return result;
        }

        public static unsafe bool InputColorArgb(string label, ref Color value, byte step, byte step_fast, string format)
        {
            var copy = new ImColorArgb(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4, step, step_fast, format);
            value = copy.ToColor();
            return result;
        }

        public static unsafe bool InputColorArgb(string label, ref Color value, byte step, byte step_fast, string format, ImGuiInputTextFlags flags)
        {
            var copy = new ImColorArgb(value);
            bool result = ImGui.InputScalarN(label, ImGuiDataType.U8, new nint(&copy), 4, step, step_fast, format, flags);
            value = copy.ToColor();
            return result;
        }

        #endregion

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

        #region Tooltips

        public static void ShowHoverTooltip(string desc)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos(450.0f);
                ImGui.TextUnformatted(desc);
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        #endregion

        public static void RightAlignedColumnText(string fmt, int padding = 0)
        {
            var posX = (ImGui.GetCursorPosX() + ImGui.GetColumnWidth() - ImGui.CalcTextSize(fmt).X
                - ImGui.GetScrollX() - padding * ImGui.GetStyle().ItemSpacing.X);
            if (posX > ImGui.GetCursorPosX())
                ImGui.SetCursorPosX(posX);
            ImGui.Text(fmt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawOver(nint user_texture_id, Vector2 location, Vector2 size)
            => ImGui.GetWindowDrawList().AddImage(user_texture_id, location, location + size);

        public static void PushStyleHideButtonBg()
        {
            ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetColorU32(ImGuiCol.WindowBg));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, ImGui.GetColorU32(ImGuiCol.WindowBg));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, ImGui.GetColorU32(ImGuiCol.WindowBg));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
        }

        public static void PopStyleHideButtonBg()
        {
            ImGui.PopStyleVar();
            ImGui.PopStyleColor(3);
        }
    }
}
