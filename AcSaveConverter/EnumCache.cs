namespace AcSaveConverterImGui
{
    internal static class EnumCache<TEnum> where TEnum : struct, Enum
    {
        static readonly string[] Names = Enum.GetNames<TEnum>();
        static readonly TEnum[] Values = Enum.GetValues<TEnum>();
        static readonly Dictionary<TEnum, int> IndexDictionary = BuildIndexDictionary();

        public static string[] GetEnumNames()
            => Names;

        public static string GetEnumName(TEnum value)
            => Names[GetEnumIndex(value)];

        public static string GetEnumName(int index)
            => Names[index];

        public static int GetEnumIndex(TEnum value)
            => IndexDictionary[value];

        public static TEnum GetEnumValue(int index)
            => Values[index];

        static Dictionary<TEnum, int> BuildIndexDictionary()
        {
            var indexDictionary = new Dictionary<TEnum, int>(Values.Length);
            for (int i = 0; i < Values.Length; i++)
            {
                indexDictionary.Add(Values[i], i);
            }

            return indexDictionary;
        }
    }
}
