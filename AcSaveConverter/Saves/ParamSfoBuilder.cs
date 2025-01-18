using AcSaveFormats.ACFA.PS3;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AcSaveConverterImGui.Saves
{
    public class ParamSfoBuilder
    {
        private readonly PARAMSFO ParamSfo;
        private readonly List<string> Files;
        private const PARAMSFO.DataFormat FormatUInt32 = PARAMSFO.DataFormat.UInt32;
        private const PARAMSFO.DataFormat FormatUTF8 = PARAMSFO.DataFormat.UTF8;
        private const PARAMSFO.DataFormat FormatUTF8S = PARAMSFO.DataFormat.UTF8S;

        public ParamSfoBuilder(PARAMSFO sfo)
        {
            ParamSfo = sfo;

            // Storing for RPCS3 BLIST later.
            Files = [];
        }

        #region Build

        public void BuildRPCS3BLIST()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Files.Count - 1; i++)
            {
                sb.Append($"{Files[i]}/");
            }

            if (Files.Count > 0)
            {
                sb.Append(Files[^1]);
            }

            SetRPCS3BLIST(sb.ToString());
        }

        #endregion

        #region Add File

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFile(string name, uint value)
            => AddFileInternal(name, value.ToString());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFile(string name, string value)
            => AddFileInternal(name, value);

        // Not the best solution for detection of value, it seems PNG has it set to 0 most of the time.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFile(string name)
            => AddFileInternal(name, name.EndsWith(".PNG", StringComparison.InvariantCultureIgnoreCase) ? "0" : "1");

        private void AddFileInternal(string name, string value)
        {
            string upper = name.ToUpperInvariant();
            AddUInt32($"*{upper}", value);
            Files.Add(upper);
        }

        #endregion

        #region Default Setters

        public void SetDefaults()
        {
            SetAccountID("0000000000000000");
            SetAttribute("0");
            SetCategory("SD");
            SetDetail(string.Empty);
            SetParams(string.Empty);
            SetParams2(string.Empty);
            SetParentalLevel("0");
            SetSaveDataDirectory(string.Empty);
            SetSaveDataListParam(string.Empty);
            SetSubTitle(string.Empty);
            SetTitle(string.Empty);
        }

        public void SetDefaultsRPCS3()
        {
            SetAccountID("0000000000000000");
            SetAttribute("0");
            SetCategory("SD");
            SetDetail(string.Empty);
            SetParams(string.Empty);
            SetParams2(string.Empty);
            SetParentalLevel("0");
            SetRPCS3BLIST(string.Empty, 0);
            SetSaveDataDirectory(string.Empty);
            SetSaveDataListParam(string.Empty);
            SetSubTitle(string.Empty);
            SetTitle(string.Empty);
        }

        public void SetAccountID(string value)
            => AddUTF8S("ACCOUNT_ID", value, 16);

        public void SetAttribute(uint value)
            => AddUInt32("ATTRIBUTE", value);

        public void SetAttribute(string value)
            => AddUInt32("ATTRIBUTE", value);

        public void SetCategory(string value)
            => AddUTF8("CATEGORY", value, 4);

        public void SetDetail(string value)
            => AddUTF8("DETAIL", value, 1024);

        public void SetParams(string value)
            => AddUTF8("PARAMS", value, 1024);

        public void SetParams2(string value)
            => AddUTF8("PARAMS2", value, 12);

        public void SetParentalLevel(uint value)
            => AddUInt32("PARENTAL_LEVEL", value);

        public void SetParentalLevel(string value)
            => AddUInt32("PARENTAL_LEVEL", value);

        private void SetRPCS3BLIST(string value, uint maxLength)
            => AddUTF8("RPCS3_BLIST", value, maxLength);

        public void SetRPCS3BLIST(string value)
        {
            // RPCS3 BLIST length is the max length of the null terminated string, aligned on a 4 byte boundary.
            int byteCount = Encoding.UTF8.GetByteCount(value + '\0');
            uint maxLength = MathHelper.BinaryAlign((uint)byteCount, 4);
            AddUTF8("RPCS3_BLIST", value, maxLength);
        }

        public void SetSaveDataDirectory(string value)
            => AddUTF8("SAVEDATA_DIRECTORY", value, 32);

        public void SetSaveDataListParam(string value)
            => AddUTF8("SAVEDATA_LIST_PARAM", value, 8);

        public void SetSubTitle(string value)
            => AddUTF8("SUB_TITLE", value, 128);

        public void SetTitle(string value)
            => AddUTF8("TITLE", value, 128);

        #endregion

        #region Add Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddInt32(string name, int value)
            => AddUInt32(name, value.ToString());

        public void AddString(string name, string value, int maxLength, bool terminated = true)
        {
            if (terminated)
            {
                AddUTF8(name, value, maxLength);
            }
            else
            {
                AddUTF8S(name, value, maxLength);
            }
        }

        public void AddUInt32(string name, uint value)
            => SetParam(name, value.ToString(), FormatUInt32, 4U);

        public void AddUInt32(string name, string value)
            => SetParam(name, value, FormatUInt32, 4U);

        public void AddUTF8(string name, string value, int maxLength)
            => SetParam(name, value, FormatUTF8, (uint)maxLength);

        public void AddUTF8(string name, string value, uint maxLength)
            => SetParam(name, value, FormatUTF8, maxLength);

        public void AddUTF8S(string name, string value, int maxLength)
            => SetParam(name, value, FormatUTF8S, (uint)maxLength);

        public void AddUTF8S(string name, string value, uint maxLength)
            => SetParam(name, value, FormatUTF8S, maxLength);

        private void SetParam(string name, string value, PARAMSFO.DataFormat format, uint maxLength)
        {
            if (!ParamSfo.Parameters.TryGetValue(name, out PARAMSFO.Parameter? param))
            {
                param = MakeParam(value, format, maxLength);
                ParamSfo.Parameters.Add(name, param);
            }

            param.Data = value;
            param.Format = format;
            param.DataMaxLength = maxLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PARAMSFO.Parameter MakeParam(string value, PARAMSFO.DataFormat format, uint maxLength)
            => new(value, format, maxLength);

        #endregion
    }
}
