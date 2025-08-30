using System;
using System.Collections.Generic;

namespace AcSaveConverter.Utilities
{
    internal static class EnumCache<TEnum> where TEnum : struct, Enum
    {
        static readonly string[] Names;
        static readonly TEnum[] Values;
        static readonly Dictionary<TEnum, int> IndexDictionary;

        static EnumCache()
        {
            Names = Enum.GetNames<TEnum>();
            Values = Enum.GetValues<TEnum>();

            IndexDictionary = new Dictionary<TEnum, int>(Values.Length);
            for (int i = 0; i < Values.Length; i++)
            {
                IndexDictionary.Add(Values[i], i);
            }
        }

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
    }
}
