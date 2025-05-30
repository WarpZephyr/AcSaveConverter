using System.Runtime.CompilerServices;

namespace AcSaveConverter.Logging
{
    internal static class Log
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(string value)
            => AppLog.Instance.Write(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLine(string value)
            => AppLog.Instance.WriteLine(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLine()
            => AppLog.Instance.WriteLine();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectWrite(string value)
            => AppLog.Instance.DirectWrite(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectWriteLine(string value)
            => AppLog.Instance.DirectWriteLine(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DirectWriteLine()
            => AppLog.Instance.DirectWriteLine();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Flush()
            => AppLog.Instance.Flush();
    }
}
