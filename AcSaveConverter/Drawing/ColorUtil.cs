using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AcSaveConverter.Drawing
{
    internal static class ColorUtil
    {
        public static Vector3 ToRgbaVector3(this Color color)
            => new Vector3(ToPercent(color.R), ToPercent(color.G), ToPercent(color.B));

        public static Vector4 ToRgbaVector4(this Color color)
            => new Vector4(ToPercent(color.R), ToPercent(color.G), ToPercent(color.B), ToPercent(color.A));

        public static Color ToRgbaColor(this Vector3 vec)
            => Color.FromArgb(FromPercent(vec.X), FromPercent(vec.Y), FromPercent(vec.Z));

        public static Color ToRgbaColor(this Vector4 vec)
            => Color.FromArgb(FromPercent(vec.W), FromPercent(vec.X), FromPercent(vec.Y), FromPercent(vec.Z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ToPercent(byte value)
            => value / 255.0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte FromPercent(float percent)
            => (byte)(float.Clamp(percent, 0.0f, 1.0f) * 255.0f);
    }
}
