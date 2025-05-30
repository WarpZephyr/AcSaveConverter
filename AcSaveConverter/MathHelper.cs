namespace AcSaveConverter
{
    internal static class MathHelper
    {
        public static int BinaryAlign(int num, int alignment)
            => (num + (--alignment)) & ~alignment;

        public static uint BinaryAlign(uint num, uint alignment)
            => (num + (--alignment)) & ~alignment;

        public static long BinaryAlign(long num, long alignment)
            => (num + (--alignment)) & ~alignment;

        public static ulong BinaryAlign(ulong num, ulong alignment)
            => (num + (--alignment)) & ~alignment;
    }
}
